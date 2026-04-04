using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Facebook.Yoga;
using static Facebook.Yoga.YGNodeAPI;
using static Facebook.Yoga.YGNodeStyleAPI;

namespace Yoga.Net.Benchmarks;

[SimpleJob(launchCount: 1, warmupCount: 5, iterationCount: 100)]
[MemoryDiagnoser]
public class CaptureBenchmark
{
    private JsonDocument _captureDoc = null!;
    private JsonElement _captureData;

    [Params("chat-mac", "feed-android", "profile-ios", "rendering-sample-mac")]
    public string CaptureFile { get; set; } = null!;

    [GlobalSetup]
    public void Setup()
    {
        var capturesDir = Path.Combine(AppContext.BaseDirectory, "Captures");
        var filePath = Path.Combine(capturesDir, $"{CaptureFile}.json");

        // Fallback: try to find from yoga repo
        if (!File.Exists(filePath))
        {
            var yogaCapturesDir = Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..", "..", "yoga", "benchmark", "captures");
            filePath = Path.GetFullPath(Path.Combine(yogaCapturesDir, $"{CaptureFile}.json"));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Capture file not found: {filePath}");
        }

        var jsonText = File.ReadAllText(filePath);
        var options = new JsonDocumentOptions { MaxDepth = 256 };
        // Keep JsonDocument alive so RootElement stays valid
        _captureDoc = JsonDocument.Parse(jsonText, options);
        _captureData = _captureDoc.RootElement;
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _captureDoc?.Dispose();
    }

    // Matches Benchmark.cpp:311-317 tree creation measurement
    // Parse once in GlobalSetup, only measure BuildTree here
    [Benchmark(Description = "Tree Creation")]
    public Node TreeCreation()
    {
        return TreeDeserializer.BuildTree(_captureData.GetProperty("tree"));
    }

    private Node _layoutTree = null!;

    // Build tree outside of measurement, only measure layout
    [IterationSetup(Target = nameof(LayoutCalculation))]
    public void SetupLayoutCalculation()
    {
        _layoutTree = TreeDeserializer.BuildTree(_captureData.GetProperty("tree"));
    }

    // Matches Benchmark.cpp:324-327 layout-only measurement
    [Benchmark(Description = "Layout Calculation")]
    public void LayoutCalculation()
    {
        var layoutInputs = _captureData.GetProperty("layout-inputs");
        float availableWidth = layoutInputs.GetProperty("available-width").GetSingle();
        float availableHeight = layoutInputs.GetProperty("available-height").GetSingle();
        var direction = TreeDeserializer.ParseDirection(
            layoutInputs.GetProperty("owner-direction").GetString()!);

        YGNodeCalculateLayout(_layoutTree, availableWidth, availableHeight, direction);
    }

    // Matches Benchmark.cpp:329-331 + 380-382 total measurement
    [Benchmark(Description = "Total")]
    public Node Total()
    {
        var tree = TreeDeserializer.BuildTree(_captureData.GetProperty("tree"));

        var layoutInputs = _captureData.GetProperty("layout-inputs");
        float availableWidth = layoutInputs.GetProperty("available-width").GetSingle();
        float availableHeight = layoutInputs.GetProperty("available-height").GetSingle();
        var direction = TreeDeserializer.ParseDirection(
            layoutInputs.GetProperty("owner-direction").GetString()!);

        YGNodeCalculateLayout(tree, availableWidth, availableHeight, direction);
        return tree;
    }
}
