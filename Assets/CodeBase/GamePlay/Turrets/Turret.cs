using UnityEngine;
using Common;

namespace SpaceShooter
{
    public class Turret : Projectile
    {
        [SerializeField] private TurretMode m_Mode;
        public TurretMode Mode => m_Mode;

        public TurretProperties GetTurretProperties()
        {
            return m_TurretProperties;
        }

        [SerializeField] private TurretProperties m_TurretProperties;

        private float m_RefireTimer;
        public bool CanFire => m_RefireTimer <= 0;

        private SpaceShip m_Ship;
        [SerializeField] private Transform[] m_Target; // Массив целей

        private bool m_SecondaryTurret;
        private float m_TurnSpeed;

        #region Unity Event
        private void Start()
        {
            m_Ship = transform.root.GetComponent<SpaceShip>();
        }
                 
        private void Update()
        {
            if (m_SecondaryTurret && m_Target != null && m_Target.Length > 0)
            {
                Transform closestTarget = FindClosestTarget();
                if (closestTarget != null)
                {
                    Vector2 direction = (Vector2)closestTarget.position - (Vector2)transform.position;
                    direction.Normalize();
                    transform.up = Vector3.Lerp(transform.up, direction, Time.deltaTime * m_TurnSpeed);
                }
            }

            if (m_RefireTimer > 0) 
            m_RefireTimer -= Time.deltaTime;
        }
        #endregion

        // Public API
        public void Fire()
        {
            if (m_TurretProperties == null)
            {
                return;
            }


            if (m_RefireTimer > 0) return;

            if (m_Ship.DrawAmmo(m_TurretProperties.AmmoUsage) == false)
            {                
                return;
            }

            if (m_Ship.DrawEnergy(m_TurretProperties.EnergyUsage) == false)
                return;            

            Projectile projectile = Instantiate(m_TurretProperties.ProjectilePrefab).GetComponent<Projectile>();
            projectile.transform.position = transform.position;
            projectile.transform.up = transform.up;

            projectile.SetParentShooter(m_Ship);

            // Цель для ракет
            if (m_SecondaryTurret && projectile is IHomingMissile)
            {               
               IHomingMissile missile = projectile as IHomingMissile;
               missile.SetTargets(new Transform[] { FindClosestTarget() }); // Передача массива целей                   
            }

            m_RefireTimer = m_TurretProperties.RateOfFire;

            {
                // SFX
            }
        }

        public void AssignLoadout(TurretProperties props)
        {
            if(m_Mode != props.Mode) return;

            m_RefireTimer = 0;
            m_TurretProperties = props;
            
        }

        private Transform FindClosestTarget()
        {
            Transform closestTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (Transform target in m_Target)
            {
                float distance = Vector3.Distance(transform.position, target.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }
    }
}