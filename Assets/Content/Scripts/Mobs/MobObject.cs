using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.Mobs
{
    [CreateAssetMenu(menuName = "Create MobObject", fileName = "MobObject", order = 0)] 
    public class MobObject : ScriptableObject
    {
        public enum MobType
        {
            Crab
        }

        [SerializeField] private MobType type;
        [SerializeField, PreviewField] private SpawnedMob prefab;
        [SerializeField] private float maxHealth = 50;

        public float MaxHealth => maxHealth;
        public SpawnedMob Prefab => prefab;
        public MobType Type => type;
    }
}
