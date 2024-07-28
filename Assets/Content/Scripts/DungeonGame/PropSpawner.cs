namespace Content.Scripts.DungeonGame
{
    public class PropSpawner : PropRandomBase
    {
        public override void Init()
        {
            base.Init();
            var prefab = prefabs[notAllocatedWeights.ChooseRandomIndexFromWeights()];
            if (prefab.Prefab != null)
            {
                Instantiate(prefab.Prefab, transform);
            }
        }
    }
}
