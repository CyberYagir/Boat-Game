using System.Collections.Generic;
using System.Linq;
using DG.DemiLib;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.IslandGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create TradesDataObject", fileName = "TradesDataObject", order = 0)]
    public class TradesDataObject : ScriptableObject
    {

        [System.Serializable]
        public class EmotionRange
        {
            [SerializeField] private Range range;
            [SerializeField, PreviewField] private Sprite sprite;
            [SerializeField] private float tradeMultiply;

            public Sprite Sprite => sprite;

            public Range Range => range;

            public float TradeMultiply => tradeMultiply;
        }

        [SerializeField] private List<TradeOfferObject> sellOffers;
        [SerializeField] private List<TradeOfferObject> buyOffers;
        [SerializeField, TableList] private List<EmotionRange> emotionalRange;
        [SerializeField] private int maxTrades = 6;
        
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

        private List<TradeOfferObject> GetRandomFromArray(Random rnd, List<TradeOfferObject> itemsByLevel)
        {
            var list = new List<TradeOfferObject>();
            var max = itemsByLevel.Count;
            if (max > maxTrades)
            {
                max = maxTrades;
            }
            
            var count = rnd.Next((int)(max/3f), max);
            for (int i = 0; i < count; i++)
            {
                TradeOfferObject item = null;
                do
                {
                    item = itemsByLevel.GetRandomItem(rnd);
                } while (list.Contains(item));
                list.Add(item);
            }

            return list;
        }

        public EmotionRange GetEmotionalData(int value)
        {
            for (int i = 0; i < emotionalRange.Count; i++)
            {
                if (emotionalRange[i].Range.IsInRange(value))
                {
                    return emotionalRange[i];
                }
            }

            return emotionalRange.First();
        }
    }
}
