using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.ItemsSystem;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class WorldPopupService : MonoBehaviour
    {
        private static WorldPopupService Instance;

        [SerializeField] private Sprite cantIcon;
        [SerializeField] private List<WorldPopup> popups;


        private Dictionary<ItemObject, int> pupupsStack = new Dictionary<ItemObject, int>();

        [Inject]
        private void Construct()
        {
            Instance = this;
        }
        
        public WorldPopup GetFromPool()
        {
            var popup = popups[0];
            popups.RemoveAt(0);
            popups.Add(popup);
            popup.gameObject.SetActive(true);
            return popup;
        }


        public void SpawnPopup(Vector3 pos, ItemObject item, int value)
        {
            if (pupupsStack.ContainsKey(item))
            {
                pupupsStack[item] += value;
            }
            else
            {
                pupupsStack.Add(item, value);

                StartCoroutine(WaitFrameToStack(pos, item));
            }
        }


        IEnumerator WaitFrameToStack(Vector3 pos, ItemObject itemObject)
        {
            yield return null;
            if (pupupsStack.ContainsKey(itemObject))
            {
                var particle = GetFromPool();
                particle.InitPopup(itemObject, pupupsStack[itemObject]);
                particle.Animate(pos);

                pupupsStack.Remove(itemObject);
            }
        }

        public void SpawnPopup(Vector3 pos, RaftStorage.StorageItem storageItem)
        {
            SpawnPopup(pos, storageItem.Item, storageItem.Count);
        }

        public void SpawnPopup(Vector3 pos, string message)
        {
            var particle = GetFromPool();
            particle.InitPopup(message);
            particle.Animate(pos);
        }

        public void SpawnPopup(Vector3 pos, Sprite sprite)
        {
            var particle = GetFromPool();
            particle.InitPopup(sprite);
            particle.Animate(pos);
        }

        public void SpawnCantPopup(Vector3 pos)
        {
            SpawnPopup(pos, cantIcon);
        }

        public static void StaticSpawnPopup(Vector3 pos, ItemObject item, int value) => Instance.SpawnPopup(pos, item, value);
        public static void StaticSpawnPopup(Vector3 pos, RaftStorage.StorageItem storageItem) => Instance.SpawnPopup(pos, storageItem);
        public static void StaticSpawnPopup(Vector3 pos, Sprite sprite) => Instance.SpawnPopup(pos, sprite);
        public static void StaticSpawnPopup(Vector3 pos, string message) => Instance.SpawnPopup(pos, message);
        public static void StaticSpawnCantPopup(Vector3 pos) => Instance.SpawnCantPopup(pos);
    }
}
