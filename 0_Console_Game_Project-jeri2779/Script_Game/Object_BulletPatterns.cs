using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;


internal class BPatterns
{
    public static void Spread3(Scene scene, float x, float y)                               // 3방향 확산
    {
        scene.AddGameObject(new Bullet(scene, x, y + 1, -0.5f, 1, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y + 1, 0f, 1, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y + 1, 0.5f, 1, 6f, "Enemy_Bullet"));
    }

    // 5방향 확산
    public static void Spread5(Scene scene, float x, float y)                               // 5방향 확산
    {
        scene.AddGameObject(new Bullet(scene, x, y + 1, -1f, 1, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y + 1, -0.5f, 1, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y + 1, 0f, 1, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y + 1, 0.5f, 1, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y + 1, 1f, 1, 6f, "Enemy_Bullet"));
    }

    // 360도 원형 (8방향)
    public static void Circle8(Scene scene, float x, float y)                               // 360도 원형 (8방향)
    {
        for (int i = 0; i < 8; i++)
        {
            double angle = i * (Math.PI * 2 / 8);
            float dirX = (float)Math.Cos(angle);
            float dirY = (float)Math.Sin(angle);
            scene.AddGameObject(new Bullet(scene, x, y, dirX, dirY, 6f, "Enemy_Bullet"));
        }
    }
}



