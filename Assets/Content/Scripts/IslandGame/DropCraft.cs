using Content.Scripts.CraftsSystem;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class DropCraft : MonoBehaviour
    {
        [SerializeField] private CraftObject craftItem;
        public CraftObject CraftItem => craftItem;
    }
}
