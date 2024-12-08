using System;
using Content.Scripts.BoatGame.Equipment;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.ItemsSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Content.Scripts.Misc
{
    public class ItemPreviewSpawner : MonoBehaviour
    {
        [SerializeField] private Transform yRotator;
        [SerializeField] private Transform rotator;
        [SerializeField] private Transform playerRotator;
        [SerializeField] private float baseRotateSpeed = -10;
        [SerializeField] private float playerRotateSpeed = 0;
        private bool active;

        public void Spawn(GameObject obj, ItemObject itemObject)
        {
            var item = Instantiate(obj, rotator);

            var renderers = item.GetComponentsInChildren<Renderer>();
            var bounds = renderers[0].bounds;

            for (int i = 0; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }


            var delta = rotator.position - bounds.center;


            item.transform.position += delta;

            if (itemObject.Equipment == EEquipmentType.Weapon)
            {
                rotator.localEulerAngles = new Vector3(5, 0, -45);
            }
            else if (itemObject.Equipment == EEquipmentType.Helmet)
            {
                rotator.localEulerAngles = new Vector3(0, -180, 0);
                item.transform.localScale = Vector3.one * 2f;
            }else if (itemObject.Equipment == EEquipmentType.Armor)
            {
                rotator.localEulerAngles = new Vector3(0, -180, 0);
                item.GetComponent<EquipmentWorkerArmor>().Clear();
                item.transform.localScale = Vector3.one * 2f;
            }
        }

        public void SetActive(bool active)
        {
            this.active = active;
        }

        private void Update()
        {
            if (InputService.IsLMBPressed && active)
            {
                playerRotator.Rotate(
                    new Vector3(InputService.MouseAxis.y, -InputService.MouseAxis.x, 0) * Time.deltaTime *
                    playerRotateSpeed, Space.World);
            }
            else
            {
                playerRotator.localRotation = Quaternion.Lerp(playerRotator.localRotation,
                    Quaternion.Euler(0, 0, 0),
                    Time.deltaTime);
                yRotator.Rotate(Vector3.up * baseRotateSpeed * Time.deltaTime, Space.Self);
            }
        }
    }
}
