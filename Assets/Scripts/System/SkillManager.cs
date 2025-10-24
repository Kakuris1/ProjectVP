using UnityEngine;

namespace Combat.Skills
{
    // ���� �� ������Ʈ�� �پ��ְ�, ��� ���� ������Ʈ�� �ڽ����� ����
    public class SkillManager : MonoBehaviour
    {
        // ���Ա�(Injector)�� �� �Ŵ����� ã�� �� �ֵ��� �̱��� ���
        public static SkillManager Instance { get; private set; }

        // ���� ���� �ν��Ͻ� (�������̽� Ÿ������ ����)
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