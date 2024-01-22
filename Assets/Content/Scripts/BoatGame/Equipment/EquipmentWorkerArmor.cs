using UnityEngine;
using static Content.Scripts.BoatGame.PlayerCharacter.AppearanceManager;

namespace Content.Scripts.BoatGame.Equipment
{
    class EquipmentWorkerArmor : EquipmentWorker
    {
        [SerializeField] private GameObject epauletLeft, epauletRight;

        public override void Init(PlayerCharacter.AppearanceManager appearanceManager)
        {
            base.Init(appearanceManager);

            SetEpaulet(epauletLeft, appearanceManager.GetBone(EBones.LeftShoulder));
            SetEpaulet(epauletRight, appearanceManager.GetBone(EBones.RightShoulder));
        }

        public void SetEpaulet(GameObject obj, Transform bone)
        {
            obj.transform.parent = bone;

            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Destroy(epauletLeft);
            Destroy(epauletRight);
        }
    }
}