using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageSlavesVisualsGenerator : MonoBehaviour
    {
        [SerializeField] private TextAsset unitsDescriptions;
        [SerializeField] private UIVillageSlavesHolder holderPrefab;

        private List<DisplayCharacter> characters = new List<DisplayCharacter>(5);

        private string lastVillage = "";

        public List<DisplayCharacter> Characters => characters;

        public void Init(
            SaveDataObject.MapData.IslandData.VillageData village,
            GameDataObject gameData,
            Random rnd,
            int islandLevel
        )
        {
            if (village.Uid != lastVillage)
            {
                lastVillage = village.Uid;

                foreach (var ch in Characters)
                {
                    Destroy(ch.Display.gameObject);
                }

                Characters.Clear();



                var slaves = Character.GetSlavesList(rnd, gameData, islandLevel);

                var lines = unitsDescriptions.text.Split("\n").ToList();


                for (int i = 0; i < slaves.Count; i++)
                {
                    Characters.Add(new DisplayCharacter(slaves[i], gameData.NativesListData.NativesList.GetRandomItem(rnd).gameObject, village.GetSlave(slaves[i].Uid), lines.GetRandomItem(rnd), (int) ((gameData.NativesListData.BaseUnitCost * islandLevel) + (rnd.Next(0, 1000) * (islandLevel / 10f)))));
                }

            }
        }

        public void Show()
        {
            DisplaysConfiguration();
        }
        
        private void DisplaysConfiguration()
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].Display == null)
                {
                    var id = i;
                    Instantiate(holderPrefab, new Vector3(i * 10, 1000, 0), Quaternion.identity)
                        .With(x => Characters[id].SetDisplay(x));
                }
                Characters[i].Display.gameObject.SetActive(true);
            }
        }


        public void Hide()
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].Display)
                {
                    Characters[i].Display.gameObject.SetActive(false);
                }
            }
        }
    }
}