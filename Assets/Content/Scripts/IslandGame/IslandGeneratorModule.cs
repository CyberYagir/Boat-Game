using Content.Scripts.Map;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public abstract class IslandGeneratorModule : MonoBehaviour
    {
        protected IslandGenerator islandGenerator;

        public virtual void Init(IslandGenerator islandGenerator)
        {
            this.islandGenerator = islandGenerator;
        }
    }
}
