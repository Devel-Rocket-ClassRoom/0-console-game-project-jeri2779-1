
using System;
using System.Collections.Generic;
using System.Text;


namespace Framework.Engine
{ 
    public class Player : GameObject
    {
        private const float k_moveIntervalX = 0.15f;             // 플레이어 이 이동하는 간격 (초)
        private const float k_moveIntervalY = 0.25f;
        Bullet Bullet; // 총알 오브젝트 참조

        private float _moveTimerX;                                  // 이동 타이머
        private float _moveTimerY;

        private int _dirX;                                           // 이동 방향 X (-1, 0, 1)
        private int _dirY;                                           // 이동 방향 Y (-1, 0, 1)

        private int _x;//
        private int _y;// 플레이어의 현재 위치
        private float _shootTimer;                                  // 총알 발사 타이머
        private const float _shootInterval = 0.8f;                  // 총알 발사 간격 (초)

        public (int X, int Y) Pos => (_x, _y);// 플레이어의 현재 위치 반환

        public float X { get; internal set; }
        public float Y { get; internal set; }

        //현재 플레이어의 위치 정보 표시 필드가 유사한게 2개 있음
        //하나로 통합하는걸 고려.



        public Player(Scene scene, int startX, int startY) : base(scene)
        {
            // 뱀 초기화 로직 (예: 초기 길이, 위치 설정)
            Name = "Player";
            _x = startX;
            _y = startY;

        }
        public override void Draw(ScreenBuffer buffer)
        {
            buffer.SetCell(_x, _y, '@', ConsoleColor.Green);


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
                Bullet = new Bullet(Scene, _x, _y - 1, 0, -1, "Player_Bullet"); // 총알 생성 (플레이어 바로 위)
                Scene.AddGameObject(Bullet);            // 총알을 씬에 추가
            }
        }
        public void Move(float deltaTime)
        {
            if (_dirX == 0 && _dirY == 0)
            {
                _moveTimerX = 0;
                _moveTimerY = 0;
                return;
            }

            int nextX = _x;
            int nextY = _y;

            if (_dirX != 0)
            {
                _moveTimerX += deltaTime;
                if (_moveTimerX >= k_moveIntervalX)
                {
                    _moveTimerX = 0;
                    nextX = _x + _dirX;
                }
            }
            else _moveTimerX = 0;

            if (_dirY != 0)
            {
                _moveTimerY += deltaTime;
                if (_moveTimerY >= k_moveIntervalY)
                {
                    _moveTimerY = 0;
                    nextY = _y + _dirY;
                }
            }
            else _moveTimerY = 0;

            if (nextX >= Wall.Left && nextX <= Wall.Right &&
                nextY >= Wall.Top && nextY <= Wall.Bottom)
            {
                _x = nextX;
                _y = nextY;
            }
        }

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