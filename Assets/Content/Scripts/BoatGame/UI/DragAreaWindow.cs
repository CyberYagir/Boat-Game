using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class DragAreaWindow : MonoBehaviour, IDragDropArea
    {
        [SerializeField] private Dragger dragger;
        [SerializeField] private bool canStackItems;
        public Dragger DragManager => dragger;


        private void LateUpdate()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                dragger.MoveTo();
                if (canStackItems)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        AddItemToStack();
                    }
                }
            }
            else
            {
                dragger.Drop();
                OnDragDropped();
            }
        }

        protected virtual void AddItemToStack()
        {
            
        }

        public void StartDrag(ItemObject item, int count, GameObject sender)
        {
            if (!dragger.IsOnDrag)
            {
                dragger.Init(item, sender);
                dragger.SetStack(count);
                OnDragStarted();
            }
        }

        public virtual void OnDragStarted()
        {
            
        }
        public virtual void OnDragDropped()
        {
            
        }

        private void OnDisable()
        {
            dragger.Disable();
        }

        public virtual void AddToInventory(ItemObject draggedItem)
        {
            
        }
    }
}