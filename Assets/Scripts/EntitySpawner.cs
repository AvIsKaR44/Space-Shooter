using UnityEngine;


namespace SpaceShooter
{
    public class EntitySpawner : MonoBehaviour
    {
        public enum SpawnMode
        {
            Start,
            Loop
        }

        [SerializeField] private AIPointPatrol[] m_PatrolPoints; //Массив точек патрулирования

        [SerializeField] private Entity[] m_EntityPrefabs;

        [SerializeField] private CircleArea m_Area;

        [SerializeField] private SpawnMode m_SpawnMode;

        [SerializeField] private int m_NumSpawns;

        [SerializeField] private float m_RespawnTime;


        [SerializeField] private float m_Timer;
        private int m_CurrentPatrolPointIndex = 0;


        private void Start()
        {
            if(m_SpawnMode == SpawnMode.Start)
            {
                SpawnEntities();                
            }

            m_Timer = m_RespawnTime;
        }

        private void Update()
        {
            if(m_Timer > 0) 
                m_Timer -= Time.deltaTime;

            if(m_SpawnMode == SpawnMode.Loop && m_Timer < 0)
            {
                SpawnEntities();

                m_Timer = m_RespawnTime;
            }
        }

        private void SpawnEntities()
        {
            for (int i = 0; i < m_NumSpawns; i++)
            {
                int index = Random.Range(0, m_EntityPrefabs.Length);

                GameObject e = Instantiate(m_EntityPrefabs[index].gameObject);

                e.transform.position = m_Area.GetRandomInsideZone();

                //Назначение точки патрулирования
                AIController aIController = e.GetComponent<AIController>();
                if (aIController != null && m_PatrolPoints.Length > 0)
                {
                    aIController.SetPatrolPoint(m_PatrolPoints[m_CurrentPatrolPointIndex]);
                    m_CurrentPatrolPointIndex = (m_CurrentPatrolPointIndex + 1) % m_PatrolPoints.Length;
                }
            }
        }
    }
}