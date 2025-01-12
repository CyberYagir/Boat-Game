using System;
using Content.Scripts.CraftsSystem;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class BuildableStructure : MonoBehaviour
    {
        [SerializeField] private Camera previewCamera;
        [SerializeField] private CraftObject craftObject;
        
        public Camera PreviewCamera => previewCamera;

        public CraftObject Craft => craftObject;


        private void Awake()
        {
            previewCamera.enabled = false;
        }
    }
}
