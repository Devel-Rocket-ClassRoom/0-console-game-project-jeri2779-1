using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;


internal class BPatterns
{
    //Bullet 매개변수 정리 
    //
    // scene: 총알이 생성될 씬
    // x, y: 총알이 생성될 위치
    // dirX, dirY: 총알의 이동 방향  
    // speed: 총알의 이동 속도
    // Bullet 카테고리 : 문자열로 구분 추후 변경 고려

    public static void Spread3(Scene scene, float x, float y)                               // 3방향 확산
    {
        scene.AddGameObject(new Bullet(scene, x, y + 1, -0.5f, 1, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y + 1, 0f, 1, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y + 1, 0.5f, 1, 6f, "Enemy_Bullet"));
    }
    //{
    //float[] dirs = { -0.5f, 0f, 0.5f };
    //foreach (var dir in dirs)
    //    scene.AddGameObject(new Bullet(scene, x, y + 1, dir, 1, 6f, "Enemy_Bullet"));
    //}


// 5방향 확산
public static void Spread5(Scene scene, float x, float y)                               // 5방향 확산
    {
        scene.AddGameObject(new Bullet(scene, x, y + 1, -1f, 1, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y + 1, -0.5f, 1, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y + 1, 0f, 1, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y + 1, 0.5f, 1, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y + 1, 1f, 1, 6f, "Enemy_Bullet"));
    }
    /*
     *{
    float[] dirs = { -1f, -0.5f, 0f, 0.5f, 1f };
    foreach (var dir in dirs)
        scene.AddGameObject(new Bullet(scene, x, y + 1, dir, 1, 6f, "Enemy_Bullet"));
} 
     */

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

    //유도 패턴의 경우 Action의 매개변수가 달라서 그대로 대입하면 전용 시그니처 필요할수 있음
    // Aimed 메서드는 플레이어 위치를 매개변수로 받아서 그 방향으로 총알을 발사하는 형태로 구현,
    // Aimed 메서드는 기존 시그니처 유지, AimedAuto 메서드에서 플레이어 좌표를 자동으로 받아와서 Aimed 메서드에 전달하는 형태로 구현.
    //실제 사용은 Auto붙은 메서드를 사용해야 에러가 나지 않음.
    public static void Aimed(Scene scene, float x, float y, float playerX, float playerY)     // 플레이어 위치를 향해 발사
    {
        float diffX = playerX - x;
        float diffY = playerY - y;
        float distance = (float)Math.Sqrt(diffX * diffX + diffY * diffY);                   // 거리 계산
        if (distance == 0) return;
        scene.AddGameObject(new Bullet(scene, x, y, diffX / distance, diffY / distance, 6f, "Enemy_Bullet"));
    }
    public static void  AimedAuto(Scene scene,float x, float y)// 플레이어 위치를 자동으로 찾아서 발사
    {
        var player = scene.FindGameObject("Player");
        float playerX = player != null ? player.X : x;
        float playerY = player != null ? player.Y : y;
        Aimed(scene, x, y, playerX, playerY); 
    }

    public static void SpreadAimed(Scene scene, float x, float y, float playerX, float playerY) // 플레이어 위치를 향해 3방향 확산 발사
    {
        float diffX = playerX - x;
        float diffY = playerY - y;
        float distance = (float)Math.Sqrt(diffX * diffX + diffY * diffY);
        if (distance == 0) return;
        float aimX = diffX / distance;
        float aimY = diffY / distance;
        // 플레이어 방향 기준으로 좌우 확산
        scene.AddGameObject(new Bullet(scene, x, y, aimX - 0.3f, aimY, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y, aimX, aimY, 6f, "Enemy_Bullet"));
        scene.AddGameObject(new Bullet(scene, x, y, aimX + 0.3f, aimY, 6f, "Enemy_Bullet"));
    }
    public static void SpreadAimedAuto(Scene scene, float x, float y)
    {
        var player = scene.FindGameObject("Player");
        float playerX = player != null ? player.X : x;
        float playerY = player != null ? player.Y : y;
        SpreadAimed(scene, x, y, playerX, playerY);
    }


}

//총알의 패턴 생성 로직을 더 N-way식으로 더 식을 통일화개선 고려
//N에 숫자를 입력해서 3,5,8way 등 다양한 패턴을 만들 수 있도록 개선 고려




