using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SagaMapUI : MonoBehaviour
{
	[SerializeField] private ScrollRect _scrollRect;
	[SerializeField] private RectTransform _stageContainer;
	[SerializeField] private SagaStage _sagaStagePrefab;
	[SerializeField] private int _levelCountDemonstration;

	[Space]
	[Header("Settings")]
	[SerializeField] private Sprite _completedLevelSubstrate;

	[SerializeField] private Sprite _currentLevelSubstrate;
	[SerializeField] private Sprite _lockedLevelSubstrate;
	[SerializeField] private Sprite _topPathSprite;
	[SerializeField] private Sprite _nextTopLevelPathSprite;
	[SerializeField] private Sprite _currentTopLevelPathSprite;
	[SerializeField] private Sprite _previousTopLevelPathSprite;
	[SerializeField] private Sprite _prepreviousTopLevelPathSprite;
	[SerializeField] private Sprite _bottomPathSprite;
	[SerializeField] private Material _previousLevelFontMaterial;
	[SerializeField] private Material _currentLevelFontMaterial;
	[SerializeField] private Material _nextLevelFontMaterial;
	[SerializeField] private float _nextLevelYPosition = -8.2f;
	[SerializeField] private float _currentLevelYPosition = 2.3f;
	[SerializeField] private float _previousLevelYPosition = -16.2f;
	[SerializeField] private int _emptyCellCount;
	[SerializeField] private Color difficultyGroupActiveColor, difficultyGroupPassiveColor;

	IEnumerator loadSagaButtonsCorotune;

	private SagaStage _currentLevelSagaStage;

    private void OnEnable()
    {
        GetComponent<CanvasGroup>().alpha = 0; // show after all stages are loaded

        var currentLevel = PlayerPrefs.GetInt("Level") + 1;

		int loopCount = currentLevel / 15;
		
		var minimalLevel = 1;
		var maximumLevel = (loopCount + 1) * 15;
		
		for (int index = maximumLevel; index >= minimalLevel; index--)
		{
			var sagaStage = Instantiate(_sagaStagePrefab, _stageContainer);
			bool hardLevel = false;
			
			sagaStage.Setup(
				GetLevelStageSubstrate(index, currentLevel),
				GetTopPathPartSprite(index, currentLevel),
				index.ToString(),
				GetLevelPosition(index, currentLevel),
				index == currentLevel, hardLevel,
				index == currentLevel ? difficultyGroupActiveColor : difficultyGroupPassiveColor);// hardLevel ? 0083E7 : 8A8A8A);
			
			if (index == currentLevel)
			{
				sagaStage.OnTapStage += OnTapStage;
				_currentLevelSagaStage = sagaStage;
			}
		}

		if (currentLevel < 4)
		{
			AddEmptyCells(_emptyCellCount);
		}

		ScrollWithDelay(0);
	}



	private void OnTapStage()
	{
		_currentLevelSagaStage.OnTapStage -= OnTapStage;
	}

	private float GetLevelPosition(int index, int currentLevel)
	{
		if(index > currentLevel)
		{
			return -8.2f;
		}
		else if(index == currentLevel)
		{
			return 2.3f;
		}
		else
		{
			return -16.2f;
		}
	}

	private Material GetFontMaterial(int index, int currentLevel)
	{
		if(index > currentLevel)
		{
			return _nextLevelFontMaterial;
		}
		else if(index == currentLevel)
		{
			return _currentLevelFontMaterial;
		}
		else
		{
			return _previousLevelFontMaterial;
		}
	}

	private Sprite GetBadgeSprite(int themeIndex, int previousThemeIndex)
	{
		if(themeIndex != previousThemeIndex)
		{
			return Resources.Load<Sprite>("ThemeImages/" + themeIndex);
		}
		else
		{
			return default;
		}
	}

	private Sprite GetTopPathPartSprite(int index, int currentLevel)
	{
		if(index == currentLevel)
		{
			return _currentTopLevelPathSprite;
		}
		else if(index < currentLevel && currentLevel - index == 1)
		{
			return _previousTopLevelPathSprite;
		}
		else if(index < currentLevel && currentLevel - index == 2)
		{
			return _prepreviousTopLevelPathSprite;
		}
		else if(index < currentLevel && currentLevel - index > 2)
		{
			return _bottomPathSprite;
		}
		else if(index > currentLevel && index - currentLevel == 1)
		{
			return _nextTopLevelPathSprite;
		}
		else if(index > currentLevel && index - currentLevel > 1)
		{
			return _topPathSprite;
		}
		else
		{
			return default;
		}
	}

	private void OnDisable()
	{
		ClearStageMap();
		if (loadSagaButtonsCorotune is not null)
			StopCoroutine(loadSagaButtonsCorotune);
	}

	private void AddEmptyCells(int emptyCellCount)
	{
		for(int index = 0; index < emptyCellCount; index++)
		{
			var sagaStage = Instantiate(_sagaStagePrefab, _stageContainer);
			sagaStage.SetupEmpty();
		}
	}

	private void ScrollWithDelay(float v)
	{
		StartCoroutine(ScrollWithDelayCoroutine(v));
	}

	private IEnumerator ScrollWithDelayCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		ScrollToElement(_currentLevelSagaStage);
	}

	private void ScrollToElement(SagaStage currentLevelSagaStage)
	{
		RectTransform contentRect = _scrollRect.content;
		RectTransform targetRect = currentLevelSagaStage.GetComponent<RectTransform>();

		Canvas.ForceUpdateCanvases();


		contentRect.anchoredPosition =
			(Vector2)_scrollRect.transform.InverseTransformPoint(contentRect.position)
			- (Vector2)_scrollRect.transform.InverseTransformPoint(targetRect.position);

        GetComponent<CanvasGroup>().alpha = 1; // show after all stages are loaded

        /*
		RectTransform target = categoryPositions.Find(x => x.HeaderType == headerType).Rect;
		Canvas.ForceUpdateCanvases();

		scroolTween?.Kill();
		scroolTween = scrollRect.content.DOAnchorPos(
				(Vector2)scrollRect.transform.InverseTransformPoint(scrollRect.content.position)
				- (Vector2)scrollRect.transform.InverseTransformPoint(target.position), _time).OnComplete(() => forceInProgress = false);
		*/
    }

    [ContextMenu("TestSnap")]
	private void ScrollToElementTest()
	{
		RectTransform contentRect = _scrollRect.content;
		RectTransform targetRect = _currentLevelSagaStage.GetComponent<RectTransform>();

		Canvas.ForceUpdateCanvases();


		contentRect.anchoredPosition =
			(Vector2)_scrollRect.transform.InverseTransformPoint(contentRect.position)
			- (Vector2)_scrollRect.transform.InverseTransformPoint(targetRect.position);


		/*
		RectTransform target = categoryPositions.Find(x => x.HeaderType == headerType).Rect;
		Canvas.ForceUpdateCanvases();

		scroolTween?.Kill();
		scroolTween = scrollRect.content.DOAnchorPos(
				(Vector2)scrollRect.transform.InverseTransformPoint(scrollRect.content.position)
				- (Vector2)scrollRect.transform.InverseTransformPoint(target.position), _time).OnComplete(() => forceInProgress = false);
		*/
	}

	private void ClearStageMap()
	{
		for(int index = 0; index < _stageContainer.childCount; index++)
		{
			Destroy(_stageContainer.GetChild(index).gameObject);
		}
	}

	private Sprite GetLevelStageSubstrate(int level, int currentLevel)
	{
		if(level > currentLevel)
		{
			return _lockedLevelSubstrate;
		}
		else if(level < currentLevel)
		{
			return _completedLevelSubstrate;
		}
		else
		{
			return _currentLevelSubstrate;
		}
	}

	private int CalculateMaximumLevel(int currentLevel)
	{
		return currentLevel + _levelCountDemonstration;
	}

	private int CalculateMinimalLevel(int currentLevel)
	{
		return Mathf.Clamp(currentLevel - _levelCountDemonstration, 1, currentLevel + _levelCountDemonstration);
	}
}