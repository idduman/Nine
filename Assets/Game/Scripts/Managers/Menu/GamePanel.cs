using System.Collections.Generic;
using TMPro;
using Garawell.Utility;
using System;
using DG.Tweening;
using Garawell.Managers.Events;
using Garawell.Managers.Currencies;
using Nine;
using UnityEngine;
using UnityEngine.UI;

namespace Garawell.Managers.Menu
{
    public class GamePanel : PanelManager
    {
        public GameObject settingsButton;
        public GameObject scorePanel;
        public GameObject goalPanel;
        public GameObject levelPanel;
        public GameObject[] tutorialTexts;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI nextLevelText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI maxScoreText;
        public List<CurrencyUI> currencyPanels = new List<CurrencyUI>();
        public FillPanel progressBarFillImage;
        public Slider _scoreSlider;
        public Transform currencyParent;
        public TextMeshProUGUI mergeText;

        private bool restartCooldown = true;
        private Coroutine restartCooldownRoutine;

        private Tween _scoreSliderTween;

        public override void Initialize()
        {
            base.Initialize();
            Appear();

            MainManager.Instance.EventManager.Register(EventTypes.CurrencySpent, SpendMoney);
            MainManager.Instance.EventManager.Register(EventTypes.CurrencyEarned, SpendMoney);
            MainManager.Instance.EventManager.Register(EventTypes.LevelFinish, LevelRestart);
            MainManager.Instance.EventManager.Register(EventTypes.LevelRestart, LevelRestart);
            MainManager.Instance.EventManager.Register(EventTypes.LevelLoaded, LevelLoaded);
            Currency[] currencies = MainManager.Instance.CurrencyManager.currencies;
            GameObject currencyPrefab = MainManager.Instance.AssetManager.GetPrefab("CurrencyUI");
            for (int i = 0; i < currencies.Length; i++)
            {
                currencyPanels.Add(Instantiate(currencyPrefab, currencyParent.position, currencyParent.rotation).GetComponent<CurrencyUI>());
                currencyPanels[i].transform.SetParent(currencyParent);
                currencyPanels[i].transform.localScale = Vector3.one;
                currencyPanels[i].transform.localPosition = Vector3.zero;

                currencyPanels[i].Initialize(currencies[i].currencySign, currencies[i].currencyAmount);
            }
        }

        public void SpendMoney(EventArgs cData)
        {
            var currencyData = cData as CurrencyArgs;
            Currency currency = MainManager.Instance.CurrencyManager.currencies[currencyData.currencyId];
            if(currencyData.addIncrementally)
            {
                currencyPanels[currencyData.currencyId].SetCurrencyIncremental(currency.currencyAmount);
            }
            else
            {
                currencyPanels[currencyData.currencyId].SetCurrency(currency.currencyAmount);
            }
        }

        public void LevelRestart(EventArgs args)
        {
            restartCooldown = true;
            for (int i = 0; i < tutorialTexts.Length; i++)
            {
                tutorialTexts[i].SetActive(false);
            }
        }

        public void LevelLoaded(EventArgs args)
        {
            if(levelText)
                levelText.text = "Level " + (PlayerPrefs.GetInt("Level") + 1);
            if(nextLevelText)
                nextLevelText.text = (PlayerPrefs.GetInt("Level") + 2).ToString();
            settingsButton.SetActive(true);
            restartCooldown = false;
            Currency[] currencies = MainManager.Instance.CurrencyManager.currencies;
            for (int i = 0; i < currencies.Length; i++)
            {
                currencyPanels[i].SetCurrency(currencies[i].currencyAmount);
            }
            for (int i = 0; i < tutorialTexts.Length; i++)
            {
                tutorialTexts[i].SetActive(false);
            }
        }

        public void ResetScore()
        {
            scoreText.text = "0";
            
            _scoreSlider.value = 0f;
        }

        public void SetScore(int score, int maxScore)
        {
            scoreText.text = $"{score.ToString()}";
            maxScoreText.text = $"{maxScore.ToString()}";
            
            var scoreValue =  Mathf.Clamp((float)score / maxScore, 0f, 1f);
            _scoreSliderTween.Kill();
            _scoreSliderTween = DOTween.To(() => _scoreSlider.value,
                x => _scoreSlider.value = x, scoreValue, 0.2f)
                .SetEase(Ease.Linear);
        }

        public void ToggleCurrencyPanel(bool active)
        {
            currencyParent.gameObject.SetActive(active);
        }

        public void ToggleLevelPanel(bool active)
        {
            levelPanel.SetActive(active);
            levelText.gameObject.SetActive(active);
        }

        public void ToggleScorePanel(bool active)
        {
            scorePanel.SetActive(active);
        }
        
        public void ToggleGoalPanel(bool active)
        {
            goalPanel.SetActive(active);
        }

        public void ToggleTutorialText(int index, bool active)
        {
            if (index > tutorialTexts.Length)
                return;
            
            tutorialTexts[index].SetActive(active);
        }
        
        
        public void MergeModeToggle()
        {
            NineManager.Instance.ToggleMergeMode();
            var mode = NineManager.Instance.MergeMode;
            mergeText.text = mode == MergeMode.Number ? "Number" : "Pair";
        }
    }
}

