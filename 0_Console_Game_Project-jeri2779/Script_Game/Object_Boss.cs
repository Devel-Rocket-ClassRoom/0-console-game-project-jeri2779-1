using System;
using Framework.Engine;

internal class Boss : GameObject
{
    private readonly int _maxHp;
    private int _currentHp;

    private readonly StageData.BossPhase[] _phases;
    private int _phaseIndex = 0;

    private bool _isInvincible = false;
    private float _shootTimer = 0f;
    private const float _shootInterval = 2.5f;

    public event GameAction OnDied;

    public Boss(Scene scene, int hp, float startX, float startY,
                StageData.BossPhase[] phases) : base(scene)
    {
        Name = "Boss";
        X = startX;
        Y = startY;
        _maxHp = hp;
        _currentHp = hp;
        _phases = phases;
        _isInvincible = true;   // 스폰 직후 무적 (첫 발사 전까지)
    }

    public override void Update(float deltaTime)
    {
        // HP 비율 계산 후 구간 전환 체크
        float hpRatio = (float)_currentHp / _maxHp;// HP 비율 계산
        int nextPhase = _phaseIndex;                // 현재 구간 유지
        for (int i = _phases.Length - 1; i >= 0; i--)// HP 비율이 낮은 구간부터 체크해서 현재 구간 찾기
        {
            if (hpRatio <= _phases[i].HpThreshold)// HP 비율이 구간의 임계값 이하이면 해당 구간으로 전환
            {
                nextPhase = i;
                break;
            }
        }
        // 구간이 바뀌었으면 무적 + 발사 타이머 리셋
        if (nextPhase != _phaseIndex)
        {
            _phaseIndex = nextPhase;
            _isInvincible = true;
            _shootTimer = 0f;
        }

        // 발사 타이머
        _shootTimer += deltaTime;
        if (_shootTimer >= _shootInterval)
        {
            _shootTimer = 0f;
            _isInvincible = false;  // 첫 발사 시 무적 해제
            _phases[_phaseIndex].Patterns?.Invoke(Scene, X, Y);
        }
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (X >= Wall.Left && X <= Wall.Right
            && Y >= Wall.Top && Y <= Wall.Bottom)
        {
            var color = _isInvincible ? ConsoleColor.DarkGray : ConsoleColor.Magenta; // 무적 상태는 어두운 회색, 일반 상태는 마젠타
            buffer.SetCell((int)X, (int)Y, 'B', color);
            buffer.SetCell((int)X - 1, (int)Y, 'W', color);
            buffer.SetCell((int)X + 1, (int)Y, 'W', color);
        }
    }

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;  // 무적 중 피격 무시
        _currentHp -= damage;
        if (_currentHp <= 0)
        {
            OnDied?.Invoke();
        }
    }
}