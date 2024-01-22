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

        public void UpdateItem(int value)
        {
            count.text = $"<size=40>{value}</size>/<size=20>{maxValue}";
        }
    }
}
