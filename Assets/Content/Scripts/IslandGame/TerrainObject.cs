using System;
using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class TerrainObject : MonoBehaviour
    {
        [SerializeField, ReadOnly] private TreeData original;
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private ActionsHolder actionsHolder;
        [SerializeField] private PlayerAction playerAction;

        [SerializeField, ReadOnly] private float targetHealth;

        public bool IsDead => targetHealth <= 0;
        
        private IslandGenerator islandGenerator;
        private int treesInstsancesCount;

        public void Init(
            int instanceID,
            TreeData original,
            SelectionService selectionService,
            GameDataObject gameDataObject,
            IslandGenerator islandGenerator
        )
        {
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
            
            
            targetHealth -= damage;

            if (targetHealth <= 0)
            {
                DestroyFromTerrain(damager);
                Destroy(gameObject);
            }
        }
        
        public void DestroyFromTerrain(Transform damager)
        {
            var instance = islandGenerator.RemoveTree(treesInstsancesCount, out float size);
            var terrSize = islandGenerator.CurrentIslandData.Terrain.terrainData.size;

            var pos = islandGenerator.CurrentIslandData.Terrain.transform.position + terrSize.MultiplyVector3(instance.position);
            
            
            var treeHolder = new GameObject("Falled " + original.name);
            treeHolder.transform.position = pos;
            treeHolder.transform.forward = (new Vector3(damager.transform.position.x, treeHolder.transform.position.y, damager.transform.position.z) - treeHolder.transform.transform.position);
            
            Instantiate(
                    original, 
                    pos,  
                    Quaternion.AngleAxis(instance.rotation * Mathf.Rad2Deg, Vector3.up),
                    treeHolder.transform)
                .With(x => x.transform.localScale = Vector3.one * size);



            treeHolder.transform.DORotateQuaternion(Quaternion.Euler(treeHolder.transform.right * -90), 5).SetEase(Ease.InQuart);

        }
    }
}
