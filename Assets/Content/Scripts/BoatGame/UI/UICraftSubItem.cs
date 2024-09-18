using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UICraftSubItem : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text count;
        private int maxValue;

        public void Init(int maxValue, Sprite item)
        {
            this.maxValue = maxValue;
            icon.sprite = item;
        }

        public void DrawValues(int value)
        {
            count.text = $"{value}";
        }

        public void UpdateItem(int value)
        {
            count.alpha = value < maxValue ? 0.6f : 1;
            count.text = $"<size=45>{value}</size>/<size=25>{maxValue}";
        }
    }
}
