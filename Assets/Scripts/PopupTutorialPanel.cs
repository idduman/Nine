using System;
using DG.Tweening;
using Garawell.Managers.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class PopupTutorialPanel : PanelManager
{
    [SerializeField] private Transform _popupPanel;
    [SerializeField] private GameObject _videoImage;
    [SerializeField] private GameObject _closeButton;
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private TMP_Text _caption;
    [SerializeField] private VideoClip[] _videos;
    [SerializeField] private string[] _texts;

    private int _stage = 0;

    public override void Initialize()
    {
        base.Initialize();
        gameObject.SetActive(false);
    }

    public override void Appear(EventArgs eventArgs = null)
    {
        if (_stage >= _videos.Length || _stage > _texts.Length)
            return;
        
        base.Appear(eventArgs);
        _closeButton.SetActive(false);
        _popupPanel.localScale = Vector3.zero;
        _popupPanel.DOScale(Vector3.one, 0.5f)
            .SetDelay(0.25f)
            .SetEase(Ease.Linear)
            .OnComplete(StartVideo);
    }
    
    public void SetTutorialStage(int stage)
    {
        if (stage >= _videos.Length || stage > _texts.Length)
            return;
        
        _stage = stage;
        _videoPlayer.clip = _videos[_stage];
        _caption.text = _texts[_stage];
    }

    public void HandleCloseTutorial()
    {
        PlayerPrefs.SetInt($"PTutorialCompleted{_stage}", 1);
        Taptic.Medium();
        _closeButton.SetActive(false);
        _videoPlayer.Pause();
        _popupPanel.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _videoPlayer.Stop();
                _videoImage.SetActive(false);
                Disappear();
            });

    }

    private void StartVideo()
    {
        _videoImage.SetActive(true);
        _videoPlayer.Play();
        _closeButton.SetActive(true);
    }
}
