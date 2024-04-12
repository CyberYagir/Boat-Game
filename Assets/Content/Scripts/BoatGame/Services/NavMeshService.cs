using System.Collections;
using UnityEngine;
using Pathfinding;

namespace Content.Scripts.BoatGame.Services
{
    public interface INavMeshProvider
    {
        public void BuildNavMesh();
        public void BuildNavMeshAsync();

        public void BuildNavMeshAsync(int id);
        NavGraph GetNavMeshByID(int i);
        int GetGraphsCount();
    }

    public class NavMeshService : MonoBehaviour, INavMeshProvider
    {
        [SerializeField] private AstarPath navMesh;

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

        IEnumerator RebuildSkipFrame()
        {
            yield return null;
            navMesh.Scan();
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
            }
        }
    }
}
