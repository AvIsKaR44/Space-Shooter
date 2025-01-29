using UnityEngine;


namespace Common
{
    public abstract class ProjectileBase : Entity
    {
        [SerializeField] private float m_Velocity;

        [SerializeField] private float m_LifeTime;

        [SerializeField] private int m_Damage;

        
        protected virtual void OnHit(Destructible destructible) { }
        protected virtual void OnHit(Collider2D collider2) { }
        protected virtual void OnProjectileLifeEnd(Collider2D col, Vector2 pos) { }

        private float m_Timer;        
        protected Destructible m_Parent;

       
        public float GetVelocity()
        {
            return m_Velocity;
        }

        void Update()
        {
            float stepLenght = Time.deltaTime * m_Velocity;
            Vector2 step = transform.up * stepLenght;
                          
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, stepLenght);

            if (hit)
            {
                OnHit(hit.collider);

                Destructible dest = hit.collider.transform.root.GetComponent<Destructible>();

                if (dest != null && dest != m_Parent)
                {
                    dest.ApplyDamage(m_Damage);

                    OnHit(dest);
                }

                OnProjectileLifeEnd(hit.collider, hit.point);
            }

            m_Timer += Time.deltaTime;

            if (m_Timer > m_LifeTime)
                OnProjectileLifeEnd(hit.collider, hit.point);


            transform.position += new Vector3(step.x, step.y, 0);
        }    
       
        public void SetParentShooter(Destructible parent)
        {
            m_Parent = parent;
        }        
    }
}