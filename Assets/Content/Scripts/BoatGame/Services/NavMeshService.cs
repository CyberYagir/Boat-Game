using System.Collections;
using UnityEngine;
using Pathfinding;
namespace Content.Scripts.BoatGame.Services
{
    public interface INavMeshProvider
    {
        public void BuildNavMesh();
        public void BuildNavMeshAsync();
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

        IEnumerator RebuildSkipFrame()
        {
            yield return null;
            navMesh.Scan();
        }
        
        IEnumerator ScanAsync()
        {
            foreach (Progress progress in navMesh.ScanAsync())
            {
                yield return null;
            }
        }
    }
}
