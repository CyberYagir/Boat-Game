using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.Map.UI;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageFightsDungeonItem : MonoBehaviour
    {
        [SerializeField] private UIMark mark;
        [SerializeField] private TMP_Text text;

        private UIVillageFightsSubWindow.DungeonDataHolder data;
        private UIVillageFightsSubWindow fightsWindow;

        public void Init(UIVillageFightsSubWindow.DungeonDataHolder data, UIVillageFightsSubWindow fightsWindow)
        {
            this.fightsWindow = fightsWindow;
            this.data = data;
            mark.Init(data.Level);
            text.text = data.Name;
        }

        public void OnClick()
        {
            fightsWindow.EnterDungeon(data);
        }
    }
}
