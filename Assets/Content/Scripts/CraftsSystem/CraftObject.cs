using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.CraftsSystem
{
    public class CraftObject : ScriptableObject
    {
        [System.Serializable]
        public class CraftItem
        {
            [SerializeField] private ItemObject item;
            [SerializeField] private int count;

            public int Count => count;

            public ItemObject ResourceName => item;
        }
        
        public enum ECraftType
        {
            Raft,
            Item
        }
        
        public enum ECraftTable{
            CraftingTable,
            Forge
        }

        [SerializeField] private string craftName;
        [SerializeField] private ECraftType type;
        [SerializeField, ShowIf("@type == ECraftType.Item")] private ECraftTable table;
        [SerializeField, ReadOnly] private string uid;
        [SerializeField] private TooltipDataObject tooltip;
        [SerializeField, PreviewField] private Sprite icon;
        [SerializeField] private List<CraftItem> ingredients;
        [SerializeField] private float craftTime;
        
        [SerializeField, ShowIf(nameof(IsNotRaftItem))] 
        private CraftItem finalItem;
        
        public List<CraftItem> Ingredients => ingredients;
        public Sprite Icon => icon;

        public string Uid => uid;

        public string CraftName => craftName;

        public float CraftTime => craftTime;

        public ECraftType CraftType => type;

        public CraftItem FinalItem => finalItem;

        public TooltipDataObject Tooltip => tooltip;

        [Button]
        public void GenerateID()
        {
            uid = Guid.NewGuid().ToString();
        }

        public bool IsNotRaftItem()
        {
            return type != ECraftType.Raft;
        }
    }
}
