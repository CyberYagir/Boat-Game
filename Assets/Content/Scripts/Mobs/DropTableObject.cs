using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.ItemsSystem;
using DG.DemiLib;
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
            [SerializeField] private Range count = new Range(1, 1);
            public float Weight => weight;

            public ItemObject Item => item;
            public int GetItemsCount()
            {
                var value = count.RandomWithin();
                if (value <= 1f)
                {
                    return 1;
                }
                return Mathf.RoundToInt(value);
            }

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

        public RaftStorage.StorageItem GetMultipleItems()
        {
            var it = GetItem();
            if (it)
            {
                return new RaftStorage.StorageItem(it, table.Find(x => x.Item == it).GetItemsCount());
            }
            else
            {
                return null;
            }
        }

        private static List<RaftStorage.StorageItem> tmpIteratedItems = new List<RaftStorage.StorageItem>(5);
        public List<RaftStorage.StorageItem> GetItemsIterated(int iterations)
        {
            tmpIteratedItems.Clear();

            for (int i = 0; i < iterations; i++)
            {
                var it = GetMultipleItems();
                if (it != null)
                {
                    tmpIteratedItems.Add(it);
                }
            }

            return tmpIteratedItems;
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
                    if (!dic.ContainsKey(table[i].Item.ItemName)){
                        dic.Add(table[i].Item.ItemName, table[i].Weight);
                    }else
                    {
                        Debug.LogError(table[i].Item.ItemName + " not one in table");
                    }
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
