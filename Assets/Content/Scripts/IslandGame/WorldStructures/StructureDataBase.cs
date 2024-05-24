using System.Collections.Generic;
using Content.Scripts.IslandGame.Natives;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class StructureDataBase : MonoBehaviour
    {
        [SerializeField] private bool habitable;
        [SerializeField] private int maxPeoples;
        [SerializeField, ReadOnly] private int seed;
        public int MaxPeoples => maxPeoples;
        public bool Habitable => habitable;

        public int Seed => seed;


        public void Init(System.Random rnd)
        {
            seed = rnd.Next(-100000, 100000);
        }

        public virtual List<ENativeType> GetTypes(int count)
        {
            return new List<ENativeType>();
        }
    }
}
