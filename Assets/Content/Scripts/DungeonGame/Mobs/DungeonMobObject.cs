using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.DungeonGame.Mobs
{
    [CreateAssetMenu(menuName = "Create DungeonMobObject", fileName = "DungeonMobObject", order = 0)]
    public class DungeonMobObject : ScriptableObject
    {
        public enum EMobDifficult
        {
            Easy,
            Medium,
            Hard,
            Boss
        }

        [SerializeField, PreviewField] private List<DungeonMob> prefabVariants;
        [SerializeField] private EMobDifficult difficult;

        public DungeonMob Prefab => prefabVariants.GetRandomItem();

        public EMobDifficult Difficult => difficult;

    }
}
