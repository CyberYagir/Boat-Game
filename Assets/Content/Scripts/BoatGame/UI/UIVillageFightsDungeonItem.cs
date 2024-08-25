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
        [SerializeField] private UIBar progressBar;

        private UIVillageFightsSubWindow.DungeonDataHolder data;
        private UIVillageFightsSubWindow fightsWindow;

        public void Init(UIVillageFightsSubWindow.DungeonDataHolder data, UIVillageFightsSubWindow fightsWindow, SaveDataObject saveDataObject)
        {
            this.fightsWindow = fightsWindow;
            this.data = data;
            mark.Init(data.Level);
            text.text = data.Name;

            var dungeon = saveDataObject.Dungeons.GetDungeonBySeed(data.Seed);

            if (dungeon != null)
            {
                progressBar.Init("Progress", dungeon.DeadMobs, dungeon.AllMobsCount);
            }
            else
            {
                progressBar.gameObject.SetActive(false);
            }
        }

        public void OnClick()
        {
            fightsWindow.EnterDungeon(data);
        }
    }
}
