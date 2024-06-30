using System;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using PathCreation.Utility;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class TriangulationService : MonoBehaviour
    {
        private List<ITriangle> tris;

        [Inject]
        private void Construct(RoomsPlacerService roomsPlacerService)
        {
            List<IPoint> points = new List<IPoint>();
            foreach (var ctr in roomsPlacerService.RoomsCenters)
            {
                points.Add((IPoint)new Point(ctr.x, ctr.z));
            }

            var delaunator = new DelaunatorSharp.Delaunator(points.ToArray());

            tris = delaunator.GetTriangles().ToList();
            
        }

        private void OnDrawGizmos()
        {
            if (tris == null) return;
            
            foreach (var triangle in tris)
            {
                IPoint lastPoint = triangle.Points.ToList()[0];
                foreach (var trianglePoint in triangle.Points)
                {
                    Gizmos.DrawLine(new Vector3((float) lastPoint.X, 0, (float) lastPoint.Y), new Vector3((float) trianglePoint.X, 0, (float) trianglePoint.Y));
                }
            }
        }
    }
}
