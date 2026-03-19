using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

public class Playing : Scene

{
    private Wall wall;                    // 벽 오브젝트
    private Player player;                // 플레이어 오브젝트
    private Enemy enemy;                  // 적 오브젝트


    private int life;                     // 생명 변수
    private bool isGameOver;              // 게임 오버 상태 변수

    public event GameAction OnPlayAgain;  // 다시 시작 이벤트
    public event GameAction OnGameOver;   // 게임 오버 이벤트

    public override void Draw(ScreenBuffer buffer)
    {
        // 게임 씬 그리기 로직 
        DrawGameObjects(buffer);
        buffer.WriteText(1, 0, $"life: {life}", ConsoleColor.Cyan); // 점수 표시 
        

        if (isGameOver)
        {
            buffer.WriteTextCentered(8, "Game Over", ConsoleColor.Red);
            buffer.WriteTextCentered(10, $"Life: {life}", ConsoleColor.Yellow);
            buffer.WriteTextCentered(12, "Press ENTER to Retry", ConsoleColor.White);
        }
        //throw new NotImplementedException();
    }
    public override void Load() //Awake()
    {
        life = 3;
        isGameOver = false;
        wall = new Wall(this);
        AddGameObject(wall);
        player = new Player(this, 20, 10);
        AddGameObject(player);
        enemy = new Enemy(this, 1, 0);
        AddGameObject(enemy);

        //throw new NotImplementedException();
    }
    public override void Unload()
    {
        ClearGameObjects();
        // 게임 씬 언로드 로직 (예: 리소스 정리)
        //throw new NotImplementedException();
    }
    public override void Update(float deltaTime)
    {
        if (isGameOver)
        {
            if (Input.IsKeyDown(ConsoleKey.Enter))
            {
                isGameOver = false;
                OnPlayAgain?.Invoke();
            }
            return;
        }

        UpdateGameObjects(deltaTime); // ← 주석 해제: 플레이어 Update/HandleInput 호출됨

        if (wall.IsCollision(player))   // 벽과 충돌여부 확인
        {
            isGameOver = true;
            return;

        }
     
    }
}

