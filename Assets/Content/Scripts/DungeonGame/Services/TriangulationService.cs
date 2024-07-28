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

        public List<ITriangle> Tris => tris;

        [Inject]
        private void Construct(RoomsPlacerService roomsPlacerService)
        {
            List<IPoint> points = new List<IPoint>();
            foreach (var ctr in roomsPlacerService.RoomsCenters)
            {
                points.Add((IPoint) new Point(ctr.x, ctr.z));
            }

            var delaunator = new DelaunatorSharp.Delaunator(points.ToArray());

            tris = delaunator.GetTriangles().ToList();
        }

        // private void OnDrawGizmos()
        // {
        //     if (Tris == null) return;
        //
        //     foreach (var triangle in Tris)
        //     {
        //         IPoint p1 = triangle.Points.ToList()[0];
        //         IPoint p2 = triangle.Points.ToList()[1];
        //         IPoint p3 = triangle.Points.ToList()[2];
        //
        //
        //         Gizmos.DrawLine(new Vector3((float) p1.X, 0, (float) p1.Y), new Vector3((float) p2.X, 0, (float) p2.Y));
        //         Gizmos.DrawLine(new Vector3((float) p2.X, 0, (float) p2.Y), new Vector3((float) p3.X, 0, (float) p3.Y));
        //         Gizmos.DrawLine(new Vector3((float) p3.X, 0, (float) p3.Y), new Vector3((float) p1.X, 0, (float) p1.Y));
        //
        //     }
        // }
    }
}
