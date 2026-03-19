using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;

//namespace Framework.Snake
//{
internal class Wall : GameObject
{
    public static int Left   { get; private set; }  
    public static int Right  { get; private set; }  
    public static int Top    { get; private set; }     
    public static int Bottom { get; private set; }

   

    public Wall(Scene scene, int width, int height) : base(scene)
    {
        Name = "Wall";
        Left = 1;
        Top = 3;
        Right = width - 2;//
        Bottom = height - 2;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.DrawBox(Left - 1, Top - 1, Right - Left + 3, Bottom - Top + 3, ConsoleColor.White);
    }

    public override void Update(float deltaTime) { }

    //public bool IsCollision(int x, int y)     => x < Left || x > Right || y < Top || y > Bottom;
    //public bool IsCollision((float X, float Y) pos) => IsCollision((int)pos.X, (int)pos.Y);
    //public bool IsCollision(Player player)      => IsCollision(player.Pos);
}
//}
