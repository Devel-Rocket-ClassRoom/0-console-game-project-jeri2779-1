using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;
//Engine 폴더내 정의된 요소들만 사용할것.

public class Run_Game : GameApp //1. GameApp을 상속받아 SnakeGame 클래스 정의
{
    private readonly SceneManager<Scene> _scenes = new SceneManager<Scene>(); // 씬 매니저 (게임 씬 관리)

    public Run_Game(int width, int height) : base(width, height)
    {
    }
    protected override void Initialize()
    {
        ChangedToTitle(); // 타이틀 씬으로 시작
        //throw new System.NotImplementedException();
        // 게임 초기화 로직 (예: 뱀과 음식 초기 위치 설정)
    }
    protected override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Escape))
        {
            Quit(); // 게임 종료
            return;
        }

        _scenes.CurrentScene?.Update(deltaTime); // 현재 씬 업데이트

        //throw new System.NotImplementedException();
        // 게임 업데이트 로직 (예: 뱀 이동, 충돌 검사, 음식 먹기 등)
    }
    protected override void Draw()
    {
        _scenes.CurrentScene?.Draw(Buffer);         // 현재 씬 그리기
        //throw new System.NotImplementedException();
        // 게임 그리기 로직 (예: 뱀과 음식 그리기)
    }

    public void ChangedToTitle()
    {
        //_scenes.ChangeScene(new TitleScene()); // 타이틀 씬으로 변경
        var title = new Start();               // 타이틀 씬 생성
        title.OnStartGame += ChangedToPlay;         // 타이틀 씬에서 게임 시작 이벤트 구독
        _scenes.ChangeScene(title);                 // 타이틀 씬으로 변경
    }
    public void ChangedToPlay()
    {
        var play = new Playing(Buffer.Width, Buffer.Height);                 // 플레이 씬 생성
        play.OnPlayAgain += ChangedToTitle;         // 플레이 씬에서 게임 오버 이벤트 구독
        _scenes.ChangeScene(play);                  // 플레이 씬으로 변경
    }
}

