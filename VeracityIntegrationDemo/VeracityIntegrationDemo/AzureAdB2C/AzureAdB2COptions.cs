using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VeracityIntegrationDemo.AzureAdB2C
{
    public class AzureAdB2COptions
    {
        public const string PolicyAuthenticationProperty = "Policy";

        public AzureAdB2COptions()
        {
            AzureAdB2CInstance = "https://login.microsoftonline.com";
        }

        public string ClientId { get; set; }
        public string AzureAdB2CInstance { get; set; }
        public string Tenant { get; set; }
        public string TenantId { get; set; }
        public string SignUpSignInPolicyId { get; set; }
        public string SignInPolicyId { get; set; }
        public string SignUpPolicyId { get; set; }
        public string ResetPasswordPolicyId { get; set; }
        public string EditProfilePolicyId { get; set; }
        public string RedirectUri { get; set; }

        public string DefaultPolicy => SignUpSignInPolicyId;
        public string Authority => $"{AzureAdB2CInstance}/tfp/{Tenant}/{DefaultPolicy}/v2.0";
        public string ClientSecret { get; set; }
        public string ApiUrl { get; set; }
        public string ApiScopes { get; set; }
        public string MicrosoftGraphScope { get; set; }
        public string MicrosoftGraphQuery { get; set; }
    }
}
