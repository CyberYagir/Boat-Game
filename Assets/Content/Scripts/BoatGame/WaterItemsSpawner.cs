using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame
{
    public class WaterItemsSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class ItemHolder
        {
            [SerializeField] private WaterItem item;
            [SerializeField] private float weight;

            public float Weight => weight;

            public WaterItem Item => item;
        }

        [SerializeField] private List<ItemHolder> itemsList;
        [SerializeField] private List<Transform> pointsHolders;
        [SerializeField] private int maxCount;
        [SerializeField] private bool drawGizmo = true;

        [SerializeField] private float cooldown;

        private float time;

        private List<WaterItem> spawnedItems = new List<WaterItem>();
        private SelectionService selectionService;


        [Inject]
        private void Construct(TickService tickService, SelectionService selectionService)
        {
            this.selectionService = selectionService;
            tickService.OnTick += OnTick;
        }

        private void OnTick(float t)
        {
            if (spawnedItems.Count <= maxCount && time <= 0)
            {
                List<float> weights = new List<float>();
                for (int i = 0; i < itemsList.Count; i++)
                {
                    weights.Add(itemsList[i].Weight);
                }

                weights.RecalculateWeights();
                var item = weights.ChooseRandomIndexFromWeights();

                var (start, end) = GetPoses(pointsHolders.GetRandomIndex());

                var spawned = Instantiate(itemsList[item].Item, start, Quaternion.identity);
                
                spawned.Init(end - start, selectionService);
                
                
                spawnedItems.Add(spawned);

                time = cooldown;
                
                
                transform.SetYLocalEulerAngles(Random.Range(0, 360));
            }

            spawnedItems.RemoveAll(x => x == null);

            time -= t;
        }

        public (Vector3, Vector3) GetPoses(int id)
        {
            return (pointsHolders[id].GetChild(0).position, pointsHolders[id].GetChild(1).position);
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmo) return;
            
            for (int i = 0; i < pointsHolders.Count; i++)
            {
                Gizmos.DrawLine(pointsHolders[i].GetChild(0).position, pointsHolders[i].GetChild(1).position);
            }
        }
    }
}
