using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIPotionItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text counter;
        private RaftStorage.StorageItem item;

        public void SetItem(RaftStorage.StorageItem item)
        {
            this.item = item;

            image.sprite = item.Item.ItemIcon;
            if (item.Count > 1)
            {
                counter.text = "x" + item.Count;
            }
            else
            {
                counter.text = string.Empty;
            }
        }
    }
}
