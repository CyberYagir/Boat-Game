using System.Collections.Generic;
using Content.Scripts.DungeonGame.Services;
using Content.Scripts.Mobs;
using DG.DemiLib;
using DG.Tweening;
using UnityEngine;
using Zenject;

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
        [SerializeField] private Transform cover;
        [SerializeField] private List<DropsByLevel> dropTable = new List<DropsByLevel>();
        
        private DungeonService dungeonService;
        public DropTableObject DropTable => dropTable.Find(x=>x.Range.IsInRange(dungeonService.Level)).DropTable;
        public void Demolish(Vector3 pos)
        {
            cover.transform.DOLocalRotate(new Vector3(-90, 0, 0), 0.5f, RotateMode.Fast);
        }
        
        [Inject]
        private void Construct(UrnCollectionService urnCollectionService, DungeonService dungeonService)
        {
            this.dungeonService = dungeonService;
            if (!gameObject.activeInHierarchy) return;
            
            urnCollectionService.AddUrn(this);
        }
    }
}