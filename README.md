# UXI.Serialization

A library for serialization and deserialization of enumerable or observable data streams. Current supported formats for serialization are JSON and CSV using [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) and [CsvHelper](https://github.com/JoshClose/CsvHelper) libraries.

The base `UXI.Serialization` library supports writing and reading data as `IEnumerable<T>`, the extension library `UXI.Serialization.Reactive` adds support for `IObservable<T>`. 



## Usage

The main access point to serialization or deserialization is the `DataIO` class with `ReadInput` and `WriteOutput` methods:

```csharp
using UXI.Serialization;

// initialize DataIO with supported serialization formats
DataIO io = // see below

// reading data
IEnumerable<Data> input = io.ReadInput<Data>("path/to/input.csv", FileFormat.CSV, null);

foreach (var item in input) 
{
    // consume data...
}


// writing data
IEnumerable<Data> output = // get data for output

io.WriteOutput(output, "path/to/output.csv", FileFormat.CSV, null);
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

The last argument of `DataIO::ReadInput` and `DataIO:WriteOutput` methods is always `object settings` which is passed to configurations. This way you can dynamically alter configurations. These settings are passed to the last argument of `Configure` method defined by the `ISerializationConfiguration` interface.


### Support for IObservable\<T\> with Reactive Extensions

`UXI.Serialization.Reactive` is an extension to the main serialization library. It adds `DataIORx` class with additional methods for consuming and producing observable streams of data for serialization. 



## Installation

UXI.Serialization libraries are available as NuGet packages in a public Azure DevOps artifacts repository.


### Add uxifiit/UXI.Serialization package source

First, add a new package source. Choose the way that fits you the best:

* Add new package source in [Visual Studio settings](https://docs.microsoft.com/en-us/azure/devops/artifacts/nuget/consume?view=azure-devops).
* Add new package source with the repository URL through command line:
```
nuget source Add -Name "UXI.Serialization Public Feed" -Source "https://pkgs.dev.azure.com/uxifiit/UXI.Serialization/_packaging/Public/nuget/v3/index.json"
```
* Create `NuGet.config` file in your project's solution directory where you add this package source:

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="UXI.Serialization Public Feed" value="https://pkgs.dev.azure.com/uxifiit/UXI.Serialization/_packaging/Public/nuget/v3/index.json" />
  </packageSources>
  <disabledPackageSources />
</configuration>
```


### Install UXI.Serialization packages

Then install a package to your project using the Visual Studio "Manage NuGet Packages..." window or use Package Manage Console:
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