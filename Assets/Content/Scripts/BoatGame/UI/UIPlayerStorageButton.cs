using System.Linq;
using Content.Scripts.Global;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIPlayerStorageButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        private int itemsCount;
        private SaveDataObject saveDataObject;

        
        
        public void Init(SaveDataObject saveDataObject)
        {
            this.saveDataObject = saveDataObject;
            saveDataObject.PlayerInventory.OnChangePlayerStorage.AddListener(UpdateButtonState);
            itemsCount = saveDataObject.PlayerInventory.PlayerStorageItems.Sum(x=>x.Count);
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            gameObject.SetActive(saveDataObject.PlayerInventory.PlayerStorageItems.Count != 0);
            text.text = saveDataObject.PlayerInventory.PlayerStorageItems.Count.ToString();
            
            text.gameObject.SetActive(saveDataObject.PlayerInventory.PlayerStorageItems.Count != 0);
            
            var newItems  = saveDataObject.PlayerInventory.PlayerStorageItems.Sum(x=>x.Count);
            if (newItems > itemsCount)
            {
                transform.DOKill();
                transform.localScale = Vector3.one;
                transform.DOPunchScale(Vector3.one / 4f, 1f, 4).SetDelay(0.5f);

                itemsCount = newItems;
            }
        }
    }
}
