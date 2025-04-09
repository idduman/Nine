using Garawell.Managers;
using Garawell.Managers.Events;
using Garawell.Managers.Menu;
using UnityEngine;

public class SettingsPanel : PanelManager
{
    [SerializeField] private Animator animator;
    [SerializeField] private ECToggle soundToggle;
    [SerializeField] private ECToggle musicToggle;
    [SerializeField] private ECToggle vibToggle;

    public override void Initialize()
    {
        base.Initialize();
        soundToggle.Setup(MainManager.Instance.SettingsManager.SoundFx, OnSoundChange);
        musicToggle.Setup(MainManager.Instance.SettingsManager.Music, OnMusicChange);
        vibToggle.Setup(MainManager.Instance.SettingsManager.VibOn, OnVibrationChange);
    }

    public void Open()
    {
        Appear();
        Taptic.Medium();
        animator.Play("Open", 0, 0f);
        TimeManager.SetTime(0, 0.3f);
        MainManager.Instance.EventManager.InvokeEvent(EventTypes.OnPause);
        //MainManager.Instance.AudioManager.PlayAudio(AudioID.NextLevel);
    }

    public void Exit()
    {
        Disappear();
        Taptic.Medium();
        animator.Play("Close", 0, 0f);
        TimeManager.SetTime(1, 0.3f);
        MainManager.Instance.EventManager.InvokeEvent(EventTypes.OnResume);
        //MainManager.Instance.AudioManager.PlayAudio(AudioID.NextLevel);
    }

    public void OnSoundChange(bool value)
    {
        MainManager.Instance.AudioManager.SetSoundVolume(value ? 0f : -80f);
        MainManager.Instance.SettingsManager.SoundFx = value;
        Taptic.Medium();
        //MainManager.Instance.AudioManager.PlayAudio(AudioID.NextLevel);
    }

    public void OnMusicChange(bool value)
    {
        MainManager.Instance.SettingsManager.Music = value;
        MainManager.Instance.AudioManager.PlayAudio(AudioID.NextLevel);
        Taptic.Medium();
    }

    public void OnVibrationChange(bool value)
    {
        Taptic.tapticOn = value;
        Taptic.Medium();
        MainManager.Instance.AudioManager.PlayAudio(AudioID.NextLevel);

        MainManager.Instance.SettingsManager.ChangeVib(value);
        MainManager.Instance.EventManager.InvokeEvent(EventTypes.VibrationChange, new BoolArgs(value));
    }

    public void Privacy()
    {

    }

    public void Rating()
    {

    }

    public void Restart()
    {
        Taptic.Medium();
        //MainManager.Instance.AudioManager.PlayAudio(AudioID.NextLevel);
        MainManager.Instance.EventManager.InvokeEvent(EventTypes.OnRestartClick);
        EventRunner.LoadSceneStart();
        Exit();
    }
}
