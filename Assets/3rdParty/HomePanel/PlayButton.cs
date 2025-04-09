using Garawell.Managers;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public void StartLevel()
    {
        Taptic.Medium();
        //MainManager.Instance.AudioManager.PlayAudio(AudioID.NextLevel);
        MainManager.Instance.GameManager.StartGame(null);
    }
}
