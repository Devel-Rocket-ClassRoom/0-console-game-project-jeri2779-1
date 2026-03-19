namespace Framework.Engine
{
    public class SceneManager<TScene> where TScene : Scene // 제네릭 타입 TScene은 Scene을 상속
    {
        private TScene _currentScene;                      // 현재 활성화된 씬

        public event GameAction<TScene> SceneChanged;      // 씬 변경 이벤트

        public TScene CurrentScene => _currentScene;      // 현재 활성화된 씬 반환

        public void ChangeScene(TScene scene)             // 씬 변경 메서드
        {
            if (_currentScene != null)
            {
                _currentScene.Unload();
            }
            _currentScene = scene;
            SceneChanged?.Invoke(_currentScene);
            _currentScene.Load();
        }
    }
}
