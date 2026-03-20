using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

public class Playing : Scene

{
    private Wall wall;                    // 벽 오브젝트
    private Player player;                // 플레이어 오브젝트
    private Enemy enemy;                  // 적 오브젝트

    private int boundWidth;                // 게임 화면 너비
    private int boundHeight;               // 게임 화면 높이


    private int life;                     // 생명 변수
    private int score;                    // 점수 변수
    private bool isGameOver;              // 게임 오버 상태 변수

    public event GameAction OnPlayAgain;  // 다시 시작 이벤트
    public event GameAction OnGameOver;   // 게임 오버 이벤트

    private StagePhase _phase = StagePhase.Waiting;
    private float _phaseTimer = 0f;          // 대기 시간용 타이머

    private int _currentStage = 1;           // 현재 스테이지 번호
    private int _currentWave = 0;            // 현재 웨이브 번호
    private int _killCount = 0;
    private int _stageScore = 0;             // 스테이지 점수

    private enum StagePhase
    {
        Waiting,
        WaveSpawn,
        BossSpawn,
        BossFight,
        StageClear,
    }

    //스테이지는 시작하고 일정시간 대기 후 적들이 스폰됨 스테이지당 웨이브가 나뉘어져 있고
    // 일반 웨이브가 끝나면 보스 스폰 후 보스가 죽으면 스테이지 클리어 되는 방식으로 진행
    //모든 웨이브(보스 포함)은 처리하는 방법과 일정시간이 지나면 다음 웨이브로 넘어가는 방식으로 진행

    public Playing(int width, int height)
    {
        // 생성자에서 필요한 초기화 작업 수행
        boundWidth = width;
        boundHeight = height;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        //wall.Draw(buffer); // 벽 그리기
        // 게임 씬 그리기 로직 
        DrawGameObjects(buffer);
        player.Draw(buffer); // 플레이어 그리기(항상 마지막)

        //player.Draw(buffer); // 플레이어 그리기
        //플레이어는 반드시 마지막에 그리도록 해야함(총알이 플레이어 덮는것 방지)
        buffer.WriteText(1, 0, $"life: {life}", ConsoleColor.Cyan); // 생명 표시
        buffer.WriteText(10, 1, $"score: {score}", ConsoleColor.Green); // 점수 표시

        if (isGameOver)
        {
            buffer.WriteTextCentered(8, "Game Over", ConsoleColor.Red);
            buffer.WriteTextCentered(10, $"Life: {life}", ConsoleColor.Yellow);
            buffer.WriteTextCentered(12, "Press ENTER to Retry", ConsoleColor.White);
        }
        //throw new NotImplementedException();
    }
    public override void Load() //Awake()
    {
        life = 3;
        isGameOver = false;


        wall = new Wall(this, boundWidth, boundHeight);
        AddGameObject(wall);
        enemy = new Enemy(this, 10, 0, BPatterns.Circle8);
        AddGameObject(enemy);
        enemy = new Enemy(this, 30, 0, BPatterns.Spread5);
        AddGameObject(enemy);

        player = new Player(this, boundWidth / 2, boundHeight - 3);
        AddGameObject(player);


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
        UpdateGameObjects(deltaTime); // ← 주석 해제: 플레이어 Update/HandleInput 호출됨
        CheckCollisions(); // 충돌 체크 호출



        if (isGameOver)
        {
            if (Input.IsKeyDown(ConsoleKey.Enter))
            {
                isGameOver = false;
                OnPlayAgain?.Invoke();
            }
            return;
        }

    }

    public void CheckCollisions()
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
                    score += 10;
                    break;                  // 한 총알이 여러 적과 충돌하는 것을 방지하기 위해 내부 루프 탈출
                }
            }
        }
        // 적 총알과 플레이어 충돌 체크
        foreach (var bullet in enemyBullets)
        {
            if (player != null && player.IsActive)
            {
                if (Math.Abs(bullet.X - player.X) <= 1f &&           // 총알과 플레이어의 충돌 범위 체크
                   Math.Abs(bullet.Y - player.Y) <= 1f)             // 충돌이 발생한 경우 총알 제거, 생명 감소
                {
                    RemoveGameObject(bullet);
                    life--;
                    if (life <= 0) isGameOver = true;
                }
            }

        }

        //var allObjects = _gameObjects;  

        //foreach (var obj in allObjects)
        //{
        //    if (obj is Bullet bullet && bullet.IsActive)
        //    {
        //        if (bullet.Name == "Player_Bullet")
        //        {
        //            // 적들과의 충돌 체크 로직 수행
        //        }
        //        else if (bullet.Name == "Enemy_Bullet")
        //        {
        //            // 플레이어와의 충돌 체크 로직 수행
        //        }
        //    }
        //}


    }    


}       

