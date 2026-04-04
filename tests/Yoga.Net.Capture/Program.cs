using Facebook.Yoga;
using Yoga.Net.Capture;

// Build a simple tree to demonstrate the capture tool
var config = YGConfigAPI.YGConfigNew();
var root = YGNodeAPI.YGNodeNewWithConfig(config);

YGNodeStyleAPI.YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);
YGNodeStyleAPI.YGNodeStyleSetWidth(root, 100);
YGNodeStyleAPI.YGNodeStyleSetHeight(root, 100);

var child0 = YGNodeAPI.YGNodeNewWithConfig(config);
YGNodeStyleAPI.YGNodeStyleSetWidth(child0, 50);
YGNodeStyleAPI.YGNodeStyleSetHeight(child0, 50);

var child1 = YGNodeAPI.YGNodeNewWithConfig(config);
YGNodeStyleAPI.YGNodeStyleSetWidth(child1, 50);
YGNodeStyleAPI.YGNodeStyleSetHeight(child1, 50);

YGNodeAPI.YGNodeInsertChild(root, child0, 0);
YGNodeAPI.YGNodeInsertChild(root, child1, 1);

// Capture the tree
Console.WriteLine("Yoga.Net Capture Tool - Demo");
Console.WriteLine("=============================");
Console.WriteLine();

string json = CaptureTree.CalculateLayoutWithCapture(
    root, float.NaN, float.NaN, YGDirection.LTR);

Console.WriteLine(json);

// Also write to file if path provided
if (args.Length > 0)
{
    CaptureTree.CalculateLayoutWithCaptureToFile(
        root, float.NaN, float.NaN, YGDirection.LTR, args[0]);
    Console.WriteLine();
    Console.WriteLine($"Written to: {args[0]}");
}

YGNodeAPI.YGNodeFreeRecursive(root);
YGConfigAPI.YGConfigFree(config);
