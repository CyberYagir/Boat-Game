using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.Items
{
    public class InHandItem : MonoBehaviour
    {
        [SerializeField] private Transform endPoint;

        public Transform EndPoint => endPoint;
    }
}
