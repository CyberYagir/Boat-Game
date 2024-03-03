using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class DroppedItem : MonoBehaviour
    {
        [SerializeField] private ItemObject item;
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private ActionsHolder actionsHolder;
        [SerializeField] private CraftObject craftItem;
        
        public ItemObject Item => item;

        public CraftObject CraftItem => craftItem;
        
        public void Animate()
        {
            rigidbody.AddForce(new Vector3(Random.value, Random.Range(0, 1f), Random.value), ForceMode.Impulse);
            rigidbody.AddTorque(new Vector3(Random.value, Random.value, Random.value), ForceMode.Impulse);
            transform.localScale = Vector3.one * 0.1f;
            transform.DOScale(Vector3.one, 0.2f);
        }

        public void DeleteItem()
        {
            Destroy(gameObject);
        }

        public void SetKinematic(bool state = true)
        {
            rigidbody.isKinematic = state;
        }
    }
}
