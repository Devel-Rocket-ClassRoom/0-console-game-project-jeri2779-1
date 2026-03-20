using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;

internal class StageData
{
    internal  class StageInfo// 스테이지 정보 클래스
    {
        public WaveData[] Waves { get; set; }            // 일반 웨이브 정보 배열
        public WaveData BossWave { get; set; }           // 보스 웨이브 정보
        public float WaitTme { get; set; }               // 스테이지 시작 전 대기 시간
    }
    internal class WaveData// 웨이브 정보 클래스
    {
        public int EnemyCount { get; set; }             // 웨이브당 적의 수
        public float WaveTime { get; set; }             // 웨이브 지속 시간
        public float ShootInterval { get; set; }        // 적이 총알을 발사하는 간격
        public Action<Scene, float, float>[] Patterns;  // 적의 총알 발사 패턴을 정의
    }



}


