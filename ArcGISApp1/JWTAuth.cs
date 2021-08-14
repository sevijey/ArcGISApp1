using ArcGISApp1.Models;
using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using static DocuSign.eSign.Client.Auth.OAuth.UserInfo;

namespace ArcGISApp1
{
    public static class JWTAuth
    {
        /// <summary>
        /// Uses Json Web Token (JWT) Authentication Method to obtain the necessary information needed to make API calls.
        /// </summary>
        /// <returns>A ApiCallsInfo, with information for acces Api: OAuthToken, accountId, baseUri, CreatedTokenTime</returns>
        public static ApiCallsInfo AuthenticateWithJWT()
        {
            var apiClient = new ApiClient();
            string ik = Properties.Settings.Default["IntegrationKey"].ToString();
            string userId = Properties.Settings.Default["userId"].ToString();
            string authServer = Properties.Settings.Default["AuthServer"].ToString();
            string rsaKeyFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\{Properties.Settings.Default["KeyFilePath"].ToString()}";

            List<string> scopes = new List<string>
            {
                "signature",
                "impersonation"
            };

            var bytes = File.ReadAllBytes(rsaKeyFilePath);
            OAuth.OAuthToken authToken = apiClient.RequestJWTUserToken(ik, userId, authServer, bytes, 1, scopes);

            apiClient.SetOAuthBasePath(authServer);
            OAuth.UserInfo userInfo = apiClient.GetUserInfo(authToken.access_token);
            Account acct = null;

            var accounts = userInfo.Accounts;
            {
                acct = accounts.FirstOrDefault(a => a.IsDefault == "true");
            }

            return new ApiCallsInfo() { AuthToken = authToken, AccountId = acct.AccountId, Baseuri = acct.BaseUri + "/restapi", CreatedTokenTime = DateTime.Now };
        }
    }
}
