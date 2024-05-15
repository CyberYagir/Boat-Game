using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.Mobs;
using UnityEngine;

namespace Content.Scripts.ItemsSystem
{
    [CreateAssetMenu(menuName = "Create WaterSpawnItemsObject", fileName = "WaterSpawnItemsObject", order = 0)]
    public class WaterSpawnItemsObject : ScriptableObject, ITableObject
    {
        [System.Serializable]
        public class ItemHolder
        {
            [SerializeField] private WaterItem item;
            [SerializeField] private float weight;

            public float Weight => weight;

            public WaterItem Item => item;

            public void SetWeight(float targetWeight)
            {
                weight = targetWeight;
            }
        }

        [SerializeField] private List<ItemHolder> itemsList;
    


        private List<float> weights = new List<float>(10);

        public WaterItem GetRandomItem()
        {
            weights.Clear();

            for (int i = 0; i < itemsList.Count; i++)
            {
                weights.Add(itemsList[i].Weight);
            }

            weights.RecalculateWeights();
            var item = weights.ChooseRandomIndexFromWeights();
            return itemsList[item].Item;
        }

        public Dictionary<string, float> GetWeights()
        {
            var dic = new Dictionary<string, float>();

            for (int i = 0; i < itemsList.Count; i++)
            {
                if (itemsList[i].Item != null)
                {
                    dic.Add(itemsList[i].Item.name, itemsList[i].Weight);
                }
                else
                {
                    dic.Add("Empty", itemsList[i].Weight);
                }
            }

            return dic;
        }

        public void ChangeWeights(List<float> targetWeights)
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                itemsList[i].SetWeight(targetWeights[i]);
            }
        }
    }
}
