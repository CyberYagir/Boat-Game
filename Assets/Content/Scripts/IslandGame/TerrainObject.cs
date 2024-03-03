using System;
using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.IslandGame
{
    public class TerrainObject : MonoBehaviour
    {
        [SerializeField, ReadOnly] private TreeData original;
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private ActionsHolder actionsHolder;
        [SerializeField] private PlayerAction playerAction;
        [SerializeField] private MeshParticles dissolveParticles;
        
        [SerializeField, ReadOnly] private float targetHealth;

        private TreeData respawnedMesh = null;
        
        public bool IsDead => targetHealth <= 0;
        
        private IslandGenerator islandGenerator;
        private int treesInstsancesCount;
        private SelectionService selectionService;
        private GameDataObject gameDataObject;
        private PrefabSpawnerFabric prefabSpawnerFabric;

        public void Init(int instanceID,
            TreeData original,
            SelectionService selectionService,
            GameDataObject gameDataObject,
            PrefabSpawnerFabric prefabSpawnerFabric,
            IslandGenerator islandGenerator)
        {
            this.prefabSpawnerFabric = prefabSpawnerFabric;
            this.gameDataObject = gameDataObject;
            this.selectionService = selectionService;
            this.treesInstsancesCount = instanceID;
            this.islandGenerator = islandGenerator;
            this.original = original;
            transform.name = "Tree Instance #" + instanceID;

            targetHealth = original.HealthRange.RandomWithin();

            original.LoadCollider(capsuleCollider);

            playerAction.SetState(this.original.Action);

            actionsHolder.Construct(selectionService, gameDataObject);
        }

        public void Damage(float damage, Transform damager)
        {
            if (IsDead) return;

            SpawnRealObjectFromTerrain();
            
            targetHealth -= damage;

            respawnedMesh.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
            
            if (targetHealth <= 0)
            {
                DestroyFromTerrain(damager);
                Destroy(gameObject);
            }
        }
        
        public void DestroyFromTerrain(Transform damager)
        {
            var treeHolder = new GameObject("Falled " + original.name);
            treeHolder.transform.position = respawnedMesh.transform.position;
            treeHolder.transform.forward = (new Vector3(damager.transform.position.x, treeHolder.transform.position.y, damager.transform.position.z) - treeHolder.transform.transform.position);
            
            respawnedMesh.transform.parent = treeHolder.transform;

            treeHolder.transform.DORotateQuaternion(Quaternion.Euler(treeHolder.transform.right * -90), 5).SetEase(Ease.InQuart).onComplete += OnFallEnded;

        }

        private void OnFallEnded()
        {
            DOVirtual.DelayedCall(1f, delegate
            {
                var particle = Instantiate(dissolveParticles, respawnedMesh.transform.position, respawnedMesh.transform.rotation)
                    .With(x => x.transform.localScale = respawnedMesh.transform.localScale)
                    .With(x => x.Init(respawnedMesh.GetComponentInChildren<MeshFilter>().sharedMesh));

                DropItems();
                
                Destroy(respawnedMesh.gameObject);
                Destroy(gameObject);
                Destroy(particle, 2f);
            });
        }

        public void DropItems()
        {
            var bounds = respawnedMesh.GetComponentInChildren<MeshRenderer>().bounds;
            var randomBoundsPoint = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));

            prefabSpawnerFabric.SpawnItemOnGround(original.DropItem, randomBoundsPoint, Quaternion.Euler(Random.insideUnitSphere), null)
                .With(x => x.Animate());
        }


        public void SpawnRealObjectFromTerrain()
        {
            if (respawnedMesh != null) return;
            
            var instance = islandGenerator.RemoveTree(treesInstsancesCount, out float size);
            var terrSize = islandGenerator.CurrentIslandData.Terrain.terrainData.size;

            var pos = islandGenerator.CurrentIslandData.Terrain.transform.position + terrSize.MultiplyVector3(instance.position);
            respawnedMesh = Instantiate(
                    original,
                    pos,
                    Quaternion.AngleAxis(instance.rotation * Mathf.Rad2Deg, Vector3.up),
                    null)
                .With(x => x.transform.localScale = Vector3.one * size);
        }
    }
}
