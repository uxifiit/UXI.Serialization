# UXIsk Data Serialization Library

[![Build Status](https://dev.azure.com/uxifiit/UXI.Libs/_apis/build/status/uxifiit.UXI.Serialization?branchName=master)](https://dev.azure.com/uxifiit/UXI.Libs/_build/latest?definitionId=4&branchName=master) [![UXI.Serialization package in Public feed in Azure Artifacts](https://feeds.dev.azure.com/uxifiit/875e4574-b18a-49ff-8cf1-55b220af2355/_apis/public/Packaging/Feeds/f25beb4b-f7d5-4466-9073-a54052469941/Packages/2c64bb35-acf0-42b1-94ee-ce7253791bb1/Badge)](https://dev.azure.com/uxifiit/UXI.Libs/_packaging?_a=package&feed=f25beb4b-f7d5-4466-9073-a54052469941&package=2c64bb35-acf0-42b1-94ee-ce7253791bb1&preferRelease=true)

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

UXI.Serialization libraries are available as NuGet packages in a public Azure DevOps artifacts repository shared with [UXI.Libs](https://github.com/uxifiit/UXI.Libs):
```
https://pkgs.dev.azure.com/uxifiit/UXI.Libs/_packaging/Public/nuget/v3/index.json
```

### Add uxifiit/UXI.Libs package source

First, add a new package source. Choose the way that fits you the best:

* Add new package source in [Visual Studio settings](https://docs.microsoft.com/en-us/azure/devops/artifacts/nuget/consume?view=azure-devops).
* Add new package source with the repository URL through command line:
```
nuget source Add -Name "UXI.Libs Public Feed" -Source "https://pkgs.dev.azure.com/uxifiit/UXI.Libs/_packaging/Public/nuget/v3/index.json"
```
* Create `NuGet.config` file in your project's solution directory where you add this package source:

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="UXI.Libs Public Feed" value="https://pkgs.dev.azure.com/uxifiit/UXI.Libs/_packaging/Public/nuget/v3/index.json" />
  </packageSources>
  <disabledPackageSources />
</configuration>
```


### Install UXI.Serialization packages

Then install the packages to your project using the Visual Studio "Manage NuGet Packages..." window or use the Package Manage Console:
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
