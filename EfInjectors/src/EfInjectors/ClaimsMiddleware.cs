using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EfInjectors
{
    public class ClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public ClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            System.Threading.Thread.CurrentPrincipal = context.User;
            if (context.User.Identity.IsAuthenticated)
            {
                var claimsIdendity = (ClaimsIdentity)context.User.Identity;
                claimsIdendity.AddClaim(new Claim("Tenant", "2"));
            }
            await _next.Invoke(context);
        }
    }

    public static class ClaimsMiddlewareExtensions
    {
        public static IApplicationBuilder UseClaimsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ClaimsMiddleware>();
        }
    }
}
