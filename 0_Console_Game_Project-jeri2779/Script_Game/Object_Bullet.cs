using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;    

namespace _0_Console_Game_Project_jeri2779.Script_Game
{
    internal class Bullet : GameObject
    {
        private float _y;                                      // Y 좌표 업데이트
        private int _direction;                                     // 총알 이동 방향 (1: 아래, -1: 위)

        private float _x;                                           // X 좌표
        //public (int X, int Y) _bulletPos;
        private float _speed = 10f;                                 // 총알 속도

        public Bullet(Scene scene, float x, float y, int direction) : base(scene)
        {
            Name = "Player_Bullet";
            _x = x;
            _y = y;
            _direction = direction;
        }

        //public override void Update(float deltaTime)
        //{
        //    _y = (_speed * _direction) * deltaTime; // 총알이 이동할 거리 계산

        //}
        public override void Update(float deltaTime)
        {
            _y -= _speed * deltaTime;                                  // 총알이 위로 이동
            if (_y < 0 || _y > Wall.Bottom + 5)
            {
                Scene.RemoveGameObject(this);                               // 총알 제거
            }
        }

        public override void Draw(ScreenBuffer buffer)
        {
            buffer.SetCell((int)_x, (int)_y, '*', ConsoleColor.Yellow); // 총알을 '*' 문자로 그리기
        }
    }
}
