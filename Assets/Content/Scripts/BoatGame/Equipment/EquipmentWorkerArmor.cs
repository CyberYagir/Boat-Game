using UnityEngine;
using static Content.Scripts.BoatGame.PlayerCharacter.AppearanceManager;

namespace Content.Scripts.BoatGame.Equipment
{
    class EquipmentWorkerArmor : EquipmentWorker
    {
        [SerializeField] private GameObject epauletLeft, epauletRight;
        [SerializeField] private GameObject kneePadLeft, kneePadRight;
        [SerializeField] private GameObject bracersLeft, bracersRight;

        public override void Init(PlayerCharacter.AppearanceManager appearanceManager)
        {
            base.Init(appearanceManager);

            SetEpaulet(epauletLeft, appearanceManager.GetBone(EBones.LeftShoulder));
            SetEpaulet(epauletRight, appearanceManager.GetBone(EBones.RightShoulder));
            
            
            SetEpaulet(kneePadLeft, appearanceManager.GetBone(EBones.LeftLeg));
            SetEpaulet(kneePadRight, appearanceManager.GetBone(EBones.RightLeg));
            
            
            SetEpaulet(bracersLeft, appearanceManager.GetBone(EBones.LeftForeArm));
            SetEpaulet(bracersRight, appearanceManager.GetBone(EBones.RightForeArm));
        }

        public void SetEpaulet(GameObject obj, Transform bone)
        {
            if (obj != null)
            {
                obj.transform.parent = bone;

                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localEulerAngles = Vector3.zero;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (epauletLeft != null)
            {
                Destroy(epauletLeft);
            }

            if (epauletRight != null)
            {
                Destroy(epauletRight);
            }
            
            if (kneePadLeft != null)
            {
                Destroy(kneePadLeft);
            }
            
            if (kneePadRight != null)
            {
                Destroy(kneePadRight);
            }
        }
    }
}