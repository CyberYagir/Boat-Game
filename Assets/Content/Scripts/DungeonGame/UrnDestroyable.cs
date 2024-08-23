using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.DungeonGame.Services;
using Content.Scripts.Misc;
using Content.Scripts.Mobs;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Content.Scripts.DungeonGame
{
    public class UrnDestroyable : MonoBehaviour, IDestroyable
    {
        [SerializeField, ReadOnly] private string uid;
        [SerializeField] private GameObject mesh;
        [SerializeField] private GameObject demolished;
        [SerializeField] private DropTableObject dropTable;
        [SerializeField] private List<Rigidbody> rb;
        [SerializeField] private float minDistance = 1f;

        public string UID => uid;
        public DropTableObject DropTable => dropTable;
        public float ActivationDistance => minDistance;
        public int DropsCount => 1;

        [Inject]
        private void Construct(UrnCollectionService urnCollectionService, DungeonService dungeonService)
        {
            if (!gameObject.activeInHierarchy) return;
            uid = urnCollectionService.GetNextGuid();
            if (dungeonService.IsUrnDead(uid))
            {
                gameObject.SetActive(false);
                return;
            }

            for (var i = 0; i < rb.Count; i++)
            {
                rb[i].solverIterations = 1;
                rb[i].solverVelocityIterations = 1;
            }
            
            mesh.gameObject.SetActive(true);
            demolished.gameObject.SetActive(false);
            
            
            urnCollectionService.AddUrn(this);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, ActivationDistance);
        }

        [Button]
        public void AddAll()
        {
            mesh = transform.GetChild(0).gameObject;
            mesh.name = "Mesh";
            demolished = transform.GetChild(1).gameObject;
            demolished.name = "Demolished";

            foreach (Transform c in demolished.transform)
            {
                if (!c.Get<Rigidbody>())
                {
                    c.AddComponent<Rigidbody>();
                    c.AddComponent<BoxCollider>();
                }
            }

            rb = demolished.GetComponentsInChildren<Rigidbody>(true).ToList();

            demolished.gameObject.SetActive(false);
            
            demolished.AddComponent<ReduceColliders>().Reduce();
        }

        private bool isDead = false;
        public void Demolish(Vector3 pos)
        {
            if (isDead) return;
            mesh.gameObject.SetActive(false);
            demolished.gameObject.SetActive(true);

            for (int i = 0; i < rb.Count; i++)
            {
                rb[i].AddExplosionForce(15, pos, 10);
            }

            DOVirtual.DelayedCall(Random.Range(10, 30), RemoveAllDebris);
            DOVirtual.DelayedCall(2, KinematicDebris);
            isDead = true;
        }

        private void KinematicDebris()
        {
            for (int q = 0; q < rb.Count; q++)
            {
                rb[q].isKinematic = true;
            }
        }

        private void RemoveAllDebris()
        {
            StartCoroutine(RemoveDebris());
        }

        IEnumerator RemoveDebris()
        {
            for (int q = 0; q < rb.Count; q++)
            {
                yield return new WaitForSeconds(Random.Range(0.5f, 2f));
                rb[q].gameObject.SetActive(false);
            }
            
            gameObject.SetActive(false);
        }
    }
}
