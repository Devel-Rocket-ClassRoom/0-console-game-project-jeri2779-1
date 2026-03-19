using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;

//namespace Framework.Snake
//{
internal class Wall : GameObject
{
    public static int Left   { get; private set; } = 1; 
    public static int Right  { get; private set; } = 38;   
    public static int Top    { get; private set; } = 3;  
    public static int Bottom { get; private set; } = 17;    

   

    public Wall(Scene scene) : base(scene)
    {
        Name = "Wall";
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.DrawBox(Left - 1, Top - 1, Right - Left + 3, Bottom - Top + 3, ConsoleColor.White);
    }

    public override void Update(float deltaTime) { }

    public bool IsCollision(int x, int y)     => x < Left || x > Right || y < Top || y > Bottom;
    public bool IsCollision((float X, float Y) pos) => IsCollision((int)pos.X, (int)pos.Y);
    public bool IsCollision(Player player)      => IsCollision(player.Pos);
}
//}
