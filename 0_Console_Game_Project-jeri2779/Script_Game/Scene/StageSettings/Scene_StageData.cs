using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;
using static StageData;

internal class StageData
{
    internal class WaveData// 웨이브 정보 클래스
    {

        public EnemyType EnemyType { get; set; } = EnemyType.EnemyA;
        public BossType BossType { get; set; } = BossType.BossA; // 보스 웨이브에서 사용할 보스 타입
        public int EnemyCount { get; set; }             // 웨이브당 적의 수
        public float WaveTime { get; set; }             // 웨이브 지속 시간

        public float SpawnY { get; set; } = Wall.Top;   // 기본값 Wall.Top (위에서 스폰)
        public bool SpawnFromSide { get; set; } = false; // 옆에서 스폰 여부

        public int EnemyHP { get; set; }                 // 적의 체력   
        public float ShootInterval { get; set; }        // 적이 총알을 발사하는 간격

        
        //public Action<Scene, float, float>[] Patterns;  // 적의 총알 발사 패턴을 정의
        public BulletPattern[] Patterns { get; set; }  // 적의 총알 발사 패턴 배열
        public BossPhase[] Phases { get; set; }              // 보스 웨이브에서 각 패턴이 적용되는 페이즈
        //public MovePattern Move { get; set; }          // 이동 방식
        public Func<MovePattern> MoveFactory { get; set; }   // 이동 패턴 팩토리 (추가)
    }

    internal class StageInfo// 스테이지 정보 클래스
    {
        public WaveData[] Waves { get; set; }            // 일반 웨이브 정보 배열
        public WaveData BossWave { get; set; }           // 보스 웨이브 정보
        public float WaitTime { get; set; }               // 스테이지 시작 전 대기 시간
    }

    internal class BossPhase// 보스 페이즈 정보 클래스
    {
        public float HpThreshold { get; set; }           // 보스 체력 임계값 (0~1 사이)
        public BulletPattern[] Patterns { get; set; }    // 해당 페이즈에서 적용되는 총알 패턴 배열
        public MovePattern Move { get; set; }            // 이동 패턴
        public float ShootInterval { get; set; } = 2.0f; // 페이즈별 발사 간격 (기본 2.0f)
    }

    

    public static StageInfo[] All { get; } = new StageInfo[]
    {
        Stage1(),
        Stage2(),
        Stage3(),
        Stage4(),
        Stage5(),
    };


    private static StageInfo Stage1() => new StageInfo
    {
        WaitTime = 2f,
        Waves = new WaveData[]
        {
        new WaveData
        {
            EnemyCount = 2,
            EnemyType = EnemyType.EnemyA,
            WaveTime = 30f,
            EnemyHP = 3,
            ShootInterval = 3.0f,
            Patterns = new BulletPattern[] { new Spread3Pattern() },
            MoveFactory = () => new DescendAndStop(6f, 5f),
        },
        new WaveData // W2: SideToSide + NWayPattern(3)
        {
            EnemyCount = 3,
            EnemyType = EnemyType.EnemyA,
            WaveTime = 30f,
            EnemyHP = 4,
            ShootInterval = 2.5f,
            Patterns = new BulletPattern[] { new NWayPattern(3, 60f) },
            MoveFactory = () => new SideToSide(4f, 8f),
        },
        },
        BossWave = new WaveData
        {
            EnemyCount = 1,
            BossType = BossType.BossA,
            WaveTime = 60f,
            EnemyHP = 12,
            Phases = new BossPhase[]
            {
                new BossPhase { HpThreshold = 1.0f, ShootInterval = 2.5f,
                    Patterns = new BulletPattern[] { new AimedPattern() },
                    Move = new DescendAndStop(7f, 5f) },
                new BossPhase { HpThreshold = 0.6f, ShootInterval = 2.0f,
                    Patterns = new BulletPattern[] { new Spread5Pattern() },
                    Move = new SideToSide(5f, 10f) },
            }
        },
    };


    private static StageInfo Stage2() => new StageInfo
    {
        WaitTime = 2f,
        Waves = new WaveData[]
        {
        new WaveData // W1: ZigZag + AimedPattern
        {
            EnemyCount = 3,
            EnemyType = EnemyType.EnemyA,
            WaveTime = 30f,
            EnemyHP = 5,
            ShootInterval = 2.5f,
            Patterns = new BulletPattern[] { new AimedPattern() },
            MoveFactory = () => new ZigZag(6f, 4f, 14f, 1.2f),
        },
        new WaveData // W2: DescendThenSide + NWayAimedPattern(3)
        {
            EnemyCount = 4,
            EnemyType = EnemyType.EnemyB,
            WaveTime = 30f,
            EnemyHP = 5,
            ShootInterval = 2.0f,
            Patterns = new BulletPattern[] { new NWayAimedPattern(3, 30f) },
            MoveFactory = () => new DescendThenSide(6f, 6f, 4f, 10f),
        },
        },
        BossWave = new WaveData
        {
            EnemyCount = 1,
            BossType = BossType.BossA,
            WaveTime = 60f,
            EnemyHP = 18,
            Phases = new BossPhase[]
            {
                new BossPhase { HpThreshold = 1.0f, ShootInterval = 2.0f,
                    Patterns = new BulletPattern[] { new Cross4Pattern() },
                    Move = new DescendAndStop(7f, 8f) },
                new BossPhase { HpThreshold = 0.6f, ShootInterval = 1.8f,
                    Patterns = new BulletPattern[] { new SpreadAimedPattern() },
                    Move = new ZigZag(6f, 3f, 8f, 1f) },
                new BossPhase { HpThreshold = 0.3f, ShootInterval = 1.5f,
                    Patterns = new BulletPattern[] { new NWayAimedPattern(3, 30f), new NWayPattern(3, 60f) },
                    Move = new DescendThenSide(5f, 8f, 6f, 12f) },
            }
        },
    };


    private static StageInfo Stage3() => new StageInfo
    {
        WaitTime = 2f,
        Waves = new WaveData[]
        {
        new WaveData // W1: CircleMove  + Circle8Pattern
        {
            EnemyCount = 4,
            EnemyType = EnemyType.EnemyB,
            WaveTime = 30f,
            EnemyHP = 6,
            ShootInterval = 2.0f,
            Patterns = new BulletPattern[] { new Circle8Pattern() },
            MoveFactory = () => new CircleMove(30f, 8f, 8f, 1.5f),
        },
        new WaveData // W2: SideToSide + SpiralPattern —
        {
            EnemyCount = 3,
            EnemyType = EnemyType.EnemyB,
            WaveTime = 30f,
            EnemyHP = 6,
            ShootInterval = 0.12f,
            Patterns = new BulletPattern[] { new SpiralPattern(18f, 7f, 90f) },
            MoveFactory = () => new SideToSide(3f, 8f),
        },
        },
        BossWave = new WaveData
        {
            EnemyCount = 1,
            BossType = BossType.BossB,
            WaveTime = 60f,
            EnemyHP = 25,
            Phases = new BossPhase[]
            {
                new BossPhase { HpThreshold = 1.0f, ShootInterval = 2.0f,
                    Patterns = new BulletPattern[] { new Circle8Pattern() },
                    Move = new DescendAndStop(7f, 8f) },
                new BossPhase { HpThreshold = 0.6f, ShootInterval = 1.8f,
                    Patterns = new BulletPattern[] { new NWayAimedPattern(5, 45f) },
                    Move = new PendulumMove(8f, 14f) },
                new BossPhase { HpThreshold = 0.3f, ShootInterval = 0.13f, // ← 나선
                    Patterns = new BulletPattern[] { new SpiralPattern(20f, 7f, 90f) },
                    Move = new SideToSide(3.5f, 9f) }, // ← 나선: 느린 좌우
            }
        },
    };


    private static StageInfo Stage4() => new StageInfo
    {
        WaitTime = 2f,
        Waves = new WaveData[]
        {
        new WaveData // W1: Figure8Move + BurstAimedPattern  
        {
            EnemyCount = 4,
            EnemyType = EnemyType.EnemyC,
            WaveTime = 30f,
            EnemyHP = 8,
            ShootInterval = 0.50f, // ← 버스트: 0.22f × 3 = 0.66s 주기
            Patterns = new BulletPattern[] { new BurstAimedPattern(3, 8f) },
            MoveFactory = () => new Figure8Move(30f, 8f, 12f, 5f, 1.2f),
        },
        new WaveData // W2: PendulumMove  + DoubleSpiralPattern 
        {
            EnemyCount = 5,
            EnemyType = EnemyType.EnemyC,
            WaveTime = 30f,
            EnemyHP = 8,
            ShootInterval = 0.70f, // ← 이중나선: 빠른 발사
            Patterns = new BulletPattern[] { new DoubleSpiralPattern(15f, 7f) },
            MoveFactory = () => new PendulumMove(4.5f, 10f),
        },
        },
        BossWave = new WaveData
        {
            EnemyCount = 1,
            BossType = BossType.BossB,
            WaveTime = 60f,
            EnemyHP = 30,
            Phases = new BossPhase[]
            {
                new BossPhase { HpThreshold = 1.0f, ShootInterval = 2.0f,
                    Patterns = new BulletPattern[] { new NWayPattern(7, 80f) },
                    Move = new DescendAndStop(7f, 8f) },
                new BossPhase { HpThreshold = 0.6f, ShootInterval = 0.17f, // ← 이중나선+버스트
                    Patterns = new BulletPattern[] { new BurstAimedPattern(3, 8f), new DoubleSpiralPattern(15f, 7f) },
                    Move = new Figure8Move(30f, 8f, 12f, 5f, 1.5f) },
                new BossPhase { HpThreshold = 0.3f, ShootInterval = 1.5f,
                    Patterns = new BulletPattern[] { new RandomSpreadPattern(6, 8f), new NWayAimedPattern(3, 30f) },
                    Move = new WaveMove(3f, 1.8f, 10f, 4f) },
            }
        },
    };


    private static StageInfo Stage5() => new StageInfo
    {
        WaitTime = 2f,
        Waves = new WaveData[]
        {
        new WaveData // W1: SemiCircleMove + SpiralPattern 
        {
            EnemyCount = 5,
            EnemyType = EnemyType.EnemyC,
            WaveTime = 30f,
            EnemyHP = 10,
            ShootInterval = 0.50f, // ← 나선: 빠른 발사
            Patterns = new BulletPattern[] { new SpiralPattern(15f, 8f, 90f) },
            MoveFactory = () => new SemiCircleMove(30f, 8f, 12f, 1.5f), // ← 반원 이동
        },
        new WaveData // W2: WaveMove + DoubleSpiralPattern/RandomSpread 교대  
        {
            EnemyCount = 5,
            EnemyType = EnemyType.EnemyC,
            WaveTime = 30f,
            EnemyHP = 10,
            ShootInterval = 0.60f, // ← 이중나선: 빠른 발사
            Patterns = new BulletPattern[] { new DoubleSpiralPattern(12f, 7f), new RandomSpreadPattern(5, 8f) },
            MoveFactory = () => new WaveMove(3f, 1.8f, 10f, 4f),
            SpawnY = 3f,
        },
        },
        BossWave = new WaveData
        {
            EnemyCount = 1,
            BossType = BossType.BossC,
            WaveTime = 90f,
            EnemyHP = 40,
            Phases = new BossPhase[]
            {
                new BossPhase { HpThreshold = 1.0f, ShootInterval = 2.0f,
                    Patterns = new BulletPattern[] { new NWayPattern(7, 80f), new AimedPattern() },
                    Move = new DescendAndStop(7f, 4f) },
                new BossPhase { HpThreshold = 0.7f, ShootInterval = 1.8f,
                    Patterns = new BulletPattern[] { new Circle8Pattern(), new NWayAimedPattern(5, 40f) },
                    Move = new CircleMove(30f, 8f, 8f, 1.5f) },
                new BossPhase { HpThreshold = 0.4f, ShootInterval = 0.14f, // ← 이중나선
                    Patterns = new BulletPattern[] { new DoubleSpiralPattern(15f, 7f) },
                    Move = new SemiCircleMove(30f, 8f, 12f, 1.5f) },  // ← 느린 반원
                new BossPhase { HpThreshold = 0.15f, ShootInterval = 0.2f,  
                    Patterns = new BulletPattern[] { new BurstAimedPattern(3, 9f), new RandomSpreadPattern(6, 8f) },
                    Move = new Figure8Move(30f, 8f, 6f, 4f, 2.0f) },
            }
        },
    };

    //테스트 진행중 스테이지 구성========================================================================================================
}
     