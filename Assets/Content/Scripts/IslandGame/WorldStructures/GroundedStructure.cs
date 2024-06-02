using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class GroundedStructure : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Grounder")] private LayerMask mask;
        [SerializeField, FoldoutGroup("Grounder")] private bool keepRotation = true;
        [SerializeField, FoldoutGroup("Grounder")] private Vector3 rotationOffset;

        private void Awake()
        {
            if (Physics.Raycast(transform.position + Vector3.up * 100, Vector3.down, out RaycastHit hit, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
            {
                transform.position = hit.point;
                if (keepRotation)
                {
                    transform.rotation = hit.normal.ToRotation(transform) * Quaternion.Euler(rotationOffset);
                }
            }
        }
    }
}