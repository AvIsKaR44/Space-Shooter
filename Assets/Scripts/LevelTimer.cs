using UnityEngine;
using UnityEngine.UI;


namespace SpaceShooter
{
    /// <summary>
    /// ����������� �� ������� �� ����������� ������
    /// </summary>
    public class LevelTimer : MonoBehaviour
    {
        [SerializeField] private float timeLimit = 300.0f;
        [SerializeField] private Text timerText;

        private float currentTime;
        private bool IsTimerRunning = false;

        private void Start()
        { 
            currentTime = timeLimit;
            UpdateTimerText();
            StartTimer();
        }

        private void Update()
        {
            if (IsTimerRunning)
            {
                currentTime -= Time.deltaTime;
                UpdateTimerText();

                if (currentTime <= 0)
                {
                    currentTime = 0;
                    IsTimerRunning = false;
                    OnTimerEnd();
                }
            }
        }

        private void UpdateTimerText()
        {
            if (timerText != null)
            {
                timerText.text = "�����: " + Mathf.CeilToInt(currentTime).ToString();
            }
        }

        private void OnTimerEnd()
        {
            Debug.Log("Time`s up! Game Over."); // ������ ��� ��������� ����� �������
        }

        public void StartTimer()
        {
            IsTimerRunning = true;
        }

        public void StopTimer()
        {
            IsTimerRunning = false;
        }
    }  
}