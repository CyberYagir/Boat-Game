using System.Collections.Generic;
using Content.Scripts.IslandGame.Natives;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class VillageDataCollector : MonoBehaviour
    {
        [SerializeField, ReadOnly] private List<StructureDataBase> spawnedActiveStructures;
        [SerializeField, ReadOnly] private List<NativesSit> spawnedSits;
        [SerializeField, ReadOnly] private List<NativeController> spawnedNatives;


        public void Init(List<StructureDataBase> spawnedActiveStructures, List<NativeController> spawnedNatives)
        {
            this.spawnedActiveStructures = spawnedActiveStructures;
            this.spawnedNatives = spawnedNatives;

            foreach (var item in spawnedActiveStructures)
            {
                spawnedSits.AddRange(item.NativeSits);
            }
        }

        public NativesSit GetRandomAvailableSit()
        {
            return spawnedSits.FindAll(x => x.IsNotEmpty).GetRandomItem();
        }
    }
}
