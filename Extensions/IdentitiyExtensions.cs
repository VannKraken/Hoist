using System.Security.Claims;
using System.Security.Principal;

namespace Hoist.Extensions
{
    public static class IdentitiyExtensions 
    {

        public static int GetCompanyId(this IIdentity identity) //
        {

            Claim claim = ((ClaimsIdentity)identity).FindFirst("CompanyId")!;
            return int.Parse(claim.Value);
            
        }

    }
}
