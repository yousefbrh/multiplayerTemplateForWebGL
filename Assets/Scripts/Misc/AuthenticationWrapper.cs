using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Enums;
using Managers;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Random = System.Random;

namespace Misc
{
    public class AuthenticationWrapper
    {
        public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

        public static async Task<AuthState> DoAuth(int maxTries = 5)
        {
            if (AuthState == AuthState.Authenticated) return AuthState;
            if (AuthState == AuthState.Authenticating)
            {
                Debug.LogWarning("Already authenticating!!!");
                await Authenticating();
                return AuthState;
            }
            await SignInAnonymouslyAsync(maxTries);
            return AuthState;
        }

        private static async Task<AuthState> Authenticating()
        {
            while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
            {
                await Task.Delay(200);
            }

            return AuthState;
        }

        private static async Task SignInAnonymouslyAsync(int maxTries)
        {
            AuthState = AuthState.Authenticating;
            var retries = 0;
            while (AuthState == AuthState.Authenticating && retries < maxTries)
            {
                try
                {
                    if (PlayerPrefs.HasKey("Username"))
                    {
                        var username = PlayerPrefs.GetString("Username");
                        var password = PlayerPrefs.GetString("Password");
                        await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                    }
                    else
                    {
                        var username = CreateUsername();
                        var password = CreatePassword();
                        PlayerPrefs.SetString("Username", username);
                        PlayerPrefs.SetString("Password", password);
                        
                        await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                    }
                    await UniTask.DelayFrame(1);
                    
                    if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                    {
                        AuthState = AuthState.Authenticated;
                        break;
                    }
                }
                catch (AuthenticationException e)
                {
                    Debug.LogError(e);
                    AuthState = AuthState.Error;
                }
                catch (RequestFailedException e)
                {
                    Debug.LogError(e);
                    UIManager.Instance.ShowMessageOnNetworkMessagePanel("Please Check Your Connection");
                    AuthState = AuthState.Error;
                }

                retries++;
                await Task.Delay(1000);
            }

            if (AuthState != AuthState.Authenticated)
            {
                Debug.LogWarning($"Player was not signed in successfully after {retries} retries");
                AuthState = AuthState.TimeOut;
            }
        }

        private static string CreateUsername()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var stringChars = new char[16];
            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
        
        private static string CreatePassword()
        {
            return PasswordGenerator.GenerateSecurePassword();
        }
    }
}