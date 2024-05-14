using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Scriptable;
using Content.Scripts.Global;
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
            private RaftsPriorityObject raftsData;

            public Part(RaftsPriorityObject raftsData)
            {
                this.raftsData = raftsData;
            }

            public List<RaftBase> Rafts => rafts;

            public int PriorityIndex => priorityIndex;


            public void AddRaft(RaftBase raftBase) => rafts.Add(raftBase);

            public void CalculatePriorityIndex()
            {
                for (int i = 0; i < rafts.Count; i++)
                {
                    priorityIndex += raftsData.GetIndex(rafts[i].RaftType);
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
        private RaftsPriorityObject raftsData;
        private Dictionary<Vector3Int, RaftBase> raftsMap;

        public List<Part> CalculateParts(List<RaftBase> rafts, WorldGridService worldGridService, GameDataObject gameDataObject)
        {
            raftsData = gameDataObject.RaftsPriorityData;
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
                
                var part = new Part(raftsData);
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