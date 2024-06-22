using System.Collections.Generic;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Natives;
using Content.Scripts.IslandGame.Sources;
using Content.Scripts.Map;
using DG.DemiLib;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class VillageDataCollector : MonoBehaviour
    {
        [SerializeField, ReadOnly] private List<StructureDataBase> spawnedActiveStructures;
        [SerializeField, ReadOnly] private List<NativesSit> spawnedSits;
        [SerializeField, ReadOnly] private List<NativesWaterSource> spawnedWaterSources;
        private Bounds villageBounds;
        private string villageID;
        private IslandSeedData islandData;
        private SaveDataObject.MapData.IslandData.VillageData villageData;
        
        public string VillageID => villageID;

        public IslandSeedData IslandData => islandData;

        public SaveDataObject.MapData.IslandData.VillageData VillageData => villageData;


        public void Init(
            List<StructureDataBase> spawnedActiveStructures,
            Bounds villageBounds,
            string villageID,
            IslandSeedData islandData,
            SaveDataObject saveDataObject
        )
        {
            this.islandData = islandData;
            this.villageID = villageID;
            this.villageBounds = villageBounds;
            this.spawnedActiveStructures = spawnedActiveStructures;

            villageData = saveDataObject.GetTargetIsland().GetVillage(this.villageID);
            
            foreach (var item in spawnedActiveStructures)
            {
                spawnedSits.AddRange(item.NativeSits);
                spawnedWaterSources.AddRange(item.GetComponentsInChildren<NativesWaterSource>());
            }
        }

        public NativesSit GetRandomAvailableSit()
        {
            if (spawnedSits.Count != 0)
            {
                var empty = spawnedSits.FindAll(x => !x.IsNotEmpty && x.gameObject.activeInHierarchy);
                if (empty.Count != 0)
                {
                    return empty.GetRandomItem();
                }
            }

            return null;
        }
        
        public NativesWaterSource GetRandomAvailableWaterSource()
        {
            if (spawnedSits.Count != 0)
            {
                var empty = spawnedWaterSources.FindAll(x => !x.IsNotEmpty && x.gameObject.activeInHierarchy);
                if (empty.Count != 0)
                {
                    return empty.GetRandomItem();
                }
            }

            return null;
        }

        public Bounds Bounds() => villageBounds;

        public void RemoveSocialRating(int value)
        {
            villageData.AddSocialRating(-value);
        }
    }
}
