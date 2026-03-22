 
using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;


public class Start : Scene
{
    public event GameAction OnStartGame; // 게임 시작 이벤트
    public override void Draw(ScreenBuffer buffer)
    {
        // 상단 별 장식
        buffer.WriteTextCentered(1, "* . * . * . * . * . * . * . * . * . * . * . *", ConsoleColor.DarkYellow);

        // 타이틀 박스 (x=10, y=3, w=40, h=7)
        buffer.DrawBox(10, 3, 40, 7, ConsoleColor.Cyan);
        buffer.WriteTextCentered(5, "<< ShootingX >>", ConsoleColor.Yellow);
        buffer.WriteTextCentered(7, "- Space Shooter -", ConsoleColor.DarkCyan);

        // 우주선 아스키아트 (6칸 고정 폭, 가운데 정렬)
        int shipX = buffer.Width / 2 - 3;
        buffer.WriteText(shipX, 11, @"  /\  ", ConsoleColor.White);
        buffer.WriteText(shipX, 12, @" /  \ ", ConsoleColor.White);
        buffer.WriteText(shipX, 13, @"/====\", ConsoleColor.Cyan);
        buffer.WriteText(shipX, 14, @"  ||  ", ConsoleColor.DarkCyan);

        // 구분선
        buffer.WriteTextCentered(16, new string('=', 44), ConsoleColor.DarkGray);

        // 시작 안내
        buffer.WriteTextCentered(18, ">>> PRESS [ ENTER ] TO START <<<", ConsoleColor.Green);

        // 구분선
        buffer.WriteTextCentered(20, new string('=', 44), ConsoleColor.DarkGray);

        // 조작키 안내
        buffer.WriteTextCentered(22, "[ Space ] : SLOW", ConsoleColor.Gray);

        // 하단 별 장식
        buffer.WriteTextCentered(28, "* . * . * . * . * . * . * . * . * . * . * . *", ConsoleColor.DarkYellow);
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
