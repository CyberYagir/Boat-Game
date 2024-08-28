using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIOptionsButton : MonoBehaviour
    {
        [System.Serializable]
        private class AuthButton
        {
            [SerializeField] private Button authButton;
            [SerializeField] private TMP_Text text;

            [SerializeField] private GameObject loadingIcon;
            [SerializeField] private GameObject notLoggedIcon;
            [SerializeField] private GameObject loggedIcon;
            private PlayerAuthService playerAuthService;

            public void Init(PlayerAuthService playerAuthService)
            {
                this.playerAuthService = playerAuthService;
                if (playerAuthService.IsAuthed())
                {
                    SetAuthedVisuals();
                }
                else
                {
                    authButton.onClick.AddListener(playerAuthService.AuthButton);
                    playerAuthService.OnLoggedStart += SetLoggedProcessVisuals;
                    playerAuthService.OnLoggedFailed += SetNotLoggedVisuals;
                    playerAuthService.OnLoggedIn += SetAuthedVisuals;
                    SetNotLoggedVisuals();
                }
            }

            private void SetNotLoggedVisuals()
            {
                
                authButton.interactable = true;
                text.text = "Authorize";
                loadingIcon.gameObject.SetActive(false);
                notLoggedIcon.gameObject.SetActive(true);
                loggedIcon.gameObject.SetActive(false);
            }

            private void SetLoggedProcessVisuals()
            {
                authButton.interactable = false;
                text.text = "Waiting...";
                loadingIcon.gameObject.SetActive(true);
                notLoggedIcon.gameObject.SetActive(false);
                loggedIcon.gameObject.SetActive(false);
            }

            private void SetAuthedVisuals()
            {
                authButton.interactable = false;
                text.text = "Authorized";
                loadingIcon.gameObject.SetActive(false);
                notLoggedIcon.gameObject.SetActive(false);
                loggedIcon.gameObject.SetActive(true);
            }

            public void Discard()
            {
                playerAuthService.OnLoggedStart += SetLoggedProcessVisuals;
                playerAuthService.OnLoggedFailed += SetNotLoggedVisuals;
                playerAuthService.OnLoggedIn += SetAuthedVisuals;
            }
        }

        [System.Serializable]
        private class CloudButton
        {
            [SerializeField] private Button authButton;
            private PlayerAuthService playerAuthService;

            public void Init(PlayerAuthService playerAuthService)
            {
                this.playerAuthService = playerAuthService;
                UpdateButtonState();
                if (!authButton.interactable)
                {
                    playerAuthService.OnLoggedIn += UpdateButtonState;
                    playerAuthService.OnLoggedFailed += UpdateButtonState;
                    playerAuthService.OnLoggedStart += UpdateButtonState;
                }
            }

            private void UpdateButtonState()
            {
                authButton.interactable = playerAuthService.IsAuthed();
            }
            
            public void Discard()
            {
                playerAuthService.OnLoggedStart -= UpdateButtonState;
                playerAuthService.OnLoggedFailed -= UpdateButtonState;
                playerAuthService.OnLoggedIn -= UpdateButtonState;
            }
        }

        [SerializeField] private RectTransform withStoragePos, withoutStoragePos;
        [SerializeField] private RectTransform button;
        [SerializeField] private AnimatedWindow window;
        [SerializeField] private AnimatedWindow downloadWindow;
        [SerializeField] private AuthButton authButton;
        [SerializeField] private CloudButton cloudButton;
        
        private PlayerAuthService authService;
        private ISaveServiceBase saveService;
        private CloudService cloudService;
        private SaveDataObject saveDataObject;
        private ScenesService scenesService;
        private UIMessageBoxManager messageBoxManager;

        public void Init(UIStoragesCounter resourcesCounter, PlayerAuthService authService, ISaveServiceBase saveService, CloudService cloudService, ScenesService scenesService, UIMessageBoxManager messageBoxManager)
        {
            this.messageBoxManager = messageBoxManager;
            this.scenesService = scenesService;
            this.cloudService = cloudService;
            this.saveService = saveService;
            this.authService = authService;
            window.gameObject.SetActive(false);
            resourcesCounter.OnCounterStateChange += UpdateButtonPos;
            UpdateButtonPos(resourcesCounter.IsVisible);

            authButton.Init(authService);
            cloudButton.Init(authService);
        }

        public async void DownloadCloudSave()
        {
            messageBoxManager.ShowMessageBox("Are you sure you want to download the latest save? It destroys the current one.", DownloadFromCloud);
        }

        private async void DownloadFromCloud()
        {
            downloadWindow.ShowWindow();
            try
            {
                var json = await cloudService.DownloadSave();
                saveService.ReplaceJson(json);
                scenesService.FadeScene(ESceneName.Boot);
            }
            catch (Exception e)
            {
                downloadWindow.CloseWindow();
                
                messageBoxManager.ShowMessageBox("The download ended with an error.", null, "Ok", "_disabled");
            }
        }

        private void OnDestroy()
        {
            authButton.Discard();
        }

        public void SaveButton()
        {
            saveService.SaveWorld();
            HideWindow();
        }

        public void CloseApp()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }


        public void OpenWindow()
        {
            window.ShowWindow();
        }
        
        public void HideWindow()
        {
            window.CloseWindow();
        }

        private void UpdateButtonPos(bool isVisible)
        {
            if (isVisible)
            {
                button.anchoredPosition = withStoragePos.anchoredPosition;
            }
            else
            {
                button.anchoredPosition = withoutStoragePos.anchoredPosition;
            }
        }
        
    }
}
