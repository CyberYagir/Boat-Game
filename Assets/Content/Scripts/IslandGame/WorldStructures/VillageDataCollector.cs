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
        private Bounds villageBounds;


        public void Init(List<StructureDataBase> spawnedActiveStructures, Bounds villageBounds)
        {
            this.villageBounds = villageBounds;
            this.spawnedActiveStructures = spawnedActiveStructures;
            foreach (var item in spawnedActiveStructures)
            {
                spawnedSits.AddRange(item.NativeSits);
            }
        }

        public NativesSit GetRandomAvailableSit()
        {
            if (spawnedSits.Count != 0)
            {
                var empty = spawnedSits.FindAll(x => !x.IsNotEmpty);
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
