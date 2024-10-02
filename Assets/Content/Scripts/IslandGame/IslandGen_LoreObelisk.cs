using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class IslandGen_LoreObelisk : IslandGen_SpawnModule
    {
        public override bool SpawnAfterConstruct() => false;

        public override void Init(IslandGenerator islandGenerator)
        {
            base.Init(islandGenerator);
            if (islandGenerator.SaveData.Map.IsHavePlotOnIsland(islandGenerator.SaveData.GetTargetIsland().IslandSeed))
            {
                generateObjectCalculator.SetIslandGenerator(islandGenerator);
                SpawnObject(prefab);
            }
        }
    }
}
