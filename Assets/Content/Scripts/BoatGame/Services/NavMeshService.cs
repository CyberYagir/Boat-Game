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

        IEnumerator RebuildSkipFrame()
        {
            yield return null;
            navMesh.Scan();
        }
        
        IEnumerator ScanAsync()
        {
            yield return null;
            foreach (Progress progress in navMesh.ScanAsync())
            {
                yield return null;
            }
        }
        
        IEnumerator ScanAsync(NavGraph navGraph)
        {
            yield return null;
            foreach (Progress progress in navMesh.ScanAsync(navGraph))
            {
                yield return null;
            }
        }
    }
}
