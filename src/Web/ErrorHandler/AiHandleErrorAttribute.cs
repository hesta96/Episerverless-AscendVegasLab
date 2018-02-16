using System;using System.Web.Mvc;using Microsoft.ApplicationInsights;namespace Web1.ErrorHandler{    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]    public class AiHandleErrorAttribute : HandleErrorAttribute    {        public override void OnException(ExceptionContext filterContext)        {            if (filterContext?.HttpContext != null && filterContext.Exception != null)                if (filterContext.HttpContext.IsCustomErrorEnabled)                {                    var ai = new TelemetryClient();                    ai.TrackException(filterContext.Exception);                }            base.OnException(filterContext);        }    }}