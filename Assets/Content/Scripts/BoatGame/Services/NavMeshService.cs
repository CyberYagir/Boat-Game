using System;
using System.Collections;
using UnityEngine;
using Pathfinding;

namespace Content.Scripts.BoatGame.Services
{
    public class NavMeshService : MonoBehaviour, INavMeshProvider
    {

        [SerializeField] private AstarPath navMesh;

        public event Action OnNavMeshBuild;

        public void BuildNavMesh()
        {
            StartCoroutine(RebuildSkipFrame());
        }

        public void BuildNavMeshAsync()
        {
            StartCoroutine(ScanAsync());
        }

        public void BuildNavMeshAsync(int id)
        {
            StartCoroutine(ScanAsync(GetNavMeshByID(id)));
        }

        public NavGraph GetNavMeshByID(int i)
        {
            return AstarPath.active.graphs[i];
        }

        public int GetGraphsCount()
        {
            return AstarPath.active.graphs.Length;
        }

        public bool IsAvailablePoint(Vector3 pos)
        {
            var graph = navMesh.data.gridGraph;
            return graph.IsInsideBounds(pos);
        }

        IEnumerator RebuildSkipFrame()
        {
            yield return null;
            navMesh.Scan();
            yield return null;
            OnNavMeshBuild?.Invoke();
        }

        private bool isInProgress = false;

        IEnumerator ScanAsync()
        {
            if (!isInProgress)
            {
                isInProgress = true;
                yield return null;
                foreach (Progress progress in navMesh.ScanAsync())
                {
                    yield return null;
                }

                isInProgress = false;
                yield return null;
                OnNavMeshBuild?.Invoke();
            }
        }

        IEnumerator ScanAsync(NavGraph navGraph)
        {
            if (!isInProgress)
            {
                isInProgress = true;
                yield return null;
                foreach (Progress progress in navMesh.ScanAsync(navGraph))
                {
                    yield return null;
                }

                isInProgress = false;
                yield return null;
                OnNavMeshBuild?.Invoke();
            }
        }
    }
}
