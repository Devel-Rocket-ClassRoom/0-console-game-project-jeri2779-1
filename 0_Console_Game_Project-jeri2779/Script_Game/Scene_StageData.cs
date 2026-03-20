using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;

internal class StageData
{
    internal class WaveData// 웨이브 정보 클래스
    {
        public int EnemyCount { get; set; }             // 웨이브당 적의 수
        public float WaveTime { get; set; }             // 웨이브 지속 시간
        public float ShootInterval { get; set; }        // 적이 총알을 발사하는 간격
        public Action<Scene, float, float>[] Patterns;  // 적의 총알 발사 패턴을 정의
    }
    internal class StageInfo// 스테이지 정보 클래스
    {
        public WaveData[] Waves { get; set; }            // 일반 웨이브 정보 배열
        public WaveData BossWave { get; set; }           // 보스 웨이브 정보
        public float WaitTime { get; set; }               // 스테이지 시작 전 대기 시간
    }

    public static StageInfo[] All { get; } = new StageInfo[] // 모든 스테이지 정보
    {
        Stage1(),
        Stage2(),
    };

    private static StageInfo Stage1() => new StageInfo      //밑의 StageInfo 정보 생성
    {
        // 스테이지 1 
        WaitTime = 2f,
        Waves = new WaveData[]
        {
            new WaveData { EnemyCount = 2, WaveTime = 30f,
                Patterns = new Action<Scene, float, float>[] { BPatterns.Spread3 } },
            new WaveData { EnemyCount = 3, WaveTime = 30f,
                Patterns = new Action<Scene, float, float>[] { BPatterns.Spread5 } },
        },
        BossWave = new WaveData
        {
            EnemyCount = 1,
            WaveTime = 60f,
            Patterns = new Action<Scene, float, float>[] { BPatterns.Circle8 }
        },
    };

    private static StageInfo Stage2() => new StageInfo
    {
        // 스테이지 2
        WaitTime = 2f,
        Waves = new WaveData[]
        {
            new WaveData { EnemyCount = 4, WaveTime = 30f,
                Patterns = new Action<Scene, float, float>[] { BPatterns.Spread3 } },
            new WaveData { EnemyCount = 5, WaveTime = 30f,
                Patterns = new Action<Scene, float, float>[] { BPatterns.Spread5, BPatterns.Circle8 } },
        },
        BossWave = new WaveData
        {
            EnemyCount = 1,
            WaveTime = 60f,
            Patterns = new Action<Scene, float, float>[] { BPatterns.Circle8,BPatterns.Spread5, BPatterns.Spread3 }
        },
    };



}


