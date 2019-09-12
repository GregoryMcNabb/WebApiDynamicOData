using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataService.Extensions
{
    static class ODataUriSegmentExtensions
    {
        public static string GenerateKey(this KeySegment keyValueSegment)
        {
            return string.Join(",", keyValueSegment
               .Keys.Select(k => $"{k.Key}={k.Value}"));
        }
    }
}
