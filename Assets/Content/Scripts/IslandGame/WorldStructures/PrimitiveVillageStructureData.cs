using System.Collections.Generic;
using Content.Scripts.IslandGame.Natives;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class PrimitiveVillageStructureData : StructureDataBase
    {
        [SerializeField, Range(0, 1f)] private float girlChance = 0.75f;
        [SerializeField] private bool haveShaman;
        public override List<ENativeType> GetTypes(int count)
        {
            var rnd = new System.Random(Seed);
            var list = new List<ENativeType>();

            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    list.Add(haveShaman ? ENativeType.Shaman : ENativeType.Man);
                }
                else
                {
                    list.Add(rnd.NextDouble() < girlChance ? ENativeType.Female : ENativeType.Man);
                }
            }

            return list;
        }
    }
}