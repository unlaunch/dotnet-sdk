# Unlaunch .NET SDK

| main                                                                                                                | development                                                                                                                |
|---------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------|
| [![Build Status](https://app.travis-ci.com/unlaunch/dotnet-sdk.svg?branch=master)](https://app.travis-ci.com/unlaunch/dotnet-sdk) | [![Build Status](https://app.travis-ci.com/unlaunch/dotnet-sdk.svg?branch=develop)](https://app.travis-ci.com/unlaunch/dotnet-sdk) |

## Overview
The Unlaunch .NET SDK provides .NET Framework or .NET Core API to access Unlaunch feature flags and other features. Using the SDK, you can easily build .NET applications that can evaluate feature flags, dynamic configurations, and more.

### Important Links

- To create feature flags to use with .NET SDK, login to your Unlaunch Console at [https://app.unlaunch.io](https://app.unlaunch.io)
- [Official Guide](https://docs.unlaunch.io/docs/sdks/dotnet-sdk)
- [Nuget](https://www.nuget.org/packages/unlaunch)

### Compatibility
.NET Framework 4.5+ and .NET Core 2.0+

### This is a server-side SDK
This SDK is server-side and should be used in applications that you run on your own servers such as backend
 services or web servers. For more information, see [this](https://docs.unlaunch.io/docs/sdks/client-vs-server-side-sdks).

## Getting Started
Here is a simple example. 

First import the SDK using Nuget. 

```
Install-Package unlaunch -Version 1.0.0
```

Here's an example showing how you'd use Unlaunch .NET SDK in your application.

```csharp
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
                client.AwaitUntilReady(TimeSpan.FromSeconds(2));
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

### Singleton in web application

```csharp
var unlaunchClient = UnlaunchClient.Create("INSERT_YOUR_SDK_KEY");
builder.Register(c => unlaunchClient).As<IUnlaunchClient>().SingleInstance();
```            

## Customization

You can use builder to customize the client. For more information, see the [official guide](https://docs.unlaunch.io/docs/sdks/dotnet-sdk).

```csharp
var client = UnlaunchClient.Builder()
                .SdkKey("INSERT_YOUR_SDK_KEY")
                .PollingIntervalInSeconds(TimeSpan.FromSeconds(60))
                .EventsFlushIntervalInSeconds(TimeSpan.FromSeconds(30))
                .EventsQueueSize(500)
                .MetricsFlushIntervalInSeconds(TimeSpan.FromSeconds(30))
                .MetricsQueueSize(100)
                .Build();
```

## License
Licensed under the Apache License, Version 2.0. See: [Apache License](LICENSE.md).
