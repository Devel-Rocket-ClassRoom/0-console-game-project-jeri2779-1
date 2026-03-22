 
using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;

class Enemy : GameObject

{
    //Bullet Bullet; // 총알 오브젝트 참조\

    //private Action<Scene, float, float> _shootPattern;                        

    public event GameAction OnDied;                      // 적이 죽었을 때 발생하는 이벤트
    private readonly EnemyType _type;



    private float _speed = 8.0f;                    // 적의 이동 속도
    private float _targetPos = 5.0f;                // 적이 이동할 목표 Y 좌표 (적이 이 위치에 도달하면 정지)

    private float _shootTimer;
    private float _shootInterval = 3.0f;            // 총알 발사 간격 (초)

    private int _health = 10;                       // 적의 체력   

    private BulletPattern _bulletPattern;                   // 총알 발사 패턴 (추가) BulletPattern 클래스 참조
    private MovePattern _movePattern;                   // 적의 이동 패턴 (추가) MovePattern 클래스 참조


    // 적이 이동할 목표 위치
    public Enemy(Scene scene, 
        int hp, 
        float startX, 
        float startY, 
        BulletPattern shootPattern, 
        MovePattern movePattern, 
        EnemyType type,
        float shootInterval) : base(scene)
        {
            Name = "Enemy";
            X = startX;
            Y = startY;                     // 적의 초기 위치 설정
            _health = hp;                   // 적의 체력 설정
            _bulletPattern = shootPattern; // 총알 발사 패턴 설정
            _movePattern = movePattern;     // 이동 패턴 설정
            _type = type;                   // 적의 타입 설정
            _shootInterval = shootInterval; // 총알 발사 간격 설정
        }


    public override void Draw(ScreenBuffer buffer)
    {
        if (X < Wall.Left || X > Wall.Right || Y < Wall.Top || Y > Wall.Bottom) return;

        switch (_type)
        {
            case EnemyType.EnemyA:
                buffer.SetCell((int)X, (int)Y, 'O', ConsoleColor.Red);
                buffer.SetCell((int)X - 1, (int)Y, 'V', ConsoleColor.Red);
                buffer.SetCell((int)X + 1, (int)Y, 'V', ConsoleColor.Red);
                break;
            case EnemyType.EnemyB:
                buffer.SetCell((int)X, (int)Y, 'o', ConsoleColor.Cyan);
                buffer.SetCell((int)X - 1, (int)Y, '-', ConsoleColor.Cyan);
                buffer.SetCell((int)X + 1, (int)Y, '-', ConsoleColor.Cyan);
                break;
            case EnemyType.EnemyC:
                buffer.SetCell((int)X, (int)Y, 'Q', ConsoleColor.Yellow);
                buffer.SetCell((int)X - 1, (int)Y, '#', ConsoleColor.Yellow);
                buffer.SetCell((int)X + 1, (int)Y, '#', ConsoleColor.Yellow);
                break;
        }
    }
    public override void Update(float deltaTime)
    {
        //적이 위에서 내려오면서 일정시간 후 정지
       

        if(_movePattern != null)
        {
            var (moveX, moveY) = _movePattern.GetMovement(X, Y, deltaTime); // 이동 패턴에서 이동량 계산
            float nextX = X + moveX; // 다음 프레임에서의 X 좌표 계산
            float nextY = Y + moveY; // 다음 프레임에서의 Y 좌표 계산
            if(nextX >= Wall.Left && nextX <= Wall.Right) // 다음 프레임에서의 X 좌표가 벽의 범위 내에 있는지 확인
            {
                X = nextX; // X 좌표 업데이트
            }
            if(nextY >= Wall.Top && nextY <= Wall.Bottom) // 다음 프레임에서의 Y 좌표가 벽의 범위 내에 있는지 확인
            {
                Y = nextY; // Y 좌표 업데이트
            }

        }


        _shootTimer += deltaTime;                       // 총알 발사 타이머 업데이트
        if (_shootTimer >= _shootInterval)              // 총알 발사 간격이 지났는지 확인
        {
            _shootTimer = 0;

            if (_bulletPattern != null)
            {
                _bulletPattern.Fire(Scene, X, Y);      // 총알 발사 패턴 실행
            }
           
        }


    }

    public void TakeDamage(int damage)
    {
        _health -= damage;                           // 적의 체력 감소
        if (_health <= 0)                           // 적의 체력이 0 이하가 되면 제거
        {
            OnDied?.Invoke();                   // 적이 죽었을 때 이벤트 발생
        }
    }   


}
internal enum EnemyType { EnemyA, EnemyB, EnemyC }

