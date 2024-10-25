using EPOOutline;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.Misc
{
    public class SetMeshesToOutliner : MonoBehaviour
    {
        [Button]
        public void ApplyMeshes()
        {
            GetComponent<Outlinable>().AddAllChildRenderersToRenderingList(transform.parent);
        }
    }
}
