using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.ItemsSystem
{

    public enum EItemType
    {
        Item,
        Armor
    }
    public class ItemObject : ScriptableObject
    {
        [SerializeField, ReadOnly] private string id;
        [SerializeField] private string itemName;
        [SerializeField] private EResourceTypes type;
        [SerializeField] private EItemType itemType;
        [SerializeField, PreviewField] private Sprite itemIcon;
        [SerializeField] private Character.ParametersData parametersData;
        [SerializeField, ShowIf("@itemType == EItemType.Armor")] private GameObject prefab;
        public Sprite ItemIcon => itemIcon;

        public string ItemName => itemName;

        public string ID => id;

        public Character.ParametersData ParametersData => parametersData;

        public EResourceTypes Type => type;

        public EItemType ItemType => itemType;


        [Button]
        public void GenerateID()
        {
            id = Guid.NewGuid().ToString();
        }
    }
}
