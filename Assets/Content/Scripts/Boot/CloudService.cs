using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Boot
{
    public class CloudService : MonoBehaviour
    {
        public const string SAVE_FILE_KEY_NAME = "save_file";

        private PlayerAuthService authService;

        [Inject]
        private void Construct(PlayerAuthService authService)
        {
            this.authService = authService;
        }


        public async Task SaveJson(string json)
        {
            if (authService.IsAuthed())
            {
                var data = new Dictionary<string, object>();
                data.Add(SAVE_FILE_KEY_NAME, json);
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
                Debug.LogError("Saved to Cloud");
            }
        }

        public async Task<string> DownloadSave()
        {
            if (authService.IsAuthed())
            {
                var results = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {SAVE_FILE_KEY_NAME});


                if (results.TryGetValue(SAVE_FILE_KEY_NAME, out var item))
                {
                    var json = item.Value.GetAs<string>();
                    return json;
                }
            }

            return null;
        }
    }
}
