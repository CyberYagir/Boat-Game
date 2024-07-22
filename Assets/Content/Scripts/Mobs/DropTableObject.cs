using System.Collections.Generic;
using Content.Scripts.ItemsSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.Mobs
{
    
    
    [CreateAssetMenu(menuName = "Create DropTable", fileName = "DropTable", order = 0)]
    [System.Serializable]
    public class DropTableObject : ScriptableObject, ITableObject
    {
        public static List<float> weights = new List<float>(10);

        [System.Serializable]
        class DropItem
        {
            [SerializeField] private float weight;
            [SerializeField] private ItemObject item;

            public float Weight => weight;

            public ItemObject Item => item;

            public void SetWeight(float targetWeight)
            {
                weight = targetWeight;
            }
        }

        [SerializeField, TableList] private List<DropItem> table;

        public ItemObject GetItem()
        {
            if (table.Count == 0) return null;

            if (weights.Count != table.Count)
            {
                weights.Clear();

                for (int i = 0; i < table.Count; i++)
                {
                    weights.Add(table[i].Weight);
                }

                weights.RecalculateWeights();
            }

            var index = weights.ChooseRandomIndexFromWeights();

            return table[index].Item;
        }

        public ItemObject GetItem(System.Random rnd)
        {
            if (table.Count == 0) return null;
            weights.Clear();

            for (int i = 0; i < table.Count; i++)
            {
                weights.Add(table[i].Weight);
            }

            weights.RecalculateWeights();

            var index = weights.ChooseRandomIndexFromWeights(rnd);

            return table[index].Item;
        }

        public Dictionary<string, float> GetWeights()
        {
            var dic = new Dictionary<string, float>();

            for (int i = 0; i < table.Count; i++)
            {
                if (table[i].Item != null)
                {
                    dic.Add(table[i].Item.ItemName, table[i].Weight);
                }
                else
                {
                    dic.Add("Empty", table[i].Weight);
                }
            }

            return dic;
        }

        public void ChangeWeights(List<float> targetWeights)
        {
            for (int i = 0; i < table.Count; i++)
            {
                table[i].SetWeight(targetWeights[i]);
            }
        }


    }
}
