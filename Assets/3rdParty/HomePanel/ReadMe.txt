------In MenuManager,
->  add
[SerializeField] private PanelManager mainMenuPanel;

->  add in Initialize
mainMenuPanel.Initialize();

->  add in LevelStart Function
mainMenuPanel.Disappear();

->  add in public IEnumerator PostLevelRestart() function
	mainMenuPanel.Appear();

------In GameManager
->  Remove from public void LevelLoaded(EventArgs args) function
	EventRunner.LevelStart();

------PanelManager
->  add into public IEnumerator DisappearRoutine()
	canvasGroup = GetComponent<CanvasGroup>();

	--- In Editor, set Main Menu Panel variable to Menu Manager Component

Check Playbutton events in editor !!