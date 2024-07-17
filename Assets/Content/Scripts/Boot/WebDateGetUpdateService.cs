using System;
using System.Collections;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace Content.Scripts.Boot
{
    public class WebDateGetUpdateService : MonoBehaviour
    {
        [SerializeField] private string webURL = "https://yagir.xyz/timeservice/";

        private void Awake()
        {
            StartCoroutine(WaitForPingRealTime());
        }

        IEnumerator WaitForPingRealTime()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(webURL))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.Log("Connection Error: " + webRequest.error);
                    
                    
                    
                    DateService.SetDateTime(DateTime.UtcNow, DateService.WebTimeType.Local);
                }
                else
                {
                    var json = webRequest.downloadHandler.text;

                    var time = JsonUtility.FromJson<JsonDateTime>(json);

                    if (time == null)
                    {
                        DateService.SetDateTime(DateTime.UtcNow, DateService.WebTimeType.Local);
                    }
                    else
                    {
                        DateService.SetDateTime(time.GetDateTime(), DateService.WebTimeType.FromWeb);
                    }
                }
            }
            
            StartCoroutine(AddSecondLoop());
        }

        IEnumerator AddSecondLoop()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1f);
                DateService.AddDateTime();
            }
        }
    }
}
