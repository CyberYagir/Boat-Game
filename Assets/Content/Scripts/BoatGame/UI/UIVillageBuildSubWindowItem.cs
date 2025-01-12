using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.WorldStructures;
using Content.Scripts.ItemsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageBuildSubWindowItem : UICraftsItem
    {
        private IResourcesService resourcesService;
        private Structure structure;
        private StructuresBuildService structureBuildService;
        private BuildableStructure buildableStructure;

        public void SetBuildData(Structure structure, BuildableStructure buildableStructure, StructuresBuildService structureBuildService)
        {
            this.buildableStructure = buildableStructure;
            this.structureBuildService = structureBuildService;
            this.structure = structure;
            text.text = buildableStructure.Craft.CraftName;
            UpdateItem();
        }


        public override void Build()
        {
            structureBuildService.SetTargetCraft(buildableStructure.Craft);
            uiService.ChangeGameStateToBuildStructures();
            uiService.CloseAllWindows();
        }
    }
}
