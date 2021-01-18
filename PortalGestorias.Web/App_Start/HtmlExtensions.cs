using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace PortalGestorias.Web
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString ActionLinkAuthorize(this HtmlHelper html, string linkText, string actionName, ResourceAuthorizationContext authContext,
            object routeValues = null, object htmlAttributes = null)
        {
            return ActionLinkAuthorize(html, linkText, actionName, null, authContext, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ActionLinkAuthorize(this HtmlHelper html, string linkText, string actionName, string controllerName, ResourceAuthorizationContext authContext, object routeValues = null, object htmlAttributes = null)
        {
            var authManager = new AuthorizationManager();
            return authManager.CheckAccessAsync(authContext).Result
                ? html.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes)
                : new MvcHtmlString(string.Empty);
        }

        public static ResourceAuthorizationContext GetAuthContext(this ClaimsPrincipal principal, string action, string resource)
        {
            return new ResourceAuthorizationContext(principal, action, resource);
        }

        public static bool ResourceAccessValidation(this ClaimsPrincipal principal, string action, string resource)
        {
            var authContext = new ResourceAuthorizationContext(principal, action, resource);
            var authManager = new AuthorizationManager();
            var accessResult = authManager.CheckAccessAsync(authContext).Result;
            return accessResult;
        }

        public static WebGridColumn Column<TModel, TValue>(this HtmlHelper<IEnumerable<TModel>> html,
            Expression<Func<TModel, TValue>> expression, Func<TModel, object> format = null, string header = null, string style = null,
            bool canSort = true)
        {
            var columnHeader = header ?? HttpUtility.HtmlDecode(html.DisplayNameFor(expression).ToString());
            Func<dynamic, object> localFormat = f => format?.Invoke(f.Value);
            if (format == null)
            {
                Func<TModel, object> displayFunction = model =>
                {
                    var memberExpression = expression.Body as MemberExpression;
                    if (memberExpression == null)
                    {
                        return string.Empty;
                    }

                    var lambda = Expression.Lambda<Func<IEnumerable<TModel>, TValue>>(
                        memberExpression.Update(memberExpression.Replace(Expression.Constant(model, typeof(TModel)))),
                        Expression.Parameter(typeof(IEnumerable<TModel>), "lista"));
                    return html.DisplayFor(lambda);
                };
                localFormat = f => displayFunction.Invoke(f.Value);
            }

            return new WebGridColumn
            {
                ColumnName = (expression.Body as MemberExpression)?.Member.Name,
                Header = columnHeader,
                Format = localFormat,
                Style = style,
                CanSort = canSort
            };
        }

        private static Expression Replace(this MemberExpression exp, Expression replacement)
        {
            var innerExpression = exp.Expression as MemberExpression;
            if (innerExpression == null && exp.Expression.NodeType == ExpressionType.Parameter)
            {
                return replacement;
            }
            var replace = innerExpression.Replace(replacement);
            var expression = (exp.Expression as MemberExpression)?.Update(replace);
            return expression;
        }
    }
}