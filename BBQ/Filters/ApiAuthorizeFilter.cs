using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace BBQ.Filters
{    
    [AttributeUsage(AttributeTargets.All, AllowMultiple=false, Inherited=true)]
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {            
            var req = actionContext.Request;

            var auth = req.Headers.Authorization;
            if(auth!=null)
            {
                var parser = Repository.RepositoryFactory.getTokenParser(auth.Scheme);
                if(parser.Parse(auth.Parameter))
                {
                    var auth_me = Repository.RepositoryFactory.getAuthorizationStrategy();
                    var repo = Repository.RepositoryFactory.createAuthorizationRepository(Guid.Empty);

                    if (auth_me.Authenticate(repo, parser.AccountID, parser.Secret))
                    {
                        var user = new GenericPrincipal(new GenericIdentity(parser.AccountID.ToString()), null);
                        Thread.CurrentPrincipal = user;
                        if (HttpContext.Current != null)
                        {
                            HttpContext.Current.User = user;
                        }
                        return true;
                    }
                }
            }

            //something didn't work, not auth()
            actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            return false;
        }
    }
}