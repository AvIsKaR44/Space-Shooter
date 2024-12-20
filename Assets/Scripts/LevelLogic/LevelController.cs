using UnityEngine;

namespace SpaceShooter
{
    public class LevelController : SingletonBase<LevelController>
    {
        [SerializeField] private LevelCondition[] m_Conditions;

        private bool m_IsLevelCompleted;

        private void Update()
        {
            if (m_IsLevelCompleted == true) return;
            
            int numCompleted = 0;

            for (int i = 0; i < m_Conditions.Length; i++)
            {                               
                if (m_Conditions[i].IsCompleted == true)
                {
                    numCompleted++;                        
                }                             
            }
          

            if (numCompleted == m_Conditions.Length)
            {
                m_IsLevelCompleted = true;              
            }
           
               
        }
    }
}