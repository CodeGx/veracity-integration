
# Veracity Integration
The repository is about how to integrate your existing app with veracity platform. Currently, only ASP.NET core 2.0 is provided. More frameworks will be added in the future.

The identity and access management of veracity platform is fully based on Microsoft Azure AD B2C. Therefore, your app must obtain the configuration details for your app on Microsoft Azure AD B2C, which you can contact 8989@dnvgl.com (Global Service Desk of DNV GL) to help you with it.


## Azure AD B2C Configuration details
*Once your app is configured by Veracity Team, you will receive the following information via email. The settings are expected to be kept safely in your app somewhere, normally in app configuration file. Let's take a look at a sample for the TEST environment.* 

**Client ID**:       a GUID string , the unique id for your app <br/>
**Client Secret**:   a Key to call veracity API which is secured by Azure AD B2C <br/>
**Reply URL (Redirect Uri)**:    the URL you provided where the tokens will be posted back to. Although, multiple urls are allowed, all the urls are supposed to be under the same domain. The following example illustrates what the domain means.<br/>

|                | URL 1                    | URL 2                  |
|----------------|--------------------------|------------------------|
|Same Domain     | https://localhost:134679 | https://localhost:43300 |  
|Different Domain| https://localhost:134679 | https://someapp.azurewebsites.net | 

As a result, you will need corresponding Azure AD B2C configurations for each domain. <br/>
> You also need to ensure the RedirectUri ends with signin-oidc, like *https://localhost:43300/signin-oidc*, This is the default value for the OIDC client middleware.<br/>

**Tenant name**:      dnvglb2ctest.onmicrosoft.com (id: ed815121-cdfa-4097-b524-e2b23cd36eb6) <br/>
**Policy name**:      B2C_1A_SignInWithADFSIdp <br/>
**Veracity Service API (APIv3)**:<br/>
 * Host name : https://myapiv3test.dnvgl.com
 * Identifier in Azure AD B2C: https://dnvglb2ctest.onmicrosoft.com/a4a8e726-c1cc-407c-83a0-4ce37f1ce130

**ServiceID in My Services**: a GUID string, used when you call the Veracity Service API to for example update subscriptions<br/>

## Step-by-Step Instruction (ASP.NET core 2.0)
1. **Install Microsoft.Identity.Client in your NuGet package manager**: The package is currently in preview so you need to check **Include prerelease** to search for it. This is the package which makes it easier to obtain tokens from Microsoft Azure AD B2C.
2. **Modify appsettings.json**: add the following configuration details for Azure AD B2C. <br/>
```
"Authentication": {
    "AzureAdB2C": {
      "Tenant": "dnvglb2ctest.onmicrosoft.com",
      "TenantId": "ed815121-cdfa-4097-b524-e2b23cd36eb6",
      "ClientId": "**Your Client ID**",
      "ClientSecret": "**Your Client Secret**",
      "RedirectUri": "https://localhost:44330/signin-oidc",  
      "SignUpSignInPolicyId": "B2C_1A_SignInWithADFSIdp",
      "ResetPasswordPolicyId": "B2C_1A_SignInWithADFSIdp",
      "EditProfilePolicyId": "B2C_1A_SignInWithADFSIdp",
      "ApiUrl": "https://myapiv3test.dnvgl.com",
      "ApiScopes": "https://dnvglb2ctest.onmicrosoft.com/a4a8e726-c1cc-407c-83a0-4ce37f1ce130/user_impersonation"
    }
  },
  "VeracityService": {
    "HostName": "https://myapiv3test.dnvgl.com",
    "Identifier": "https://dnvglb2ctest.onmicrosoft.com/a4a8e726-c1cc-407c-83a0-4ce37f1ce130", 
    "ServiceId": "**Your Service ID**"
  }
```
