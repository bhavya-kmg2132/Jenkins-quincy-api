using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.ExternalPolicy.Rules
{
    // DB2's DMBP130P table stores OBILMD as a thousands-scale code (100/300/500/1000)
    // instead of the actual limit (100000/300000/500000/1000000). The ChangePolicy read
    // scales the code back up so callers see real dollar limits.
    public static class OptionalBodilyInjuryLimitScaler
    {
        private const string FieldName = "OBILMD";

        private static readonly IReadOnlyDictionary<decimal, decimal> ScreenToDb2Codes = new Dictionary<decimal, decimal>
        {
            [100000m] = 100m,
            [300000m] = 300m,
            [500000m] = 500m,
            [1000000m] = 1000m
        };

        private static readonly IReadOnlyDictionary<decimal, decimal> Db2CodesToScreen =
            ScreenToDb2Codes.ToDictionary(kv => kv.Value, kv => kv.Key);

        public static string ScaleUpFromDb2(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return json;

            JsonNode root;
            try
            {
                root = JsonNode.Parse(json);
            }
            catch (JsonException)
            {
                return json;
            }

            if (root == null)
                return json;

            ScaleNode(root, Db2CodesToScreen);
            return root.ToJsonString();
        }

        private static void ScaleNode(JsonNode node, IReadOnlyDictionary<decimal, decimal> map)
        {
            switch (node)
            {
                case JsonObject obj:
                    var key = obj.Select(p => p.Key).FirstOrDefault(k => string.Equals(k, FieldName, StringComparison.OrdinalIgnoreCase));
                    if (key != null && obj[key] is JsonValue value
                        && TryParseAmount(value, out var amount, out var wasString) && map.TryGetValue(amount, out var scaled))
                    {
                        obj[key] = ToScaledNode(scaled, wasString);
                    }

                    foreach (var property in obj.ToList())
                    {
                        if (!string.Equals(property.Key, key, StringComparison.OrdinalIgnoreCase))
                            ScaleNode(property.Value, map);
                    }
                    break;

                case JsonArray array:
                    foreach (var item in array)
                        ScaleNode(item, map);
                    break;
            }
        }

        private static bool TryParseAmount(JsonValue value, out decimal amount, out bool wasString)
        {
            if (value.TryGetValue(out decimal d))
            {
                wasString = false;
                amount = d;
                return true;
            }

            if (value.TryGetValue(out string s))
            {
                wasString = true;
                return TryParseAmountString(s, out amount);
            }

            wasString = false;
            amount = 0;
            return false;
        }

        private static bool TryParseAmountString(string raw, out decimal amount)
            => decimal.TryParse(raw?.Replace(",", string.Empty), NumberStyles.Number, CultureInfo.InvariantCulture, out amount);

        private static JsonNode ToScaledNode(decimal scaled, bool wasString)
            => wasString ? JsonValue.Create(((long)scaled).ToString(CultureInfo.InvariantCulture)) : JsonValue.Create(scaled);
    }
}
