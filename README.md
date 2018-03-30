
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
```Json
"Authentication": {
    "AzureAdB2C": {
      "Tenant": "dnvglb2ctest.onmicrosoft.com",
      "TenantId": "ed815121-cdfa-4097-b524-e2b23cd36eb6",
      "ClientId": "**Your Client ID**",
      "ClientSecret": "**Your Client Secret**",
      "RedirectUri": "https://localhost:44330/signin-oidc",  
      "SignUpSignInPolicyId": "B2C\_1A\_SignInWithADFSIdp",
      "ResetPasswordPolicyId": "B2C\_1A\_SignInWithADFSIdp",
      "EditProfilePolicyId": "B2C\_1A\_SignInWithADFSIdp",
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
3. **Create a class to map your settings**:
```C#
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
```

4.**Edit Startup.cs**: Add the following code. Ignore the error for now.
*In the ConfigureServices method*
```C#
services.Configure<MvcOptions>(options =>
	{
		options.Filters.Add(new RequireHttpsAttribute());
	});

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

services.AddAuthentication(sharedOptions =>
	{
		sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
	})
	.AddAzureAdB2C(options => Configuration.Bind("Authentication:AzureAdB2C", options))
	.AddCookie();
```
*In the Configure method*
```
app.UseSession();
app.UseAuthentication();
```
5. **Create a class to setup openid**:
``` C#
public static class AzureAdB2CAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAzureAdB2C(this AuthenticationBuilder builder)
            => builder.AddAzureAdB2C(_ =>
            {
            });
 
        public static AuthenticationBuilder AddAzureAdB2C(this AuthenticationBuilder builder, Action<AzureAdB2COptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, OpenIdConnectOptionsSetup>();
            builder.AddOpenIdConnect();
            return builder;
        }
 
        public class OpenIdConnectOptionsSetup : IConfigureNamedOptions<OpenIdConnectOptions>
        {
 
            public OpenIdConnectOptionsSetup(IOptions<AzureAdB2COptions> b2cOptions)
            {
                AzureAdB2COptions = b2cOptions.Value;
            }
 
            public AzureAdB2COptions AzureAdB2COptions { get; set; }
 
            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = AzureAdB2COptions.ClientId;
                options.Authority = AzureAdB2COptions.Authority;
                options.UseTokenLifetime = true;
                options.TokenValidationParameters = new TokenValidationParameters() { NameClaimType = "userId" };
 
                options.Events = new OpenIdConnectEvents()
                {
                    OnRedirectToIdentityProvider = OnRedirectToIdentityProvider,
                    OnRemoteFailure = OnRemoteFailure,
                    OnAuthorizationCodeReceived = OnAuthorizationCodeReceived
                };
            }
 
            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
 
            public Task OnRedirectToIdentityProvider(RedirectContext context)
            {
                var defaultPolicy = AzureAdB2COptions.DefaultPolicy;
                if (context.Properties.Items.TryGetValue(AzureAdB2COptions.PolicyAuthenticationProperty, out var policy) &&
                    !policy.Equals(defaultPolicy))
                {
                    context.ProtocolMessage.Scope = OpenIdConnectScope.OpenIdProfile;
                    context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.IdToken;
                    context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress.ToLower().Replace(defaultPolicy.ToLower(), policy.ToLower());
                    context.Properties.Items.Remove(AzureAdB2COptions.PolicyAuthenticationProperty);
                }
                else if (!string.IsNullOrEmpty(AzureAdB2COptions.ApiUrl))
                {
                    context.ProtocolMessage.Scope += $" offline_access {AzureAdB2COptions.ApiScopes}";
                    context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                }
                return Task.FromResult(0);
            }
 
            public Task OnRemoteFailure(RemoteFailureContext context)
            {
                context.HandleResponse();
                // Handle the error code that Azure AD B2C throws when trying to reset a password from the login page 
                // because password reset is not supported by a "sign-up or sign-in policy"
                if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("AADB2C90118"))
                {
                    // If the user clicked the reset password link, redirect to the reset password route
                    context.Response.Redirect("/Session/ResetPassword");
                }
                else if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("access_denied"))
                {
                    context.Response.Redirect("/");
                }
                else
                {
                    context.Response.Redirect("/Home/Error?message=" + context.Failure.Message);
                    //context.Response.Redirect("/Home/Error?message=" + "unauthorized");
                }
                return Task.FromResult(0);
            }
 
            public async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
            {
                // Use MSAL to swap the code for an access token
                // Extract the code from the response notification
                var code = context.ProtocolMessage.Code;
 
                string signedInUserID = context.Principal.FindFirst("userId")?.Value;
                TokenCache userTokenCache = new MSALSessionCache(signedInUserID, context.HttpContext).GetMsalCacheInstance();
                ConfidentialClientApplication cca = new ConfidentialClientApplication(AzureAdB2COptions.ClientId, AzureAdB2COptions.Authority, AzureAdB2COptions.RedirectUri, new ClientCredential(AzureAdB2COptions.ClientSecret), userTokenCache, null);
                try
                {
                    AuthenticationResult result = await cca.AcquireTokenByAuthorizationCodeAsync(code, AzureAdB2COptions.ApiScopes.Split(' '));
 
 
                    context.HandleCodeRedemption(result.AccessToken, result.IdToken);
                }
                catch (Exception ex)
                {
                    //TODO: Handle
                    throw;
                }
            }
        }
    }
```
