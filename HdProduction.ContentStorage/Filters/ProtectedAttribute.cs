using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HdProduction.ContentStorage.Filters
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class ProtectedAttribute : Attribute, IActionFilter
  {
    private const string HttpHeaderProtectedKey = "access-key";
    private const string ProtectedKey = "25666AB4CE0D3512E1C1FD1BA044C460E4814F66A9ABFCD4A81FAF7C2BE9A9C9";

    public void OnActionExecuting(ActionExecutingContext context)
    {
      if (context.HttpContext.Request.Headers[HttpHeaderProtectedKey] != ProtectedKey)
      {
        context.Result = new UnauthorizedResult();
      }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
  }
}