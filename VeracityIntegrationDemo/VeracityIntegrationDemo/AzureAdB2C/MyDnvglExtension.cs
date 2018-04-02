using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace VeracityIntegrationDemo.AzureAdB2C
{
    public static class MyDnvglExtension
    {
        public static string Name(this ClaimsPrincipal user)
        {
            return user.FindFirst("name")?.Value;
        }

        public static string MyDnvglId(this ClaimsPrincipal user)
        {
            return user.FindFirst("userId")?.Value;
        }

        public static string DnvglAccountName(this ClaimsPrincipal user)
        {
            return user.FindFirst("dnvglAccountName")?.Value;
        }

        public static string Email(this ClaimsPrincipal user)
        {
            return user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
        }
    }
}
