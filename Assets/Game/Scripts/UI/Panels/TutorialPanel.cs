using System;
using Garawell.Managers;
using Garawell.Managers.Menu;
using UnityEngine;

public class TutorialPanel : PanelManager
{
    public GameObject[] tutorialPanels;
    
    private int tutorialId;
    private bool _initalShape = true;
    private GamePanel _gamePanel;
    
    public override void Initialize()
    {
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
            return;

        base.Initialize();
        
        gameObject.SetActive(false);
        _initalShape = true;
        
        MainManager.Instance.EventManager.Register(EventTypes.LevelStart, OnLevelStart);
        
        tutorialId = 0;
        if (tutorialId != -1)
            tutorialPanels[tutorialId].SetActive(true);
        
        Appear();
    }

    private void OnLevelStart(EventArgs args)
    {
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
            return;
        
        tutorialId = 0;
        gameObject.SetActive(true);
        MainManager.Instance.EventManager.Register(EventTypes.OnShapePickedUp, OnShapePickedUp);
        MainManager.Instance.EventManager.Register(EventTypes.OnShapeReturned, OnShapeReturned);
        MainManager.Instance.EventManager.Register(EventTypes.OnShapeAdded, OnShapeAdded);
        MainManager.Instance.EventManager.Register(EventTypes.OnMoveCompleted, OnMoveCompleted);
        
        _gamePanel = MainManager.Instance.MenuManager.GamePanel;
        _gamePanel.ToggleTutorialText(tutorialId, true);
    }
    
    private void OnShapePickedUp(EventArgs args)
    {
        if (tutorialId >= tutorialPanels.Length)
            return;
        
        tutorialPanels[tutorialId].SetActive(false);
    }

    private void OnShapeReturned(EventArgs args)
    {
        if (tutorialId >= tutorialPanels.Length)
            return;
        
        tutorialPanels[tutorialId].SetActive(true);
    }
    
    private void OnShapeAdded(EventArgs args)
    {
        if (_initalShape)
        {
            _initalShape = false;
            return;
        }
        
        _gamePanel.ToggleTutorialText(tutorialId, false);
        if (tutorialId < tutorialPanels.Length - 1)
        {
            tutorialPanels[tutorialId].SetActive(false);
        }
        else
        {
            _gamePanel.ToggleCurrencyPanel(true);
            MainManager.Instance.EventManager.Unregister(EventTypes.OnShapeAdded, OnShapeAdded);
            PlayerPrefs.SetInt("TutorialCompleted", 1);
            gameObject.SetActive(false);
        }
        tutorialId++;
    }

    private void OnMoveCompleted(EventArgs args)
    {
        if (tutorialId >= tutorialPanels.Length)
        {
            return;
        }
        
        _gamePanel.ToggleTutorialText(tutorialId, true);
        tutorialPanels[tutorialId].SetActive(true);
    }
}
