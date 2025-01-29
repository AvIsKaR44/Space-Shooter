using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Common;

namespace SpaceShooter
{
    [RequireComponent(typeof(SpaceShip))]
    public class AIController : MonoBehaviour
    {
       public enum AIBehaviour
        {
            Null,
            Patrol
        }

        [SerializeField] private AIBehaviour m_AIBehaviour;

        [SerializeField] private AIPointPatrol m_PatrolPoint;

        [Range(0f, 1f)]
        [SerializeField] private float m_NavigationLinear;

        [Range(0f, 1f)]
        [SerializeField] private float m_NavigationAngular;

        [SerializeField] private float m_RandomSelectMovePointTime;

        [SerializeField] private float m_FindNewTargetTime;

        [SerializeField] private float m_ShootDelay;

        [SerializeField] private float m_EvadeRayLenght;

        [SerializeField] private float m_EvadeTime; //Время уклонения

        private SpaceShip m_SpaceShip;        

        private Vector3 m_MovePosition;

        private Destructible m_SelectedTarget;        

         
        private Timer m_RandomizeDirectionTimer;
        private Timer m_FireTimer;
        private Timer m_FindNewTargetTimer;

        private Timer m_EvadeTimer;

        private bool m_IsEvading = false; // Добавлен флаг уклонения



        private void Start()
        {
            m_SpaceShip = GetComponent<SpaceShip>();

            InitTimers();

            m_EvadeTimer = new Timer(m_EvadeTime);
        }

        private void Update()
        {
            UpdateTimers();

            UpdateAI();
        }

        private void UpdateAI()
        {           
            if (m_AIBehaviour == AIBehaviour.Patrol)
            {
                UpdateBehaviourPatrol();
            }
        }

        private void UpdateBehaviourPatrol()
        {
            ActionFindNewMovePosition();
            ActionControlShip();
            ActionFindNewAttackTarget();
            ActionFire();
            ActionEvadeCollision();
        }

       
        /// <summary>
        /// Метод "MakeLead" для упреждения цели
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetVelocity"></param>
        /// <param name="projectileSpeed"></param>
        /// <returns></returns>
        private Vector3 MakeLead(Vector3 targetPosition, Vector3 targetVelocity, float projectileSpeed)
        {
            Vector3 targetToShooter = transform.position - targetPosition;
            float distance = targetToShooter.magnitude;

            float timeToImpact = distance / projectileSpeed;

            Vector3 leadPosition = targetPosition + targetVelocity * timeToImpact;

            return leadPosition;
        }
       

        private void ActionFindNewMovePosition()
        {
            if (m_IsEvading)
            {
                //Если корабль уклоняется, не меняем m_MovePosition
                return;
            }
            
            if (m_AIBehaviour == AIBehaviour.Patrol) 
            {
                if (m_SelectedTarget != null)
                {
                    //Для метода MakeLead получаем скорость цели
                    Rigidbody2D targetRigidbody = m_SelectedTarget.GetComponent<Rigidbody2D>();
                    Vector3 targetVelocity = targetRigidbody != null ? targetRigidbody.velocity : Vector3.zero;

                    //Вычисление точки упреждения
                    float projectileSpeed = GetProjectileSpeed();
                    Vector3 leadPosition = MakeLead(m_SelectedTarget.transform.position, targetVelocity, projectileSpeed);

                    m_MovePosition = leadPosition;
                }
                else
                {
                    if (m_PatrolPoint != null)
                    {
                        bool isInsidePatrolZone = Vector3.Distance(transform.position, m_PatrolPoint.transform.position) < m_PatrolPoint.Radius;
                        if (isInsidePatrolZone == true)
                        {
                            if (m_RandomizeDirectionTimer.IsFinished == true)
                            {
                                Vector2 newPoint = UnityEngine.Random.onUnitSphere * m_PatrolPoint.Radius + m_PatrolPoint.transform.position;
                                
                                m_MovePosition = newPoint;
                                m_RandomizeDirectionTimer.Start(m_RandomSelectMovePointTime);
                            }
                        }
                        else
                        {
                            m_MovePosition = m_PatrolPoint.transform.position;
                        }
                    }
                }

            }
        }
        private float GetProjectileSpeed()
        {
            Turret[] turrets = m_SpaceShip.GetComponentsInChildren<Turret>();

            foreach (Turret turret in turrets)
            {
                if (turret.Mode == TurretMode.Primary)
                {
                    TurretProperties properties = turret.GetTurretProperties();
                    return properties.ProjectilePrefab.GetComponent<Projectile>().GetVelocity();
                }
            }
            return 10.0f;
        }

        private void ActionEvadeCollision()
        {
            //Проверка столкновений с помощью Raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, m_EvadeRayLenght);

            if (hit.collider != null && hit.collider != GetComponent<Collider2D>())
            {
                Debug.Log("Collision detected! Evading...");

                // Если обнаружено столкновение, вибирается новая позиция для избегания
                Vector2 evadeDirection = Vector2.Perpendicular(hit.normal).normalized;

                //добавляем случайность в выборе направления
                if (Random.value > 0.5f)
                {
                    evadeDirection = -evadeDirection;
                }

                m_MovePosition = (Vector2)transform.position + evadeDirection * 10.0f;

                //Устанавливаем флаг уклонения
                m_IsEvading = true;               
            }   

            else
            {
                // Если препятствие не обнаружено, сбрасываем флаг уклонения
                m_IsEvading = false;
            }
        }

        private void ActionControlShip()
        {
            //Если корабль уклоняется, используем m_MovePosition для управления
            if (m_IsEvading)
            {
                m_SpaceShip.ThrustControl = m_NavigationLinear;
                m_SpaceShip.TorqueControl = ComputeAliginTorqueNormalized(m_MovePosition, m_SpaceShip.transform) * m_NavigationAngular;

                return;
            }
            else
            {
                //Если корабль не уклоняется, используем обычную логику
                m_SpaceShip.ThrustControl = m_NavigationLinear;
                m_SpaceShip.TorqueControl = ComputeAliginTorqueNormalized(m_MovePosition, m_SpaceShip.transform) * m_NavigationAngular;

            }
        }

        private const float MAX_ANGLE = 45.0f;

        private static float ComputeAliginTorqueNormalized(Vector3 targetPosition, Transform ship)
        {
            Vector2 directionToTarget = (targetPosition - ship.position).normalized;
            float angle = Vector2.SignedAngle(ship.up, directionToTarget);
            return Mathf.Clamp(angle / MAX_ANGLE, -1f, 1f);
        }
        private void ActionFindNewAttackTarget()
        {

            if (m_FindNewTargetTimer.IsFinished == true)
            {
                m_SelectedTarget = FindNearestDestructibleTarget();

                m_FindNewTargetTimer.Start(m_ShootDelay);
            }
        }
        private void ActionFire()
        {
            if (m_SelectedTarget != null)
            {
                if (m_FireTimer.IsFinished == true)
                {
                    m_SpaceShip.Fire(TurretMode.Primary);

                    m_FireTimer.Start(m_ShootDelay);
                }
            }
        }

        private Destructible FindNearestDestructibleTarget()
        {
            float maxDist = float.MaxValue;

            Destructible potentialTarget = null;

            foreach (var v in Destructible.AllDestructibles)
            {
                if (v.GetComponent<SpaceShip>() == m_SpaceShip) continue;

                if (v.TeamId == Destructible.TeamIdNeutral) continue;

                if (v.TeamId == m_SpaceShip.TeamId) continue;

                float dist = Vector2.Distance(m_SpaceShip.transform.position, v.transform.position);

                if (dist > 20f) continue;

                if (dist < maxDist)
                {
                    RaycastHit2D hit = Physics2D.Linecast(
                        m_SpaceShip.transform.position,
                        v.transform.position,
                        LayerMask.GetMask("Obstacles")
                        );

                    if (hit.collider == null) 
                    {
                        maxDist = dist;
                        potentialTarget = v;
                    }
                }

            }
            return potentialTarget;
        }
        #region Timers
        private void InitTimers()
        {
            m_RandomizeDirectionTimer = new Timer(m_RandomSelectMovePointTime);
            m_FireTimer = new Timer(m_ShootDelay);
            m_FindNewTargetTimer = new Timer(m_FindNewTargetTime);

            m_FindNewTargetTimer.Start(m_FindNewTargetTime);
            m_FireTimer.Start(m_ShootDelay);

            m_RandomizeDirectionTimer.Start(m_RandomSelectMovePointTime);                     
        }

        private void UpdateTimers()
        {
            m_RandomizeDirectionTimer.RemoveTime(Time.deltaTime);
            m_FireTimer.RemoveTime(Time.deltaTime);
            m_FindNewTargetTimer.RemoveTime(Time.deltaTime);
        }
        
        public void SetPatrolPoint(AIPointPatrol patrolPoint)
        {
            m_AIBehaviour = AIBehaviour.Patrol;
            m_PatrolPoint = patrolPoint;
        }
        #endregion
    }
}
