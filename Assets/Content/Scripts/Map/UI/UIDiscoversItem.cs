using System;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.Global;
using TMPro;
using UnityEngine;

namespace Content.Scripts.Map.UI
{
    public class UIDiscoversItem : MonoBehaviour
    {
        [SerializeField] private UIMark markIcon;
        [SerializeField] private TMP_Text text;
        private UIMessageBoxManager messageBoxManager;
        private UIDiscoversTooltip discoversTooltip;
        private SaveDataObject.MapData.IslandData island;


        public void Init(IslandSeedData islandData, SaveDataObject.MapData.IslandData island, UIMessageBoxManager uiMessageBoxManager, UIDiscoversTooltip discoversTooltip)
        {
            this.island = island;
            this.discoversTooltip = discoversTooltip;
            messageBoxManager = uiMessageBoxManager;
            markIcon.Init(islandData, String.Empty);
            text.text = island.IslandName;
        }

        public void MoveToIsland()
        {
            messageBoxManager.ShowMessageBox("Want to spend a paddle to get to the island?", OkAction);
        }

        private void OkAction()
        {
            discoversTooltip.GoToIsland(island.IslandSeed);
        }
    }
}
