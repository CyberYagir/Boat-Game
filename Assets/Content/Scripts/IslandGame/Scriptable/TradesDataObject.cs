using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.IslandGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create TradesDataObject", fileName = "TradesDataObject", order = 0)]
    public class TradesDataObject : ScriptableObject
    {
        [SerializeField] private List<TradeOfferObject> sellOffers;
        [SerializeField] private List<TradeOfferObject> buyOffers;

        
        public List<TradeOfferObject> GetRandomSellsByLevel(int level, System.Random rnd)
        {
            var itemsByLevel = sellOffers.FindAll(x => x.LevelRange.IsInRange(level));
            var list = GetRandomFromArray(rnd, itemsByLevel);
            return list;
        }
        
        public List<TradeOfferObject> GetRandomBuysByLevel(int level, System.Random rnd)
        {
            var itemsByLevel = buyOffers.FindAll(x => x.LevelRange.IsInRange(level));
            var list = GetRandomFromArray(rnd, itemsByLevel);
            return list;
        }

        private static List<TradeOfferObject> GetRandomFromArray(Random rnd, List<TradeOfferObject> itemsByLevel)
        {
            var list = new List<TradeOfferObject>();
            var count = rnd.Next(3, 6);
            for (int i = 0; i < count; i++)
            {
                TradeOfferObject item = null;
                do
                {
                    item = itemsByLevel.GetRandomItem();
                } while (list.Contains(item));
                list.Add(item);
            }

            return list;
        }
    }
}
