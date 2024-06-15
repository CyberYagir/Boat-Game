using System.Collections.Generic;
using Content.Scripts.IslandGame.Natives;
using Content.Scripts.IslandGame.Sources;
using Content.Scripts.Map;
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
        private MapIsland.IslandData islandData;

        public string VillageID => villageID;

        public MapIsland.IslandData IslandData => islandData;


        public void Init(List<StructureDataBase> spawnedActiveStructures, Bounds villageBounds, string villageID, MapIsland.IslandData islandData)
        {
            this.islandData = islandData;
            this.villageID = villageID;
            this.villageBounds = villageBounds;
            this.spawnedActiveStructures = spawnedActiveStructures;
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
    }
}
