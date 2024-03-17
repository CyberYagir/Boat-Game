using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;
using Range = DG.DemiLib.Range;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class RoadsGenerator : MonoBehaviour
    {
        [SerializeField] private RoadSO roadSo;
        [SerializeField] private Range maxRoadsCount;
        [SerializeField] private int seed;


        [SerializeField] private List<Vector3Int> points;
        [SerializeField] private List<RoadBuilder> ends = new List<RoadBuilder>();

        private int targetCount;
        private int count;

        private Random rnd;

        public List<Vector3Int> Points => points;

        private void Awake()
        {
            SpawnNewRoad(0);
        }

        private void SpawnNewRoad(int iteration = 0)
        {
            count = 0;
            var spawned = Instantiate(roadSo.GetRoadByEnum(RoadSO.ERoadsType.Start), new Vector3(0, iteration, 0), Quaternion.identity);
            ends.Clear();
            Points.Clear();
            rnd = new Random(seed + iteration);
            targetCount = (int) maxRoadsCount.RandomWithin();
            Points.Add(Vector3Int.RoundToInt(spawned.transform.position));
            BuildRoad(spawned);
        }


        void BuildRoad(RoadBuilder point)
        {
            for (int i = 0; i < point.NextSpawnPoints.Count; i++)
            {
                var r = Vector3Int.RoundToInt(point.NextSpawnPoints[i].position);
                if (Points.Contains(r))
                {
                    point.DestroyPoint(i);
                }
                else
                {
                    Points.Add(r);
                }
            }


            if (point.NextSpawnPoints.Find(x=>x != null) == null)
            {
                var item = Instantiate(roadSo.GetRoadByEnum(RoadSO.ERoadsType.End), point.transform.parent)
                    .With(x => x.transform.localEulerAngles = Vector3.zero)
                    .With(x => x.transform.localPosition = Vector3.zero);
                AddEnd(item);
                Destroy(point.gameObject);
                return;
            }


            for (int i = 0; i < point.NextSpawnPoints.Count; i++)
            {
                if (point.NextSpawnPoints[i] == null) continue;

                var randomItem = point.RoadVariants.GetRandomItem(rnd);
                
                if (count >= targetCount)
                {
                    randomItem = RoadSO.ERoadsType.End;
                }
                
                
                var item = Instantiate(roadSo.GetRoadByEnum(randomItem), point.NextSpawnPoints[i].transform)
                    .With(x => x.transform.localEulerAngles = Vector3.zero)
                    .With(x => x.transform.localPosition = Vector3.zero);

                if (randomItem != RoadSO.ERoadsType.End)
                {
                    count++;
                    BuildRoad(item.GetComponent<RoadBuilder>());
                }else{
                    AddEnd(item);
                }
            }
        }

        private void AddEnd(RoadBuilder item)
        {
            var r = Vector3Int.RoundToInt(item.transform.position);
            if (!Points.Contains(r))
            {
                Points.Add(r);
            }
            ends.Add(item);
        }
    }
}