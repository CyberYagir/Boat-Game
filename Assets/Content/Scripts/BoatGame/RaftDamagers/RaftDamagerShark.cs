using Content.Scripts.BoatGame.Services;
using UnityEngine;

namespace Content.Scripts.BoatGame.RaftDamagers
{
    public class RaftDamagerShark : RaftDamager
    {
        [System.Serializable]
        public class DamageSharkAnimator
        {
            [SerializeField] private DamagerSharkAnimatorEvents events;
            [SerializeField] private ParticleSystem damageParticles;
            [SerializeField] private GameObject sharkMesh;

            public void Init()
            {
                events.OnChangeLayerToDefault += EventsOnOnChangeLayerToDefault;
                events.OnChangeLayerToRaft += EventsOnOnChangeLayerToRaft;
                events.OnDamage += EventsOnOnDamage;
            }

            private void EventsOnOnDamage()
            {
                damageParticles.Play();
            }

            private void EventsOnOnChangeLayerToRaft()
            {
                sharkMesh.ChangeLayerWithChilds(LayerMask.NameToLayer("Raft"));
            }

            private void EventsOnOnChangeLayerToDefault()
            {
                sharkMesh.ChangeLayerWithChilds(LayerMask.NameToLayer("Default"));
            }
        }

        [SerializeField] private DamageSharkAnimator animatorManager;

        public override void Init(RaftBase targetRaft, RaftDamagerService raftDamagerService)
        {
            base.Init(targetRaft, raftDamagerService);
            
            animatorManager.Init();

            transform.parent = targetRaft.transform;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;

            
            if (DamagerService.IsEmpty(TargetRaft.Coords, Vector3Int.forward))
            {
                RotateShark(Vector3Int.back);
            }else
            if (DamagerService.IsEmpty(TargetRaft.Coords, Vector3Int.back))
            {
                RotateShark(Vector3Int.forward);
            }else
            if (DamagerService.IsEmpty(TargetRaft.Coords, Vector3Int.right))
            {
                RotateShark(Vector3Int.left);
            }else
            if (DamagerService.IsEmpty(TargetRaft.Coords, Vector3Int.left))
            {
                RotateShark(Vector3Int.right);
            }
            
        }

        public void RotateShark(Vector3Int target)
        {
            transform.rotation = Quaternion.LookRotation(TargetRaft.Coords - target);
        }
    }
}
