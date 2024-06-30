using System;
using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.DungeonGame
{
    public class RoomGrid : MonoBehaviour
    {
        [SerializeField] private Vector2Int size;


        private void OnDrawGizmos()
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    Gizmos.DrawCube(transform.position + new Vector3(i, 0, j), Vector3.one);
                }
            }
        }

        List<Vector3> list = new List<Vector3>();
        public List<Vector3> GetPoints(Vector3 pos)
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
    }
}
