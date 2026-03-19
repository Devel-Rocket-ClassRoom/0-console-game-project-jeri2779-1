using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;


internal class Bullet : GameObject
{
     

    private float _speed = 15f;                                        // 총알 속도

    private float _dirX, _dirY;                                       // 총알 이동 방향 벡터

   


    public Bullet(Scene scene, float x, float y, 
                    float dirX, float dirY, float speed, string name) : base(scene)
    {
         
        Name = name;
        X = x;                                                          // 총알의 초기 X 좌표  
        Y = y;                                                          // 총알의 초기 Y 좌표  
        _dirX = dirX;                                                   // 총알의 이동 방향 벡터 X 
        _dirY = dirY;                                                   // 총알의 이동 방향 벡터 Y  
        _speed = speed;                                               // 총알의 속도 설정
    }


    public override void Update(float deltaTime)
    {
        X += (_speed * _dirX) * deltaTime;                                  // 총알이 X 방향으로 이동
        Y += (_speed * _dirY) * deltaTime;                                  // 총알이 Y 방향으로 이동

        if (X < Wall.Left || X > Wall.Right ||                              //wall의 왼쪽과 오른쪽 경계를 벗어나면 제거
            Y < Wall.Top || Y > Wall.Bottom)                                //wall의 위쪽과 아래쪽 경계를 벗어나면 제거
        {
            Scene.RemoveGameObject(this);
            return;
        }


        

    }
                     
    



    public override void Draw(ScreenBuffer buffer)  
    {

        if(X < Wall.Left || X > Wall.Right ||                                  //wall의 왼쪽과 오른쪽 경계를 벗어나면 그리지 않음
           Y < Wall.Top || Y > Wall.Bottom)                                    //wall의 위쪽과 아래쪽 경계를 벗어나면 그리지 않음
        {
            return;
        }
        if (Name == "Player_Bullet")
        {
            buffer.SetCell((int)X, (int)Y, '*', ConsoleColor.Yellow);          // 플레이어 총알은 노란색으로 표시

        }
        else if (Name == "Enemy_Bullet")
        {
            buffer.SetCell((int)X, (int)Y, '.', ConsoleColor.White);             // 적 총알은 흰색 표시

        }
    }
}

