using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Test.auth.Models;

namespace Test.auth.Extentions
{
    public static class ClientStoreExtension
    {
        /// <summary>
        /// Determines whether the client is configured to use PKCE.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="client_id">The client identifier.</param>
        /// <returns></returns>
        public static async Task<bool> IsPkceClientAsync(this IClientStore store, string client_id)
        {
            if (!string.IsNullOrWhiteSpace(client_id))
            {
                var client = await store.FindEnabledClientByIdAsync(client_id);
                return client?.RequirePkce == true;
            }

            return false;
        }

        public static IActionResult RedirectPage(this PageModel pageModel, string redirectUrl)
        {
            pageModel.HttpContext.Response.StatusCode = 200;
            pageModel.HttpContext.Response.Headers["Location"] = "";

            return pageModel.RedirectToPage("/Redirect", new { redirectUrl });
        }

        public static IActionResult LoadingPage(this PageModel pageModel, string pageName, string redirectUrl)
        {
            pageModel.HttpContext.Response.StatusCode = 200;
            pageModel.HttpContext.Response.Headers["Location"] = "";

            return pageModel.RedirectToPage(pageName, new { redirectUrl } );
        }
    }
}
