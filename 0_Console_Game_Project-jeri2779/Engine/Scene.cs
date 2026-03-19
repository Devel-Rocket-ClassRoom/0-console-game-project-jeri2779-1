using System.Collections.Generic;

namespace Framework.Engine
{
    public abstract class Scene
    {
        private readonly List<GameObject> _gameObjects = new List<GameObject>();
        private readonly List<GameObject> _pendingAdd = new List<GameObject>();
        private readonly List<GameObject> _pendingRemove = new List<GameObject>();
        private bool _isUpdating;

        public abstract void Load();                    // 씬이 활성화될 때 호출
        public abstract void Update(float deltaTime);   // 씬이 매 프레임 업데이트될 때 호출
        public abstract void Draw(ScreenBuffer buffer); // 씬이 매 프레임 그려질 때 호출
        public abstract void Unload();                  // 씬이 비활성화될 때 호출

        public void AddGameObject(GameObject gameObject)// 게임 오브젝트 추가
        {
            if (_isUpdating)                            // 업데이트 중이면 대기 리스트에 추가
            {
                _pendingAdd.Add(gameObject);
            }
            else
            {
                _gameObjects.Add(gameObject);
            }
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            if (_isUpdating)
            {
                _pendingRemove.Add(gameObject);
            }
            else
            {
                _gameObjects.Remove(gameObject);
            }
        }

        public void ClearGameObjects()
        {
            _gameObjects.Clear();
            _pendingAdd.Clear();
            _pendingRemove.Clear();
        }

        protected void UpdateGameObjects(float deltaTime)
        {
            FlushPending();
            _isUpdating = true;

            for (int i = 0; i < _gameObjects.Count; i++)
            {
                if (_gameObjects[i].IsActive)
                {
                    _gameObjects[i].Update(deltaTime);
                }
            }

            _isUpdating = false;
        }

        protected void DrawGameObjects(ScreenBuffer buffer)
        {
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                if (_gameObjects[i].IsActive)
                {
                    _gameObjects[i].Draw(buffer);
                }
            }
        }

        public GameObject FindGameObject(string name)  // 이름으로 게임 오브젝트 찾기
        {
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                if (_gameObjects[i].Name == name)
                {
                    return _gameObjects[i];
                }
            }

            for (int i = 0; i < _pendingAdd.Count; i++)
            {
                if (_pendingAdd[i].Name == name)
                {
                    return _pendingAdd[i];
                }
            }

            return null;
        }

        public List<GameObject> FindGameObjectsAll(string name)
        {
            var result = new List<GameObject>();
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                if (_gameObjects[i].Name == name)
                {
                    result.Add(_gameObjects[i]);
                }
            }
            for(int i = 0; i < _pendingAdd.Count; i++)
            {
                if (_pendingAdd[i].Name == name)
                {
                    result.Add(_pendingAdd[i]);
                }
            }
            return result;
        }

        private void FlushPending() // 대기 중인 게임 오브젝트를 처리
        {
            if (_pendingRemove.Count > 0)
            {
                for (int i = 0; i < _pendingRemove.Count; i++)
                {
                    _gameObjects.Remove(_pendingRemove[i]);
                }
                _pendingRemove.Clear();
            }

            if (_pendingAdd.Count > 0)
            {
                _gameObjects.AddRange(_pendingAdd);
                _pendingAdd.Clear();
            }
        }
    }
}
