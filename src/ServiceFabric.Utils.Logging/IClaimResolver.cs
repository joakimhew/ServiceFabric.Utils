using Microsoft.Owin;
using System.Collections.Generic;
using System.Security.Claims;

namespace ServiceFabric.Utils.Logging
{
    /// <summary>
    /// Provides a way to extract <see cref="Claim"/>(s) from an <see cref="IOwinContext"/>.
    /// </summary>
    public interface IClaimResolver
    {
        /// <summary>
        /// Get the claims from the <see cref="IOwinContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="IOwinContext"/></param>
        /// <returns>An collection of <see cref="Claim"/></returns>
        IEnumerable<Claim> GetClaims(IOwinContext context);
    }
}
