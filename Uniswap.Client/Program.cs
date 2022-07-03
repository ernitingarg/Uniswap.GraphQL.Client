using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;

namespace Uniswap.Client
{
    class Program
    {
        static readonly Stopwatch StopWatch = new Stopwatch();
        const string UniswapUrl = "https://api.thegraph.com/subgraphs/name/uniswap/uniswap-v3";

        static async Task Main(string[] args)
        {
            Console.WriteLine("1. Please input pool id.");

            int poolId;
            while (!int.TryParse(Console.ReadLine(), out poolId))
            {
                Console.WriteLine(
                    "Entered pool id is not in correct integer format, please input again.");
            }

            Console.WriteLine(
                "2. Please input decimal precision for amount0 or input 0 for default.");
            int precision0;
            while (!int.TryParse(Console.ReadLine(), out precision0))
            {
                Console.WriteLine(
                    "Entered precision is not in correct integer format, please input again.");
            }

            Console.WriteLine(
                "3. Please input decimal precision for amount1 or input 0 for default.");
            int precision1;
            while (!int.TryParse(Console.ReadLine(), out precision1))
            {
                Console.WriteLine(
                    "Entered precision is not in correct integer format, please input again.");
            }

            Console.WriteLine(
                "4. Please input time interval in seconds to fetch the data.");
            int interval;
            while (!int.TryParse(Console.ReadLine(), out interval))
            {
                Console.WriteLine(
                    "Entered time interval is not in correct integer format, please input again.");
            }

            var graphQlOptions = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(UniswapUrl)
            };

            var graphQlClient = new GraphQLHttpClient(
                graphQlOptions,
                new SystemTextJsonSerializer());

            var unisawp = new GraphQL.Uniswap(graphQlClient);

            var t = new Timer();
            t.Elapsed += (sender, e) => Callback(unisawp,
                poolId,
                precision0 == 0 ? null : (int?)precision0,
                precision1 == 0 ? null : (int?)precision1);

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
            int? precision0,
            int? precision1)
        {
            StopWatch.Restart();
            var result = await uniswapGraphQl.GetLiquidityPosition(poolId, precision0, precision1);

            Console.WriteLine(
                $"[{DateTime.Now}] " +
                $"{result.Position.Token0.Symbol} position {result.Position.Amount0}, " +
                $"{result.Position.Token1.Symbol} position {result.Position.Amount1} " +
                $"[Time taken: {StopWatch.ElapsedMilliseconds} ms]");
            StopWatch.Stop();
        }
    }
}
