using System.Collections.Generic;
using Content.Scripts.Mobs.Mob;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.Mobs
{
    [CreateAssetMenu(menuName = "Create MobObject", fileName = "MobObject", order = 0)] 
    public class MobObject : ScriptableObject
    {
        public enum EMobType
        {
            Crab,
            Snake,
            Bighorn,
            GoldenEagle,
            Peacock,
            Pronghorn,
            Gaur,
            
            Dungeon_Specter,
            Dungeon_Skeleton,
            Dungeon_Golem,
            Dungeon_Rat
        }

        [SerializeField] private EMobType type;
        [SerializeField, PreviewField] private List<SpawnedMob> prefabVariants;

        public SpawnedMob Prefab => prefabVariants.GetRandomItem();
        public EMobType Type => type;

        public SpawnedMob PrefabBySeed(System.Random rnd)
        {
            return prefabVariants.GetRandomItem(rnd);
        }
    }
}
