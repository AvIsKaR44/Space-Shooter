using UnityEngine;


namespace SpaceShooter
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class Powerup : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            SpaceShip ship = collision.transform.root.GetComponent<SpaceShip>();

            if (ship != null && Player.Instance.ActiveShip)
            {
                Debug.Log("Powerup triggered by ship: " + ship.name);

                OnPickedUp(ship);

                Destroy(gameObject);
            }
        }

        protected abstract void OnPickedUp(SpaceShip ship);
    }
}