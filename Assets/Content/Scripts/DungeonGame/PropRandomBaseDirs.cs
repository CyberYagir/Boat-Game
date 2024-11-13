using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.DungeonGame
{
    public class PropRandomBaseDirs : PropRandomBase
    {
        protected List<Vector3> dirs = new List<Vector3>();

        public void SetDirs(List<Vector3> dir)
        {
            dirs = dir;
        }

        public override void Init(Random random)
        {
            if (dirs.Count == 4)
            {
                base.Init(random);
            }
        }
    }
}