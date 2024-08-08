using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.Misc
{
    public class ReduceColliders : MonoBehaviour
    {
        [Button]
        public void Reduce()
        {
            foreach (var c in GetComponentsInChildren<BoxCollider>())
            {
                c.size /= 2f;
            }
        }
    }
}
