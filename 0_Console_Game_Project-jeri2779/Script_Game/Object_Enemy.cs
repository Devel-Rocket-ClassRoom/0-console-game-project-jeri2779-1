 
using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;

class Enemy : GameObject

{
    Bullet Bullet; // 총알 오브젝트 참조\

    private Action<Scene, float, float> _shootPattern;                        
    private Action<Scene, float, float, float, float> _aimPattern; // 적의 총알 발사 패턴을 정의 델리게이트 (확장) 유도 패턴용

    //private float _posY;                    // 적의 Y 좌표  
    //private float _posX; 

    private float _speed = 8.0f;                // 적의 이동 속도
    private float _targetPos = 5.0f;             // 적이 이동할 목표 Y 좌표 (적이 이 위치에 도달하면 정지)

    private float _shootTimer;
    private float _shootInterval = 3.0f;       // 총알 발사 간격 (초)


    // 적이 이동할 목표 위치
    public Enemy(Scene scene, float startX, float startY, 
                Action<Scene, float, float> shootPattern) : base(scene)
    {
        Name = "Enemy";
        X = startX;
        Y = startY;                     // 적의 초기 위치 설정
        _shootPattern = shootPattern; // 총알 발사 패턴 설정
    }

    public override void Draw(ScreenBuffer buffer)
    {
        //적이 wall안에 있을때만 그려지게
        if(X >= Wall.Left && X <= Wall.Right        // 적의 X 좌표가 벽의 왼쪽과 오른쪽 사이에 있는지 확인
            && Y >= Wall.Top && Y <= Wall.Bottom)   // 적의 Y 좌표가 벽의 위쪽과 아래쪽 사이에 있는지 확인
        {
            buffer.SetCell((int)X, (int)Y, 'V', ConsoleColor.Red);
        }
    }
    public override void Update(float deltaTime)
    {
        //적이 위에서 내려오면서 일정시간 후 정지
        if(Y < _targetPos)                      // 적이 화면 상단에서 내려오는 상태
        {
            Y += _speed * deltaTime;     
        }
        else
        {
                                                    // 적이 일정 위치에 도달하면 정지
            Y = _targetPos;  
        }
        

        _shootTimer += deltaTime;                   // 총알 발사 타이머 업데이트
        if (_shootTimer >= _shootInterval)
        {
            _shootTimer = 0;                        // 총알 발사 타이머 초기화
            _shootPattern(Scene, X, Y);             // 설정된 총알 발사 패턴 실행

        }
    }

 
}

