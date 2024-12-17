using UnityEngine;

namespace SpaceShooter
{
    public class ColisionDamageApplicator : MonoBehaviour
    {
        public static string IgnoreTag = "WorldBoundary";

        [SerializeField] private float m_VelosityDamageModifier;

        [SerializeField] private float m_DamageConstant;

        private void OnCollisionEnter2D(Collision2D collision)
        {

            if (collision.transform.CompareTag(gameObject.tag) || collision.transform.CompareTag("IgnoreCollision"))

                return;

            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("IgnoreCollision"))
            {
                return;
            }

            var destructable = transform.root.GetComponent<Destructible>();

            if (destructable != null)
            {
                destructable.ApplyDamage((int)m_DamageConstant + 
                                                      (int)(m_VelosityDamageModifier * collision.relativeVelocity.magnitude));
            }
        }
    }
}