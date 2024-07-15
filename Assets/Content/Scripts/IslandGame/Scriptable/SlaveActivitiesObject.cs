using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.IslandGame.Natives;
using Content.Scripts.ItemsSystem;
using Content.Scripts.Mobs;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.IslandGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create SlaveActivitiesObject", fileName = "SlaveActivitiesObject", order = 0)]
    public class SlaveActivitiesObject : ScriptableObject
    {
        [SerializeField, ReadOnly] private string uid;
        [SerializeField] private string activityName;
        [SerializeField] private float itemsPerTime;
        [SerializeField] private float typeBoostMultiplier = 0.5f;
        [SerializeField] private float skillPerItem = 0.1f;
        [SerializeField, FoldoutGroup("Income")] private ItemObject incomeItemObject;
        [SerializeField, FoldoutGroup("Income")] private int incomeItemCount;
        [SerializeField, FoldoutGroup("Drop Table")] private DropTableObject itemsIncomes;

        [SerializeField] private List<ENativeType> boostTypes = new List<ENativeType>();

        public string ActivityName => activityName;

        public string Uid => uid;

        public float SkillPerItem => skillPerItem;


        [Button]
        public void GenerateID() => uid = Guid.NewGuid().ToString();

        public RaftStorage.StorageItem GetActivityResourcesByTime(Random rnd, bool IsStorage, float modify)
        {
            if (IsStorage && itemsIncomes != null)
            {
                return new RaftStorage.StorageItem(itemsIncomes.GetItem(rnd), Mathf.RoundToInt(1 * modify));
            }
            else if (incomeItemObject != null)
            {
                return new RaftStorage.StorageItem(incomeItemObject, Mathf.RoundToInt(incomeItemCount * modify));
            }

            return null;
        }

        public float GetItemsPerTime(ENativeType type)
        {
            if (boostTypes.Contains(type))
            {
                return itemsPerTime * typeBoostMultiplier;
            }

            return itemsPerTime;
        }
    }
}
