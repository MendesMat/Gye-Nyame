using UnityEngine;

namespace GyeNyame.UI.Views
{
    public class UIHealthBar : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image fillImage;

        public void UpdateFill(float normalizedValue)
        {
            if (fillImage == null) return;

            fillImage.fillAmount = normalizedValue;
        }
    }
}
