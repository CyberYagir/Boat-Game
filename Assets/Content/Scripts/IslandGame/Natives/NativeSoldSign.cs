using UnityEngine;

namespace Content.Scripts.IslandGame.Natives
{
    public class NativeSoldSign : MonoBehaviour
    {
        [SerializeField] private Transform neckBone;

        public Transform NeckBone => neckBone;
    }
}
