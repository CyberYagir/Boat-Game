using System.Collections.Generic;
using Content.Scripts.ItemsSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.Mobs
{
    [CreateAssetMenu(menuName = "Create DropTable", fileName = "DropTable", order = 0)]
    [System.Serializable]
    public class DropTableObject : ScriptableObject
    {
        public static List<float> weights = new List<float>(10);

        [System.Serializable]
        class DropItem
        {
            [SerializeField] private float weight;
            [SerializeField] private ItemObject item;

            public float Weight => weight;

            public ItemObject Item => item;
        }

        [SerializeField, TableList] private List<DropItem> table;

        public ItemObject GetItem()
        {
            if (table.Count == 0) return null;
            weights.Clear();

            for (int i = 0; i < table.Count; i++)
            {
                weights.Add(table[i].Weight);
            }
            
            weights.RecalculateWeights();

            var index = weights.ChooseRandomIndexFromWeights();

            return table[index].Item;
        }
    }
}
