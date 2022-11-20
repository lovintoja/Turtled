using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtledDictionary.Consts
{
    public class AuthenticationJWTConst
    {
        public class JWTClaimTypes
        {
            public const string RefreshId = "RefreshId";
        }

        public class IdentityRoles
        {
            public const string Guest = "Guest";
            public const string Owner = "Owner";
        }
    }
}
