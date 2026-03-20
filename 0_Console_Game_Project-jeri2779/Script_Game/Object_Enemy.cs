 
using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;

class Enemy : GameObject

{
    //Bullet Bullet; // 총알 오브젝트 참조\

    private Action<Scene, float, float> _shootPattern;                        
    private Action<Scene, float, float, float, float> _aimPattern; // 적의 총알 발사 패턴을 정의 델리게이트 (확장) 유도 패턴용

    public event GameAction OnDied;                      // 적이 죽었을 때 발생하는 이벤트

    //private float _posY;                          // 적의 Y 좌표  
    //private float _posX; 

    private float _speed = 8.0f;                    // 적의 이동 속도
    private float _targetPos = 5.0f;                // 적이 이동할 목표 Y 좌표 (적이 이 위치에 도달하면 정지)

    private float _shootTimer;
    private float _shootInterval = 3.0f;            // 총알 발사 간격 (초)

    private int _health = 10;                       // 적의 체력   


    // 적이 이동할 목표 위치
    public Enemy(Scene scene, int hp, float startX, float startY, 
                Action<Scene, float, float> shootPattern) : base(scene)
    {
        Name = "Enemy";
        X = startX;
        Y = startY;                     // 적의 초기 위치 설정
        _health = hp;                   // 적의 체력 설정
        _shootPattern = shootPattern; // 총알 발사 패턴 설정
    }
    public Enemy(Scene scene, int hp, float startX, float startY,
                Action<Scene, float, float, float, float> aimPattern) : base(scene)//유도 패턴 생성자 확장
    {
        Name = "Enemy";
        X = startX;
        Y = startY;                     // 적의 초기 위치 설정
        _health = hp;                   // 적의 체력 설정
        _aimPattern = aimPattern;       // 유도 패턴 설정 (확장)
    }

    public override void Draw(ScreenBuffer buffer)
    {
        //적이 wall안에 있을때만 그려지게
        if(X >= Wall.Left && X <= Wall.Right        // 적의 X 좌표가 벽의 왼쪽과 오른쪽 사이에 있는지 확인
            && Y >= Wall.Top && Y <= Wall.Bottom)   // 적의 Y 좌표가 벽의 위쪽과 아래쪽 사이에 있는지 확인
        {
            buffer.SetCell((int)X, (int)Y, 'O', ConsoleColor.Red);
            buffer.SetCell((int)X - 1, (int)Y, 'V', ConsoleColor.Red);
            buffer.SetCell((int)X + 1, (int)Y, 'V', ConsoleColor.Red);
        }
    }
    public override void Update(float deltaTime)
    {
        //적이 위에서 내려오면서 일정시간 후 정지
        if(Y < _targetPos)                              // 적이 화면 상단에서 내려오는 상태
        {
            Y += _speed * deltaTime;     
        }
        else
        {
                                                        // 적이 일정 위치에 도달하면 정지
            Y = _targetPos;  
        }


        _shootTimer += deltaTime;                       // 총알 발사 타이머 업데이트
        if (_shootTimer >= _shootInterval)              // 총알 발사 간격이 지났는지 확인
        {
            _shootTimer = 0;

            if (_shootPattern != null)
            {
                _shootPattern.Invoke(Scene, X, Y);      // 총알 발사 패턴 실행
            }
            else if (_aimPattern != null)
            {
                var player = Scene.FindGameObject("Player"); // 플레이어 오브젝트 참조 (유도 패턴용)
                float px = player != null ? player.X : X;    // 플레이어의 X 좌표, 플레이어가 없으면 적의 X 좌표 사용
                float py = player != null ? player.Y : Y;
                _aimPattern.Invoke(Scene, X, Y, px, py);     //  유도 패턴 실행 (적의 위치와 플레이어의 위치 전달)
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

