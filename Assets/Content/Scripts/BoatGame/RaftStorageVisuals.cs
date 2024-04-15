using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    [RequireComponent(typeof(RaftStorage))]
    public class RaftStorageVisuals : MonoBehaviour
    {
        [System.Serializable]
        public class FishDataVisuals
        {
            [SerializeField] private Transform stick;
            [SerializeField] private GameObject fishMesh;

            public GameObject FishMesh => fishMesh;

            public Transform Stick => stick;
        }

        [SerializeField] private List<FishDataVisuals> fishesVisuals = new List<FishDataVisuals>();
        [SerializeField] private List<GameObject> logsVisuals;
        [SerializeField] private List<GameObject> waterBuckets;
        [SerializeField] private List<GameObject> bagsVisuals;
        
        private RaftStorage raftStorage;

        private void Awake()
        {
            raftStorage = GetComponent<RaftStorage>();
            raftStorage.OnStorageChange += OnStorageChange;
            OnStorageChange();
        }

        private void OnStorageChange()
        {
            UpdateFishes();
            UpdateBuilds();
            UpdateWater();
            UpdateCoins();
        }

        private void UpdateBuilds()
        {
            var buildItems = raftStorage.Items.FindAll(x => x.Item.Type == EResourceTypes.Build);
            for (int i = 0; i < logsVisuals.Count; i++)
            {
                logsVisuals[i].SetActive(i < buildItems.Count);
            }
        }

        private void UpdateFishes()
        {
            var eatItems = raftStorage.Items.FindAll(x => x.Item.Type == EResourceTypes.Eat);
            foreach (var fv in fishesVisuals)
            {
                fv.FishMesh.gameObject.SetActive(false);
                fv.Stick.gameObject.SetActive(false);
            }

            for (int i = 0; i < eatItems.Count; i++)
            {
                if (fishesVisuals.Count < i)
                {
                    fishesVisuals[i].Stick.gameObject.SetActive(true);
                    fishesVisuals[i].FishMesh.gameObject.SetActive(true);
                }
            }
        }

        private void UpdateWater()
        {
            var waterItems = raftStorage.Items.FindAll(x => x.Item.Type == EResourceTypes.Water);
            for (int i = 0; i < waterBuckets.Count; i++)
            {
                waterBuckets[i].SetActive(i < waterItems.Count);
            }
        }
        
        private void UpdateCoins()
        {
            var coinsItems = raftStorage.Items.FindAll(x => x.Item.Type == EResourceTypes.Money);
            for (int i = 0; i < bagsVisuals.Count; i++)
            {
                bagsVisuals[i].SetActive(i < coinsItems.Count);
            }
        }
    }
}
