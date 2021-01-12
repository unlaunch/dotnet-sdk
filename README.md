# Unlaunch .Net SDK

## Overview
The .Net SDK provides .Net Framework or .Net Core API to access Unlaunch feature flags and other features. Using the SDK, you can
 easily build .Net applications that can evaluate feature flags, dynamic configurations, and more.

### Important Links

- To create feature flags to use with Java SDK, login to your Unlaunch Console at [https://app.unlaunch.io](https://app.unlaunch.io)
- [Official Guide](https://github.com/unlaunch/dotnet-sdk)
- [Nuget](https://www.nuget.org/packages/unlaunch)

### Compatibility
.Net Framework 4.5+ and .Net Core 2.0+

## Getting Started
Here is a simple example. 

Here's how you'd use the .Net SDK in your application.

```
using System;
using io.unlaunch;

namespace your.namespace
{
    class Program
    {
        static void Main(string[] args)
        {
            // initialize the client
            var client = UnlaunchClient.Create("INSERT_YOUR_SDK_KEY");

            // wait for the client to be ready
            try
            {
                client.AwaitUntilReady(2000);
            }
            catch (TimeoutException e) {
                Console.WriteLine("client wasn't ready " + e.Message);
            }
            
            // get variation
            var variation = client.GetVariation("flagKey", "userId123");

            // take action based on the returned variation
            if (variation == "on")
            {
                Console.WriteLine("Variation is on");
            }
            else if (variation == "off")
            {
                Console.WriteLine("Variation is off");
            }
            else
            {
                Console.WriteLine("control variation");
            }

            // If you attached (key-value) configuration to your feature flag variations, 
            // here's how you can retrieve it:
            var feature = client.GetFeature("new_login_ui", "userId");
            var colorHexCode = feature.GetVariationConfig().GetString("login_btn_clr", "#cd5c5c");

            // shutdown the client to flush any events or metrics 
            client.Shutdown();
        }
    }
}
```

An example, how to use it in a web application

```
var unlaunchClient = UnlaunchClient.Create("INSERT_YOUR_SDK_KEY");
builder.Register(c => unlaunchClient).As<IUnlaunchClient>().SingleInstance();
```            

## Customization

You can use builder to customize the client. For more information, see the [official guide](https://docs.unlaunch.io/docs/sdks/dotnet-sdk).

```
var client = UnlaunchClient.Builder()
                .SdkKey("INSERT_YOUR_SDK_KEY")
                .PollingIntervalInSeconds(60)
                .EventsFlushIntervalInSeconds(30)
                .EventsQueueSize(500)
                .MetricsFlushIntervalInSeconds(30)
                .MetricsQueueSize(100)
                .Build();
```

## License
Licensed under the Apache License, Version 2.0. See: [Apache License](LICENSE.md).
