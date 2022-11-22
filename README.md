# HalfLife Unified SDK CSharp tools

Tools for the [Half-Life Unified SDK](https://github.com/SamVanheer/halflife-unified-sdk) written in C#.

Note: the Unified SDK itself is written in C++.

You will need to install the NET 6 runtime or newer to run the tools included in this repository.
You can download the runtime here (included with the NET SDK): https://dotnet.microsoft.com/en-us/download

Uses ImageSharp to handle image encoding: https://github.com/SixLabors/ImageSharp

## Install using NuGet

[![Nuget](https://img.shields.io/nuget/v/HalfLife.UnifiedSdk.Utilities?color=fcba03&label=HalfLife.UnifiedSdk.Utilities&logo=nuget)](https://www.nuget.org/packages/HalfLife.UnifiedSdk.Utilities/)

## HalfLife.UnifiedSdk.Utilities

This library contains utility functionality for opening, analyzing, modifying, converting and upgrading Half-Life 1 maps made for the GoldSource engine.

A list of Valve-made games can be accessed through the `HalfLife.UnifiedSdk.Utilities.Games.ValveGames` class.
A set of `GameInfo` objects provides basic information about each game, such as the engine the game runs on, the mod directory its content is installed in and a list of official maps.

A map can be loaded into a `HalfLife.UnifiedSdk.Utilities.Maps.Map` object. You can load vanilla Half-Life 1 maps as well as Blue Shift maps. This provides basic information about the map such as whether it is a map source file (`.rmf`, `.map`) or a compiled map file (`.bsp`, `.ent`), the file name and base name (used in `trigger_changelevel`, console commands and the `Create Server` dialog).

It also provides access to the map's entity list through the `Entities` property. This is an `HalfLife.UnifiedSdk.Utilities.Entities.EntityList` object containing the list of all entities in the map. 

Entity objects are of the type `HalfLife.UnifiedSdk.Utilities.Entities.Entity` and can be manipulated in various ways. All entities except `worldspawn` can be removed from a map and have their class name changed.

All entities can have their keyvalues added, removed and modified. New entities can be created and added to a map (it is not possible to add entities that the mod the map is played with does not support).

Helper methods exist to simplify certain tasks. For example the contents of a map can be replaced with those of another using the `EntityListExtensions.ReplaceWith` LINQ method. This behavior is identical to what Ripent does.

The `HalfLife.UnifiedSdk.Utilities.Tools` namespace provides various tools for the automation of tasks:
* `MapFormats`: Provides methods to load maps into memory, automatically detecting the format based on file extension. Direct access to serializers is also provided.
* `KeyValueUtilities`: Provides constants for commonly used keyvalues as well as methods for identifying values.
* `ModUtilities`: Provides methods to enumerate mods in a GoldSource engine installation and loading the `liblist.gam` file.
* `ParsingUtilities`: Provides methods for converting between strings and commonly used data types using locale-independent conversions.
* `SteamUtilities`: Provides methods to query Steam registry values on Windows.
* `Ripent`: Provides methods to import and export `.ent` files the way Ripent does. This is a convenience wrapper around lower level methods that can perform Ripent functionality in-memory.
* `UpgradeTool`: This namespace provides classes to help automate the upgrading of maps and map source files from older versions of a game to newer versions.

The [`Sledge.Formats.Bsp` and `Sledge.Formats.Map`](https://github.com/LogicAndTrick/sledge-formats) libraries are used to load Half-Life 1 maps and map source files. Many thanks to Daniel Walder for creating these libraries.

## Documentation

The documentation for these tools can be found in the main Unified SDK repository: https://github.com/SamVanheer/halflife-unified-sdk/blob/master/docs/README.md#tools

# LICENSE

See [LICENSE](/LICENSE) for the MIT license
See [ImageSharp_LICENSE](/ImageSharp_LICENSE) for the Apache License, Version 2.0
