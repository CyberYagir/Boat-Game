using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Global;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Content.Scripts.BoatGame
{
    public class UnderwaterManager : MonoBehaviour
    {
        [SerializeField] private List<Transform> pointsHolders;
        [SerializeField] private List<GameObject> fishesPrefabs;
        [SerializeField] private float spawnedFishesLimit;

        [SerializeField] private bool drawGizmo = true;
        
        private List<GameObject> spawnedFishes = new List<GameObject>();
        private SaveDataObject saveDataObject;


        [Inject]
        private void Construct(SaveDataObject saveDataObject)
        {
            this.saveDataObject = saveDataObject;
            if (gameObject.activeSelf)
            {
                StartCoroutine(Loop());
            }
        }

        IEnumerator Loop()
        {
            while (true)
            {
                if (saveDataObject.CrossGame.Statistics.CountFishCatched == 0)
                {
                    yield return null;
                    continue;
                }
                if (spawnedFishes.Count < spawnedFishesLimit)
                {
                    var points = pointsHolders.GetRandomItem();
                    var y = Random.Range(-2f, -5f) * Vector3.up;

                    var start = points.GetChild(0).position + y;
                    var end = points.GetChild(1).position + y;

                    var fish = Instantiate(fishesPrefabs.GetRandomItem(), start, Quaternion.LookRotation(end - start), transform);


                    fish.transform.DOMove(end, Random.Range(20, 60)).SetLink(fish.gameObject).onComplete += () =>
                    {
                        spawnedFishes.Remove(fish);
                        Destroy(fish.gameObject);
                    };
                    spawnedFishes.Add(fish);
                    yield return new WaitForSeconds(Random.Range(0.2f, 5f));
                }

                yield return null;
            }
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
