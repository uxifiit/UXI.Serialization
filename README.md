# UXIsk Data Serialization Library

[![Build Status](https://dev.azure.com/uxifiit/UXI.Libs/_apis/build/status/uxifiit.UXI.Serialization?branchName=master)](https://dev.azure.com/uxifiit/UXI.Libs/_build/latest?definitionId=4&branchName=master) [![UXI.Serialization package in Public feed in Azure Artifacts](https://feeds.dev.azure.com/uxifiit/905a1e2c-1aff-45b3-bc72-dba43be0a133/_apis/public/Packaging/Feeds/990007cf-a847-406c-9fa5-dec22ee2ccdc/Packages/9d8d0ff5-d02d-4399-99d5-a097b2850312/Badge)](https://dev.azure.com/uxifiit/Packages/_packaging?_a=package&feed=990007cf-a847-406c-9fa5-dec22ee2ccdc&package=9d8d0ff5-d02d-4399-99d5-a097b2850312&preferRelease=true)

A wrapper library over data serialization for generic reading and writing of data in enumerable or observable streams. Current supported formats for serialization are JSON and CSV using the [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) and [CsvHelper](https://github.com/JoshClose/CsvHelper) libraries. UXI.Serialization is part of the common libraries in [UXI.Libs](https://github.com/uxifiit/UXI.Libs).

The base `UXI.Serialization` library supports writing and reading data as `IEnumerable<T>`, the extension library `UXI.Serialization.Reactive` adds support for `IObservable<T>`. 



## Usage

The main access point to the library functionality is the `DataIO` class with `ReadInput` and `WriteOutput` methods. Example with enumerable data:

```csharp
using UXI.Serialization;

// initialize DataIO with supported serialization formats and converters
DataIO io = // see below

// reading data
IEnumerable<Data> input = io.ReadInput<Data>("path/to/input.csv", FileFormat.CSV);

foreach (var item in input) 
{
    // consume data...
}


// writing data
IEnumerable<Data> output = // generate data for output

io.WriteOutput(output, "path/to/output.csv", FileFormat.CSV);
```

### Define serialization formats

Initialize DataIO with factories for serialization formats to specify which formats it will support. These factories are located in the `UXI.Serializtion.Formats.*` namespaces:

* `UXI.Serialization.Formats.Csv.CsvSerializationFactory` for CSV serialization with `CsvSerializerContext` as a wrapper to `CsvSerializer` from the CsvHelper library.
* `UXI.Serialization.Formats.Json.JsonSerializationFactory` for JSON serialization with `JsonSerializer` from the Newtonsoft.Json library.

Each factory accepts list of configurations implementing interface `ISerializationConfiguration`. The configurations are applied during creation of specific serializers for reading or writing data. To introduce custom data converters into serializers, apply them with these configurations.

The following configurations are readily available in the library:
* `UXI.Serialization.Configurations` namespace:
    * `SerializationConfiguration<TSerializer>` - base generic abstract implementation of the interface, where `TSerializer` is `Newtonsoft.Json.JsonSerializer` or `UXI.Serialization.Formats.Csv.CsvSerializerContext`. 
    * `RelaySerializationConfiguration<TSerializer>` - direct implementation which executes given lambda function for configuration.
* `UXI.Serialization.Formats.Csv.Configurations` namespace:
    * `CsvConvertersSerializationConfiguration` - configures the serializer with CSV converters.
    * `CsvHeaderToLowerCaseSerializationConfiguration` - sets the header of the CSV file to lower case (by default, PascalCase is used).
* `UXI.Serialization.Formats.Json.Configurations` namespace:
    * `JsonConvertersSerializationConfiguration` - configures the serializer with JSON converters.

Example of DataIO initialization with serialization factories and configurations:

```csharp
DataIO io = new DataIO(
    // factories for supported data formats:
    new CsvSerializationFactory(
        // configurations:
        new CsvConvertersSerializationConfiguration(
            // specify all custom converters:
            new MyCustomDataCsvConverter(),
        ),
        new CsvHeaderToLowerCaseSerializationConfiguration()
    ),
    new JsonSerializationFactory(
        new JsonConvertersSerializationConfiguration(
            new MyCustomDataJsonConverter()
        )
    )
);
```


### Dynamic configuration

The last argument of `DataIO::ReadInput` and `DataIO:WriteOutput` methods is always `object settings` (optional) which is passed to configurations to the last argument of the `Configure` method defined by the `ISerializationConfiguration` interfafce. This way, you can dynamically alter configurations.


### Support for IObservable\<T\> with Reactive Extensions

`UXI.Serialization.Reactive` is an extension to the main serialization library. It adds `DataIORx` class with additional methods for consuming and producing observable streams of data for serialization. 

Example with observable data streams:

```csharp
using System.Reactive;
using System.Reactive.Linq;
using UXI.Serialization;
using UXI.Serialization.Reactive;

// initialize DataIO with supported serialization formats and converters
DataIO io = // same as for example with enumerable data

IObservable<Data> input = io.ReadInputAsObservable("path/to/input.csv", FileFormat.CSV);

// apply operators from the Reactive Extensions library
var data = input.Where(i => i.Name.StartsWith("User_"))
                .Select(i => i.Score);

IObservable<int> output = io.WriteOutput(data, "path/to/output.csv", FileFormat.CSV);

output.Subscribe();
```



## Installation

UXI.Serialization libraries are available as NuGet packages in the public Azure DevOps artifacts repository for all UXIsk packages:
```
https://pkgs.dev.azure.com/uxifiit/Packages/_packaging/Public/nuget/v3/index.json
```

### Add UXIsk Packages to package sources
First, add a new package source. Choose the way that fits you the best:
* Add new package source in [Visual Studio settings](https://docs.microsoft.com/en-us/azure/devops/artifacts/nuget/consume?view=azure-devops).
* Add new package source from command line:
```
nuget source Add -Name "UXIsk Packages" -Source "https://pkgs.dev.azure.com/uxifiit/Packages/_packaging/Public/nuget/v3/index.json"
```
* Create or edit `NuGet.config` file in your project's solution directory where you specify this package source:
```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="UXIsk Packages" value="https://pkgs.dev.azure.com/uxifiit/Packages/_packaging/Public/nuget/v3/index.json" />
    <!-- other package sources -->
  </packageSources>
  <disabledPackageSources />
</configuration>
```

### Install packages

Use the Visual Studio "Manage NuGet Packages..." window or the Package Manager Console:
```
PM> Install-Package UXI.Serialization
```

```
PM> Install-Package UXI.Serialization.Reactive
```



## Author

* Martin Konôpka - [@martinkonopka](https://github.com/martinkonopka)



## License

Projects in this repository are licensed under the MIT License - see [LICENSE.txt](LICENSE.txt).



## Contacts

* UXIsk
  * User eXperience and Interaction Research Center
  * Faculty of Informatics and Information Technologies, Slovak University of Technology in Bratislava
  * Web: https://www.uxi.sk/
* Martin Konopka
  * E-mail: martin (underscore) konopka (at) stuba (dot) sk
