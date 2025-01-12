using System;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using Content.Scripts.IslandGame;
using Content.Scripts.IslandGame.WorldStructures;
using Content.Scripts.IslandGame.WorldStructures.Productor;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Content.Scripts.BoatGame.Services
{
    public class StructuresBuildService : MonoBehaviour
    {

        private Collider[] result = new Collider[10];
        
        private CraftObject targetCraft;
        private GameDataObject gameDataObject;
        private SelectionService selectionService;


        private GameObject spawnedStructure;
        private StructuresService structuresService;

        private List<Renderer> renderers = new List<Renderer>();


        [SerializeField] private Material noneMat, okMat;
        [SerializeField] private StructureBuildProcess buildProgress;
        
        
        bool isCanBuild = false;
        private Bounds bounds;
        public event Action OnBuildStarted;
        public event Action OnBuildEnd;
        public GameObject SpawnedBuild => spawnedStructure;
        public CraftObject Craft => targetCraft;


        [Inject]
        private void Construct(
            GameDataObject gameDataObject, 
            GameStateService gameStateService, 
            SelectionService selectionService, 
            StructuresService structuresService, 
            SaveDataObject saveDataObject, 
            INavMeshProvider navMeshProvider,
            PrefabSpawnerFabric fabric,
            IResourcesService resourcesService,
            SaveService saveService)
        {
            this.saveService = saveService;
            this.resourcesService = resourcesService;
            this.fabric = fabric;
            this.navMeshProvider = navMeshProvider;
            this.saveDataObject = saveDataObject;
            this.gameStateService = gameStateService;
            this.structuresService = structuresService;
            this.selectionService = selectionService;
            this.gameDataObject = gameDataObject;
            
            gameStateService.OnChangeEState += GameStateServiceOnOnChangeEState;
            selectionService.OnTapOnTerrain += SelectionServiceOnOnTapOnTerrain;


        }

        private void Start()
        {
            SpawnStructures();
        }

        public void SpawnStructures()
        {
            if (saveDataObject.GetTargetIsland() == null) return;
            
            var structures = saveDataObject.GetTargetIsland().PlayerStructuresData;
            for (int i = 0; i < structures.Count; i++)
            {
                var craft = gameDataObject.Crafts.Find(x => x.Uid == structures[i].CraftID);

                var spawnedStructure = structuresService.GetPlayerStructure(structures[i].Uid);

                var buildState = structures[i].IsBuilded(craft.CraftTime);

                
                if (spawnedStructure != null)
                {
                    if (spawnedStructure.IsBuilded != buildState)
                    {
                        structuresService.DestroyPlayerBuilding(structures[i].Uid);
                    }
                }
                
                if (!buildState)
                {
                    StructureBuildProcess item = fabric.SpawnItemOnGround(buildProgress, structures[i].Pos, Quaternion.Euler(0, structures[i].Rot, 0));
                    item.Init(craft.Uid, structures[i].StartBuildDate);
                    var playerStructure = new StructuresService.PlayerStructure(structures[i].Uid, false, item.gameObject);
                    structuresService.AddPlayerBuilding(playerStructure);
                }
                else if ((spawnedStructure != null && spawnedStructure.IsBuilded != buildState) || spawnedStructure == null)
                {
                    BuildableStructure building = gameDataObject.BuildableStructures.Find(x => x.Craft == craft);

                    var item = fabric.SpawnItemOnGround(building, structures[i].Pos,
                        Quaternion.Euler(0, structures[i].Rot, 0));
                    var structure = item.GetComponent<Structure>();
                    structure.Init(new Random(0), structuresService.Island.TargetBiome);

                    var playerStructure =
                        new StructuresService.PlayerStructure(structures[i].Uid, true, item.gameObject);
                    structuresService.AddPlayerBuilding(playerStructure);
                }
            }
        }


        public void StopBuilding()
        {
            gameStateService.ChangeGameState(GameStateService.EGameState.Normal);
        }
        
        private void SelectionServiceOnOnTapOnTerrain(Vector3 obj)
        {
            if (spawnedStructure != null)
            {
                spawnedStructure.transform.position = obj;


                var isCanPlaceNow = IsCanPlace();


                if (isCanBuild != isCanPlaceNow)
                {
                    
                    ChangeMaterials(isCanPlaceNow ? okMat : noneMat);
                    
                    isCanBuild = isCanPlaceNow;
                }
            }
        }

        public void ChangeMaterials(Material mat)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                var mats = renderers[i].materials;

                for (int j = 0; j < mats.Length; j++)
                {
                    mats[j] = mat;
                }

                renderers[i].materials = mats;
            }
        }

        private void GameStateServiceOnOnChangeEState(GameStateService.EGameState obj)
        {
            if (obj == GameStateService.EGameState.BuildingStructures)
            {
                if (targetCraft != null)
                {
                    ActivateBuilding();
                }
            }
            else
            {
                if (spawnedStructure)
                {
                    Destroy(spawnedStructure.gameObject);
                    
                    OnBuildEnd?.Invoke();
                }
            }
        }

        private void ActivateBuilding()
        {
            angle = 0;
            isRotating = false;
            var building = gameDataObject.BuildableStructures.Find(x => x.Craft == targetCraft);

            if (building != null)
            {
                spawnedStructure = Instantiate(building.gameObject, selectionService.LastWorldClick, Quaternion.identity);
                var structure = spawnedStructure.GetComponent<Structure>();
                structure.Init(new Random(0), structuresService.Island.TargetBiome);
                structure.DisableAllRandom();


                renderers = structure.GetComponentsInChildren<Renderer>().ToList();

                var particles = structure.GetComponentsInChildren<ParticleSystem>(true);

                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].gameObject.SetActive(false);
                }
                
                
                var animations = structure.GetComponentsInChildren<IVisualAnimation>(true);

                for (int i = 0; i < animations.Length; i++)
                {
                    (animations[i] as MonoBehaviour).enabled = false;
                }
                
                var isCanPlaceNow = IsCanPlace();


                ChangeMaterials(isCanPlaceNow ? okMat : noneMat);

                var colliders = structure.GetComponentsInChildren<Collider>();

                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = false;
                }

                selectionService.BuildingStateStructureSelectionLogic(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
                
                isCanBuild = isCanPlaceNow;
                
                OnBuildStarted?.Invoke();
            }
        }

        List<float> yList = new List<float>(5);
        List<float> deltaPoses = new List<float>(5);
        public bool IsCanPlace()
        {
            bounds = renderers[0].bounds;

            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].gameObject.activeInHierarchy)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
            }



            bool isCanAngle = true;
            var mask = LayerMask.GetMask("Builds", "Obstacle", "Default", "Trees");
            var tarrainmask = LayerMask.GetMask("Terrain");
            var colliders = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, result, spawnedStructure.transform.rotation, mask);

            
            

            if (colliders == 0)
            {
                yList.Clear();
                deltaPoses.Clear();

                Vector3 center = bounds.center;
                if (Physics.Raycast(bounds.min + Vector3.up * 10, Vector3.down, out var hit, Mathf.Infinity, tarrainmask,
                        QueryTriggerInteraction.Ignore))
                {
                    yList.Add(hit.point.y);
                }
                
                if (Physics.Raycast(bounds.max + Vector3.up * 10, Vector3.down, out hit, Mathf.Infinity, tarrainmask,
                        QueryTriggerInteraction.Ignore))
                {
                    yList.Add(hit.point.y);
                }
                
                
                if (Physics.Raycast(bounds.center + Vector3.up * 10, Vector3.down, out hit, Mathf.Infinity, tarrainmask,
                        QueryTriggerInteraction.Ignore))
                {
                    center = hit.point;
                }

                if (yList.Count != 0)
                {
                    for (int i = 0; i < yList.Count; i++)
                    {
                        deltaPoses.Add(center.y-yList[i]);
                    }

                    float diff = deltaPoses.Sum() / deltaPoses.Count;

                    
                    print(diff);
                    
                    if (diff > 1.5f || diff < -1.5f)
                    {
                        isCanAngle = false;
                    }
                }

            }
            
            
            return colliders == 0 && spawnedStructure.transform.position.y > 1 && isCanAngle;
        }

        private void OnDrawGizmos()
        {
            if (spawnedStructure != null)
            {
                Gizmos.DrawCube(bounds.center, bounds.size);
            }
        }


        public void SetTargetCraft(CraftObject craftObject)
        {
            targetCraft = craftObject;
        }

        private bool isRotating = false;
        private float angle = 0;
        private GameStateService gameStateService;
        private SaveDataObject saveDataObject;
        private INavMeshProvider navMeshProvider;
        private PrefabSpawnerFabric fabric;
        private IResourcesService resourcesService;
        private SaveService saveService;

        public void RotateBuilding()
        {
            if (isRotating) return;
            isRotating = true;
            angle += 45f;
            spawnedStructure.transform.DORotate(new Vector3(0, angle, 0), 0.5f).onComplete +=
                () => { isRotating = false; };
        }

        public void ApplyBuilding()
        {
            if (isCanBuild && !isRotating)
            {
                saveDataObject.GetTargetIsland().AddPlayerStructure(new SaveDataObject.MapData.IslandData.PlayerStructure(targetCraft.Uid, spawnedStructure.transform.position, angle));
                StopBuilding();
                SpawnStructures();
                resourcesService.RemoveItemsForCraft(targetCraft);
                navMeshProvider.BuildNavMeshAsync();
                saveService.SaveWorld();
            }
        }
    }
}
