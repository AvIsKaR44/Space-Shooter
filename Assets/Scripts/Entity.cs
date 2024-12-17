using UnityEngine;


namespace SpaceShooter
{
    /// <summary>
    /// ������� ����� ���� ������������� ������� �������� �� �����
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        /// <summary>
        /// �������� ������� ��� ������������
        /// </summary>
        [SerializeField]
        public string m_Nickname;
        public string Nickname => m_Nickname;
    }
}
