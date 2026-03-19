using System;
using System.Threading;

namespace Framework.Engine
{
    public abstract class GameApp
    {
        private const int k_TargetFrameTime = 33; // 약 30 FPS (1000ms / 30 = 33.3ms)
        private bool _isRunning;

        protected ScreenBuffer Buffer { get; private set; }

        public event GameAction GameStarted;    // 게임 시작 이벤트
        public event GameAction GameStopped;    // 게임 종료 이벤트

        protected GameApp(int width, int height)// 생성자에서 화면 버퍼 초기화
        {
            Buffer = new ScreenBuffer(width, height);
        }

        public void Run()
        {
            Console.CursorVisible = false;              // 커서 숨기기
            Console.Clear();                            // 화면 초기화

            // 콘솔 창/버퍼 크기를 ScreenBuffer 크기에 맞게 설정
            if (OperatingSystem.IsWindows())
            {
                try
                {
                    // 버퍼 크기는 반드시 창 크기 이상이어야 함 (높이 +1: 스크롤바 방지)
                    Console.SetBufferSize(Buffer.Width, Buffer.Height + 1);
                    Console.SetWindowSize(Buffer.Width, Buffer.Height);
                }
                catch { /* 시스템 최대 크기 초과 시 무시 */ }
            }

            _isRunning = true;                          // 게임 루프 시작
            Initialize();                               // 게임 초기화
            GameStarted?.Invoke();                      // 게임 시작 이벤트 호출

            // [개선안] Environment.TickCount는 Windows에서 약 15ms 단위로 갱신되어 deltaTime 오차 발생
            //          Stopwatch는 나노초 단위로 더 정밀함
            // var _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            // long previousTime = _stopwatch.ElapsedMilliseconds;
            int previousTime = Environment.TickCount;   // 이전 프레임 시간 기록

            while (_isRunning)                                          // 게임 루프 
            {
                // [개선안]
                // long currentTime = _stopwatch.ElapsedMilliseconds;
                // float deltaTime = (currentTime - previousTime) / 1000f;
                // previousTime = currentTime;
                int currentTime = Environment.TickCount;                // 현재 프레임 시간 기록
                float deltaTime = (currentTime - previousTime) / 1000f; // 델타 타임 계산 (초 단위)
                previousTime = currentTime;                             // 입력 처리, 게임 업데이트, 화면 그리기

                Input.Poll();                                           //입력 처리
                Update(deltaTime);                                      // 게임 업데이트. deltaTime을 전달하여 프레임 독립적인 업데이트 수행
                Buffer.Clear();                                         // 화면 초기화
                Draw();                                                 // 화면 그리기
                Buffer.Present();                                       // 화면 출력
                // [개선안] int elapsed = (int)(_stopwatch.ElapsedMilliseconds - currentTime);
                int elapsed = Environment.TickCount - currentTime;      // 경과 시간 계산
                int sleepTime = k_TargetFrameTime - elapsed;            // 남은 시간 계산
                if (sleepTime > 0)                                      // 남은 시간이 양수인 경우에만 대기
                {
                    Thread.Sleep(sleepTime);                            //프레임 타임을 맞추기 위해 대기
                }
            }

            GameStopped?.Invoke();                                      // 게임 종료 이벤트 호출
            Console.CursorVisible = true;                               // 커서 표시
            Console.ResetColor();                                       // 콘솔 색상 초기화
            Console.Clear();                                            // 화면 초기화
        }

        protected void Quit()
        {
            _isRunning = false;
        }

        protected abstract void Initialize();// 게임 초기화 메서드. 게임 시작 시 필요한 리소스 로드, 초기 설정 등을 수행
        protected abstract void Update(float deltaTime);// 게임 업데이트 메서드. 매 프레임 호출되며, deltaTime을 사용하여 프레임 독립적인 업데이트 수행
        protected abstract void Draw();// 게임 그리기 메서드. 매 프레임 호출되며, 화면에 게임 요소를 그리는 작업 수행
    }
}
