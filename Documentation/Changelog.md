# What's New in Gapotchenko.FX

## 2021

### Gapotchenko FX 2021.2

Release date: December 31, 2021

- Introduced `Gapotchenko.FX.Math.Topology` module that provides `Graph<T>` type and accompanying primitives including topological sorting
- New `FileSystem.PathEquivalenceComparer` property returns the file path string comparer that takes into account path normalization and equivalence rules of the host environment
- New `AssociativeArray` key/value map primitive which is similar to `Dictionary<TKey, TValue>` but handles the whole space of key values including `null`
- New string metric functions: `DamerauLevenshteinDistance`, `HammingDistance`, `JaroDistance`, `LcsDistance`, `OsaDistance`
- Added `PriorityQueue` polyfill
- Added `IEnumerable<byte> AsEnumerable()` polyfill for `System.IO.Stream`
- Added `LongIndexOf` LINQ polyfill
- Added `TryGetNonEnumeratedCount` LINQ polyfill
- Improved support for .NET 6.0 target framework
- Improved `AssemblyAutoLoader` which can now work simultaneously with app domains and assembly load contexts
- String metric functions now take an optional `maxDistance` parameter that limits computational resources required to calculate the distance

### Gapotchenko FX 2021.1

Release date: July 6, 2021

- Added support for .NET 6.0 target framework
- Introduced `Gapotchenko.FX.Memory` module
- Introduced `Gapotchenko.FX.Math.Geometry` module
- Introduced `Gapotchenko.FX.Math.Combinatorics` module
- Added `MathEx.Clamp` function that clamps a value to the specified range
- Added `MathEx.Lerp` function that performs linear interpolation between two values by the specified coefficient
- Added `AppInformation.For(assembly)` static function that retrieves app information for a specified assembly
- Added LINQ function that simultaneously determines whether any elements of a sequence satisfy the specified conditions (`(bool, bool) IEnumerable<T>.Any(Func<T, bool> predicate1, Func<T, bool> predicate2)` with higher dimensional overloads)
- Added `ConsoleEx.ReadPassword` function for reading a password from the console
- Added `System.Runtime.CompilerServices.ModuleInitializerAttribute` polyfill
- Added `SkipLast` and `TakeLast` LINQ polyfills
- Added `System.Collections.Generic.ReferenceEqualityComparer` polyfill
- Added `MemberNotNullAttribute` and `MemberNotNullWhenAttribute` nullability annotation polyfills
- Added `GetValueOrDefault` polyfills for `IReadOnlyDictionary<TKey, TValue>`
- Added `TryAdd` and `Remove(key, out value)` polyfills for `IDictionary<TKey, TValue>`
- Added `System.Numerics.BitOperations.PopCount(ulong)` polyfill
- Improved performance of a thread-safe LINQ memoization
- Fixed nullability annotations for `MathEx.Min` and `MathEx.Max` functions
- Fixed nullability annotations for `LazyInitializerEx` class
- Fixed issue with UTF-8 BOM encoding returned by `CommandLine.OemEncoding` on Windows when system locale is set to UTF-8
  (cmd.exe cannot consume UTF-8 with BOM)
- Fixed potential thread safety issues that could occur on architectures with weaker memory models

## 2020

### Gapotchenko.FX 2020.1

Release date: November 5, 2020

- Added support for .NET 5.0 target framework
- Introduced `Gapotchenko.FX.AppModel.Information` module that allows to programmatically retrieve information about the app
- Introduced `Gapotchenko.FX.Console` module that provides virtual terminal functionality, console traits and a powerful `MoreTextWriter` primitive for improved user friendliness of your console apps
- Added nullability annotations for public module APIs
- Added `System.MathF` polyfill
- Added polyfills for nullable annotation attributes
- Added polyfill for init-only property setters
- Added `Fn.Ignore(value)` function that ignores a specified value for languages that do not have a built-in equivalent. A typical usage is to fire and forget a parallel `Task` without producing a compiler warning
- Added `Process.GetImageFileName()` method that allows to retrieve the file name of a running process without security limitations imposed by the host OS
- Added `bool ISet.AddRange(collection)` extension method that allows to add elements to a set in bulk
- Added `CommandLine.EscapeArgument` and `CommandLine.EscapeFileName` methods
- .NET Framework 4.0 target is retired. The minimal supported .NET Framework version is 4.5
- Fixed issue with ambiguous match of `IsNullOrEmpty` polyfill method of `HashSet<T>` type that occurred in .NET 4.6+, .NET Standard 2.0+ and .NET Core 2.0+ target frameworks
- Fixed issue with ambiguous match of `ToHashSet` polyfill method of `IEnumerable<T>` type that occurred in .NET 4.7.2+ target frameworks
- Fixed issue in `Process.ReadEnvironmentVariables()` method with environment variable blocks longer than 32,768 bytes
- Fixed issue in `WebBrowser.Launch(url)` method that might cause an erroneous interpretation of `?` and `&` URL symbols by a web browser on Windows hosts

## 2019

### Gapotchenko.FX 2019.3

Release date: November 4, 2019

- Added support for .NET Core 3.0 target framework
- Introduced `Sequential` and `DebuggableParallel` primitives in `Gapotchenko.FX.Threading` module.
Both primitives constitute drop-in replacements for `System.Threading.Tasks.Parallel` and are useful for debugging purposes
- Added `CommandLine.OemEncoding` property that gets OEM encoding used by Windows command line and console applications
- Added ability to create LINQ expressions from functions via `Gapotchenko.FX.Fn` primitive
- Added `IndexOf(IEnumerable<T> source, IEnumerable<T> value)` and `IndexOf(IEnumerable<T> source, IEnumerable<T> value, IEqualityComparer<T> comparer)` LINQ methods
- Implemented polyfill for `Enumerable.ToHashSet<T>` operation
- Implemented polyfills for `BitConverter.SingleToInt32Bits` and `Int32BitsToSingle` operations
- Implemented polyfills to the future for `WaitForExit(CancellationToken)` and `WaitForExit(int, CancellationToken)` methods of `System.Diagnostics.Process` type
- Implemented polyfill for `SwitchExpressionException`
- `Empty.Task` is not suggested by the code editor when it is natively provided by the host platform
- Fixed issue with ambiguous match of `Append` and `Prepend` polyfills for `IEnumerable<T>` type for some target frameworks
- Fixed issue with binding redirects handling in `Gapotchenko.FX.Reflection.Loader` module that could lead to `StackOverflowException` under specific conditions

### Gapotchenko.FX 2019.2

Release date: August 14, 2019

- Added support for .NET Standard 2.1 target framework
- Added `System.Numerics.BitOperations` polyfill with `Log2` and `PopCount` hardware-accelerated operations
- Added ability to nullify specific integer values in `Empty` functional primitive
- Added ability to canonicalize a file path with `FileSystem.CanonicalizePath` method. It works as follows: the alternative directory separators are replaced with native ones; the duplicate adjacent separators are removed
- Introduced `Gapotchenko.FX.Reflection.Loader` module. It provides a versatile `AssemblyAutoLoader` type that can be used to automatically find and load assembly dependencies in various dynamic scenarios
- Introduced `Gapotchenko.FX.Data.Linq` module. Currently it provides async operations for LINQ to SQL technology
- Introduced `Gapotchenko.FX.Runtime.InteropServices.MemoryOperations` class that provides highly-optimized block operations for memory
- Introduced intrinsic compiler for hardware-accelerated operations 
- Implemented `ProcessArchitecture` and `OSArchitecture` properties in `System.Runtime.InteropServices.RuntimeInformation` polyfill
- Improved wording of `ProgramExitException` message

### Gapotchenko.FX 2019.1

Release date: March 29, 2019

- First public release
- Improved wording in summary tags
- Fixed a critical omission with `TaskBridge` namespace
