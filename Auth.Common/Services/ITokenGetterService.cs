using System;
using System.Collections.Generic;

namespace Auth.Common.Services
{
    public interface ITokenGetterService
    {
        string GetToken(string userId, DateTime from, IEnumerable<string> roles,
            IDictionary<string, string> additionalClaims = null);
    }
}
