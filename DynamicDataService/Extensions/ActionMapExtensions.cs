using System.Linq;
using System.Web.Http.Controllers;

namespace DynamicDataService.Extensions
{
    internal static class ActionMapExtensions
    {
        public static string FindMatchingAction(this ILookup<string, HttpActionDescriptor> actionMap, params string[] targetActionNames)
        {
            foreach (string targetActionName in targetActionNames)
            {
                if (actionMap.Contains(targetActionName))
                {
                    return targetActionName;
                }
            }

            return null;
        }
    }
}
