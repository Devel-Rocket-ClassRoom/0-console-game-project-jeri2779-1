using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;


internal class StageManager
{
    private readonly Scene _scene;
    private readonly GameData _gameData;

    private StagePhase _phase = StagePhase.Waiting;
    private float _phaseTimer = 0f;
    private float _bossTimer = 0f;

    private float _waveTimer = 0f;

    public float _waveTimeRemains => CurrentStage.Waves[_currentWave].WaveTime - _waveTimer; // 현재 웨이브 남은 시간 계산
    public float _phaseTimeRemains => CurrentStage.WaitTime - _phaseTimer; // 다음 웨이브 스폰까지 남은 시간 계산
    public float _bossTimeRemains => CurrentStage.BossWave.WaveTime - _bossTimer; // 보스 전투 남은 시간 계산

    private int _currentWave = 0;
    //private int _killCount = 0;

    private StageData.StageInfo CurrentStage => StageData.All[_gameData.Stage - 1]; // 현재 스테이지 정보

    public event GameAction OnAllStageClear; // 스테이지 클리어 이벤트
    public event GameAction OnStageClear;

    internal enum StagePhase
    {
        Waiting,
        WaveSpawn,
        BossSpawn,
        BossFight,
        StageClear,
    }

    public StagePhase Phase => _phase;  // Playing이 현재 페이즈 읽기 용도

    public StageManager(Scene scene, GameData gameData)
    {
        _scene = scene;
        _gameData = gameData;
    }

    public void Update(float deltaTime)
    {
        switch (_phase)
        {
            case StagePhase.Waiting: UpdateWaiting(deltaTime); break; // 대기 시간 동안 타이머 업데이트
            case StagePhase.WaveSpawn: UpdateWaveSpawn(deltaTime); break;// 웨이브 스폰 중에는 적이 모두 스폰될 때까지 대기
            case StagePhase.BossSpawn: UpdateBossSpawn(deltaTime); break;// 보스 스폰 중에는 보스가 모두 스폰될 때까지 대기
            case StagePhase.BossFight: UpdateBossFight(deltaTime); break;// 보스와의 전투 중에는 보스가 처치될 때까지 대기
            case StagePhase.StageClear: UpdateStageClear(deltaTime); break;// 스테이지 클리어 후에는 다음 스테이지로 넘어가기 전까지 대기
        }
    }

    private void UpdateWaiting(float deltaTime)
    {
        _phaseTimer += deltaTime;                   // 대기 시간 업데이트
        if (_phaseTimer >= CurrentStage.WaitTime)   // 대기 시간이 끝나면 웨이브 스폰 단계로 전환
        {
            _phaseTimer = 0f;
            _currentWave = 0;
            SpawnWave(_currentWave);
            _phase = StagePhase.WaveSpawn;
        }
    }
    private void UpdateWaveSpawn(float deltaTime)
    {
        _waveTimer += deltaTime;

        var enemies = _scene.FindGameObjectsAll("Enemy"); // 씬에서 모든 적 객체 찾기
        bool timeOver = _waveTimer >= CurrentStage.Waves[_currentWave].WaveTime;//웨이브 시간이 초과되었는지 확인
        bool allKilled = enemies.Count == 0;
        if (!timeOver && !allKilled) return;

        if(timeOver)
        {
            foreach(var enemys in enemies)
            {
                _scene.RemoveGameObject(enemys); // 웨이브 시간이 초과되면 남아있는 적 모두 제거
                 
            }
        }
        _waveTimer = 0f;
        _currentWave++;

        if (_currentWave < CurrentStage.Waves.Length)    // 다음 웨이브가 남아있으면 스폰
        {
            SpawnWave(_currentWave);
        }
        else
        {
            _phase = StagePhase.BossSpawn;                      // 모든 웨이브가 끝나면 보스 스폰 단계로 전환
        }
    }
    private void UpdateBossSpawn(float deltaTime)
    {
        SpawnBoss();                                  // 보스 스폰
        _phase = StagePhase.BossFight;                // 보스 전투 단계로 전환

    }
    private void UpdateBossFight(float deltaTime)             // 보스와의 전투 중 보스가 처치되면 스테이지 클리어로 전환
    {
        _bossTimer += deltaTime;

        var bosses = _scene.FindGameObjectsAll("Enemy");
        if (bosses.Count == 0 || _bossTimer >= CurrentStage.BossWave.WaveTime)
        {
            _bossTimer = 0f;
            _phase = StagePhase.StageClear;              // 보스 처치되면 스테이지 클리어 전환
        }
    }
    private void UpdateStageClear(float deltaTime)          // 스테이지 클리어 후 다음 스테이지로 넘어가기 전까지 대기
    {
        if (_gameData.Stage >= StageData.All.Length)
        {
            OnAllStageClear?.Invoke(); // 모든 스테이지 클리어 이벤트 호출
            return;
        }
        OnStageClear?.Invoke();    // 스테이지 클리어 이벤트 호출

        _gameData.Stage++;          // 다음 스테이지로 이동
        _currentWave = 0;           // 웨이브 초기화
        _phaseTimer = 0f;           // 타이머 초기화
        _phase = StagePhase.Waiting;// 대기 단계로 전환
    }
    private void SpawnWave(int waveIndex)// 웨이브 스폰 메서드
    {
        var wave = CurrentStage.Waves[waveIndex];                       // 웨이브 정보
        int spacing = (Wall.Right - Wall.Left) / (wave.EnemyCount + 1);// 적 간격 계산
        for (int i = 0; i < wave.EnemyCount; i++)
        {
            float spawnX = Wall.Left + spacing * (i + 1);               // 스폰 위치 X 계산

            var pattern = wave.Patterns[i % wave.Patterns.Length];      // 웨이브 패턴 선택
            _scene.AddGameObject(new Enemy(_scene, wave.EnemyHP, spawnX, Wall.Top, pattern)); // 씬에 적 추가
        }
    }

    private void SpawnBoss()// 보스 스폰 메서드
    {
        var bossWave = CurrentStage.BossWave;                           // 보스 웨이브 정보
        float centerX = (Wall.Left + Wall.Right) / 2f;
        var pattern = bossWave.Patterns[0];//보스 패턴 여러개일경우 절차적으로 선택 가능하도록 수정 필요                             

        _scene.AddGameObject(new Enemy(_scene, bossWave.EnemyHP, centerX, Wall.Top, pattern));
        //임시로 Enemy 클래스 사용, 보스 전용 클래스로 변경 필요
    }
}

    
 
