using UnityEngine;


namespace SpaceShooter
{
    public class PowerupWeapon : Powerup
    {
        [SerializeField] private TurretProperties m_Properties;
        protected override void OnPickedUp(SpaceShip ship)
        {
            Debug.Log("Powerup picked up: " + m_Properties.name);
            ship.AssignWeapon(m_Properties);
            ship.AddAmmo(m_Properties.AmmoUsage);// добавление ракет в инвентарь
        }
    }
}