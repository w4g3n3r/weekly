using Meziantou.Framework.Win32;
using System.Text;

namespace Weekly.Infrastructure
{
    public class TokenManager
    {
        private const string TokenPrefix = "weekly";
        public string GetBasicAuthToken(string name)
        {
            var credential = GetCredential(name);

            if (credential == null)
                return null;

            var basicAuthString = $"{credential.UserName}:{credential.Password}";
            var basicAuthBytes = Encoding.UTF8.GetBytes(basicAuthString);

            return Convert.ToBase64String(basicAuthBytes);
        }

        public bool TokenExists(string name)
        {
            var token = GetCredential(name);

            return token != null;
        }

        public string GetBearerAuthToken(string name)
        {
            var credential = GetCredential(name);

            if (credential == null)
                return null;

            return credential.Password;
        }

        public void PutToken(string name, string userName, string token)
        {
            var tokenName = GetTokenName(name);


            CredentialManager.WriteCredential(tokenName, userName, token, CredentialPersistence.LocalMachine);
        }

        public void DeleteToken(string name)
        {
            var tokenName = GetTokenName(name);

            CredentialManager.DeleteCredential(tokenName);
        }

        public void ClearTokens()
        {
            foreach (var credential in CredentialManager.EnumerateCredentials())
            {
                if (credential.ApplicationName.StartsWith(TokenPrefix))
                {
                    CredentialManager.DeleteCredential(credential.ApplicationName);
                }
            }
        }

        private Credential? GetCredential(string name)
        {
            var tokenName = GetTokenName(name);

            return CredentialManager.ReadCredential(tokenName);
        }

        private string GetTokenName(string name)
        {
            return $"{TokenPrefix}.{name.ToLower()}";
        }
    }
}
