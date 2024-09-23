using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIPlayerStorageButton : MonoBehaviour
    {
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
        }
    }
}
