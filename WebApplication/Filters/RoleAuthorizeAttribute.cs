﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Common;

namespace WebApplication
{

    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {

        private bool isAllowed = true;
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return isAllowed;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        { 
            string conName = filterContext.RouteData.Values["controller"].ToString().ToLower();
            if (conName.Contains("login") || conName.Contains("front"))
            {
                return;//如果为主页则无需验证权限
            }
                
            string actName = filterContext.RouteData.Values["action"].ToString();
            if (UserHelper.GetCurrentUser == null)
            {
                var Url = new UrlHelper(filterContext.RequestContext);
                var url = Url.Action("login", "login", new { area = "" });
                filterContext.Result = new RedirectResult(url);
            }
            else
            {
                string role = UserHelper.GetCurrentUser.Role;
                if (role == "管理员" || conName == "front" || conName == "home")
                {
                    isAllowed = true;
                }
                else if (role == "客服" || role == "生产员")
                {
                    if (conName != "order")
                    {
                        isAllowed = false;
                    }
                }
            }
            if (!isAllowed)
            {
                var Url = new UrlHelper(filterContext.RequestContext);
                var url = Url.Action("Result", "Front", new { message = "您没有该页面的权限！" });
                filterContext.Result = new RedirectResult(url);
            }
            //base.OnAuthorization(filterContext);

        } 
    }

}