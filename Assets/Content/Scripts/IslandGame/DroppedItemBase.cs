using System;
using Content.Scripts.BoatGame;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Guid = Pathfinding.Util.Guid;
using Random = UnityEngine.Random;

namespace Content.Scripts.IslandGame
{
    public class DroppedItemBase : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody; 
        protected RaftStorage.StorageItem itemData;
        protected SaveDataObject saveData;
        protected string dropID = null;
        private IIslandDroppedItemData[] datas;
        public string DropID => dropID;
        public RaftStorage.StorageItem StorageItem => itemData;
        

        [Inject]
        private void Construct(SaveDataObject saveDataObject)
        {
            this.saveData = saveDataObject;
            
            if (string.IsNullOrEmpty(dropID))
                dropID = Guid.NewGuid().ToString();
        }
        

        public virtual void DeleteItem()
        {
            foreach (var d in datas)
            {
                d.DeleteItem();
            }
            Destroy(gameObject);
        }
        
        
        public void SetItem(RaftStorage.StorageItem item)
        {
            itemData = item;
            AfterItemSet();
        }

        public virtual void AfterItemSet()
        {
            datas = GetComponentsInChildren<IIslandDroppedItemData>();
            foreach (var d in datas)
            {
                d.AfterInject(saveData);
            }
        }

        public void SetItem(ItemObject item)
        {
            SetItem(new RaftStorage.StorageItem(item, 1));
        }

        public void Animate()
        {
            if (rigidbody)
            {
                rigidbody.AddForce(new Vector3(Random.value, Random.Range(0, 1f), Random.value), ForceMode.Impulse);
                rigidbody.AddTorque(new Vector3(Random.value, Random.value, Random.value), ForceMode.Impulse);
            }

            transform.localScale = Vector3.one * 0.1f;
            transform.DOScale(Vector3.one, 0.2f);
        }
        public void SetKinematic(bool state = true)
        {
            if (rigidbody == null) return;
            rigidbody.isKinematic = state;
        }
        
        public void LoadItem(SaveDataObject.MapData.IslandData.DroppedItemData droppedItemData)
        {
            dropID = droppedItemData.DropID;
        }
    }
}