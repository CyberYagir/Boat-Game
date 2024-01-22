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

        private void Awake()
        {
            var raftStorage = GetComponent<RaftStorage>();
            raftStorage.OnStorageChange += OnStorageChange;

            foreach (var it in raftStorage.Items)
            {
                OnStorageChange(it.ResourcesType, it);
            }
        }

        private void OnStorageChange(EResourceTypes item, RaftStorage.ResourceTypeHolder holder)
        {
            if (item == EResourceTypes.Eat )
            {
                foreach (var fv in fishesVisuals)
                {
                    fv.FishMesh.gameObject.SetActive(false);
                    fv.Stick.gameObject.SetActive(false);
                }

                for (int i = 0; i < holder.Count; i++)
                {
                    fishesVisuals[i].Stick.gameObject.SetActive(true);
                    fishesVisuals[i].FishMesh.gameObject.SetActive(true);
                }
            }else
            if (item == EResourceTypes.Build)
            {
                for (int i = 0; i < logsVisuals.Count; i++)
                {
                    logsVisuals[i].SetActive(i < holder.Count);
                }
            }else if (item == EResourceTypes.Water)
            {
                var percent = holder.Count / (float) holder.MaxCount;
                var count = Mathf.Ceil(waterBuckets.Count * percent);
                for (int i = 0; i < waterBuckets.Count; i++)
                {
                    waterBuckets[i].SetActive(i < count);
                }
            }else if (item == EResourceTypes.Money)
            {
                var percent = holder.Count / (float) holder.MaxCount;
                var count = Mathf.Ceil(bagsVisuals.Count * percent);
                for (int i = 0; i < bagsVisuals.Count; i++)
                {
                    bagsVisuals[i].SetActive(i < count);
                }
            }
        }
    }
}
