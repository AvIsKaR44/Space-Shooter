using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class ResultPanel : MonoBehaviour
    {
        private const string PassedText = "Passed";
        private const string LoseText = "Lose";
        private const string RestartText = "Restart";
        private const string NextText = "Next";
        private const string MainMenuText = "Main menu";
        private const string KillsTextPrefix = "Kills";
        private const string ScoreTextPrefix = "Score";
        private const string TimeTextPrefix = "Time";



       [SerializeField] private Text m_Kills;
        [SerializeField] private Text m_Score;
        [SerializeField] private Text m_Time;
        [SerializeField] private Text m_Result;
        [SerializeField] private Text m_ButtonNextText;

        private bool m_LevelPassed = false;

        private void Start()
        {
            gameObject.SetActive(false);
            LevelController.Instance.LevelLost += OnLevelLost;
            LevelController.Instance.LevelPassed += OnLevelPassed;
        }

        private void OnDestroy()
        {
            if (LevelController.Instance != null)
            {
                LevelController.Instance.LevelLost -= OnLevelLost;
                LevelController.Instance.LevelPassed -= OnLevelPassed;
            }
        }

        private void OnLevelPassed()
        {
            Debug.Log("Level Passed Event Triggered");
            gameObject.SetActive(true);

            m_LevelPassed = true;

            FillLevelStatistics();

            m_Result.text = "Passed";

            if (LevelController.Instance.HasNextLevel == true)
            {
                m_ButtonNextText.text = "Next";
            }
            else
            {
                m_ButtonNextText.text = "Main Menu";
            }
        }

        private void OnLevelLost()
        {
            Debug.Log("Level Lost Event Triggered");
            gameObject.SetActive(true);

            FillLevelStatistics();

            m_Result.text = "Lose";
            m_ButtonNextText.text = "Restart";
        }



        private void FillLevelStatistics()
        {
            m_Kills.text = "Kills : " + Player.Instance.NumKills.ToString();
            m_Score.text = "Scores : " + Player.Instance.Score.ToString();
            m_Time.text = "Time : " + LevelController.Instance.LevelTime.ToString("F0");
        }

        public void OnButtonNextAction()
        {
            gameObject.SetActive(false);

            if (m_LevelPassed == true)
            {
                LevelController.Instance.LoadNextLevel();
            }
            else
            {
                LevelController.Instance.RestartLevel();
            }
        }
    }
}