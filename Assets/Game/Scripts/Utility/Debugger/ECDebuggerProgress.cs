using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ECDebuggerProgress : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI title;

    public void Setup(string objectName, float maxValue, float value, bool showTitle)
    {
        slider.maxValue = maxValue;
        slider.value = value;
        if (showTitle)
        {
            title.text = objectName + " :";
        }
        
        title.gameObject.SetActive(showTitle);
    }

    public void SetValue(float value) => slider.value = value;
    
    public void SetMaxValue(float maxValue) => slider.maxValue = maxValue;
}
