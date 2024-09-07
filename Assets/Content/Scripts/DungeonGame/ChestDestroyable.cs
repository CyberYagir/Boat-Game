using System;
using System.Collections.Generic;
using Content.Scripts.DungeonGame.Services;
using Content.Scripts.Mobs;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Range = DG.DemiLib.Range;

namespace Content.Scripts.DungeonGame
{
    public class ChestDestroyable : MonoBehaviour, IDestroyable
    {
        [System.Serializable]
        public class DropsByLevel
        {
            [SerializeField] private Range range;
            [SerializeField] private DropTableObject dropTable;

            public DropTableObject DropTable => dropTable;

            public Range Range => range;
        }
        
        
        [SerializeField, ReadOnly] private string uid;
        [SerializeField] private Transform cover;
        [SerializeField] private float minDistance;
        [SerializeField] private int dropIterations;
        [SerializeField] private List<DropsByLevel> dropTable = new List<DropsByLevel>();
        public string UID => uid;
        public event Action OnOpen;


        public float ActivationDistance => minDistance;
        public int DropsCount => dropIterations;
        private DungeonService dungeonService;
        public DropTableObject DropTable => dropTable.Find(x=>x.Range.IsInRange(dungeonService.Level)).DropTable;
        public void Demolish(Vector3 pos)
        {
            cover.transform.DOLocalRotate(new Vector3(-90, 0, 0), 0.5f, RotateMode.Fast);
            OnOpen?.Invoke();
        }
        
        [Inject]
        private void Construct(UrnCollectionService urnCollectionService, DungeonService dungeonService)
        {
            this.dungeonService = dungeonService;
            if (!gameObject.activeInHierarchy) return;
            
            uid = urnCollectionService.GetNextGuid();
            if (dungeonService.IsUrnDead(uid))
            {
                Demolish(transform.position);
                return;
            }
            
            urnCollectionService.AddUrn(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, ActivationDistance);
        }
    }
}