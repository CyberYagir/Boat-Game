using System.Collections.Generic;
using Content.Scripts.IslandGame.Natives;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class OneManStructureData : StructureDataBase
    {
        [SerializeField] private ENativeType workerType;
        public override List<ENativeType> GetTypes(int count)
        {
            var rnd = new System.Random(Seed);
            var list = new List<ENativeType>();

            for (int i = 0; i < count; i++)
            {
                list.Add(workerType);
            }

            return list;
        }
    }
}