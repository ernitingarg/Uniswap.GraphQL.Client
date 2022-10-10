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
        [Route("positions/{id}/{precision0?}/{precision1?}/{precisionForCurrentPrice?}")]
        public async Task<IHttpActionResult> GetPosition(
            int id,
            int? precision0 = null,
            int? precision1 = null,
            int? precisionForCurrentPrice = null)
        {
            var result = await _uniswapGraphQl.GetLiquidityPosition(
                id,
                3,
                precision0,
                precision1,
                precisionForCurrentPrice);

            var ret = new
            {
                Token0 = new
                {
                    result.Position.Token0.Symbol,
                    Amount = result.Position.Position0Amount
                },
                Token1 = new
                {
                    result.Position.Token1.Symbol,
                    Amount = result.Position.Position1Amount
                },
                CurrentPrice = result.Position.Price0Amount
            };

            return Ok(ret);
        }
    }
}
