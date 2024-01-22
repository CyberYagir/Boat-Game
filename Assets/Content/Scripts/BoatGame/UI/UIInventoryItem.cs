using Content.Scripts.ItemsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIInventoryItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;
        [SerializeField] private ItemObject item;

        
        public ItemObject Item => item;


        public void Init(ItemObject item)
        {
            this.item = item;
            image.sprite = item.ItemIcon;
            text.text = item.ItemName;
        }
    }
}
