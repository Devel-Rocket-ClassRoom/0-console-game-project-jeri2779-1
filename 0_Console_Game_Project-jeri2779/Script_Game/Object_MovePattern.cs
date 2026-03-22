using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;



internal abstract class MovePattern
{
    public abstract (float moveX, float moveY) GetMovement(float x, float y, float deltaTime);// 현재 위치(x, y)와 deltaTime을 받아 이동할 좌표를 정하는 메서드

    public virtual void SetOffset(int index) { } // 이동 패턴에 인덱스에 따른 오프셋을 설정하는 가상 메서드, 필요에 따라 오버라이드하여 사용)
}

internal class DescendAndStop : MovePattern// 일정 속도로 아래로 이동하다가 특정 Y 좌표에 도달하면 멈추는 이동 패턴 클래스 MovePattern을 상속받아 구현
{
    private readonly float _speed;
    private readonly float _targetY;

    public DescendAndStop(float speed, float targetY)
    {
        _speed = speed;
        _targetY = targetY;
    }

    public override (float moveX, float moveY) GetMovement(float x, float y, float deltaTime)
    {
        if(y < _targetY)
        {
            return (0f, _speed * deltaTime);
        }
        return(0f, 0f); 
         
    }
}

internal class SideToSide : MovePattern// 좌우로 움직이는 이동 패턴 클래스 MovePattern을 상속받아 구현
{
    private readonly float _speed;
    private readonly float _moveRange;

    private float _timer;
    private float _centerX;
    private bool _initialized = false;

    public SideToSide(float speed, float moveRange)
    {
        _speed = speed;
        _moveRange = moveRange;
    }

    public override (float moveX, float moveY) GetMovement(float x, float y, float deltaTime)
    {
        if(!_initialized)
        {
            _centerX = x;
            _initialized = true;
        }
        _timer += deltaTime; // 타이머 업데이트
        float targetX = (float)(_centerX + Math.Cos(_timer * _speed) * _moveRange); // 타이머에 따라 좌우로 움직이는 목표 X 좌표 계산
        
        return (targetX - x, 0f);
    }
    public override void SetOffset(int index)
    {
        _timer = index * 0.5f; // 인덱스에 따라 타이머 초기값을 다르게 설정하여 적들이 서로 다른 위치에서 좌우로 움직이도록 함
    }
}

internal class ZigZag : MovePattern
{
    private readonly float _speedX;
    private readonly float _speedY;
    private readonly float _targetY;
    private readonly float _switchInterval; // 방향 전환 간격 (초)
    private float _timer;
    private int _dirX = 1;


    public ZigZag(float speedX, float speedY, float targetY, float switchInterval = 1.0f, float initialTimer = 0f)
    {
        _speedX = speedX;
        _speedY = speedY;
        _targetY = targetY;
        _switchInterval = switchInterval;
        _timer = initialTimer;
    }

    public override (float moveX, float moveY) GetMovement(float x, float y, float deltaTime) 
    {
        if (y >= _targetY) return (0f, 0f);

        _timer += deltaTime;
        if (_timer >= _switchInterval)
        {
            _dirX *= -1;
            _timer = 0f;
        }

        float moveX = _dirX * _speedX * deltaTime;
        float moveY = y < _targetY ? _speedY * deltaTime : 0f;

        return (moveX, moveY);
    }

    public override void SetOffset(int index)
    {
        _timer = index * 0.4f; // 인덱스에 따라 타이머 초기값을 다르게 설정하여 적들이 지그재그 패턴에서 서로 다른 위치에 있도록 함
    }
}

internal class CircleMove : MovePattern
{
    private readonly float _speed;      // 회전 속도 (라디안/초)
    private readonly float _radius;     // 회전 반경
    private readonly float _centerX;    // 회전 중심 X
    private readonly float _centerY;    // 회전 중심 Y
    private float _angle;

    public CircleMove(float centerX, float centerY, float radius, float speed)
    {
        _centerX = centerX;
        _centerY = centerY;
        _radius = radius;
        _speed = speed;
    }

    public override (float moveX, float moveY) GetMovement(float x, float y, float deltaTime)
    {
        
        _angle += _speed * deltaTime;
        float targetX = _centerX + MathF.Cos(_angle) * _radius;
        float targetY = _centerY + MathF.Sin(_angle) * _radius;
        return (targetX - x, targetY - y);  // 현재 위치에서 목표 위치까지의 delta 반환
    }
}
internal class PendulumMove : MovePattern
{
    private readonly float _speed;
    private readonly float _range;
    private float _centerX;
    private bool _initialized = false;
    private int _dir = 1;
    private float _currentX;

    public PendulumMove(float speed, float range)
    {
        _speed = speed;
        _range = range;
    }

    public override (float moveX, float moveY) GetMovement(float x, float y, float deltaTime)
    {
        if (!_initialized)
        {
            _centerX = x;
            _currentX = x;
            _initialized = true;
        }

        _currentX += _dir * _speed * deltaTime;

        if (_currentX >= _centerX + _range) { _currentX = _centerX + _range; _dir = -1; }
        if (_currentX <= _centerX - _range) { _currentX = _centerX - _range; _dir = 1; }

        return (_currentX - x, 0f);
    }

    public override void SetOffset(int index)
    {
        _dir = index % 2 == 0 ? 1 : -1; // 짝수/홀수 인덱스로 방향 엇갈리게
    }
}

internal class Figure8Move : MovePattern
{
    private readonly float _speed;
    private readonly float _radiusX;
    private readonly float _radiusY;
    private readonly float _centerX;
    private readonly float _centerY;
    private float _angle = 0f;

    public Figure8Move(float centerX, float centerY, float radiusX, float radiusY, float speed)
    {
        _centerX = centerX;
        _centerY = centerY;
        _radiusX = radiusX;
        _radiusY = radiusY;
        _speed = speed;
    }

    public override (float moveX, float moveY) GetMovement(float x, float y, float deltaTime)
    {
        _angle += _speed * deltaTime;
        // 리사주 곡선 (8자): X=sin(t), Y=sin(2t)/2
        float targetX = _centerX + MathF.Sin(_angle) * _radiusX;
        float targetY = _centerY + MathF.Sin(_angle * 2f) * _radiusY;
        return (targetX - x, targetY - y);
    }

    public override void SetOffset(int index)
    {
        _angle = index * (MathF.PI / 3f);
    }
}

internal class DescendThenSide : MovePattern
{
    private readonly float _descendSpeed;
    private readonly float _targetY;
    private readonly float _sideSpeed;
    private readonly float _sideRange;

    private bool _descended = false;
    private float _timer = 0f;
    private float _centerX;
    private bool _initialized = false;

    public DescendThenSide(float descendSpeed, float targetY, float sideSpeed, float sideRange)
    {
        _descendSpeed = descendSpeed;
        _targetY = targetY;
        _sideSpeed = sideSpeed;
        _sideRange = sideRange;
    }

    public override (float moveX, float moveY) GetMovement(float x, float y, float deltaTime)
    {
        if (!_descended)
        {
            if (y < _targetY) return (0f, _descendSpeed * deltaTime);
            _descended = true;
            _centerX = x;
        }

        if (!_initialized) { _initialized = true; }

        _timer += deltaTime;
        float targetX = _centerX + MathF.Cos(_timer * _sideSpeed) * _sideRange;
        return (targetX - x, 0f);
    }

    public override void SetOffset(int index)
    {
        _timer = index * 0.5f;
    }
}

internal class SemiCircleMove : MovePattern
{
    private readonly float _centerX;
    private readonly float _centerY;
    private readonly float _radius;
    private readonly float _speed;
    private float _angle;
    private int _dir = 1;

    public SemiCircleMove(float centerX, float centerY, float radius, float speed)
    {
        _centerX = centerX;
        _centerY = centerY;
        _radius = radius;
        _speed = speed;
        _angle = 0f; // 오른쪽에서 시작
    }

    public override (float moveX, float moveY) GetMovement(float x, float y, float deltaTime)
    {
        _angle += _dir * _speed * deltaTime;

        // 0 ~ PI (오른쪽 → 왼쪽 반원) 범위에서 왕복
        if (_angle >= MathF.PI) { _angle = MathF.PI; _dir = -1; }
        if (_angle <= 0f) { _angle = 0f; _dir = 1; }

        float targetX = _centerX + MathF.Cos(_angle) * _radius;
        float targetY = _centerY + MathF.Sin(_angle) * _radius;
        return (targetX - x, targetY - y);
    }

    public override void SetOffset(int index)
    {
        _angle = index % 2 == 0 ? 0f : MathF.PI; // 반씩 엇갈려서 시작
    }
}

internal class WaveMove : MovePattern
{
    private readonly float _speedX;
    private readonly float _speedY;
    private readonly float _rangeX;
    private readonly float _rangeY;
    private float _centerX;
    private float _centerY;
    private bool _initialized = false;
    private float _timer = 0f;

    public WaveMove(float speedX, float speedY, float rangeX, float rangeY)
    {
        _speedX = speedX;
        _speedY = speedY;
        _rangeX = rangeX;
        _rangeY = rangeY;
    }

    public override (float moveX, float moveY) GetMovement(float x, float y, float deltaTime)
    {
        if (!_initialized) { _centerX = x; _centerY = y; _initialized = true; }

        _timer += deltaTime;
        float targetX = _centerX + MathF.Sin(_timer * _speedX) * _rangeX;
        float targetY = _centerY + MathF.Sin(_timer * _speedY * 0.5f) * _rangeY;
        return (targetX - x, targetY - y);
    }
}