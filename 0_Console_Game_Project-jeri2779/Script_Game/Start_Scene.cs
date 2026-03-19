 
using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;


public class Start : Scene
{
    public event GameAction OnStartGame; // 게임 시작 이벤트
    public override void Draw(ScreenBuffer buffer)
    {
        // 타이틀 화면 그리기 로직
        buffer.WriteTextCentered(6, " Shooting", ConsoleColor.Yellow);
         

        //throw new NotImplementedException();
    }

    public override void Load()
    {
        //throw new NotImplementedException();
    }

    public override void Unload()
    {
        //throw new NotImplementedException();
    }

    public override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            OnStartGame?.Invoke(); // 게임 시작 이벤트 호출
        }

        //throw new NotImplementedException();
    }
}
