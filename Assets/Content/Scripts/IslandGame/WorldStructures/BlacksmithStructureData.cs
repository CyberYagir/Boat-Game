﻿using System.Collections.Generic;
using Content.Scripts.IslandGame.Natives;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class BlacksmithStructureData : StructureDataBase
    {
        public override List<ENativeType> GetTypes(int count)
        {
            var rnd = new System.Random(Seed);
            var list = new List<ENativeType>();

            for (int i = 0; i < count; i++)
            {
                list.Add(ENativeType.Blacksmith);
            }

            return list;
        }
    }
}