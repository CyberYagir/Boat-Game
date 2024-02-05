using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI.UIEquipment;
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
    }

    [System.Serializable]
    public class ItemsParameters : Character.ParametersData
    {
        [SerializeField] private float damage;

        public float Damage => damage;
    }
    public class ItemObject : ScriptableObject
    {
        [SerializeField, ReadOnly] private string id;
        [SerializeField] private string itemName;
        [SerializeField] private EResourceTypes type;
        [SerializeField] private EItemType itemType;
        [SerializeField, PreviewField] private Sprite itemIcon;
        [SerializeField] private ItemsParameters parametersData;
        [SerializeField, ShowIf("@itemType == EItemType.Armor")] private GameObject prefab;
        [SerializeField, ShowIf("@itemType == EItemType.Armor")] private UIEquipmentBase.EEquipmentType equipment;
        [SerializeField, ShowIf("@equipment == UIEquipmentBase.EEquipmentType.Weapon")] private EWeaponAnimationType animationType;
        public Sprite ItemIcon => itemIcon;

        public string ItemName => itemName;

        public string ID => id;

        public ItemsParameters ParametersData => parametersData;

        public EResourceTypes Type => type;

        public EItemType ItemType => itemType;

        public UIEquipmentBase.EEquipmentType Equipment => equipment;

        public GameObject Prefab => prefab;

        public EWeaponAnimationType AnimationType => animationType;


        [Button]
        public void GenerateID()
        {
            id = Guid.NewGuid().ToString();
        }
    }
}
