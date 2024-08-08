using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;

namespace Content.Scripts.DungeonGame
{
    public class RoomGrid : MonoBehaviour
    {
        [SerializeField] private Vector2Int size;
        public Vector2Int Size => size;

        private void OnDrawGizmos()
        {
            
            if (Application.isPlaying) return;
            
           // Gizmos.DrawCube(transform.position + (new Vector3(size.x, 0, size.y)/2f) - new Vector3(0.5f, 0, 0.5f), new Vector3(size.x, 1, size.y));


             for (int i = 0; i < size.x; i++)
             {
                 for (int j = 0; j < size.y; j++)
                 {
                     if (i % 2 == 0 && j % 2 == 0)
                     {
                         Gizmos.color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.25f);
                     }
                     else
                     {
                         Gizmos.color = new Color(Color.green.r/2f, Color.green.g/2f, Color.green.b/2f, 0.25f);
                     }
                     Gizmos.DrawCube(transform.position + new Vector3(i, -1, j), Vector3.one);
                 }
             }
        }

        private List<Vector3> list = new List<Vector3>();
        public List<Vector3> GetPointsInGlobalSpace(Vector3 pos)
        {
            list.Clear();
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    list.Add(pos + new Vector3(i, 0, j));
                }
            }

            return list;
        }

        public Vector3 GetCenter() => transform.position + (new Vector3(size.x, 0, size.y) / 2f) - Vector3.one * 0.5f;

        public void CheckAllTiles(WorldGridServiceTyped worldGridService)
        {
            var environments = GetComponent<RoomEnvironments>();

            if (environments != null)
            {
                environments.CalculateRoomEnters(worldGridService);
            }
        }
    }
}
