using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    [Serializable]
    public class RaftNodesChecker
    {
        [System.Serializable]
        public class Part
        {
            private List<RaftBase> rafts = new List<RaftBase>(10);
            private int priorityIndex;
            public List<RaftBase> Rafts => rafts;

            public int PriorityIndex => priorityIndex;


            public void AddRaft(RaftBase raftBase) => rafts.Add(raftBase);

            public void CalculatePriorityIndex()
            {
                for (int i = 0; i < rafts.Count; i++)
                {
                    switch (rafts[i].RaftType)
                    {
                        case RaftBuildService.RaftItem.ERaftType.Default:
                            priorityIndex += 1;
                            break;
                        case RaftBuildService.RaftItem.ERaftType.Storage:
                            priorityIndex += 5;
                            break;
                        case RaftBuildService.RaftItem.ERaftType.Building:
                            priorityIndex += 0;
                            break;
                        case RaftBuildService.RaftItem.ERaftType.CraftTable:
                            priorityIndex += 4;
                            break;
                        case RaftBuildService.RaftItem.ERaftType.Moored:
                            priorityIndex += 4;
                            break;
                        case RaftBuildService.RaftItem.ERaftType.Fishing:
                            priorityIndex += 4;
                            break;
                        case RaftBuildService.RaftItem.ERaftType.Furnace:
                            priorityIndex += 3;
                            break;
                        default:
                            priorityIndex += 1;
                            break;
                    }
                }
            }

            public void KillRafts()
            {
                for (int i = 0; i < rafts.Count; i++)
                {
                    rafts[i].Kill();
                }
            }
        }

        [SerializeField] private List<Part> parts = new List<Part>();
        [SerializeField] private List<RaftBase> calculatedRafts = new List<RaftBase>(10);
        private WorldGridService worldGridService;
        private Dictionary<Vector3Int, RaftBase> raftsMap;

        public List<Part> CalculateParts(List<RaftBase> rafts, WorldGridService worldGridService)
        {
            parts.Clear();
            calculatedRafts.Clear();

            this.worldGridService = worldGridService;
            raftsMap = rafts.ToDictionary(x => x.Coords);

            if (rafts.Count <= 1) return parts;

            int trys = 0;
            RaftBase start;
            do
            {
                start = rafts.Find(x => !calculatedRafts.Contains(x));
                if (start == null) break;
                
                calculatedRafts.Add(start);
                
                var part = new Part();
                part.AddRaft(start);

                CalculateRaftsRecursive(start, part);

                parts.Add(part);

                trys++;

                if (trys > 5)
                {
                    break;
                }

            } while (start != null);


            return parts;
        }

        private void CalculateRaftsRecursive(RaftBase start, Part part)
        {
            AddToPart(part, start.Coords + Vector3Int.forward);
            AddToPart(part, start.Coords + Vector3Int.back);
            AddToPart(part, start.Coords + Vector3Int.left);
            AddToPart(part, start.Coords + Vector3Int.right);
        }

        private void AddToPart(Part part, Vector3Int pos)
        {
            if (worldGridService.IsHavePoint(pos))
            {
                if (!part.Rafts.Contains(raftsMap[pos]))
                {
                    if (!calculatedRafts.Contains(raftsMap[pos]))
                    {
                        part.Rafts.Add(raftsMap[pos]);
                        calculatedRafts.Add(raftsMap[pos]);
                        CalculateRaftsRecursive(raftsMap[pos], part);
                    }
                }
            }
        }
    }
}