using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace BBQ.Filters
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple=false, Inherited=true)]
    public class CallCountAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            Controllers.StatsController.setStat("TotalCalls", Controllers.StatsController.getStat("TotalCalls") + 1);
            base.OnActionExecuting(actionContext);
        }
    }
}