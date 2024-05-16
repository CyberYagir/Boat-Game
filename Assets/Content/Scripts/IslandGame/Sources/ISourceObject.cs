using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.IslandGame.Sources
{
    public interface ISourceObject
    {
        public bool IsValidSource { get; }
        public ItemObject GetFromItem();
    }
}
