
using System;
using System.Collections.Generic;
using System.Text;


namespace Framework.Engine
{ 
    public class Player : GameObject
    {
        //private const float k_moveIntervalX = 0.35f;             // 플레이어 이 이동하는 간격 (초)
        //private const float k_moveIntervalY = 0.45f;
        private const float SpeedX = 15.0f; // 1초에 가로로 15칸 이동
        private const float SpeedY = 10.0f;
        Bullet Bullet; // 총알 오브젝트 참조

        private float _moveTimerX;                                  // 이동 타이머
        private float _moveTimerY;

        private int _dirX;                                           // 이동 방향 X (-1, 0, 1)
        private int _dirY;                                           // 이동 방향 Y (-1, 0, 1)

        //private int _x;//
        //private int _y;// 플레이어의 현재 위치
        private float _shootTimer;                                  // 총알 발사 타이머
        private const float _shootInterval = 0.1f;                  // 총알 발사 간격 (초)

        public (int X, int Y) Pos => ((int)X, (int)Y);              // 플레이어의 현재 위치 반환

       

        //현재 플레이어의 위치 정보 표시 필드가 유사한게 2개 있음
        //하나로 통합하는걸 고려.



        public Player(Scene scene, int startX, int startY) : base(scene)
        {
            // 뱀 초기화 로직 (예: 초기 길이, 위치 설정)
            Name = "Player";
            X = startX;
            Y = startY;

        }
        public override void Draw(ScreenBuffer buffer)
        {
            buffer.SetCell((int)X, (int)Y, '@', ConsoleColor.Green);


            //throw new NotImplementedException();
        }

        public override void Update(float deltaTime)
        {
            HandleInput(); // 키 입력 처리
            Move(deltaTime);
            Shoot(deltaTime);
            //Bullet = new Bullet(Scene, _x, _y - 1);   // 총알 생성 (플레이어 바로 위)
            //Scene.AddGameObject(Bullet);              // 총알을 씬에 추가
           
            //throw new NotImplementedException();
        }


         //추가 메서드 목록======================================================================================
        public void Shoot(float deltaTime)
        {
            _shootTimer += deltaTime;                   // 총알 발사 타이머 업데이트
            if (_shootTimer >= _shootInterval)
            {
                _shootTimer = 0;                        // 총알 발사 타이머 초기화
                Bullet = new Bullet(Scene, X, Y - 1, 0, -1, "Player_Bullet"); // 총알 생성 (플레이어 바로 위)
                Scene.AddGameObject(Bullet);            // 총알을 씬에 추가
            }
        }
        public void Move(float deltaTime)
        {
            // 입력 방향(_dirX, _dirY) + 속도와 deltaTime을 곱해줌
             
            float nextX = X + (_dirX * SpeedX * deltaTime);
            float nextY = Y + (_dirY * SpeedY * deltaTime);

            // 3. 벽 충돌 검사 (float 좌표로 정밀하게 체크)
            // 캐릭터의 크기(Width/Height)가 있다면 여기에서 가감해줍니다.
            if (nextX >= Wall.Left && nextX <= Wall.Right)
            {
                X = nextX;
            }

            if (nextY >= Wall.Top && nextY <= Wall.Bottom)
            {
                Y = nextY;
            }
        }
        //public void Move(float deltaTime)
        //{
        //    if (_dirX == 0 && _dirY == 0)
        //    {
        //        _moveTimerX = 0;
        //        _moveTimerY = 0;
        //        return;
        //    }

        //    float nextX = X;
        //    float nextY = Y;

        //    if (_dirX != 0)
        //    {
        //        _moveTimerX += deltaTime;
        //        if (_moveTimerX >= k_moveIntervalX)
        //        {
        //            _moveTimerX = 0;
        //            nextX = X + _dirX;
        //        }
        //    }
        //    else _moveTimerX = 0;

        //    if (_dirY != 0)
        //    {
        //        _moveTimerY += deltaTime;
        //        if (_moveTimerY >= k_moveIntervalY)
        //        {
        //            _moveTimerY = 0;
        //            nextY = Y + _dirY;
        //        }
        //    }
        //    else _moveTimerY = 0;

        //    if (nextX >= Wall.Left && nextX <= Wall.Right &&
        //        nextY >= Wall.Top && nextY <= Wall.Bottom)
        //    {
        //        X = nextX;
        //        Y = nextY;
        //    }
        //}

        private void HandleInput()
        {
            _dirX = 0;
            _dirY = 0;
            if (Input.IsKey(ConsoleKey.UpArrow)) _dirY = -1;  // IsKeyDown → IsKey (지속 입력)
            if (Input.IsKey(ConsoleKey.DownArrow)) _dirY = 1;
            if (Input.IsKey(ConsoleKey.LeftArrow)) _dirX = -1;
            if (Input.IsKey(ConsoleKey.RightArrow)) _dirX = 1;



        }
    }
}