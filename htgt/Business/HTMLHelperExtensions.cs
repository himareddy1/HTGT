using System.Web.Mvc;

namespace htgt.Business
{
    public static class HTMLHelperExtensions
    {
        public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }
    }
}