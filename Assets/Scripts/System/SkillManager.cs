using UnityEngine;

namespace Combat.Skills
{
    // 씬의 빈 오브젝트에 붙어있고, 모든 서비스 컴포넌트를 자식으로 가짐
    public class SkillManager : MonoBehaviour
    {
        // 주입기(Injector)가 이 매니저를 찾을 수 있도록 싱글톤 사용
        public static SkillManager Instance { get; private set; }

        // 실제 서비스 인스턴스 (인터페이스 타입으로 소유)
        public ITimeSource TimeSource { get; private set; }
        public ISpawner Spawner { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                TimeSource = GetComponentInChildren<UnityTimeSource>();
                Spawner = GetComponentInChildren<SimpleSpawner>();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}