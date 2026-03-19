
using System;
using System.Collections.Generic;
using System.Text;


namespace Framework.Engine
{ 
    public class Player : GameObject
    {
        private const float k_moveInterval = 0.15f;             // 뱀이 이동하는 간격 (초)
        Bullet Bullet; // 총알 오브젝트 참조

        private float _moveTimer;                                  // 이동 타이머
        private int _dirX;                                           // 이동 방향 X (-1, 0, 1)
        private int _dirY;                                           // 이동 방향 Y (-1, 0, 1)

        private int _x;//
        private int _y;// 플레이어의 현재 위치
        private float _shootTimer;                                  // 총알 발사 타이머
        private const float _shootInterval = 0.5f;                  // 총알 발사 간격 (초)

        public (int X, int Y) Pos => (_x, _y);// 플레이어의 현재 위치 반환
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
            //Bullet = new Bullet(Scene, _x, _y - 1);   // 총알 생성 (플레이어 바로 위)
            //Scene.AddGameObject(Bullet);              // 총알을 씬에 추가
            _shootTimer += deltaTime;                   // 총알 발사 타이머 업데이트
            if (_shootTimer >= _shootInterval)
            {
                _shootTimer = 0;                        // 총알 발사 타이머 초기화
                Bullet = new Bullet(Scene, _x, _y - 1 , 0,  -1, "Player_Bullet"); // 총알 생성 (플레이어 바로 위)
                Scene.AddGameObject(Bullet);            // 총알을 씬에 추가
            }
            //throw new NotImplementedException();
        }
        public void Move(float deltaTime)
        {
            //키입력시 플레이어의 위치를 업데이트
            if(_dirX == 0 && _dirY == 0) 
            {
                _moveTimer = 0;                          // 이동 방향이 없으면 이동 타이머 초기화
                return;                                  // 이동 방향이 없으면 이동하지 않음
            }
            _moveTimer += deltaTime;                     // 이동 타이머 업데이트

            if(_moveTimer < k_moveInterval) return;      // 이동 간격이 되지 않았으면 이동하지 않음

            _moveTimer = k_moveInterval;                 // 이동 타이머 초기화

            int nextX = _x + _dirX;                      // 다음 위치 계산
            int nextY = _y + _dirY;

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