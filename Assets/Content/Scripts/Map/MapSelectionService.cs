using System;
using Content.Scripts.Boot;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Map
{
    public class MapSelectionService : MonoBehaviour
    {
        private MapSpawnerService mapSpawnerService;
        private MapIslandCollector mapIslandCollector;
        private ScenesService scenesService;


        private MapIsland selectedIsland;

        public event Action<MapIsland> OnSelectIsland;
        
        
        [Inject]
        private void Construct(
            MapSpawnerService mapSpawnerService, 
            MapIslandCollector mapIslandCollector, 
            ScenesService scenesService)
        {
            this.scenesService = scenesService;
            this.mapIslandCollector = mapIslandCollector;
            this.mapSpawnerService = mapSpawnerService;
            
            
            scenesService.OnChangeActiveScene += OnChangeScene;
            OnChangeScene(scenesService.GetActiveScene());
        }

        private void OnChangeScene(ESceneName scene)
        {
            gameObject.SetActive(scene == ESceneName.Map);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
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
    }
}