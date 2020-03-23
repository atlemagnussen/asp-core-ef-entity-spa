using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Test.model.Users;

namespace Test.webapi.Filter
{
    //public class MyFilter : IAuthorizationFilter
    //{
    //    public void OnAuthorization(AuthorizationFilterContext context)
    //    {
    //        var user = context.HttpContext.User;
    //        if (!user.Identity.IsAuthenticated)
    //            context.Result = new UnauthorizedResult();

    //        var roleClaim = user.Claims.FirstOrDefault();
    //    }
    //}

    public class MyRequirement : IAuthorizationRequirement
    {
        public string Role { get; }
        public MyRequirement(string role)
        {
            Role = role;
        }
    }

    public class MyHandler : AuthorizationHandler<MyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MyRequirement requirement)
        {
            var user = context.User;
            if (!user.Identity.IsAuthenticated)
                return Task.CompletedTask;
            
            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (roleClaim != null && roleClaim.Value == requirement.Role)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
