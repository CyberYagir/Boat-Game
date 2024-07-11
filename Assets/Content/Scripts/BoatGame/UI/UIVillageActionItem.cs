using Content.Scripts.Global;
using Content.Scripts.IslandGame.Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageActionItem : MonoBehaviour
    {
        [SerializeField] private GameObject checkIcon;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Button button;
        
        private SlaveActivitiesObject activity;
        private UIVillageInfoSubWindow window;

        public SlaveActivitiesObject Activity => activity;

        public void Init(SlaveActivitiesObject activity, UIVillageInfoSubWindow window)
        {
            this.window = window;
            this.activity = activity;
            text.text = activity.ActivityName;
        }

        public void Enabled(bool state) => button.interactable = state;

        public void SetActive(bool state) => checkIcon.gameObject.SetActive(state);


        public void Toggle()
        {
            window.ToggleActivity(activity);
        }
    }
}
