using System;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Timers;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using log4net;

namespace Uniswap.Client
{
    public class Program
    {
        static readonly Stopwatch StopWatch = new Stopwatch();
        const string UniswapUrl = "https://api.thegraph.com/subgraphs/name/uniswap/uniswap-v3";

        static readonly ILog SLogger = LogManager.GetLogger(
            MethodBase.GetCurrentMethod()?.DeclaringType);

        static void Main(string[] args)
        {
            bool validInput = true;
            if (!int.TryParse(ConfigurationManager.AppSettings["PoolId"], out var poolId))
            {
                Console.WriteLine(
                    "Configuration for key 'PoolId' is not in correct integer format.");
                validInput = false;
            }

            if (!int.TryParse(
                    ConfigurationManager.AppSettings["PrecisionForPosition0"],
                    out var precisionForPosition0))
            {
                Console.WriteLine(
                    "Configuration for key 'PrecisionForPosition0' is not in correct integer format.");
                validInput = false;
            }

            if (!int.TryParse(
                    ConfigurationManager.AppSettings["PrecisionForPosition1"],
                    out var precisionForPosition1))
            {
                Console.WriteLine(
                    "Configuration for key 'PrecisionForPosition1' is not in correct integer format.");
                validInput = false;
            }

            if (!int.TryParse(
                    ConfigurationManager.AppSettings["PrecisionForCurrentPrice"],
                    out var precisionForCurrentPrice))
            {
                Console.WriteLine(
                    "Configuration for key 'PrecisionForCurrentPrice' is not in correct integer format.");
                validInput = false;
            }

            if (!int.TryParse(
                    ConfigurationManager.AppSettings["IntervalInSeconds"],
                    out var interval))
            {
                Console.WriteLine(
                    "Configuration for key 'IntervalInSeconds' is not in correct integer format.");
                validInput = false;
            }

            if (!validInput)
            {
                return;
            }

            var graphQlOptions = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(UniswapUrl)
            };

            var graphQlClient = new GraphQLHttpClient(
                graphQlOptions,
                new SystemTextJsonSerializer());

            var unisawp = new GraphQL.Uniswap(graphQlClient);

            int retry = 0;
            int sum = 0;
            do
            {
                retry++;
                sum += 5 * retry;
            } while (sum < interval);

            var t = new Timer();
            t.Elapsed += (sender, e) => Callback(unisawp,
                poolId,
                retry - 1,
                precisionForPosition0 == 0 ? null : (int?)precisionForPosition0,
                precisionForPosition1 == 0 ? null : (int?)precisionForPosition1,
                precisionForCurrentPrice == 0 ? null : (int?)precisionForCurrentPrice);

            t.Interval = TimeSpan.FromSeconds(interval).TotalMilliseconds;
            t.Enabled = true;

            Console.WriteLine("Press \'q\' to quit the sample.");
            Console.WriteLine();

            Console.WriteLine($"*********** Liquidity position for pool id : {poolId} ***********");
            Console.WriteLine("***************************************************************");

            while (Console.Read() != 'q') { };
        }

        static async void Callback(
            GraphQL.Uniswap uniswapGraphQl,
            int poolId,
            int maxRetryCount,
            int? precisionForPosition0,
            int? precisionForPosition1,
            int? precisionForCurrentPrice)
        {
            StopWatch.Restart();
            string msg = string.Empty;

            try
            {
                var result = await uniswapGraphQl.GetLiquidityPosition(
                    poolId,
                    maxRetryCount,
                    precisionForPosition0,
                    precisionForPosition1,
                    precisionForCurrentPrice);

                msg = $"[{DateTime.Now}] " +
                      $"{result.Position.Token0.Symbol} position {result.Position.Position0Amount}, " +
                      $"{result.Position.Token1.Symbol} position {result.Position.Position1Amount}, " +
                      $"Current Price {result.Position.Price0Amount} ({result.Position.Token0.Symbol} per {result.Position.Token1.Symbol}) " +
                      $"[Time taken: {StopWatch.ElapsedMilliseconds} ms]";
            }
            catch(Exception ex)
            {
                SLogger.Error(ex);
                msg = $"[{DateTime.Now}] Unable to retrieve the data from server after {maxRetryCount} retries.";
            }
            finally
            {
                SLogger.Info(msg);
                Console.WriteLine(msg);
            }

            StopWatch.Stop();
        }
    }
}
