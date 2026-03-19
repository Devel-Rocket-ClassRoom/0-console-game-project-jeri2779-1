 
using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;

class Enemy : GameObject

{
    Bullet Bullet; // 총알 오브젝트 참조

    private float _posY;                     // 적의 Y 좌표  
    private float _posX; 

    private float _speed = 1f;               // 적의 이동 속도
    private float _targetPos = 5;

    private float _shootTimer;
    private float _shootInterval = 1.5f; // 총알 발사 간격 (초)

    // 적이 이동할 목표 위치
    public Enemy(Scene scene, float startX, float startY) : base(scene)
    {
        Name = "Enemy";
        _posX = startX;
        _posY = startY;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.SetCell((int)_posX, (int)_posY, 'V', ConsoleColor.Red);
    }
    public override void Update(float deltaTime)
    {
        //적이 위에서 내려오면서 일정시간 후 정지
        if(_posY < _targetPos)                      // 적이 화면 상단에서 내려오는 상태
        {
            _posY += _speed * deltaTime;     
        }
        else
        {
                                                    // 적이 일정 위치에 도달하면 정지
            _posY = _targetPos;  
        }

        _shootTimer += deltaTime;                   // 총알 발사 타이머 업데이트
        if (_shootTimer >= _shootInterval)
        {
            _shootTimer = 0;                        // 총알 발사 타이머 초기화
            Bullet = new Bullet(Scene, _posX, _posY + 1, 0.5f ,1, "Enemy_Bullet"); // 총알 생성 (적 바로 아래)
            Scene.AddGameObject(Bullet);            // 총알을 씬에 추가
            Bullet = new Bullet(Scene, _posX, _posY + 1, 0f, 1, "Enemy_Bullet"); // 총알 생성 (적 바로 아래)
            Scene.AddGameObject(Bullet);
            Bullet = new Bullet(Scene, _posX, _posY + 1, -0.5f, 1, "Enemy_Bullet"); // 총알 생성 (적 바로 아래)
            Scene.AddGameObject(Bullet);
            Bullet = new Bullet(Scene, _posX, _posY + 1, -1f, 1, "Enemy_Bullet"); // 총알 생성 (적 바로 아래)
            Scene.AddGameObject(Bullet);
            Bullet = new Bullet(Scene, _posX, _posY + 1, 1f, 1, "Enemy_Bullet"); // 총알 생성 (적 바로 아래)
            Scene.AddGameObject(Bullet);
        }
    }

    public bool IsCollision(float x, float y)       // 총알과 적의 충돌 여부 확인
    {
        return (int)x == (int)_posX && (int)y == (int)_posY;
    }

    internal bool IsCollision(Player player)
    {
        return (int)player.Pos.X == (int)_posX && player.Pos.Y == (int)_posY;
    }
}

