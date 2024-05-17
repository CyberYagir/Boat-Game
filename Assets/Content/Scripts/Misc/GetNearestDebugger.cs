using System;
using Content.Scripts.BoatGame.Services;
using Pathfinding;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Misc
{
    public class GetNearestDebugger : MonoBehaviour
    {
        
        private Vector3 point;
        private SelectionService selectionService;

        [Inject]
        private void Construct(SelectionService selectionService)
        {
            this.selectionService = selectionService;
        }
        
        private void Update()
        {
            transform.position = selectionService.LastWorldClick;
            var constraint = NNConstraint.Default;
            constraint.constrainWalkability = true;
            constraint.walkable = true;
            constraint.constrainTags = true;
            constraint.tags = ~0;
            constraint.graphMask = ~0;
            NNInfo info = new NNInfo();

            info = AstarPath.active.GetNearest(transform.position, constraint);
            
            
            point = info.position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(point, 2f);
        }
    }
}
