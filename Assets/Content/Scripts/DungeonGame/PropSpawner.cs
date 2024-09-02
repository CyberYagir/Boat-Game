namespace Content.Scripts.DungeonGame
{
    public class PropSpawner : PropRandomBase
    {
        public override void Init(System.Random rnd)
        {
            base.Init(rnd);
            var prefab = prefabs[notAllocatedWeights.ChooseRandomIndexFromWeights(rnd)];
            if (prefab.Prefab != null)
            {
                Instantiate(prefab.Prefab, transform);
            }
        }
    }
}
