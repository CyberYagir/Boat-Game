using System.Collections.Generic;
using Content.Scripts.IslandGame.Scriptable;
using DG.DemiLib;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageSocialRatingCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image icon;
        private TradesDataObject tradesData;


        public void Init(TradesDataObject tradesData)
        {
            this.tradesData = tradesData;
        }

        public void Redraw(int value)
        {
            text.text = "Reputation: " + value.ToString("0000");
            icon.sprite = tradesData.GetEmotionalData(value).Sprite;
        }
    }
}
