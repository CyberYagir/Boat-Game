using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

namespace Content.Scripts.Boot
{
    public class PlayerAuthService : MonoBehaviour
    {
        public const string TOKEN_AUTH_PREF_NAME = "Auth_Token";
        public const string TOKEN_PLAYER_PREF_NAME = "Player_Token";
    
        [System.Serializable]
        public class PlayerAccount
        {
            [SerializeField] private string idToken;
            [SerializeField] private string accessToken;


            private bool hasCache => PlayerPrefs.HasKey(TOKEN_PLAYER_PREF_NAME);
            private string cachedToken => PlayerPrefs.GetString(TOKEN_PLAYER_PREF_NAME);
            public string AccessToken => accessToken;

            public string IDToken => idToken;
            public bool IsAuth => PlayerAccountService.Instance.IsSignedIn;

            public event Action OnSignIn;
            public event Action OnSignStart;
            public event Action OnSignOut;

            public async Task Init()
            {
                try
                {
                    OnSignStart?.Invoke();
                    PlayerAccountService.Instance.SignedIn -= OnSign;
                    PlayerAccountService.Instance.SignedIn += OnSign;
                    if (!hasCache)
                    {
                        await PlayerAccountService.Instance.StartSignInAsync();
                    }
                    else
                    {
                        await SignCache();
                    }

                } catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            private void OnSign()
            {
                idToken = PlayerAccountService.Instance.IdToken;
                accessToken = PlayerAccountService.Instance.AccessToken;
                PlayerPrefs.SetString(TOKEN_PLAYER_PREF_NAME, accessToken);
                OnSignIn?.Invoke();
            }

            public void SignOut()
            {
                PlayerAccountService.Instance.SignOut();
                OnSignOut?.Invoke();
            }

            public async Task SignCache()
            {
                accessToken = cachedToken;
            }
        }

        [System.Serializable]
        public class AuthData
        {
            [SerializeField] private string accessToken;
            public bool IsAuth => AuthenticationService.Instance.IsAuthorized;


            public event Action OnLoggedStart;
            public event Action OnLoggedIn;
            public event Action OnLoggedFailed;

            public async Task Init(PlayerAccount playerAccount)
            {
                OnLoggedStart?.Invoke();
                
                await SignInWithUnityAsync(playerAccount);
            }

            async Task SignInWithUnityAsync(PlayerAccount playerAccount)
            {
                if (IsAuth)
                {
                    OnLoggedIn?.Invoke();
                    return;
                }
                try
                {
                    await AuthenticationService.Instance.SignInWithUnityAsync(playerAccount.AccessToken);
                    accessToken = AuthenticationService.Instance.AccessToken;
                    PlayerPrefs.SetString(TOKEN_AUTH_PREF_NAME, accessToken);
                    OnLoggedIn?.Invoke();
                }
                catch (AuthenticationException ex)
                {
                    Debug.LogException(ex);
                    playerAccount.SignOut();
                    OnLoggedFailed?.Invoke();
                }
                catch (RequestFailedException ex)
                {
                    Debug.LogException(ex);
                    OnLoggedFailed?.Invoke();
                    playerAccount.SignOut();
                }
            }

            public async Task SignCache(PlayerAccount playerAccount)
            {
                if (!AuthenticationService.Instance.SessionTokenExists) 
                {
                    // if not, then do nothing
                    return;
                }
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                accessToken = AuthenticationService.Instance.AccessToken;
            }
        }

        [SerializeField] private PlayerAccount playerAccountData = new PlayerAccount();
        [SerializeField] private AuthData authData = new AuthData();
        
        
        public event Action OnLoggedStart;
        public event Action OnLoggedFailed;
        public event Action OnLoggedIn;


        async void Awake()
        {

            playerAccountData.OnSignOut += OnLogOut;
            playerAccountData.OnSignStart += OnLogStart;
            authData.OnLoggedFailed += OnLogOut;
            authData.OnLoggedIn += OnLogIn;
            authData.OnLoggedStart += OnLogStart;
            
            playerAccountData.OnSignIn += OnSigned;
            
            await UnityServices.InitializeAsync();
            await playerAccountData.SignCache();
            await authData.SignCache(playerAccountData);
        }

        private async void OnSigned()
        {
            await authData.Init(playerAccountData);
        }

        private void OnLogStart()
        {
            OnLoggedStart?.Invoke();
        }

        private void OnLogIn()
        {
            OnLoggedIn?.Invoke();
        }

        private void OnLogOut()
        {
            OnLoggedFailed?.Invoke();
        }

        public async void AuthButton(){

            if (!playerAccountData.IsAuth)
            {
                await playerAccountData.Init();
            }
        }
        
        public bool IsAuthed()
        {
            return authData.IsAuth;
        }
    }
}
