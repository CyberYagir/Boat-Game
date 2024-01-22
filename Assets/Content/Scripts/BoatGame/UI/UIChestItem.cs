using Content.Scripts.ItemsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIChestItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;
        private RaftStorage targetStorage;
        private ItemObject targetItem;

        public void Init(RaftStorage targetStorage, ItemObject targetItem, int value)
        {
            this.targetItem = targetItem;
            this.targetStorage = targetStorage;
            
            
            image.sprite = targetItem.ItemIcon;
            text.text = "x" + value;
        }


        public void DropItem()
        {
            targetStorage.RemoveFromStorage(targetItem);
        }
    }
}
