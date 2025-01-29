using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Common
{
    /// <summary>
    /// Уничтожаемый объект на сцене. То что может иметь хитпоинты.
    /// </summary>
    public class Destructible : Entity
    {
        #region Properties
        /// <summary>
        /// Object ignor damage.
        /// </summary>
        [SerializeField] private bool m_Indestructible;
        public bool IsIndestructible => m_Indestructible;

        /// <summary>
        /// Starting hitpoints.
        /// </summary>
        [SerializeField] private int m_HitPoints;
        public int MaxHitPoints => m_HitPoints;


        /// <summary>
        /// Current hitpoints
        /// </summary>
        private int m_CurrentHitPoints;
        public int HitPoints => m_CurrentHitPoints;
        /// <summary>
        /// Префаб взрыва корабля
        /// </summary>
        [SerializeField] private GameObject m_ExplosionPrefab; 

        #endregion

        #region Unity Events

        protected virtual void Start()
        {
            m_CurrentHitPoints = m_HitPoints;

            transform.SetParent(null);
        }

        #endregion

        #region Public API
        /// <summary>
        /// Apply damage an to object
        /// </summary>
        /// <param name="damage">Damage dealt to an object</param>
        public void ApplyDamage(int damage)
        {
            if (m_Indestructible) return;

            m_CurrentHitPoints -= damage;

            if (m_CurrentHitPoints <= 0)
                OnDeath();

        }

        #endregion
        /// <summary>
        /// Переопределяемое событие уничтожения объекта, когда хитпоинты ниже нуля. (An overridden object destruction event when the hit is below zero).
        /// </summary>
        protected virtual void OnDeath()
        {
            if(m_ExplosionPrefab != null)
            {
               GameObject explosion = Instantiate(m_ExplosionPrefab, transform.position, Quaternion.identity);

                Destroy(explosion, 2.0f);
            }

            if(!IsOnFinishPoint())
            {
                Destroy(gameObject);
                m_EventOnDeath?.Invoke();
            }          
        }

        private static HashSet<Destructible> m_AllDestructibles;

        public static IReadOnlyCollection<Destructible> AllDestructibles => m_AllDestructibles;

        protected virtual void OnEnable()
        {
            if (m_AllDestructibles == null)
                m_AllDestructibles = new HashSet<Destructible>();

            m_AllDestructibles.Add(this);
        }

        protected virtual void OnDestroy()
        {
              m_AllDestructibles.Remove(this);
        }
            
        public const int TeamIdNeutral = 0;

        [SerializeField] private int m_TeamId;
        public int TeamId => m_TeamId;

        #region IsOnFinishPoint
        /// <summary>
        /// Проверка нахождения корабля на точке финиша
        /// </summary>
        /// <returns></returns>
        private bool IsOnFinishPoint()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("FinishPoint"))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion         

        [SerializeField] private UnityEvent m_EventOnDeath;
        public UnityEvent EventOnDeath => m_EventOnDeath;

        [SerializeField] private int m_ScoreValue;
        public int ScoreValue => m_ScoreValue;
    }
}
