using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using log4net;
using Uniswap.GraphQL.Entities;

namespace Uniswap.GraphQL
{
    public class Uniswap : IUniswap
    {
        readonly IGraphQLClient _graphQlClient;
        static readonly ILog SLogger = LogManager.GetLogger(
            MethodBase.GetCurrentMethod()?.DeclaringType);

        public Uniswap(IGraphQLClient graphQlClient)
        {
            _graphQlClient = graphQlClient
                             ?? throw new ArgumentNullException(
                                 nameof(graphQlClient));
        }


        public async Task<LiquidityPosition> GetLiquidityPosition(
            int poolId,
            int maxRetryCount,
            int? precisionForPosition0 = null,
            int? precisionForPosition1 = null,
            int? precisionForCurrentPrice = null)
        {
            int retry = 0;

            var query = new GraphQLRequest
            {
                Query = @"
                {
                  position(id: " + poolId + @") {
                    id
                    owner
                    token0 {
                      symbol
                      decimals
                    }
                    token1 {
                      symbol
                      decimals
                    }
                    pool {
                      sqrtPrice
                      token0Price
                    }
                    liquidity
                    tickLower {
                      price0
                    }
                    tickUpper {
                      price0
                    }
                  }
                }
                "
            };

        API:
            GraphQLResponse<LiquidityPosition> response;
            try
            {
                response = await _graphQlClient.SendQueryAsync<LiquidityPosition>(query);
            }
            catch (Exception)
            {
                retry++;
                if (retry <= maxRetryCount)
                {
                    int sleepInterval = 5000 * retry;

                    string msg = $"[{DateTime.Now}] Retry[{retry}] after {sleepInterval} seconds.";
                    SLogger.Info(msg);
                    Console.WriteLine(msg);

                    Thread.Sleep(sleepInterval);
                    goto API;
                }

                throw;
            }

            var result = response.Data;

            var positionAmounts = PriceHelper.GetPositionAmounts(
                response.Data,
                precisionForPosition0,
                precisionForPosition1);

            var priceAmounts = PriceHelper.GetPriceAmounts(
                response.Data,
                precisionForCurrentPrice);

            result.Position.Position0Amount = positionAmounts.Position0;
            result.Position.Position1Amount = positionAmounts.Position1;
            result.Position.Price0Amount = priceAmounts.Price0;

            return result;
        }
    }
}