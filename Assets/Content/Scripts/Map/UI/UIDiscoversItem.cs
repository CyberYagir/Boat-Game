using Content.Scripts.BoatGame.UI;
using TMPro;
using UnityEngine;

namespace Content.Scripts.Map.UI
{
    public class UIDiscoversItem : MonoBehaviour
    {
        [SerializeField] private UIMark markIcon;
        [SerializeField] private TMP_Text text;
        private UIMessageBoxManager messageBoxManager;

        public void Init(IslandSeedData islandData, string islandName, UIMessageBoxManager uiMessageBoxManager)
        {
            messageBoxManager = uiMessageBoxManager;
            markIcon.Init(islandData);
            text.text = islandName;
        }

        public void MoveToIsland()
        {
            messageBoxManager.ShowMessageBox("Want to spend a paddle to get to the island?", OkAction);
        }

        private void OkAction()
        {
            
        }
    }
}
