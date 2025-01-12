using System;
using Content.Scripts.BoatGame.Services;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIPlaceBuildingOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text buildingNameText;
        private StructuresBuildService structuresBuildService;
        private SelectionService selectionService;
        private GameStateService gameStateService;

        public void Init(StructuresBuildService structuresBuildService, SelectionService selectionService, GameStateService gameStateService)
        {
            this.gameStateService = gameStateService;
            this.selectionService = selectionService;
            this.structuresBuildService = structuresBuildService;
            structuresBuildService.OnBuildStarted += ActivateOverlay;
            structuresBuildService.OnBuildEnd += DesactivateOverlay;

            DesactivateOverlay();
        }

        private void DesactivateOverlay()
        {
            gameObject.SetActive(false);
        }

        private void ActivateOverlay()
        {
            gameObject.SetActive(true);
            buildingNameText.text = structuresBuildService.Craft.CraftName;
        }


        private void Update()
        {
            if (structuresBuildService.SpawnedBuild != null)
            {
                var screenPos = selectionService.Camera.WorldToScreenPoint(
                    structuresBuildService.SpawnedBuild.transform.position, Camera.MonoOrStereoscopicEye.Mono);
                transform.position = screenPos;
            }
        }


        public void RotateBuilding()
        {
            structuresBuildService.RotateBuilding();
        }

        public void CancelBuilding()
        {
            structuresBuildService.StopBuilding();
        }

        public void ApplyBuilding()
        {
            structuresBuildService.ApplyBuilding();
        }
    }
}
