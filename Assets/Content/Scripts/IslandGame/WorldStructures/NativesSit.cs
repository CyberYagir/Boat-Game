using System;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class NativesSit : GroundedStructure
    {
        [SerializeField] private bool isNotEmpty;

        public bool IsNotEmpty => isNotEmpty;

        public void SetState(bool state)
        {
            isNotEmpty = state;
        }


    }
}
