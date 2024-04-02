using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class RandomCampStructureMaterials : RandomStructureMaterialsBase
    {

        public override void Init(TerrainBiomeSO biome)
        {
            var holder = gameData.StructuresMaterials.Find(x => x.Biome.Contains(biome));
            if (holder != null)
            {
                for (int i = 0; i < renderer.Length; i++)
                {
                    this.renderer[i].sharedMaterial = holder.Material;
                }
            }
        }
    }
}
