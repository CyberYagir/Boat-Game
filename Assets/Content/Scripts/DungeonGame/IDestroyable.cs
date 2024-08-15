using Content.Scripts.Mobs;
using UnityEngine;

namespace Content.Scripts.DungeonGame
{
    public interface IDestroyable
    {
        Transform transform { get; }
        DropTableObject DropTable { get; }
        void Demolish(Vector3 pos);
    }
}