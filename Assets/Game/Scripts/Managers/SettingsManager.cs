using UnityEngine;
using Garawell.Managers.Events;

namespace Garawell.Managers
{
    [CreateAssetMenu(fileName = "SettingsManager", menuName = "Scriptlable Objects/Settings Manager")]
    public class SettingsManager : ScriptableObject
    {
        private bool vibOn = true;
        private bool soundFx = true;
        private bool music = true;

        public bool VibOn { get => vibOn; }
        public bool SoundFx
        {
            get => soundFx;
            set
            {
                PlayerPrefs.SetInt("SoundFxState", value ? 1 : 0);
                soundFx = value;
                MainManager.Instance.AudioManager.SetSoundVolume(value ? 0 : -80, 0.5f);
            }
        }

        public bool Music
        {
            get => music;
            set
            {
                PlayerPrefs.SetInt("MusicState", value ? 1 : 0);
                music = value;
                MainManager.Instance.AudioManager.SetMusicVolume(value ? 0 : -80, 0.5f);

            }
        }

        public void Initialize()
        {
            vibOn = PlayerPrefs.GetInt("VibrationState", 1) == 1;
            soundFx = PlayerPrefs.GetInt("SoundFxState", 1) == 1;
            music = PlayerPrefs.GetInt("MusicState", 1) == 1;
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.VibrationChange, new BoolArgs(vibOn));
        }

        public void ChangeVib(bool newVib)
        {
            vibOn = newVib;
            PlayerPrefs.SetInt("VibrationState", newVib ? 1 : 0);
            EventRunner.ChangeVibMode(vibOn);
        }
    }
}