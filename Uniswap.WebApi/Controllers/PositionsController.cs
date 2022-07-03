using System;
using System.Threading.Tasks;
using System.Web.Http;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;

namespace Uniswap.WebApi.Controllers
{
    public class PositionsController : ApiController
    {
        readonly GraphQLHttpClientOptions _graphQlOptions = new GraphQLHttpClientOptions
        {
            EndPoint = new Uri("https://api.thegraph.com/subgraphs/name/uniswap/uniswap-v3")
        };

        readonly GraphQL.Uniswap _uniswapGraphQl;


        public PositionsController()
        {
            var client = new GraphQLHttpClient(_graphQlOptions, new SystemTextJsonSerializer());
            _uniswapGraphQl = new GraphQL.Uniswap(client);
        }

        [HttpGet]
        [Route("positions/{id}/{precision0?}/{precision1?}")]
        public async Task<IHttpActionResult> GetPosition(
            int id,
            int? precision0 = null,
            int? precision1 = null)
        {
            var result = await _uniswapGraphQl.GetLiquidityPosition(
                id,
                precision0,
                precision1);

            var ret = new
            {
                Token0 = new
                {
                    result.Position.Token0.Symbol,
                    Amount = result.Position.Amount0
                },
                Token1 = new
                {
                    result.Position.Token1.Symbol,
                    Amount = result.Position.Amount1
                }
            };

            return Ok(ret);
        }
    }
}
