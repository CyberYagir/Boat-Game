using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.BoatGame.PlayerActions
{
    public class ActionHolderTransfered : MonoBehaviour, ISelectable
    {
        public virtual ActionsHolder GetActionHolder()
        {
            return null;
        }

        public List<PlayerAction> PlayerActions => GetActionHolder().PlayerActions;
        public Transform Transform => GetActionHolder().Transform;
        public Transform TransformOrCustomTransform => null;
        public ISelectable Transfered => GetActionHolder();
    }
}
