using System;
using Garawell.Managers.Events;
using TMPro;
using UnityEngine;

namespace Garawell.Managers.Menu
{
    public class FailPanel : PanelManager
    {
        [SerializeField] private TextMeshProUGUI levelText;
        
        public override void Initialize()
        {
            base.Initialize();
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
        public void HandleRestartButton()
        {
            Disappear();
            EventRunner.LoadSceneStart();
            //MainManager.Instance.AudioManager.PlayAudio(AudioID.NextLevel);
            Taptic.Medium();
        }

        public override void Appear(EventArgs eventArgs = null)
        {
            base.Appear(eventArgs);
            MainManager.Instance.AudioManager.PlayAudio(AudioID.Lose);
            levelText.text = "Level " + (PlayerPrefs.GetInt("Level") + 1);
        }
    }
}