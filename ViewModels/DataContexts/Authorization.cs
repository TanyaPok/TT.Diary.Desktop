using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using System.Net.Http;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Interfaces;
using TT.Diary.Desktop.ViewModels.Extensions;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public class Authorization
    {
        // HERE MUST BE YOUR Client ID (https://console.developers.google.com/apis/credentials)
        private readonly string _clientID = @"";
        // HERE MUST BE YOUR Client secret (https://console.developers.google.com/apis/credentials)
        private readonly string _clientSecret = "";
        private readonly string _authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private readonly string _userInfoRequestUri = "https://www.googleapis.com/oauth2/v3/userinfo";
        private readonly string _tokenRequestUri = "https://www.googleapis.com/oauth2/v4/token";
        private readonly string _codeChallengeMethod = "S256";

        private readonly string _authorizationRequest =
            "{0}?response_type=code&scope=openid%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}";

        private readonly string _responseString =
            "<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>";

        private readonly string _tokenRequestBody =
            "code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code";

        private readonly string _contentType = "application/x-www-form-urlencoded";
        private readonly string _accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

        public RelayCommand<IWindowService> SignInCommand { get; }

        public RelayCommand<IWindowService> CloseWindowCommand { get; }

        public Authorization()
        {
            SignInCommand = new RelayCommand<IWindowService>((window) => SignIn(window), true);

            CloseWindowCommand = new RelayCommand<IWindowService>((window) =>
                {
                    if (window == null)
                        return;
                    window.Close();
                }
                , true);
        }

        private async void SignIn(IWindowService window)
        {
            // Generates state and Proof Key for Code Exchange values.
            var state = RandomDataBase64Url(32);
            var codeVerifier = RandomDataBase64Url(32);
            var codeChallenge = Base64UrlencodeNoPadding(Sha256(codeVerifier));

            // Creates a redirect URI using an available port on the loopback address.
            var redirectUri = $@"http://{IPAddress.Loopback}:{GetRandomUnusedPort()}/";

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectUri);
            http.Start();

            // Creates the OAuth 2.0 authorization request.
            var authorizationRequest = string.Format(_authorizationRequest,
                _authorizationEndpoint,
                Uri.EscapeDataString(redirectUri),
                _clientID,
                state,
                codeChallenge,
                _codeChallengeMethod);

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest);

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Brings this app back to the foreground.
            window.Activate();

            // Sends an HTTP response to the browser.
            var response = context.Response;
            var buffer = Encoding.UTF8.GetBytes(_responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            var responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                http.Close();
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                throw new Exception(string.Format(ErrorMessages.OAuthAuthorizationError.GetDescription(),
                    context.Request.QueryString.Get("error")));
            }

            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                throw new Exception(string.Format(ErrorMessages.InvalidAuthorizationResponse.GetDescription(),
                    context.Request.QueryString));
            }

            // extracts the code
            var code = context.Request.QueryString.Get("code");
            var incomingState = context.Request.QueryString.Get("state");

            // Compares the received state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incomingState != state)
            {
                throw new Exception(string.Format(ErrorMessages.RequestInvalidState.GetDescription(), incomingState));
            }

            // Starts the code exchange at the Token Endpoint.
            var accessToken = await PerformCodeExchange(code, codeVerifier, redirectUri);

            var userInfo = await GetUserInfo(accessToken);
            userInfo.Id = await SetUser(userInfo);
            window.ShowWindow(new Context(userInfo));
            window.Close();
        }

        private async Task<int> SetUser(User userInfo)
        {
            using (var response =
                await Context.DiaryHttpClient.PostAsJsonAsync(ServiceOperationContract.SetUser, userInfo))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<int>();
                }

                throw new Exception(string.Format(ErrorMessages.SetUserError.GetDescription(), response.StatusCode));
            }
        }

        // ref http://stackoverflow.com/a/3978040
        private int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint) listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        private string RandomDataBase64Url(uint length)
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return Base64UrlencodeNoPadding(bytes);
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private string Base64UrlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);
            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");
            return base64;
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        /// <param name="inputStirng"></param>
        /// <returns></returns>
        private byte[] Sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        private async Task<string> PerformCodeExchange(string code, string codeVerifier, string redirectUri)
        {
            // builds the request
            var tokenRequestBody = string.Format(_tokenRequestBody,
                code,
                Uri.EscapeDataString(redirectUri),
                _clientID,
                codeVerifier,
                _clientSecret
            );

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest) WebRequest.Create(_tokenRequestUri);
            tokenRequest.Method = WebRequestMethods.Http.Post;
            tokenRequest.ContentType = _contentType;
            tokenRequest.Accept = _accept;
            byte[] byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(byteVersion, 0, byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                Dictionary<string, string> tokenEndpointDecoded;
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    var responseText = await reader.ReadToEndAsync();
                    // converts to dictionary
                    tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);
                }

                tokenResponse.Close();
                return tokenEndpointDecoded["access_token"];
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            var responseText = await reader.ReadToEndAsync();
                            throw new Exception(responseText, ex);
                        }
                    }
                }
            }

            return string.Empty;
        }

        private async Task<User> GetUserInfo(string accessToken)
        {
            // sends the request
            HttpWebRequest userinfoRequest = (HttpWebRequest) WebRequest.Create(_userInfoRequestUri);
            userinfoRequest.Method = WebRequestMethods.Http.Get;
            userinfoRequest.Headers.Add($"Authorization: Bearer {accessToken}");
            userinfoRequest.ContentType = _contentType;
            userinfoRequest.Accept = _accept;

            // gets the response
            WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
            string userinfoResponseText;
            using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
            {
                // reads response body
                userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
            }

            userinfoResponse.Close();
            return JsonConvert.DeserializeObject<User>(userinfoResponseText);
        }
    }
}