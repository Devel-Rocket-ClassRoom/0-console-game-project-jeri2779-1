using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;


class Boss : GameObject
{
    public Boss(Scene scene, float startX, float startY) : base(scene)
    {
        Name = "Boss";
        X = startX;
        Y = startY;                     // 보스의 초기 위치 설정
    }
    public override void Draw(ScreenBuffer buffer)
    {
        buffer.SetCell((int)X, (int)Y, 'B', ConsoleColor.Magenta);
    }
    public override void Update(float deltaTime)
    {
        // 보스의 행동 로직 구현 (예: 이동, 공격 패턴 등)
    }
}