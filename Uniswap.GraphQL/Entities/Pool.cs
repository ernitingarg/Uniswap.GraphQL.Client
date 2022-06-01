using System.Text.Json.Serialization;

namespace Uniswap.GraphQL.Entities
{
    public class Pool
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string SqrtPrice { get; set; }
    }
}
