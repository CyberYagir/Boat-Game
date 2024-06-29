using System;
using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Content.Scripts.IslandGame
{
    public class DroppedItem : MonoBehaviour
    {
        [SerializeField] private ItemObject item;
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private CraftObject craftItem;
        [SerializeField] private bool saveItem = true;
        private SaveDataObject saveData;
        private string dropID = null;

        public ItemObject Item => item;

        public CraftObject CraftItem => craftItem;

        public string DropID => dropID;

        [Inject]
        public void Construct(SaveDataObject saveData)
        {
            this.saveData = saveData;
            
            print("barrel inject");
            
            if (string.IsNullOrEmpty(dropID))
                dropID = Guid.NewGuid().ToString();

            if (saveItem)
            {
                var island = saveData.GetTargetIsland();
                island.AddDroppedItem(this);
            }
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

        public void DeleteItem()
        {
            saveData.GetTargetIsland().RemoveDroppedItem(this);
            Destroy(gameObject);
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
