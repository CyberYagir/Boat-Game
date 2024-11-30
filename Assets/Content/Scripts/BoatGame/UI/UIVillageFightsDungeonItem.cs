using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame;
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
        [SerializeField] private GameObject bossRemain;
        [SerializeField] private GameObject completedText;

        private DungeonDataHolder data;
        private UIVillageFightsSubWindow fightsWindow;

        public void Init(DungeonDataHolder data, UIVillageFightsSubWindow fightsWindow, SaveDataObject saveDataObject)
        {
            this.fightsWindow = fightsWindow;
            this.data = data;
            mark.Init(data.Level);
            text.text = data.Name;

            var dungeon = saveDataObject.Dungeons.GetDungeonBySeed(data.Seed);

            progressBar.gameObject.SetActive(false);
            completedText.gameObject.SetActive(false);
            bossRemain.gameObject.SetActive(false);
            
            if (dungeon != null)
            {
                if (dungeon.DeadMobs < dungeon.AllMobsCount && !dungeon.IsBossDead)
                {
                    progressBar.Init("Progress", dungeon.DeadMobs, dungeon.AllMobsCount);
                    progressBar.gameObject.SetActive(true);
                }
                else if (dungeon.DeadMobs >= dungeon.AllMobsCount && !dungeon.IsBossDead)
                {
                    bossRemain.gameObject.SetActive(true);
                }
                else
                {
                    completedText.gameObject.SetActive(true);
                }
            }
        }

        public void OnClick()
        {
            fightsWindow.EnterDungeon(data);
        }
    }
}
