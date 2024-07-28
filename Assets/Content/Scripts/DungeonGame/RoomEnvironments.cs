using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.DungeonGame
{
    public class RoomEnvironments : MonoBehaviour
    {
        [System.Serializable]
        public class TransformsInCells
        {
            [SerializeField] private Vector3Int pos;
            [SerializeField] private List<Transform> transforms;

            public TransformsInCells(Vector3Int pos, List<Transform> transforms)
            {
                this.pos = pos;
                this.transforms = transforms;
            }

            public List<Transform> Transforms => transforms;

            public Vector3Int Pos => pos;

            public void Disable()
            {
                foreach (var t in transforms)
                {
                    if (t != null)
                    {
                        t.gameObject.SetActive(false);
                    }
                }
            }
        }

        [SerializeField] private RoomGrid roomGrid;
        [SerializeField] private List<Transform> items;
        [SerializeField] private List<TransformsInCells> itemsInCell;
        private List<Vector3> points;
        private WorldGridServiceTyped worldGridService;


        [Button]
        public void Collect()
        {
            itemsInCell.Clear();
            foreach (var it in roomGrid.GetPointsInGlobalSpace(transform.position))
            {
                var localPos = transform.InverseTransformPoint(it);
                var renderer = transform.GetComponent<Renderer>();
                if (renderer)
                {
                    localPos = transform.InverseTransformPoint(renderer.bounds.center);
                }
                for (int i = 0; i < items.Count; i++)
                {
                    if (IsInBounds(items[i], localPos))
                    {
                        var roundedPos = Vector3Int.RoundToInt(localPos);
                        var holder = itemsInCell.Find(x => x.Pos == roundedPos);
                        if (holder == null)
                        {
                            holder = new TransformsInCells(roundedPos, new List<Transform>());
                            itemsInCell.Add(holder);
                        }
                        
                        holder.Transforms.Add(items[i]);
                    }
                }
            }
        }

        public bool IsInBounds(Transform p, Vector3 pos)
        {
            Bounds b = new Bounds(pos + Vector3.down * 5, new Vector3(1, 10, 1));
            
            return b.Contains(transform.InverseTransformPoint(p.transform.position));
        }

        public void CalculateRoomEnters(WorldGridServiceTyped worldGridService)
        {
            this.worldGridService = worldGridService;
            points = roomGrid.GetPointsInGlobalSpace(transform.position);
            foreach (var globalSpacePoint in points)
            {
                var roundedVector = Vector3Int.RoundToInt(globalSpacePoint);
                if (HasEnter(roundedVector))
                {

                    var item = itemsInCell.Find(x => x.Pos == Vector3Int.RoundToInt(transform.InverseTransformPoint(roundedVector)));
                    if (item != null)
                    {
                        item.Disable();
                    }
                }
            }
        }

        private bool HasEnter(Vector3Int globalPos)
        {
            if (
                IsRoomAndNotCurrent(globalPos + Vector3Int.forward) ||
                IsRoomAndNotCurrent(globalPos + Vector3Int.back) ||
                IsRoomAndNotCurrent(globalPos + Vector3Int.right) ||
                IsRoomAndNotCurrent(globalPos + Vector3Int.left)
            )
            {
                return true;
            }

            return false;
        }

        private bool IsRoomAndNotCurrent(Vector3Int pos)
        {
            worldGridService.IsHavePoint(pos, out var type);
            if (type == WorldGridServiceTyped.ECellType.Filled)
            {
                return true;
            }
            else if (type == WorldGridServiceTyped.ECellType.Room && !points.Contains(pos))
            {
                return true;
            }
            return false;
        }
    }
}
