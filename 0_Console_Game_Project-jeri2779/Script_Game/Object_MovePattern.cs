using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;



internal abstract class MovePattern
{
    public abstract (float moveX, float moveY) GetMovement(float x, float y, float deltaTime);// 현재 위치(x, y)와 deltaTime을 받아 이동할 좌표 변화량(dx, dy)을 반환하는 추상 메서드
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
    private float _timer;

    public SideToSide(float speed)
    {
        _speed = speed;
    }

    public override (float moveX, float moveY) GetMovement(float x, float y, float deltaTime)
    {
        _timer += deltaTime;
        return (MathF.Cos(_timer * 2.0f) * _speed * deltaTime, 0f);
    }
}