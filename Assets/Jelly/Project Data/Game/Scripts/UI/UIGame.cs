using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Watermelon.JellyMerge;

namespace Watermelon
{
    public class UIGame : UIPage
    {
        [SerializeField] TMP_Text levelText;
        [SerializeField] Button skipLevelButton;
        [SerializeField] Button restartButton;

        [Space]
        [SerializeField] GameObject tutorialPanel;
        [SerializeField] GameObject horizontalTutorialPanel;
        [SerializeField] GameObject verticalTutorialPanel;

        public override void Initialise()
        {
            restartButton.onClick.AddListener(RestartButton);
            skipLevelButton.onClick.AddListener(SkipLevelButton);
        }

        private void OnEnable()
        {
            AdsManager.OnRewardedAdLoadedEvent += OnRewardedAdLoaded;
        }

        private void OnDisable()
        {
            AdsManager.OnRewardedAdLoadedEvent -= OnRewardedAdLoaded;
        }

        private void OnRewardedAdLoaded()
        {
            skipLevelButton.gameObject.SetActive(true);
        }

        public void ShowTutorialPanel(bool horizontal)
        {
            tutorialPanel.SetActive(true);

            if (horizontal)
            {
                horizontalTutorialPanel.SetActive(true);
                verticalTutorialPanel.SetActive(false);
            }
            else
            {
                horizontalTutorialPanel.SetActive(false);
                verticalTutorialPanel.SetActive(true);
            }
        }

        public void HideTutorialPanel()
        {
            tutorialPanel.SetActive(false);
            verticalTutorialPanel.SetActive(false);
            horizontalTutorialPanel.SetActive(false);
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            levelText.text = "LEVEL " + (GameController.CurrentLevelIndex + 1);
            skipLevelButton.gameObject.SetActive(AdsManager.IsRewardBasedVideoLoaded());

            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            UIController.OnPageClosed(this);
        }

        #endregion

        #region Buttons

        public void RestartButton()
        {
            GameController.Restart();

            AudioController.PlaySound(AudioController.Sounds.buttonSound);
        }

        public void SkipLevelButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
            // Lấy Token hiện tại từ PlayerPrefs (mặc định là 0 nếu chưa lưu)
            int currentToken = PlayerPrefs.GetInt("Token", 0);
            // Kiểm tra nếu Token <= 0
            if (currentToken <= 0)
            {
                Debug.Log("Token is 0 or less. Cannot subtract.");
                return;
            }
            // Trừ 1 Token
            currentToken -= 1;
            // Lưu lại giá trị mới vào PlayerPrefs
            PlayerPrefs.SetInt("Token", currentToken);
            PlayerPrefs.Save();
            LevelController.SkipLevel();
            // In ra log để kiểm tra
            Debug.Log($"Token subtracted. New Token amount: {currentToken}");
            return; // Trừ thành công
        }

        #endregion
    }
}
