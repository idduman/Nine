using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SagaStage : MonoBehaviour
{
	public event Action OnTapStage;

	[SerializeField] private GameObject difficultyGroup;
	[SerializeField] private Image _stageImage;
	[SerializeField] private Button _selectStageButton;
	[SerializeField] private GameObject _badgeContainer;
	[SerializeField] private GameObject _glowEffect;
	[SerializeField] private Image _badgeImage;
	[SerializeField] private Image _currentLevelMarkImage;
	[SerializeField] private TextMeshProUGUI _levelNumber;
	[SerializeField] private Image _topPathPartImage;
	[SerializeField] private Vector2 _notCurrentLevelBadgeSize;

	private RectTransform _rectTransform;
	private RectTransform _levelSubstrateRectTransform;

	public void Setup(Sprite stageSprite,
		Sprite topPathPartSprite,
		string levelNumber,
		float yPosition,
		bool currentLevel,
		bool hardLevel,
		Color difficultyColor)
	{
		_stageImage.sprite = stageSprite;
		_levelSubstrateRectTransform.anchoredPosition = new Vector2(_levelSubstrateRectTransform.anchoredPosition.x, yPosition);
		_levelNumber.text = levelNumber;
		
		_selectStageButton.enabled = currentLevel;
		//_glowEffect.SetActive(currentLevel);
		_topPathPartImage.sprite = topPathPartSprite;
		_currentLevelMarkImage.gameObject.SetActive(currentLevel);
		difficultyGroup.SetActive(hardLevel);
		difficultyGroup.GetComponent<Image>().color = difficultyColor;
	}

	public void SetupEmpty()
	{
		difficultyGroup.SetActive(false);
		_selectStageButton.enabled = false;
		_stageImage.gameObject.SetActive(false);
		_currentLevelMarkImage.gameObject.SetActive(false);
		_levelNumber.gameObject.SetActive(false);
		_rectTransform.sizeDelta /= 2;
		RectTransform topPathPartRectTransform = _topPathPartImage.GetComponent<RectTransform>();
		topPathPartRectTransform.anchoredPosition = Vector3.zero;
		topPathPartRectTransform.sizeDelta = new Vector2(topPathPartRectTransform.sizeDelta.x, 238.27f);
	}

	private void SetBadge(Sprite badgeSprite, bool currentLevel)
	{
		if(badgeSprite == default)
		{
			_badgeContainer.SetActive(false);
		}
		else
		{
			_badgeImage.sprite = badgeSprite;
			_badgeContainer.SetActive(true);
		}

		if(!currentLevel)
		{
			_levelSubstrateRectTransform.sizeDelta = _notCurrentLevelBadgeSize;
		}
	}

	private void TapStageButton()
	{
		OnTapStage?.Invoke();
	}

	private void Awake()
	{
		_badgeImage.sprite = default;
		_selectStageButton.onClick.AddListener(TapStageButton);

		_rectTransform = gameObject.GetComponent<RectTransform>();
		_levelSubstrateRectTransform = _stageImage.GetComponent<RectTransform>();
	}
}