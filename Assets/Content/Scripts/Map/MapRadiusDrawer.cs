using UnityEngine;
using Zenject;

namespace Content.Scripts.Map
{
    public class MapRadiusDrawer : MonoBehaviour
    {
        private int islandRadiusOffset = 5;

        [Inject]
        private void Construct(MapIslandCollector mapIslandCollector)
        {
            var radius = (mapIslandCollector.Radius * 2) - islandRadiusOffset;
            transform.localScale = new Vector3(radius, 1, radius);
        }
    }
}
