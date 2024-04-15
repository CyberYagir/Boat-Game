using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIResourcesCounterItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;
        public void Init(Sprite icon, int value)
        {
            image.sprite = icon;
            text.text = value.ToString();
        }
    }
}
