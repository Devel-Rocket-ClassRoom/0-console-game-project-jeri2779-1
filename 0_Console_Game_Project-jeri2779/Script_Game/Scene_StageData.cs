using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;
using static StageData;

internal class StageData
{
    internal class WaveData// 웨이브 정보 클래스
    {
        public int EnemyCount { get; set; }             // 웨이브당 적의 수
        public float WaveTime { get; set; }             // 웨이브 지속 시간

        public int EnemyHP { get; set; }                 // 적의 체력   
        public float ShootInterval { get; set; }        // 적이 총알을 발사하는 간격
        //public Action<Scene, float, float>[] Patterns;  // 적의 총알 발사 패턴을 정의
        public BulletPattern[] Patterns { get; set; }  // 적의 총알 발사 패턴 배열
        public BossPhase[] Phases { get; set; }              // 보스 웨이브에서 각 패턴이 적용되는 페이즈
        public MovePattern Move { get; set; }          // 이동 방식
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
        //public Action<Scene, float, float> Patterns;  // 해당 페이즈에서 적용되는 총알 패턴
        public BulletPattern[] Patterns { get; set; }  // 해당 페이즈에서 적용되는 총알 패턴 배열
        public MovePattern Move { get; set; }          // 이동 (추가)
    }

    public static StageInfo[] All { get; } = new StageInfo[] // 모든 스테이지 정보
    {
        Stage1(),
        Stage2(),
        Stage3(),
    };

    private static StageInfo Stage1() => new StageInfo      //밑의 StageInfo 정보 생성
    {
        // 스테이지 1 
        WaitTime = 2f,
        Waves = new WaveData[]
        {
            new WaveData
            {
                EnemyCount = 2,
                WaveTime = 30f,
                EnemyHP = 3,
                //Patterns = new Action<Scene, float, float>[] { BPatterns.Spread3 },
                Patterns = new BulletPattern[] { new Spread3Pattern() },
                Move = new DescendAndStop(8.0f, 5.0f)
            },
            new WaveData
            {
                EnemyCount = 3,
                WaveTime = 30f,
                EnemyHP = 4,
                //Patterns = new Action<Scene, float, float>[] { BPatterns.SpreadAimedAuto },
                Patterns = new BulletPattern[] { new Spread5Pattern() },

                Move = new SideToSide(5.0f)
            },
        },
        BossWave = new WaveData
        {
            EnemyCount = 1,
            WaveTime = 60f,
            EnemyHP = 10,
            Phases = new BossPhase[]
             
            {
                new BossPhase { HpThreshold = 1.0f, Patterns = new BulletPattern[] { new Spread3Pattern() }, Move = new DescendAndStop(7.0f, 5.0f) },
                new BossPhase { HpThreshold = 0.7f, Patterns = new BulletPattern[] { new Circle8Pattern() }, Move = new SideToSide(5.0f) },
                new BossPhase { HpThreshold = 0.4f, Patterns = new BulletPattern[] { new Spread5Pattern() }, Move = new SideToSide(5.0f) },
            }
        },
    };

    private static StageInfo Stage2() => new StageInfo
    {
        // 스테이지 2
        WaitTime = 2f,
        Waves = new WaveData[]
        {
            new WaveData
            {
                EnemyCount = 4,
                WaveTime = 30f,
                EnemyHP = 5,
                Patterns = new BulletPattern[] { new Spread3Pattern() }
            },
            new WaveData
            {
                EnemyCount = 5,
                WaveTime = 30f,
                EnemyHP = 6,
                Patterns = new BulletPattern[] { new Spread5Pattern(), new Circle8Pattern()  },
                Move = new SideToSide(5.0f)
            },
        },
        BossWave = new WaveData
        {
            EnemyCount = 1,
            WaveTime = 60f,
            EnemyHP = 15,
            Phases = new BossPhase[]
            {
                new BossPhase { HpThreshold = 1.0f, Patterns = new BulletPattern[] { new Spread5Pattern() }, Move = new DescendAndStop(7.0f, 3.0f) },
                new BossPhase { HpThreshold = 0.7f, Patterns = new BulletPattern[] { new Circle8Pattern() }, Move = new SideToSide(5.0f) },
                new BossPhase { HpThreshold = 0.4f, Patterns = new BulletPattern[] { new Spread3Pattern() }, Move = new SideToSide(5.0f) },
            }
        },
    };



    private static StageInfo Stage3() => new StageInfo
    {
        WaitTime = 2f,
        Waves = new WaveData[]
    {
        // 웨이브 1 - ZigZag 이동 + Spread3
        new WaveData
        {
            EnemyCount = 3,
            WaveTime = 30f,
            EnemyHP = 5,
            Patterns = new BulletPattern[] { new Spread3Pattern() },
            Move = new ZigZag(8.0f, 4.0f, 5.0f)
        },
        // 웨이브 2 - SideToSide + Spiral (stateful 테스트)
        new WaveData
        {
            EnemyCount = 3,
            WaveTime = 30f,
            EnemyHP = 5,
            Patterns = new BulletPattern[] { new SpiralPattern(30f, 6f) },
            Move = new SideToSide(5.0f)
        },
    },
        BossWave = new WaveData
        {
            EnemyCount = 1,
            WaveTime = 60f,
            EnemyHP = 20,
            Phases = new BossPhase[]
        {
            // 페이즈 1 - DescendAndStop + Spread5 + AimedPattern
            new BossPhase
            {
                HpThreshold = 1.0f,
                Patterns = new BulletPattern[] { new Spread5Pattern(), new AimedPattern() },
                Move = new DescendAndStop(7.0f, 4.0f)
            },
            // 페이즈 2 - SideToSide + Circle8 + SpreadAimed
            new BossPhase
            {
                HpThreshold = 0.7f,
                Patterns = new BulletPattern[] { new Circle8Pattern(), new SpreadAimedPattern() },
                Move = new SideToSide(6.0f)
            },
            // 페이즈 3 - SideToSide + Spiral (stateful 보스 테스트)
            new BossPhase
            {
                HpThreshold = 0.4f,
                Patterns = new BulletPattern[] { new SpiralPattern(20f, 8f) },
                Move = new SideToSide(8.0f)
            },
        }
        },
    };


}
