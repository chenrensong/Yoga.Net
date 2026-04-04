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
    private string _captureName = null!;
    private JsonElement _captureData;
    private string _jsonText = null!;

    [Params("chat-mac", "feed-android", "profile-ios", "rendering-sample-mac")]
    public string CaptureFile { get; set; } = null!;

    [GlobalSetup]
    public void Setup()
    {
        _captureName = CaptureFile;
        var capturesDir = Path.Combine(AppContext.BaseDirectory, "Captures");
        var filePath = Path.Combine(capturesDir, $"{_captureName}.json");

        // Fallback: try to find from yoga repo
        if (!File.Exists(filePath))
        {
            var yogaCapturesDir = Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..", "..", "yoga", "benchmark", "captures");
            filePath = Path.GetFullPath(Path.Combine(yogaCapturesDir, $"{_captureName}.json"));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Capture file not found: {filePath}");
        }

        _jsonText = File.ReadAllText(filePath);
        var options = new JsonDocumentOptions { MaxDepth = 256 };
        _captureData = JsonDocument.Parse(_jsonText, options).RootElement;
    }

    [Benchmark(Description = "Tree Creation")]
    public Node TreeCreation()
    {
        // Re-parse JSON for fair comparison with C++
        var options = new JsonDocumentOptions { MaxDepth = 256 };
        var capture = JsonDocument.Parse(_jsonText, options).RootElement;
        return TreeDeserializer.BuildTree(capture.GetProperty("tree"));
    }

    [Benchmark(Description = "Layout Calculation")]
    public Node LayoutCalculation()
    {
        var options = new JsonDocumentOptions { MaxDepth = 256 };
        var capture = JsonDocument.Parse(_jsonText, options).RootElement;
        var root = TreeDeserializer.BuildTree(capture.GetProperty("tree"));

        var layoutInputs = capture.GetProperty("layout-inputs");
        float availableWidth = layoutInputs.GetProperty("available-width").GetSingle();
        float availableHeight = layoutInputs.GetProperty("available-height").GetSingle();
        var direction = ParseDirection(layoutInputs.GetProperty("owner-direction").GetString()!);

        YGNodeCalculateLayout(root, availableWidth, availableHeight, direction);
        return root;
    }

    private static YGDirection ParseDirection(string str)
    {
        return str switch
        {
            "ltr" => YGDirection.LTR,
            "rtl" => YGDirection.RTL,
            "inherit" => YGDirection.Inherit,
            _ => throw new ArgumentException($"Invalid direction: {str}")
        };
    }
}
