using Content.Scripts.BoatGame;
using DG.DemiLib;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Content.Scripts.IslandGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create TradeObject", fileName = "TradeObject", order = 0)]
    public class TradeOfferObject : ScriptableObject
    {
        [SerializeField] private Range levelRange = new Range(1, 3);
        [SerializeField] private int socialRatingPoints = 10;
        [SerializeField] private RaftStorage.StorageItem sellItem;
        [SerializeField] private RaftStorage.StorageItem resultItem;

        public RaftStorage.StorageItem ResultItem => resultItem;

        public RaftStorage.StorageItem SellItem => sellItem;

        public Range LevelRange => levelRange;

        public int SocialRatingPoints => socialRatingPoints;


#if UNITY_EDITOR

        [Button]
        public void RenameObject()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), (sellItem.Item.ItemName.Replace(" ", "") + "_to_" + resultItem.Item.ItemName.Replace(" ", "")).ToLower());
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        [Button]
        public void ReverseItem()
        {
            (resultItem, sellItem) = (sellItem, resultItem);
        }
        
        [Button]
        public void InflateBags()
        {
            if (resultItem.Item.ItemName == "Gold Bag")
            {
                resultItem.Add(9); 
            }
            else
            {
                sellItem.Add(9); 
            }
        }
#endif
    }
}
