using System.Text.Json.Serialization;

namespace Uniswap.GraphQL.Entities
{
    public class Tick
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Price0 { get; set; }
    }
}
