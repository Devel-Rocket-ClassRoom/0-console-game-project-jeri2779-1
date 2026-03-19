using System;

namespace Framework.Engine
{
    public abstract class GameObject
    {
        public string Name { get; set; } = "";      //오브젝트 이름
        public bool IsActive { get; set; } = true;  // 오브젝트 활성 상태
        public Scene Scene { get; }                 // 오브젝트가 속한 씬

        //좌표   프로퍼티 추가
        public float X { get; protected set; }                  // X 좌표
        public float Y { get; protected set; }                  // Y 좌표

        protected GameObject(Scene scene) // 생성자에서 씬 참조를 받아 초기화
        {
            Scene = scene;
        }

        public abstract void Update(float deltaTime);//
        public abstract void Draw(ScreenBuffer buffer);
    }
}
