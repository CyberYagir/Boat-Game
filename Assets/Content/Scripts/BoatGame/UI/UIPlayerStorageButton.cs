using Content.Scripts.Global;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIPlayerStorageButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        private SaveDataObject saveDataObject;

        public void Init(SaveDataObject saveDataObject)
        {
            this.saveDataObject = saveDataObject;
            saveDataObject.PlayerInventory.OnChangePlayerStorage.AddListener(UpdateButtonState);
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            gameObject.SetActive(saveDataObject.PlayerInventory.PlayerStorageItems.Count != 0);
            text.text = saveDataObject.PlayerInventory.PlayerStorageItems.Count.ToString();
            
            text.gameObject.SetActive(saveDataObject.PlayerInventory.PlayerStorageItems.Count != 0);
        }
    }
}
