using System.Collections.Generic;
using UnityEngine;
using Garawell.Managers.Audio;
using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

namespace Garawell.Managers
{
	public class AudioManager : MonoBehaviour
	{
		[SerializeField] private ScriptableAudio scriptableAudio;
		[SerializeField] private AudioID testAudioID;
		[SerializeField] private AudioMixerGroup soundMixerGroup;
		[SerializeField] private AudioMixerGroup musicMixerGroup;
		
		private Dictionary<AudioID, AudioData> soundDict = new Dictionary<AudioID, AudioData>();

		private bool isInitlialize;
		private Tween soundTween;
		private Tween musicTween;
		private Tween musicLowpassTween;
		private bool isStarted = false;

		public void Initialize()
		{
			if (!isInitlialize)
			{
				isInitlialize = true;
				foreach (AudioID foo in Enum.GetValues(typeof(AudioID)))
				{
					soundDict.Add(foo, scriptableAudio.AudioData[(int)foo]);
					AudioSource source = gameObject.AddComponent<AudioSource>();
					scriptableAudio.AudioData[(int)foo].Initialize(source);

					if (scriptableAudio.AudioData[(int) foo].IsMusic)
					{
						source.outputAudioMixerGroup = musicMixerGroup;
						source.loop = true;
						source.clip = scriptableAudio.AudioData[(int)foo].Clip;
						source.Play();
					}
					else
					{
						source.outputAudioMixerGroup = soundMixerGroup;
					}
				}
            }
			
			SetSoundVolume(MainManager.Instance.SettingsManager.SoundFx ? 0f : -80f);
			SetMusicVolume(MainManager.Instance.SettingsManager.Music ? 0f : -80f);
			SetMusicLowpass(100);
			
			MainManager.Instance.EventManager.Register(EventTypes.LevelStart, arg0 =>
			{
				MusicFreqHigh();
				isStarted = true;
			});
			MainManager.Instance.EventManager.Register(EventTypes.LevelFail, MusicFreqLow);
			MainManager.Instance.EventManager.Register(EventTypes.LevelSuccess, MusicFreqLow);
			MainManager.Instance.EventManager.Register(EventTypes.OnRestartClick, MusicFreqLow);
			MainManager.Instance.EventManager.Register(EventTypes.LevelLoaded, arg0 => isStarted = false);
			MainManager.Instance.EventManager.Register(EventTypes.OnPause, arg0 =>
			{
				if (isStarted)
					MusicFreqLow();
			});
			MainManager.Instance.EventManager.Register(EventTypes.OnResume, arg0 =>
			{
				if (isStarted)
					MusicFreqHigh();
			});
		}

		public void PlayAudio(AudioID audioEnum)
		{
		    soundDict[audioEnum].PlaySound();
		}

		[Button("Play Test Audio")]
		public void PlayTestAudio()
        {
			PlayAudio(testAudioID);
        }

		public void SetSoundVolume(float volume)
		{
			soundTween?.Kill();
			soundMixerGroup.audioMixer.SetFloat("SoundVolume", volume);
		}

		public void SetSoundVolume(float volume, float duration)
		{
			soundTween?.Kill();
			float vol = 0f;
			soundMixerGroup.audioMixer.GetFloat("SoundVolume", out vol);
			soundTween = DOTween.To(() => vol, x => soundMixerGroup.audioMixer.SetFloat("SoundVolume", x), volume, duration)
				.SetUpdate(true);
		}
		
		public void SetMusicVolume(float volume)
		{
			musicTween?.Kill();
			musicMixerGroup.audioMixer.SetFloat("MusicVolume", volume);
		}
		
		public void SetMusicVolume(float volume, float duration)
		{
			musicTween?.Kill();
			float vol = 0f;
			musicMixerGroup.audioMixer.GetFloat("MusicVolume", out vol);
			musicTween = DOTween.To(() => vol, x => musicMixerGroup.audioMixer.SetFloat("MusicVolume", x), volume, duration)
				.SetUpdate(true);
		}
		
		public void SetMusicLowpass(float cutoffFreq)
		{
			musicLowpassTween?.Kill();
			musicMixerGroup.audioMixer.SetFloat("MusicLowpass", cutoffFreq);
		}


		private void MusicFreqLow(EventArgs args = null)
		{
			SetMusicLowpass(100, 1);
		}
		private void MusicFreqHigh(EventArgs args = null)
		{
			SetMusicLowpass(5000, 1);
		}
		
		public void SetMusicLowpass(float cutoffFreq, float duration)
		{
			musicLowpassTween?.Kill();
			float freq = 0f;
			musicMixerGroup.audioMixer.GetFloat("MusicLowpass", out freq);
			musicLowpassTween = DOTween.To(() => freq, x => musicMixerGroup.audioMixer.SetFloat("MusicLowpass", x), cutoffFreq, duration)
				.SetUpdate(true);
		}
	}
}