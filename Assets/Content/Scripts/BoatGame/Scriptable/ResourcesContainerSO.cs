using UnityEngine;

namespace Content.Scripts.BoatGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create ResourcesContainerSO", fileName = "ResourcesContainerSO", order = 0)]
    public class ResourcesContainerSO : ScriptableObject
    {
        [SerializeField] private RaftStorage.StorageItem[] resources;

        public RaftStorage.StorageItem[] Resources => resources;
    }
}
