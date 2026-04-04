# Yoga.Net

[![NuGet](https://img.shields.io/nuget/v/Yoga.Net.svg)](https://www.nuget.org/packages/Yoga.Net)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A high-performance C# port of [Meta's Yoga](https://github.com/facebook/yoga) layout engine — implements **Flexbox** and **CSS Grid** layout for .NET.

Yoga enables building UI layouts using Flexbox and CSS Grid on platforms that don't natively support them, including .NET desktop, mobile, and server applications.

## Features

- **1:1 C++ port** — faithful translation of the original C++ Yoga engine (v3.2.1)
- **Flexbox & CSS Grid** — complete Flexbox algorithm and CSS Grid layout support
- **High performance** — zero reflection, zero LINQ, `Span<T>` optimizations, `AggressiveInlining` on hot paths, struct value types for reduced allocations
- **AOT/NativeAOT compatible** — fully trimming-safe, no runtime code generation
- **Multi-target** — supports `net8.0`, `net9.0`, `net10.0`
- **826 tests** — comprehensive test suite mirroring the original C++ gtest tests 1:1 (35 skipped tests match upstream C++ `GTEST_SKIP()`)
- Measure callbacks for integrating with text measurement
- Caching for layout performance
- Deterministic layout (no undefined behavior from rounding)

## Installation

```bash
dotnet add package Yoga.Net
```

## Quick Start

```csharp
using Facebook.Yoga;
using static Facebook.Yoga.YGNodeAPI;
using static Facebook.Yoga.YGNodeStyleAPI;
using static Facebook.Yoga.YGNodeLayoutAPI;

// Create nodes
var root = YGNodeNew();
var child0 = YGNodeNew();
var child1 = YGNodeNew();

// Build tree
YGNodeInsertChild(root, child0, 0);
YGNodeInsertChild(root, child1, 1);

// Set styles
YGNodeStyleSetWidth(root, 300);
YGNodeStyleSetHeight(root, 200);
YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);

YGNodeStyleSetFlexGrow(child0, 1);
YGNodeStyleSetFlexGrow(child1, 2);

// Calculate layout
YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

// Read results
Console.WriteLine($"Root:   {YGNodeLayoutGetWidth(root)} x {YGNodeLayoutGetHeight(root)}");
Console.WriteLine($"Child0: {YGNodeLayoutGetWidth(child0)} x {YGNodeLayoutGetHeight(child0)} @ ({YGNodeLayoutGetLeft(child0)}, {YGNodeLayoutGetTop(child0)})");
Console.WriteLine($"Child1: {YGNodeLayoutGetWidth(child1)} x {YGNodeLayoutGetHeight(child1)} @ ({YGNodeLayoutGetLeft(child1)}, {YGNodeLayoutGetTop(child1)})");

// Clean up
YGNodeFreeRecursive(root);
```

Output:
```
Root:   300 x 200
Child0: 100 x 200 @ (0, 0)
Child1: 200 x 200 @ (100, 0)
```

## Building from Source

```bash
# Build
dotnet build

# Run tests (xunit.v3 standalone runner)
dotnet run --project tests/Yoga.Net.Tests/Yoga.Net.Tests.csproj

# Pack NuGet
dotnet pack --configuration Release
```

## Benchmarks

Yoga.Net includes a benchmark suite to measure layout performance.

### Running Benchmarks

```bash
# Run JIT (non-AOT) benchmark
dotnet run --project tests/Yoga.Net.Benchmarks/Yoga.Net.Benchmarks.csproj --configuration Release -- --simple

# Run NativeAOT benchmark
dotnet publish tests/Yoga.Net.Benchmarks/AotBenchmark.csproj -c Release -r win-x64
./bin/Release/net10.0/win-x64/publish/AotBenchmark.exe

# Run full BenchmarkDotNet suite (requires capture files from yoga repo)
dotnet run --project tests/Yoga.Net.Benchmarks/Yoga.Net.Benchmarks.csproj --configuration Release
```

### JIT vs NativeAOT Performance

**Environment:**
- Runtime: .NET 10.0.5
- OS: Windows 11 (10.0.26200)
- CPU: 13th Gen Intel Core i9-13900HX
- AOT binary size: 1.4 MB

**Results (lower is better):**

| Test | JIT (ms/op) | JIT (ops/sec) | NativeAOT (ms/op) | NativeAOT (ops/sec) | AOT Speedup |
|------|-----------:|--------------:|-------------------:|--------------------:|:-----------:|
| Stack with flex (10 children) | 0.11 | 9,163 | 0.029 | 35,442 | **3.9x** |
| Align stretch (10 children) | 0.08 | 12,104 | 0.020 | 50,003 | **4.1x** |
| Simple layout (5 nodes) | 0.03 | 31,455 | 0.008 | 125,696 | **4.0x** |
| Row layout (10 children) | 0.07 | 16,158 | 0.016 | 61,613 | **3.8x** |

> NativeAOT delivers ~4x throughput improvement over JIT by eliminating runtime overhead, enabling aggressive cross-method inlining, and producing CPU-optimized native code.

## Project Structure

The project mirrors the original C++ source layout:

```
Yoga.Net/
├── src/Yoga.Net/              # Main library (namespace: Facebook.Yoga)
│   ├── algorithm/             # Core layout algorithms (FlexLine, CalculateLayout, PixelGrid...)
│   ├── config/                # Configuration (Config, ExperimentalFeature)
│   ├── debug/                 # Debug/assertion utilities
│   ├── enums/                 # Flexbox & Grid enumerations
│   ├── event/                 # Event system
│   ├── node/                  # Node implementation (Node, LayoutResults, CachedMeasurement)
│   ├── numeric/               # Numeric utilities (FloatOptional, Comparison)
│   ├── style/                 # Style properties (Style, StyleLength, StyleSizeLength)
│   ├── YGNode.cs              # Public C-style Node API
│   ├── YGNodeStyle.cs         # Public C-style Style API
│   ├── YGNodeLayout.cs        # Public C-style Layout API
│   ├── YGConfig.cs            # Public C-style Config API
│   └── YGEnums.cs             # Public YG-prefixed enums
└── tests/Yoga.Net.Tests/      # xUnit v3 tests (1:1 with C++ gtest)
```

## Performance Optimizations

Compared to a naive C# port, Yoga.Net includes the following optimizations:

| Optimization | Description |
|---|---|
| `[Flags]` enum bit fields | `ExperimentalFeature` uses bit flags instead of `HashSet<T>` |
| Value-type structs | `FlexLineRunningLayout`, `FloatOptional`, `CachedMeasurement` are structs |
| `IEquatable<T>` | Avoids boxing in equality checks for structs |
| `AggressiveInlining` | Hot-path methods (`FloatOptional`, `Comparison`, etc.) are inlined |
| `Span<T>` / `stackalloc` | Stack-allocated buffers in `LayoutableChildren.Iterator` and `Event.PublishCore` |
| Zero LINQ | No LINQ usage — all iterations are manual loops |
| Zero reflection | No `System.Reflection` usage — fully AOT compatible |

## API Style

The library exposes two layers:

1. **C-style API** (`YGNodeAPI`, `YGNodeStyleAPI`, `YGNodeLayoutAPI`) — mirrors the original C/C++ Yoga API for 1:1 test compatibility
2. **Internal C# types** (`Node`, `Config`, `Style`) — use idiomatic C# naming (PascalCase properties, methods, enums)

## Version Alignment

| Yoga.Net Version | Upstream C++ Yoga Version |
|---|---|
| 3.2.1 | [v3.2.1](https://github.com/facebook/yoga) |

## Acknowledgments

This is a C# port of [facebook/yoga](https://github.com/facebook/yoga) by Meta Platforms, Inc.

## License

[MIT](LICENSE)
