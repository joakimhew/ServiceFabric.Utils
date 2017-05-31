using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ServiceFabric.Utils.Http.Logging
{
    /// <summary>
    /// Provides a way to extract <see cref="Claim"/>(s) from a Jwt Authentication Header
    /// </summary>
    public class JwtClaimResolver : IClaimResolver
    {
        /// <summary>
        /// Get the claims from the <see cref="IOwinContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="IOwinContext"/></param>
        /// <returns>An collection of <see cref="Claim"/></returns>
        /// <exception cref="ArgumentNullException"/>
        public IEnumerable<Claim> GetClaims(IOwinContext context)
        {
            var request = context.Request ?? throw new ArgumentNullException(nameof(context));

            var header = request.Headers.Get("Authorization");
            if (string.IsNullOrWhiteSpace(header))
                return new Claim[] { };

            var auhorizationHeader = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(header);
            var sth = new JwtSecurityTokenHandler();
            var token = sth.ReadJwtToken(auhorizationHeader.Parameter);
            return token.Claims;
        }
    }
}
