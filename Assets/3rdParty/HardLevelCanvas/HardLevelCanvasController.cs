using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HardLevelCanvasController : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image hardLogo;
    [SerializeField] private Image hardLevelText;

    public void Start()
    {
        StartHardAnimation();
    }

    public void StartHardAnimation()
    {
        background.gameObject.SetActive(true); // open all

        float duration = 1f;
        hardLogo.transform.localScale = Vector3.one * 1.2f;
        hardLevelText.transform.localScale = Vector3.one * 1.2f;

        background.DOFade(1, duration);
        hardLogo.DOFade(1, duration);
        hardLevelText.DOFade(1, duration);

        hardLogo.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                GetComponent<CanvasGroup>().DOFade(0, 1f).SetDelay(duration)
                    .OnComplete(() => Destroy(gameObject));
            });
    }
}

