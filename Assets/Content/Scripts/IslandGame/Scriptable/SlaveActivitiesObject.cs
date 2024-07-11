using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame;
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
        [SerializeField] private ItemObject incomeItemObject;
        [SerializeField] private int incomeItemCount;
        [SerializeField] private DropTableObject itemsIncomes;
        
        
        public string ActivityName => activityName;

        public string Uid => uid;

        public float ItemsPerTime => itemsPerTime;

        [Button]
        public void GenerateID() => uid = Guid.NewGuid().ToString();

        public RaftStorage.StorageItem GetActivityResourcesByTime(Random rnd, bool IsStorage)
        {
            if (IsStorage && itemsIncomes != null)
            {
                return new RaftStorage.StorageItem(itemsIncomes.GetItem(rnd), 1);
            }
            else if (incomeItemObject != null)
            {
                return new RaftStorage.StorageItem(incomeItemObject, incomeItemCount);
            }

            return null;
        }
    }
}
