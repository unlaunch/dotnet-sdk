using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using io.unlaunch;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Dns.GetHostName());
            Console.WriteLine(Environment.Version.ToString());
            
            var client = UnlaunchClient.Builder().Host("http://localhost:5000").SdkKey("test-sdk-fc257fb3-7868-4b74-8b7d-17cf27f4e66a").Build();

            try
            {
                client.AwaitUntilReady(6000);
                Console.WriteLine("Client is ready ......!!!");
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e.Message);
            }

            var array = new []
            {
                UnlaunchAttribute.NewSet("elite", new HashSet<string>(new []{"ba1","tua","hi1"}))
            };

            var feature = client.GetFeature("moreflag", "4039032985", array);
            Console.WriteLine("Variation: " + feature.GetVariation());
            Console.WriteLine("Evaluation reason: ...: " + feature.GetEvaluationReason());
            
            client.Shutdown();
            
            Thread.Sleep(200000);
        }
    }
}
