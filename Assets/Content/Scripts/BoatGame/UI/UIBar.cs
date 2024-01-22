using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIBar : MonoBehaviour
    {
        [SerializeField] private TMP_Text displayText;
        [SerializeField] private TMP_Text expText;

        [SerializeField] private RectTransform value;
        [SerializeField] private RectTransform backgroundValue;
        private float maxValue;


        public virtual void Init(string text, float num, float maxValue)
        {
            this.maxValue = maxValue;
            displayText.text = text;
            
            UpdateBar(num);
        }

        public void UpdateBar(float num)
        {
            value.gameObject.SetActive((num / maxValue) != 0);
            value.ChangeSizeDeltaX((num / maxValue)*backgroundValue.sizeDelta.x);
            expText.text = num.ToString("F0") + "/" + maxValue;
        }
    }
}