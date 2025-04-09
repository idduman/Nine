using UnityEngine;
using UnityEngine.UI;

namespace Garawell.Utility
{
    public class FillPanel : MonoBehaviour
    {
        public Image fillImage;

        public virtual void SetFill(float fillAmount)
        {
            fillImage.fillAmount = fillAmount;
        }
    }
}

