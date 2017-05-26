using System.Globalization;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Helpers
{
    public static class JsonHelper
    {
        public static decimal? ToDecimal(this JToken token) => decimal.TryParse((string)token, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d as decimal? : null;

        public static int? ToInt(this JToken token) => int.TryParse((string)token, out var i) ? i as int? : null;

    }
}