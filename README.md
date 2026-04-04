# Yoga.Net

A C# port of [Facebook's Yoga](https://github.com/facebook/yoga) — a cross-platform layout engine that implements Flexbox.

Yoga enables building UI layouts using Flexbox on platforms that don't natively support it, including .NET desktop, mobile, and server applications.

## Status

This project is a work-in-progress C# translation of the original C++ Yoga engine, targeting .NET 10.

## Features (from original)

- Complete Flexbox algorithm implementation
- Measure callbacks for integrating with text measurement
- Caching for layout performance
- Deterministic layout (no undefined behavior from rounding)

## Getting Started

```bash
# Build
dotnet build

# Run tests
dotnet test

# Pack NuGet
dotnet pack
```

## Usage

```csharp
using Yoga;

var config = new YGConfig();
var root = YGNode.New(config);
var child = YGNode.New(config);

root.InsertChild(child, 0);
root.SetWidth(100);
root.SetHeight(100);

root.CalculateLayout();

Console.WriteLine($"Child width: {child.LayoutWidth}");
Console.WriteLine($"Child height: {child.LayoutHeight}");
```

## Project Structure

The project mirrors the original C++ source layout:

| Directory | Original (C++) | Description |
|-----------|----------------|-------------|
| `Algorithm/` | `yoga/algorithm/` | Core layout algorithms |
| `Config/` | `yoga/config/` | Configuration handling |
| `Debug/` | `yoga/debug/` | Debug utilities |
| `Enums/` | `yoga/enums/` | Flexbox enumerations |
| `Event/` | `yoga/event/` | Event system |
| `Node/` | `yoga/node/` | Node implementation |
| `Numeric/` | `yoga/numeric/` | Numeric utilities |
| `Style/` | `yoga/style/` | Style properties |

## Acknowledgments

This is a C# port of [facebook/yoga](https://github.com/facebook/yoga) by Meta Platforms, Inc.

## License

[MIT](LICENSE)
