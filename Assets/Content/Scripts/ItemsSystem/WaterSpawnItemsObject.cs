using System.Collections.Generic;
using Content.Scripts.BoatGame;
using UnityEngine;

namespace Content.Scripts.ItemsSystem
{
    [CreateAssetMenu(menuName = "Create WaterSpawnItemsObject", fileName = "WaterSpawnItemsObject", order = 0)]
    public class WaterSpawnItemsObject : ScriptableObject
    {
        [System.Serializable]
        public class ItemHolder
        {
            [SerializeField] private WaterItem item;
            [SerializeField] private float weight;

            public float Weight => weight;

            public WaterItem Item => item;
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
    }
}
