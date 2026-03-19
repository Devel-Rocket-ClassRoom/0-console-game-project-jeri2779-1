using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;


internal class Bullet : GameObject
{
    private int _direction;                                     // 총알 이동 방향 (1: 아래, -1: 위)

    //private float _y;                                           // Y 좌표 업데이트
    //private float _x;                                           // X 좌표

    private float _speed = 4f;                                 // 총알 속도

    private float _dirX, _dirY;                                 // 총알 이동 방향 벡터

   


    public Bullet(Scene scene, float x, float y, float dirX, float dirY, string name) : base(scene)
    {
        //Name = (direction == -1) ? "Player_Bullet" : "Enemy_Bullet";
        Name = name;
        X = x;                                                 // 총알의 초기 X 좌표  
        Y = y;                                                 // 총알의 초기 Y 좌표  
        _dirX = dirX;                                           // 총알의 이동 방향 벡터 X 
        _dirY = dirY;                                           // 총알의 이동 방향 벡터 Y  
        //_direction = direction;
    }


    public override void Update(float deltaTime)
    {
        X += (_speed * _dirX) * deltaTime;                                  // 총알이 X 방향으로 이동
        Y += (_speed * _dirY) * deltaTime;                                  // 총알이 Y 방향으로 이동

        if(Y < 0 || Y > Wall.Bottom + 2 || X < 0 || X > Wall.Right + 2)     // 총알이 화면 밖으로 나가면 제거
        {
            Scene.RemoveGameObject(this);                                         // 총알 제거
            return;
        }

        
        //
         
    }
                    // 총알과 플레이어 또는 적의 충돌 여부 확인
    



    public override void Draw(ScreenBuffer buffer)  
    {
        if (Name == "Player_Bullet")
        {
            buffer.SetCell((int)X, (int)Y, '*', ConsoleColor.Yellow);          // 플레이어 총알은 노란색으로 표시

        }
        else if (Name == "Enemy_Bullet")
        {
            buffer.SetCell((int)X, (int)Y, '.', ConsoleColor.White);             // 적 총알은 빨간색으로 표시

        }
    }
}

