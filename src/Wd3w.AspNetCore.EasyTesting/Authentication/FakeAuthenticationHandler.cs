using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Wd3w.AspNetCore.EasyTesting.Authentication
{
    public interface IFakeAuthenticationHandler : IAuthenticationHandler
    {
        void SetSuccess(ClaimsPrincipal claimsPrincipal);

        void SetFail(Exception exception);

        void SetFail(string message);

        void SetResult(AuthenticateResult result);
    }

    // Do not remove this.
    // ReSharper disable once UnusedTypeParameter
    public class FakeAuthenticationHandler<TOptions> : IFakeAuthenticationHandler where TOptions : AuthenticationSchemeOptions, new()
    {
        private AuthenticateResult _authenticateResult;

        internal HttpContext Context { get; set; }

        internal HttpRequest Request => Context.Request;

        internal HttpResponse Response => Context.Response;

        public AuthenticationScheme Scheme { get; set; }

        public void SetResult(AuthenticateResult result)
        {
            _authenticateResult = result;
        }

        public void SetSuccess(ClaimsPrincipal claimsPrincipal)
        {
            _authenticateResult = AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
        }

        public void SetFail(Exception exception)
        {
            _authenticateResult = AuthenticateResult.Fail(exception);
        }

        public void SetFail(string message)
        {
            _authenticateResult = AuthenticateResult.Fail(message);
        }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            return Task.FromResult(_authenticateResult);
        }

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;
            return Task.CompletedTask;
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            Scheme = scheme;
            Context = context;

            return Task.CompletedTask;
        }
    }
}