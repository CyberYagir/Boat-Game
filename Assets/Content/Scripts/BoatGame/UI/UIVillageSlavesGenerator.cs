using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageSlavesGenerator : MonoBehaviour
    {
        [SerializeField] private TextAsset unitsDescriptions;
        [SerializeField] private UIVillageSlavesHolder holderPrefab;

        [SerializeField] private List<SlaveCreatedCharacterInfo> slavesInfos = new List<SlaveCreatedCharacterInfo>(5);
        [SerializeField] private List<DisplayCharacter> slavesVisuals = new List<DisplayCharacter>(5);

        private string lastVillage = "";

        public List<DisplayCharacter> SlavesVisuals => slavesVisuals;

        public List<SlaveCreatedCharacterInfo> SlavesInfos => slavesInfos;

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

                foreach (var ch in SlavesVisuals)
                {
                    Destroy(ch.Display.gameObject);
                }

                SlavesVisuals.Clear();



                var slaves = Character.GetSlavesList(rnd, gameData, islandLevel);
                slaves.AddRange(Character.GetSlavesFromList(village.GetTrasferedSlaves(), gameData));
                
                
                var lines = unitsDescriptions.text.Split("\n").ToList();


                foreach (var slave in slaves)
                {
                    var cost = (int) ((gameData.NativesListData.BaseUnitCost * slave.IslandLevel) + (rnd.Next(0, 1000) * (slave.IslandLevel / 10f)));
                    var slaveInfo = new SlaveCreatedCharacterInfo(slave.Character, slave.Type, slave.Seed, slave.IslandLevel, slave.SkinID, village.GetSlave(slave.Character.Uid), cost);
                    
                    SlavesInfos.Add(slaveInfo);
                }

                foreach (var slave in slavesInfos)
                {
                    var visuals = new DisplayCharacter(
                        gameData.NativesListData.GetSkinByID(slave.SkinID).gameObject,
                        lines.GetRandomItem(new Random(slave.Seed))
                    );
                    SlavesVisuals.Add(visuals);
                }
            }
        }

        public void Show()
        {
            DisplaysConfiguration();
        }
        
        private void DisplaysConfiguration()
        {
            for (int i = 0; i < SlavesVisuals.Count; i++)
            {
                if (SlavesVisuals[i].Display == null)
                {
                    var id = i;
                    Instantiate(holderPrefab, new Vector3(i * 10, 1000, 0), Quaternion.identity)
                        .With(x => x.transform.name += id)
                        .With(x => SlavesVisuals[id].SetDisplay(x, slavesInfos[id].SlaveData));
                }
                SlavesVisuals[i].Display.gameObject.SetActive(true);
            }
        }


        public void Hide()
        {
            for (int i = 0; i < SlavesVisuals.Count; i++)
            {
                if (SlavesVisuals[i].Display)
                {
                    SlavesVisuals[i].Display.gameObject.SetActive(false);
                }
            }
        }
    }
}