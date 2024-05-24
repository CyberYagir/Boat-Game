using System.Collections.Generic;
using Content.Scripts.IslandGame.Natives;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class PrimitiveVillageStructure : StructureDataBase
    {
        [SerializeField, Range(0, 1f)] private float girlChange;
        public override List<ENativeType> GetTypes(int count)
        {
            var rnd = new System.Random(Seed);
            var list = new List<ENativeType>();

            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    list.Add(ENativeType.Man);
                }
                else
                {
                    list.Add(rnd.NextDouble() < girlChange ? ENativeType.Female : ENativeType.Man);
                }
            }

            return list;
        }
    }
}