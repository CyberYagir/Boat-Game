using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class TerrainObject : MonoBehaviour
    {
        [SerializeField, ReadOnly] private GameObject original;

        public void Init(int treesInstsancesCount, GameObject original)
        {
            this.original = original;
            transform.name = "Tree Instance #" + treesInstsancesCount;
        }
    }
}
