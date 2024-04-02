namespace Content.Scripts.IslandGame.WorldStructures
{
    public class RandomRoofStructureMaterials : RandomStructureMaterialsBase
    {
        public override void Init(TerrainBiomeSO biome)
        {
            var holder = gameData.StructuresRoofMaterials.Find(x => x.Biome.Contains(biome));
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