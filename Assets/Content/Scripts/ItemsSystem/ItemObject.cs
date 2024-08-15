using System;
using Content.Scripts.BoatGame.Scriptable;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.IslandGame;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.ItemsSystem
{

    public enum EItemType
    {
        Item,
        Armor
    }

    public enum EWeaponAnimationType
    {
        None = 0,
        Sword = 1,
        HeavySword = 2,
        Sabre = 3,
    }

    [Flags]
    public enum EItemFurnaceType
    {
        CanFuel = 8,
        CanSmelt = 16,
        CanDisplay = 32
    }

    [System.Serializable]
    public class ItemsParameters : Character.ParametersData
    {
        [SerializeField] private float damage;
        [SerializeField] private float defence = 0.1f;

        public float Damage => damage;

        public float Defence => defence;
    }

    [System.Serializable]
    public class ItemFurnaceParameters
    {        
        [SerializeField] private EItemFurnaceType furnaceFlags;
        [SerializeField, ShowIf("@furnaceFlags.HasFlag(EItemFurnaceType.CanFuel)")] 
        private float fuelSeconds;
        [SerializeField, ShowIf("@furnaceFlags.HasFlag(EItemFurnaceType.CanSmelt)")] 
        private float smeltSeconds;

        [SerializeField, ShowIf("@furnaceFlags.HasFlag(EItemFurnaceType.CanSmelt)")] 
        private ItemObject afterSmeltItem;
        public float FuelSeconds => fuelSeconds;

        public float SmeltSeconds => smeltSeconds;

        public EItemFurnaceType FurnaceFlags => furnaceFlags;

        public ItemObject AfterSmeltItem => afterSmeltItem;
    }
    public class ItemObject : ScriptableObject
    {
        [SerializeField, ReadOnly] private string id;
        [SerializeField] private string itemName;
        [SerializeField] private EResourceTypes type;
        [SerializeField] private EItemType itemType;
        [SerializeField, PreviewField] private Sprite itemIcon;
        [SerializeField] private ItemsParameters parametersData;
        [SerializeField] private ItemFurnaceParameters furnaceData;
        [SerializeField] private bool hasSize = true;
        [SerializeField, ShowIf("@itemType == EItemType.Item")] private DroppedItem dropPrefab;
        [SerializeField, ShowIf("@type == EResourceTypes.Potions")] private PotionLogicBaseSO potionLogic;
        [SerializeField, ShowIf("@itemType == EItemType.Armor")] private GameObject prefab;
        [SerializeField, ShowIf("@itemType == EItemType.Armor")] private EEquipmentType equipment;
        [SerializeField, ShowIf("@equipment == EEquipmentType.Weapon")] private EWeaponAnimationType animationType;
        public Sprite ItemIcon => itemIcon;

        public string ItemName => itemName;

        public string ID => id;

        public ItemsParameters ParametersData => parametersData;

        public EResourceTypes Type => type;

        public EItemType ItemType => itemType;

        public EEquipmentType Equipment => equipment;

        public GameObject Prefab => prefab;

        public EWeaponAnimationType AnimationType => animationType;

        public DroppedItem DropPrefab => dropPrefab;

        public ItemFurnaceParameters FurnaceData => furnaceData;

        public bool HasSize => hasSize;

        public PotionLogicBaseSO PotionLogic => potionLogic;


        [Button]
        public void GenerateID()
        {
            id = Guid.NewGuid().ToString();
        }
    }
}
