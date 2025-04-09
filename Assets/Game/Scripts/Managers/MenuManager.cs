using System.Collections;
using System;
using Garawell.Managers.Events;
using UnityEngine;
using Garawell.Utility;
using Garawell.Managers.Menu;
using TMPro;

namespace Garawell.Managers
{
	public class MenuManager : MonoBehaviour 
	{
		[SerializeField] private GamePanel gamePanel;
		[SerializeField] private TutorialPanel tutorialPanel;
		[SerializeField] private LoadingPanel loadingPanel;
		[SerializeField] private SettingsPanel settingsPanel;
		[SerializeField] private PanelManager failPanel;
		[SerializeField] private PanelManager successPanel;
        [SerializeField] private PanelManager mainMenuPanel;
        [SerializeField] private PopupTutorialPanel popupTutorialPanel;

        public TextMeshProUGUI[] totalcurrencies;
		private int currencycount;

		private bool youWin;
		public bool YouWin { get => youWin; }
		private bool gameOver;
		private bool isInitialized;
		private bool isEnded = false;

		private float timer;
		public float Timer { get => Convert.ToInt32(timer); }

        public GamePanel GamePanel { get => gamePanel; }
        public PanelManager FailPanel { get => failPanel; }
        public PanelManager SuccessPanel { get => successPanel; }


        private void Awake()
        {
			//SceneManager.LoadScene("ManagerScene", LoadSceneMode.Additive);
		}

        private void Update()
        {
            if (isInitialized && !isEnded)
            {
                timer += Time.deltaTime;
            }
        }

        public void Initialize()
		{
			if(!isInitialized)
            {
				gamePanel.Initialize();
				tutorialPanel.Initialize();
				settingsPanel.Initialize();
				failPanel.Initialize();
				successPanel.Initialize();
				loadingPanel.Initialize();
                mainMenuPanel.Initialize();
                popupTutorialPanel.Initialize();

                MainManager.Instance.EventManager.Register(EventTypes.LevelStart, LevelStart);
				MainManager.Instance.EventManager.Register(EventTypes.LevelSuccess, LevelSuccess);
				MainManager.Instance.EventManager.Register(EventTypes.LevelFail, LevelFail);
				MainManager.Instance.EventManager.Register(EventTypes.LevelRestart, LevelRestart);
				MainManager.Instance.EventManager.Register(EventTypes.LevelFinish, LevelFinish);
				MainManager.Instance.EventManager.Register(EventTypes.LoadSceneStart, LoadingStart);
				MainManager.Instance.EventManager.Register(EventTypes.LoadSceneFinish, LoadingFinish);
				MainManager.Instance.EventManager.Register(EventTypes.CurrencyEarned, CurrencyGainedInLevel);
				MainManager.Instance.EventManager.Register(EventTypes.PopupTutorial, OnPopupTutorial);
				isInitialized = true;
			}
			
			if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 0)
			{
				mainMenuPanel.Disappear();
			}
		}

		private void OnPopupTutorial(EventArgs args)
		{
			if (args is not IntArgs intArgs)
				return;
			
			popupTutorialPanel.SetTutorialStage(intArgs.value);
			popupTutorialPanel.Appear();
		}

		public void CurrencyGainedInLevel(EventArgs args)
        {
	        Debug.Log("GAINED");
	        var amount = args as CurrencyArgs;
	        currencycount += amount.changeAmount;
	        for (int i = 0; i < totalcurrencies.Length; i++)
	        {
		        totalcurrencies[i].text = "+ "+ currencycount;

	        }
        }

        public void LevelStart(EventArgs args)
		{
            //MainManager.Instance.joystick.gameObject.SetActive(true);
            mainMenuPanel.Disappear();
            youWin = false;
			gameOver = false;
			isEnded = false;
		}
        
		public void LevelFail(EventArgs args)
		{
			//MainManager.Instance.joystick.gameObject.SetActive(false);
			Debug.Log("Fail");
			if (youWin || gameOver)
			{
				return;
			}
			isEnded = true;
			gameOver = true;
			gamePanel.settingsButton.SetActive(false);
			Delayer.Delay(MainManager.Instance.EditorGameSettings.failPanelAppearTime, () =>
			{
				gamePanel.Disappear();
				failPanel.Appear();
			});
		}

		public void LevelSuccess(EventArgs args)
		{
			//MainManager.Instance.joystick.gameObject.SetActive(false);
			Debug.Log("Success");
			if (youWin || gameOver)
			{
				return;
			}
            isEnded = true;
            youWin = true;
			gamePanel.settingsButton.SetActive(false);
			Delayer.Delay(MainManager.Instance.EditorGameSettings.successPanelAppearTime, () =>
			{
				gamePanel.Disappear();
				successPanel.Appear();
			});
		}
		
		public void LevelFinish(EventArgs args)
		{
			StartCoroutine(PostLevelRestart());
		}

		public void LevelRestart(EventArgs args)
        {
			StartCoroutine(PostLevelRestart());
        }

		public IEnumerator PostLevelRestart()
        {
			yield return new WaitForSecondsRealtime(.5f);
			failPanel.Disappear();
			successPanel.Disappear();
			gamePanel.Appear();
			if (PlayerPrefs.GetInt("TutorialCompleted", 0) != 0)
			{
				mainMenuPanel.Appear();
			}
			else
			{
				mainMenuPanel.Disappear();
				MainManager.Instance.GameManager.StartGame(null);
			}
            mainMenuPanel.Appear();
            currencycount = 0;
        }

		public void LoadingStart(EventArgs args)
		{
	        loadingPanel.Appear(args);
        }

		public void LoadingFinish(EventArgs args)
        {
			loadingPanel.Disappear();
			
			if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 0)
			{
				MainManager.Instance.GameManager.StartGame(null);
			}
        }
	}
	public enum ControllerType
	{
		None,
		Tap,
		HoldDown,
		Swipe,
		Swerve,
		HorizontalSwerve,
	}
}



