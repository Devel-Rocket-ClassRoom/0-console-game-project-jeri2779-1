using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;



// ── BulletPattern 추상 클래스 + 하위 클래스 ──────────────────
internal abstract class BulletPattern
{
    public abstract void Fire(Scene scene, float x, float y);
}






internal class Spread3Pattern : BulletPattern
{
    public override void Fire(Scene scene, float x, float y)
        => BPatterns.Spread3(scene, x, y);
}

internal class Spread5Pattern : BulletPattern
{
    public override void Fire(Scene scene, float x, float y)
        => BPatterns.Spread5(scene, x, y);
}

internal class Cross4Pattern : BulletPattern
{
    public override void Fire(Scene scene, float x, float y)
        => BPatterns.Cross4(scene, x, y);
}

internal class Circle8Pattern : BulletPattern
{
    public override void Fire(Scene scene, float x, float y)
        => BPatterns.Circle8(scene, x, y);
}

internal class AimedPattern : BulletPattern
{
    public override void Fire(Scene scene, float x, float y)
        => BPatterns.AimedAuto(scene, x, y);
}

internal class SpreadAimedPattern : BulletPattern
{
    public override void Fire(Scene scene, float x, float y)
        => BPatterns.SpreadAimedAuto(scene, x, y);
}
//stateful 패턴 정리===================================================================================
internal class SpiralPattern : BulletPattern
{
    private float _angle = 0f;
    private readonly float _angleStep;
    private readonly float _speed;

    public SpiralPattern(float angleStep = 30f, float speed = 6f, float startAngle = 30f)
    {
        _angleStep = angleStep;
        _speed = speed;
        _angle = startAngle;
    }

    public override void Fire(Scene scene, float x, float y)
    {
        float rad = _angle * (MathF.PI / 180f);
        float dirX = MathF.Cos(rad);
        float dirY = MathF.Sin(rad);
        scene.AddGameObject(new Bullet(scene, x, y, dirX, dirY, _speed, "Enemy_Bullet"));
        _angle += _angleStep;
        if (_angle >= 360f) _angle -= 360f;
    }
}
internal class DoubleSpiralPattern : BulletPattern
{
    private float _angle = 0f;
    private readonly float _angleStep;
    private readonly float _speed;

    public DoubleSpiralPattern(float angleStep = 25f, float speed = 6f)
    {
        _angleStep = angleStep;
        _speed = speed;
    }

    public override void Fire(Scene scene, float x, float y)
    {
        for (int i = 0; i < 2; i++)
        {
            float rad = (_angle + i * 180f) * (MathF.PI / 180f);
            scene.AddGameObject(new Bullet(scene, x, y,
                MathF.Cos(rad), MathF.Sin(rad), _speed, "Enemy_Bullet"));
        }
        _angle = (_angle + _angleStep) % 360f;
    }
}

internal class NWayPattern : BulletPattern
{
    private readonly int _count;
    private readonly float _spreadAngle;  
    private readonly float _speed;

    public NWayPattern(int count, float spreadAngle = 60f, float speed = 6f)
    {
        _count = count;
        _spreadAngle = spreadAngle;
        _speed = speed;
    }

    public override void Fire(Scene scene, float x, float y)
    {
        float startAngle = 90f - _spreadAngle / 2f;  
        float step = _count > 1 ? _spreadAngle / (_count - 1) : 0f;
        for (int i = 0; i < _count; i++)
        {
            float rad = (startAngle + step * i) * (MathF.PI / 180f);
            scene.AddGameObject(new Bullet(scene, x, y,
                MathF.Cos(rad), MathF.Sin(rad), _speed, "Enemy_Bullet"));
        }
    }
}

internal class NWayAimedPattern : BulletPattern
{
    private readonly int _count;
    private readonly float _spreadAngle;
    private readonly float _speed;

    public NWayAimedPattern(int count, float spreadAngle = 30f, float speed = 6f)
    {
        _count = count;
        _spreadAngle = spreadAngle;
        _speed = speed;
    }

    public override void Fire(Scene scene, float x, float y)
    {
        var player = scene.FindGameObject("Player");
        float playerX = player != null ? player.X : x;
        float playerY = player != null ? player.Y : y;

        float diffX = playerX - x;
        float diffY = playerY - y;
        float distance = MathF.Sqrt(diffX * diffX + diffY * diffY);
        if (distance == 0) return;

        
        float baseAngle = MathF.Atan2(diffY, diffX);  
        float spread = _spreadAngle * (MathF.PI / 180f);
        float step = _count > 1 ? spread / (_count - 1) : 0f;
        float startAngle = baseAngle - spread / 2f;

        for (int i = 0; i < _count; i++)
        {
            float angle = startAngle + step * i;
            float dirX = MathF.Cos(angle);
            float dirY = MathF.Sin(angle);
            scene.AddGameObject(new Bullet(scene, x, y, dirX, dirY, _speed, "Enemy_Bullet"));
        }
    }
}
internal class BurstAimedPattern : BulletPattern
{
    private readonly int _burstCount;
    private readonly float _speed;
    private int _shotsFired = 0;

    public BurstAimedPattern(int burstCount = 3, float speed = 8f)
    {
        _burstCount = burstCount;
        _speed = speed;
    }

    public override void Fire(Scene scene, float x, float y)
    {
        if (_shotsFired >= _burstCount)
        {
            _shotsFired = 0; // 버스트 리셋
            return;
        }

        var player = scene.FindGameObject("Player");
        if (player == null || !player.IsActive) return;

        float diffX = player.X - x;
        float diffY = player.Y - y;
        float dist = MathF.Sqrt(diffX * diffX + diffY * diffY);
        if (dist == 0) return;

        scene.AddGameObject(new Bullet(scene, x, y,
            diffX / dist, diffY / dist, _speed, "Enemy_Bullet"));
        _shotsFired++;
    }
}

internal class RandomSpreadPattern : BulletPattern
{
    private readonly int _count;
    private readonly float _speed;
    private readonly Random _rng = new Random();

    public RandomSpreadPattern(int count = 5, float speed = 7f)
    {
        _count = count;
        _speed = speed;
    }

    public override void Fire(Scene scene, float x, float y)
    {
        for (int i = 0; i < _count; i++)
        {
            float angle = (float)(_rng.NextDouble() * MathF.PI * 2f);
            scene.AddGameObject(new Bullet(scene, x, y,
                MathF.Cos(angle), MathF.Sin(angle), _speed, "Enemy_Bullet"));
        }
    }
}
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

    public static void Cross4(Scene scene, float x, float y)
    {
        scene.AddGameObject(new Bullet(scene, x, y, 0f, -1f, 6f, "Enemy_Bullet")); // 위
        scene.AddGameObject(new Bullet(scene, x, y, 0f, 1f, 6f, "Enemy_Bullet")); // 아래
        scene.AddGameObject(new Bullet(scene, x, y, -1f, 0f, 6f, "Enemy_Bullet")); // 왼쪽
        scene.AddGameObject(new Bullet(scene, x, y, 1f, 0f, 6f, "Enemy_Bullet")); // 오른쪽
    }

    //유도 패턴의 경우 Action의 매개변수가 달라서 그대로 대입하면 전용 시그니처 필요할수 있음
    // Aimed 메서드는 플레이어 위치를 매개변수로 받아서 그 방향으로 총알을 발사하는 형태로 구현,
    // Aimed 메서드는 기존 시그니처 유지, AimedAuto 메서드에서 플레이어 좌표를 자동으로 받아와서 Aimed 메서드에 전달하는 형태로 구현.
    //실제 사용은 Auto붙은 메서드를 사용해야 에러가 나지 않음.
    public static void Aimed(Scene scene, float x, float y, float playerX, float playerY)    
    {
        float diffX = playerX - x;
        float diffY = playerY - y;
        float distance = (float)Math.Sqrt(diffX * diffX + diffY * diffY);                   
        if (distance == 0) return;
        scene.AddGameObject(new Bullet(scene, x, y, diffX / distance, diffY / distance, 6f, "Enemy_Bullet"));
    }
    public static void  AimedAuto(Scene scene,float x, float y)
    {
        var player = scene.FindGameObject("Player");
        float playerX = player != null ? player.X : x;
        float playerY = player != null ? player.Y : y;
        Aimed(scene, x, y, playerX, playerY); 
    }

    public static void SpreadAimed(Scene scene, float x, float y, float playerX, float playerY)  
    {
        float diffX = playerX - x;
        float diffY = playerY - y;
        float distance = (float)Math.Sqrt(diffX * diffX + diffY * diffY);
        if (distance == 0) return;
        float aimX = diffX / distance;
        float aimY = diffY / distance;
        
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

 




