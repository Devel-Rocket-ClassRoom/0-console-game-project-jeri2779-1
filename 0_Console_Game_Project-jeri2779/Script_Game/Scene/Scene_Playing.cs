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

    private float _respawnTimer = 0f;             // 피격 후 리스폰 대기 타이머
    private const float _respawnDelay = 2f;          // 피격 후 리스폰까지 대기 시간 (초)

    private float _invincibleTimer = 0f;             // 리스폰 후 무적 타이머
    private const float _invincibleDuration = 3f;    // 리스폰 후 무적 지속 시간 (초)

    public event GameAction OnPlayAgain;        // 다시 시작 이벤트
    public event GameAction OnGameOver;         // 게임 오버 이벤트

   
  

    private readonly GameData _gameData;         // 게임 데이터 참조
    private StageManager _stageManager;          // 스테이지 매니저 참조

    //private int _life;                         // 생명 변수
    //private int _stageScore;
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
        DrawGameObjects(buffer);
        if (!_isStageClear && !_isAllClear && _player.IsActive) // 플레이어는 반드시 마지막에 그리기 (총알이 덮는 것 방지)
        {
            _player.Draw(buffer);
        }

        // === HUD Row 0: Life | Stage | Score ===
        buffer.SetCell(20, 0, '|', ConsoleColor.DarkGray); // 구분선
        buffer.SetCell(39, 0, '|', ConsoleColor.DarkGray); // 구분선

        ConsoleColor lifeColor = _respawnTimer > 0f  ? ConsoleColor.DarkGray
                               : _invincibleTimer > 0f ? ConsoleColor.White
                               : ConsoleColor.Cyan;
        buffer.WriteText(1, 0, $"[Life: {_gameData.Life}]", lifeColor);
        buffer.WriteTextCentered(0, $"Stage {_gameData.Stage}", ConsoleColor.Magenta);
        string scoreText = $"Score: {_gameData.Score}";
        buffer.WriteText(58 - scoreText.Length, 0, scoreText, ConsoleColor.Green);

        // === HUD Row 1: 상황별 타이머 ===
        if (_respawnTimer > 0f)
        {
            buffer.WriteTextCentered(1, $"Respawn: {(int)_respawnTimer + 1}s", ConsoleColor.DarkGray);
        }
        else if (_stageManager.Phase == StageManager.StagePhase.BossFight)
        {
            string bossText = $"Boss: {(int)_stageManager._bossTimeRemains}s";
            buffer.WriteText(58 - bossText.Length, 1, bossText, ConsoleColor.Red);
        }
        else if (_stageManager.Phase == StageManager.StagePhase.WaveSpawn)
        {
            string waveText = $"Wave: {(int)_stageManager._waveTimeRemains}s";
            buffer.WriteText(58 - waveText.Length, 1, waveText, ConsoleColor.Yellow);
        }

        // === Overlay (공통 박스 위치: x=14, y=6, w=32) ===
        const int bx = 14, by = 6, bw = 32;

        if (_isGameOver)
        {
            buffer.DrawBox(bx, by, bw, 9, ConsoleColor.Red);
            buffer.WriteTextCentered(by + 2, "GAME  OVER", ConsoleColor.Red);
            buffer.WriteTextCentered(by + 4, $"Score: {_gameData.Score}", ConsoleColor.Yellow);
            buffer.WriteTextCentered(by + 6, "[ ENTER ]  Retry", ConsoleColor.White);
        }
        else if (_isAllClear)
        {
            buffer.DrawBox(bx, by, bw, 9, ConsoleColor.Yellow);
            buffer.WriteTextCentered(by + 2, "All  Stages  Clear!", ConsoleColor.Yellow);
            buffer.WriteTextCentered(by + 4, $"Score: {_gameData.Score}", ConsoleColor.Green);
            buffer.WriteTextCentered(by + 6, "[ ENTER ]  Play Again", ConsoleColor.White);
        }
        else if (_isStageClear)
        {
            buffer.DrawBox(bx, by, bw, 9, ConsoleColor.Cyan);
            buffer.WriteTextCentered(by + 2, $"Stage {_gameData.Stage - 1}  Clear!", ConsoleColor.Yellow);
            buffer.WriteTextCentered(by + 4, $"Score: {_gameData.Score}   Eliminates: {_killCount}", ConsoleColor.Green);
            int countdown = Math.Max(0, (int)(_stageWaitTime - _stageTimer) + 1);
            buffer.WriteTextCentered(by + 6, $"Next Stage in {countdown}s...", ConsoleColor.White);
        }
        else if (_stageManager.Phase == StageManager.StagePhase.Waiting && !_isAllClear && !_isGameOver)
        {
            buffer.DrawBox(bx, by, bw, 7, ConsoleColor.DarkCyan);
            buffer.WriteTextCentered(by + 2, $"Stage {_gameData.Stage}  Start!", ConsoleColor.Yellow);
            buffer.WriteTextCentered(by + 4, $"Ready...  {(int)_stageManager._phaseTimeRemains + 1}s", ConsoleColor.Green);
        }
    }
   
    public override void Load() //Awake()
    {
         
        _gameData.Score = 0;
        _isGameOver = false;


        _wall = new Wall(this, _boundWidth, _boundHeight);
        AddGameObject(_wall);
         

        _player = new Player(this, _boundWidth / 2, _boundHeight - 3);
        AddGameObject(_player);

        _stageManager = new StageManager(this, _gameData);// 스테이지 매니저 초기화
        _stageManager.OnStageClear += () =>             // 스테이지 클리어 시 호출되는 이벤트 핸들러
        {
            _isStageClear = true; 
            _stageTimer = 0; 
            ClearState();
        };

        _stageManager.OnAllStageClear += () =>          // 모든 스테이지 클리어 시 호출되는 이벤트 핸들러
        {
            _isAllClear = true;
            ClearState();
        };

        _stageManager.OnEnemySpawned += enemy =>        // 적이 스폰될 때마다 호출되는 이벤트 핸들러
        {
            enemy.OnDied += () =>
            {
                RemoveGameObject(enemy);                // 적이 스폰될 때마다 게임 오브젝트 리스트에 추가
                _gameData.Score += 10;                  // 적이 처치될 때마다 점수 증가
                _killCount++;                           // 적이 처치될 때마다 처치 수 증가
            };
        };

        _stageManager.OnBossSpawned += boss =>          // 보스가 스폰될 때마다 호출되는 이벤트 핸들러
        {
            boss.OnDied += () =>
            {
                RemoveGameObject(boss);                 // 보스가 죽으면 게임 오브젝트 리스트에서 제거
                _gameData.Score += 100;                 // 보스가 처치될 때마다 점수 대폭 증가
                _killCount++;
            };
        };

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
        if (_isGameOver) return; // 게임 오버 중에는 스테이지 매니저 업데이트 차단

        AllClear();// 모든 스테이지 클리어 상태
        if (_isAllClear) return; // 올 클리어 중에는 스테이지 매니저 업데이트 차단

        if (_isStageClear)// 스테이지 클리어 상태
        {
            _stageTimer += deltaTime;
            if (_stageTimer >= _stageWaitTime)
            {
                _isStageClear = false;
                _player.ResetPostion(_boundWidth / 2, _boundHeight - 3);            // 플레이어 위치 초기화
                _player.IsActive = true; // 플레이어 활성화
                  
            }
            return;
        }
        _stageManager.Update(deltaTime); // 스테이지 매니저 업데이트 호출
        UpdateGameObjects(deltaTime);

        if (_respawnTimer > 0f)                              // 리스폰 대기 중
        {
            _respawnTimer = Math.Max(0f, _respawnTimer - deltaTime); // 리스폰 타이머 감소 음수 방지 추가

            if (_respawnTimer <= 0f)                         // 대기 완료 → 리스폰
            {
                _player.ResetPostion(_boundWidth / 2, _boundHeight - 3);
                _invincibleTimer = _invincibleDuration;      // 리스폰 후 무적 시작
            }
        }
        if (_invincibleTimer > 0f) _invincibleTimer = Math.Max(0f, _invincibleTimer - deltaTime); // 무적 타이머 감소 음수 방지 추가

        CheckCollisions();              // 충돌 체크 호출

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
                _isAllClear = false;
                OnPlayAgain?.Invoke();
            }
            return;
        }

    }

    private void ClearState()
    {
        _respawnTimer = 0f;     // 리스폰 대기 취소
        _invincibleTimer = 0f;  // 무적 취소
        //플레이어를 제외한 모든 오브젝트 제거  
        foreach (var obj in FindGameObjectsAll("Player_Bullet")) // 게임 오브젝트 리스트를 순회하면서 모든 오브젝트 제거
        {
            RemoveGameObject(obj);
        }
        foreach (var obj in FindGameObjectsAll("Enemy")) // 게임 오브젝트 리스트를 순회하면서 모든 오브젝트 제거
        {
            RemoveGameObject(obj);
        }
        foreach (var obj in FindGameObjectsAll("Enemy_Bullet")) // 게임 오브젝트 리스트를 순회하면서 모든 오브젝트 제거
        {
            RemoveGameObject(obj);
        }
        _player.IsActive = false;

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
                    if (enemy is Enemy enm)
                    {
                        enm.TakeDamage(_player.AttackDamage);
                    }
                  
                    break;                              // 한 총알이 여러 적과 충돌하는 것을 방지하기 위해 내부 루프 탈출
                }
            }
        }

        var bosses = FindGameObjectsAll("Boss");
        foreach (var bullet in bullets)
        {
            foreach (var bossObj in bosses)
            {
                if (Math.Abs(bullet.X - bossObj.X) <= 1f
                && Math.Abs(bullet.Y - bossObj.Y) <= 1f)
                {
                    RemoveGameObject(bullet);
                    if (bossObj is Boss b)
                    {
                        b.TakeDamage(_player.AttackDamage);
                    }
                    break;
                }
            }
        }
        // 적 총알과 플레이어 충돌 체크
        foreach (var bullet in enemyBullets)
        {
            if (_player != null && _player.IsActive && _invincibleTimer <= 0f) // 무적 중에는 피격 무시
            {
                if (Math.Abs(bullet.X - _player.X) <= 1f &&           // 총알과 플레이어의 충돌 범위 체크
                   Math.Abs(bullet.Y - _player.Y) <= 1f)             // 충돌이 발생한 경우 총알 제거, 생명 감소
                {
                    RemoveGameObject(bullet);
                    _gameData.Life--;
                    if (_gameData.Life <= 0)
                    {
                        _isGameOver = true;
                        ClearState();
                    }
                    else
                    {
                        _player.IsActive = false;                      // 플레이어 화면에서 사라짐
                        _respawnTimer = _respawnDelay;                  // 리스폰 대기 시작
                    }
                    break; // 한 프레임에 총알 하나만 피격 처리
                }
            }
        }
        //현재방식은 모든 총알이 플레이어와 충돌체크를 하기 때문에 총알이 많아질수록 성능이 저하될 수 있음

    }



     
}       

