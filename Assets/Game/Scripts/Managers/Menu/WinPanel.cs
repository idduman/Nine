using System;
using Garawell.Managers.Events;
using Nine;
using TMPro;
using UnityEngine;

namespace Garawell.Managers.Menu
{
    public class WinPanel : PanelManager
    {
        [SerializeField] private bool isCoinAnimationActive = true;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private CoinAttraction coinAttraction;
        
        public override void Initialize()
        {
            base.Initialize();
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        public void HandleNextLevel()
        {
            Disappear();
            EventRunner.LoadSceneStart(true);
            MainManager.Instance.AudioManager.PlayAudio(AudioID.NextLevel);
            Taptic.Medium();
        }

        public override void Appear(EventArgs eventArgs = null)
        {
            base.Appear(eventArgs);
            levelText.text = NineManager.Instance.TutorialMode
                ? "Tutorial"
                : "Level " + PlayerPrefs.GetInt("Level");

            if (isCoinAnimationActive)
            {
                // Create coin animation
                Vector2 target = MainManager.Instance.MenuManager.GamePanel.currencyPanels[0]
                    .GetComponent<CurrencyUI>().currencyImage.transform.position;
                CoinAttraction coin = Instantiate(coinAttraction, transform);
                coin.Target.transform.position = target;
            }
        }
    }
}