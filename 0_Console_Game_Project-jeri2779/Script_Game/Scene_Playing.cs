using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

public class Playing : Scene

{
    private Wall _wall;                          // 벽 오브젝트
    private Player _player;                      // 플레이어 오브젝트
    //private Enemy _enemy;                      // 적 오브젝트

    private int _boundWidth;                     // 게임 화면 너비
    private int _boundHeight;                    // 게임 화면 높이

    private int _killCount;                      // 적 처치 수  

    private float _stageTimer;                  // 스테이지 타이머
    private const float _stageWaitTime = 3f;     // 스테이지 시작 대기 시간



    private bool _isGameOver;                    // 게임 오버 변수
    private bool _isAllClear = false;
    private bool _isStageClear = false;         // 스테이지 클리어 변수

    public event GameAction OnPlayAgain;        // 다시 시작 이벤트
    public event GameAction OnGameOver;         // 게임 오버 이벤트

   
  

    private readonly GameData _gameData;         // 게임 데이터 참조
    private StageManager _stageManager;          // 스테이지 매니저 참조

    //private int _life;                         // 생명 변수
    private int _stageScore;
    //private bool _isGameOver;





    //스테이지는 시작하고 일정시간 대기 후 적들이 스폰됨 스테이지당 웨이브가 나뉘어져 있고
    // 일반 웨이브가 끝나면 보스 스폰 후 보스가 죽으면 스테이지 클리어 되는 방식으로 진행
    //모든 웨이브(보스 포함)은 처리하는 방법과 일정시간이 지나면 다음 웨이브로 넘어가는 방식으로 진행

    public Playing(int width, int height, GameData gameData)
    {
        // 생성자에서 필요한 초기화 작업 수행
        _boundWidth = width;
        _boundHeight = height;
        _gameData = gameData;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        //wall.Draw(buffer); // 벽 그리기
        // 게임 씬 그리기 로직 
        DrawGameObjects(buffer);
        _player.Draw(buffer); // 플레이어 그리기(항상 마지막)

         
        //플레이어는 반드시 마지막에 그리도록 해야함(총알이 플레이어 덮는것 방지)
        buffer.WriteText(1, 0, $"Life: {_gameData.Life}", ConsoleColor.Cyan);
        buffer.WriteText(1, 1, $"Score: {_stageScore}", ConsoleColor.Green);
        buffer.WriteText(15,1, $"Stage: {_gameData.Stage}", ConsoleColor.Magenta);

        if (_isGameOver)//게임 오버 상태 화면
        {
            buffer.WriteTextCentered(8, "Game Over", ConsoleColor.Red);
            buffer.WriteTextCentered(10, $"Score: {_stageScore}", ConsoleColor.Yellow);
            buffer.WriteTextCentered(12, "ENTER to Retry", ConsoleColor.White);
        }
        if (_isAllClear)//모든 스테이지 클리어 상태 화면
        {
            buffer.WriteTextCentered(8, "All Stages Clear!", ConsoleColor.Yellow);
            buffer.WriteTextCentered(10, $"Score: {_stageScore}", ConsoleColor.Green);
            buffer.WriteTextCentered(12, "ENTER to Re-Play", ConsoleColor.White);
        }

        if (_isStageClear)
        {
            buffer.WriteTextCentered(8, $"Stage {_gameData.Stage - 1} Clear!", ConsoleColor.Yellow);
            buffer.WriteTextCentered(10, $"Score: {_stageScore}", ConsoleColor.Green);
            buffer.WriteTextCentered(12, $"Kills: {_killCount}", ConsoleColor.White);
        }
        //throw new NotImplementedException();
    }
   
    public override void Load() //Awake()
    {
         
        _stageScore = 0;
        _isGameOver = false;


        _wall = new Wall(this, _boundWidth, _boundHeight);
        AddGameObject(_wall);
         

        _player = new Player(this, _boundWidth / 2, _boundHeight - 3);
        AddGameObject(_player);

        _stageManager = new StageManager(this, _gameData);
        _stageManager.OnStageClear += () => { _isStageClear = true; _stageTimer = 0; };   // 스테이지 클리어 이벤트 구독
        _stageManager.OnAllStageClear += () => { _isAllClear = true;};  // 스테이지 올클리어 이벤트 구독


        //throw new NotImplementedException();
    }
    public override void Unload()
    {
        ClearGameObjects();
        // 게임 씬 언로드 로직 (예: 리소스 정리)
        //throw new NotImplementedException();
    }
    public override void Update(float deltaTime)
    {
        GameOver(); // 게임 오버 상태 
       
        AllClear();// 모든 스테이지 클리어 상태

        if (_isStageClear)// 스테이지 클리어 상태
        {
            _stageTimer += deltaTime;
            if (_stageTimer >= _stageWaitTime)
            {
                _isStageClear = false;
            }
            return;
        }
        UpdateGameObjects(deltaTime);    
        CheckCollisions();              // 충돌 체크 호출
        _stageManager.Update(deltaTime); // 스테이지 매니저 업데이트 호출

    }
    //추가 메서드 =======================================================================================
    public void GameOver()
    {
        if (_isGameOver)
        {
            if (Input.IsKeyDown(ConsoleKey.Enter))
            {
                _isGameOver = false;
                OnPlayAgain?.Invoke();
            }
            return;
        }
    }
     
    
    public void AllClear()
    {
        if (_isAllClear)
        {
            _player.IsActive = false; // 플레이어 비활성화
            if (Input.IsKeyDown(ConsoleKey.Enter))
            {
                _isGameOver = false;
                OnPlayAgain?.Invoke();
            }
            return;
        }

    }



    public void CheckCollisions() // 충돌 체크 메서드
    {

        var bullets = FindGameObjectsAll("Player_Bullet");
        var enemies = FindGameObjectsAll("Enemy");
        var enemyBullets = FindGameObjectsAll("Enemy_Bullet");

        //플레이어  총알과 적 충돌 체크
        foreach (var bullet in bullets)
        {
            foreach (var enemy in enemies)
            {
                if (Math.Abs(bullet.X - enemy.X) <= 1f               // 총알과 적의 충돌 범위 체크
                && Math.Abs(bullet.Y - enemy.Y) <= 1f)              // 충돌이 발생한 경우 총알과 적 제거, 점수 증가

                {

                    RemoveGameObject(bullet);
                    //체력 시스템을 구현할시 remove를 다른곳에서 하고 대신 대미지 관련 로직을 넣을수도 있음
                    RemoveGameObject(enemy);
                    _stageScore += 10;
                    _killCount++;
                    break;                              // 한 총알이 여러 적과 충돌하는 것을 방지하기 위해 내부 루프 탈출
                }
            }
        }
        // 적 총알과 플레이어 충돌 체크
        foreach (var bullet in enemyBullets)
        {
            if (_player != null && _player.IsActive)
            {
                if (Math.Abs(bullet.X - _player.X) <= 1f &&           // 총알과 플레이어의 충돌 범위 체크
                   Math.Abs(bullet.Y - _player.Y) <= 1f)             // 충돌이 발생한 경우 총알 제거, 생명 감소
                {
                    RemoveGameObject(bullet);
                    _gameData.Life--;
                    if (_gameData.Life <= 0)
                    {
                        _isGameOver = true;

                        _player.IsActive = false;
                    }

                }
            }

        }
        //현재방식은 모든 총알이 플레이어와 충돌체크를 하기 때문에 총알이 많아질수록 성능이 저하될 수 있음
        




    }


}       

