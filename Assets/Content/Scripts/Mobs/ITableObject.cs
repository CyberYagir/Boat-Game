using System.Collections.Generic;

namespace Content.Scripts.Mobs
{
    public interface ITableObject
    {
        public Dictionary<string, float> GetWeights();
        void ChangeWeights(List<float> targetWeights);
    }
}