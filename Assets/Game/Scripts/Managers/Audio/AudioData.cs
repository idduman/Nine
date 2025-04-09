using UnityEngine;

namespace Garawell.Managers.Audio
{
	[System.Serializable]
	public class AudioData
	{
		private const float MIN_TIME = .5f;

		private AudioSource source;
		[SerializeField] private string audioName;
		[SerializeField] private AudioClip clip;
		[SerializeField] private float volume;
		[Tooltip("Default Pitch for non increase mode")]
		[SerializeField] private float minPitch;
		[SerializeField] private float maxPitch;
		[SerializeField] private float pitchIncrementValue;
		[SerializeField] private bool picthIncreaseMode;
		[SerializeField] private bool isMusic;


		private float currentPitch;
		private float lastTime;

		public string AudioName { get => audioName; }

		public bool IsMusic => isMusic;
		public AudioClip Clip => clip;

		public void Initialize(AudioSource audioSource)
		{
			lastTime = Time.time;
			source = audioSource;
			source.pitch = minPitch;
			currentPitch = 0;
			source.volume = volume;
		}

		public void PlaySound()
		{
			if (picthIncreaseMode)
			{
				if (Time.time - lastTime < MIN_TIME)
				{
					currentPitch += pitchIncrementValue;
					source.pitch = Mathf.Lerp(minPitch, maxPitch, currentPitch);
				}
				else
				{
					Reset();
				}
				lastTime = Time.time;
			}
			else
			{
				source.pitch = Random.Range(minPitch, maxPitch);
			}
			source.PlayOneShot(clip, volume);
		}

		public void Reset()
		{
			currentPitch = 0;
			source.pitch = minPitch;
		}
	}
}
