using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.WorldStructures;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Random = System.Random;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageBuildSubWindow : MonoBehaviour
    {
        [SerializeField] private UIVillageBuildSubWindowItem item;


        List<UIVillageBuildSubWindowItem> items = new List<UIVillageBuildSubWindowItem>();
        private List<VillageGenerator.SubStructures.SubStructure> canBuild;
        private StructuresService.VillageStructure village;
        private IResourcesService resourcesService;
        private UIService uiService;
        private SaveDataObject saveDataObject;
        private StructuresBuildService structureBuildService;

        public void Init(
            UIVillageOptionsWindow uiVillageOptionsWindow,
            SaveDataObject.MapData.IslandData.VillageData villageData,
            StructuresService structuresService,
            IResourcesService resourcesService,
            UIService uiService,
            SaveDataObject saveDataObject,
            StructuresBuildService structureBuildService)
        {
            this.structureBuildService = structureBuildService;
            this.saveDataObject = saveDataObject;
            this.uiService = uiService;
            this.resourcesService = resourcesService;
            village = structuresService.GetVillageStructure(villageData.Uid);

            var structuresByBiome = village.VillageGenerator.GetStructuresList();


            canBuild = structuresByBiome.FindAll(x => x.Structure.GetComponent<BuildableStructure>());


            resourcesService.OnChangeResources += ResourcesServiceOnOnChangeResources;


            DrawItems();
        }

        private void ResourcesServiceOnOnChangeResources()
        {
            UpdateWindow();
        }

        private void OnEnable()
        {
            UpdateWindow();
        }

        private void UpdateWindow()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].UpdateItem();
            }
        }


        private void DrawItems()
        {
            item.gameObject.SetActive(true);
            var count = items.Count;
            for (int i = count; i < canBuild.Count; i++)
            {
                var id = i;
                var buildableStructure = canBuild[i].Structure.GetComponent<BuildableStructure>();
                // var structure = Instantiate(canBuild[i].Structure, new Vector3(0, 2000, 0), Quaternion.identity);
                //
                // structure.Init(new Random(0), village.VillageGenerator.GetBiome());
                //
                // structure.DisableAllRandom();
                //
                // structure.gameObject.ChangeLayerWithChilds(LayerMask.NameToLayer("WorldUI"));
                //
                //
                // var camera = buildableStructure.PreviewCamera;
                //
                // var targetRederer = camera.targetTexture;
                //
                //
                // camera.targetTexture = new RenderTexture(512, 512, targetRederer.depth, targetRederer.graphicsFormat);
                //
                // camera.Render();

                Instantiate(item, item.transform.parent)
                    .With(x => items.Add(x))
                    .With(x => x.Init(buildableStructure.Craft, resourcesService, uiService, null, saveDataObject))
                    .With(x => x.SetBuildData(canBuild[id].Structure, buildableStructure, structureBuildService));



                // var png = toTexture2D(camera.targetTexture).EncodeToPNG();
                //
                //
                // if (!Directory.Exists("D:/Test/"))
                // {
                //     Directory.CreateDirectory("D:/Test/");
                // }
                //
                // File.WriteAllBytes($"D:/Test/{buildableStructure.Craft.name}.png", png);
                //
                //

                // structure.gameObject.gameObject.SetActive(false);
            }

            item.gameObject.SetActive(false);
        }
    }
}
