using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Uniswap.GraphQL;

namespace Uniswap.Client
{
    class Program
    {
        static readonly Stopwatch _s_watch = new Stopwatch();

        static async Task Main(string[] args)
        {
            var graphQlOptions = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri("https://api.thegraph.com/subgraphs/name/uniswap/uniswap-v3")
            };

            var client = new GraphQLHttpClient(
                graphQlOptions,
                new SystemTextJsonSerializer());

            var uniswapGraphQl = new GraphQL.Uniswap(client);

            var t = new Timer();
            t.Elapsed += (sender, e) => Callback(sender, e, uniswapGraphQl);
            t.Interval = 60000;
            t.Enabled = true;

            Console.WriteLine("Press \'q\' to quit the sample.");
            Console.WriteLine();

            Console.WriteLine("*********** Liquidity position for pool id : 245950 ***********");
            Console.WriteLine("***************************************************************");

            while (Console.Read() != 'q') ;
        }

        static async void Callback(
            object source,
            ElapsedEventArgs e,
            GraphQL.Uniswap uniswapGraphQl)
        {
            _s_watch.Restart();
            var result = await uniswapGraphQl.GetOwnerPosition(245950);
            var adjustedAmounts = PriceHelper.GetPositionAmounts(result, 5, 2);

            Console.WriteLine(
                $"[{DateTime.Now}] " +
                $"{result.Position.Token0.Symbol} position {adjustedAmounts.Amount0}, " +
                $"{result.Position.Token1.Symbol} position {adjustedAmounts.Amount1} " +
                $"[Time taken: {_s_watch.ElapsedMilliseconds} ms]");
            _s_watch.Stop();
        }
    }
}
