using System;
using System.Collections;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using Content.Scripts.QuestsSystem;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Map
{
    public class MapSelectionService : MonoBehaviour
    {
        [SerializeField] private Canvas uiService;
    
        private OverUIChecker overUIChecker;
        private MapSpawnerService mapSpawnerService;
        private MapIslandCollector mapIslandCollector;
        private ScenesService scenesService;
        private SelectionService selectionService;

        private MapIsland selectedIsland;
        private SaveDataObject saveDataObject;

        public event Action<MapIsland> OnSelectIsland;

        bool isCanSelect = false;

        [Inject]
        private void Construct(
            MapSpawnerService mapSpawnerService,
            MapIslandCollector mapIslandCollector,
            ScenesService scenesService,
            SaveDataObject saveDataObject
        )
        {
            this.saveDataObject = saveDataObject;
            this.scenesService = scenesService;
            this.mapIslandCollector = mapIslandCollector;
            this.mapSpawnerService = mapSpawnerService;
            this.selectionService = CrossSceneContext.GetSelectionService();

            overUIChecker = new OverUIChecker(uiService.gameObject);
            
            scenesService.OnChangeActiveScene += OnChangeScene;
            scenesService.OnLoadOtherScene += ScenesServiceOnOnLoadOtherScene;
            scenesService.OnUnLoadOtherScene += ScenesServiceOnOnUnLoadOtherScene;
            
            OnChangeScene(scenesService.GetActiveScene());
        }

        private void ScenesServiceOnOnLoadOtherScene(ESceneName obj)
        {
            RemoveEvents();
        }

        private void ScenesServiceOnOnUnLoadOtherScene(ESceneName obj)
        {
            if (obj == ESceneName.Map)
            {
                RemoveEvents();
            }
        }

        private void RemoveEvents()
        {
            scenesService.OnChangeActiveScene -= OnChangeScene;
            scenesService.OnLoadOtherScene -= ScenesServiceOnOnUnLoadOtherScene;
            scenesService.OnUnLoadOtherScene -= ScenesServiceOnOnUnLoadOtherScene;
        }

        private void OnChangeScene(ESceneName scene)
        {
            gameObject.SetActive(scene == ESceneName.Map);
            if (scene == ESceneName.Map)
            {
                StartCoroutine(SkipFrameToSelect());
            }
            else
            {
                isCanSelect = false;
            }
        }

        IEnumerator SkipFrameToSelect()
        {
            yield return null;
            isCanSelect = true;
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (!isCanSelect) return;
                if (selectionService.IsUIBlocked) return;
                overUIChecker.CheckUILogic();
                if (overUIChecker.IsUIBlocked) return;
                
                var hit = mapSpawnerService.Camera.MouseRaycast(out bool isHit, Input.mousePosition, Mathf.Infinity, LayerMask.GetMask("Map"));

                if (isHit)
                {
                    var island = hit.collider.GetComponent<MapIsland>();
                    if (mapIslandCollector.IslandsInRadius.Contains(island))
                    {
                        selectedIsland = island;
                        OnSelectIsland?.Invoke(selectedIsland);
                    }
                }
            }
        }

        public void LoadIsland()
        {
            if (selectedIsland != null)
            {
                QuestsEventBus.CallOnLandIsland();
                TimeService.SetTimeRate(1f);
                saveDataObject.Global.SetIslandSeed(selectedIsland.Seed);
                scenesService.FadeScene(ESceneName.IslandGame);
            }
        }
    }
}
