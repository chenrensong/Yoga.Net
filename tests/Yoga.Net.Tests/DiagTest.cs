using Facebook.Yoga;
using static Facebook.Yoga.YGNodeAPI;
using static Facebook.Yoga.YGNodeStyleAPI;
using static Facebook.Yoga.YGNodeLayoutAPI;

var config = new Config();
var root = YGNodeNewWithConfig(config);
YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
YGNodeStyleSetWidth(root, 100);
YGNodeStyleSetHeight(root, 100);

Console.WriteLine($"Style Dimension Width: {root.Style.Dimension(Dimension.Width)}");
Console.WriteLine($"Style Dimension Height: {root.Style.Dimension(Dimension.Height)}");

root.ProcessDimensions();
Console.WriteLine($"After ProcessDimensions:");
Console.WriteLine($"ProcessedDimension Width: {root.ProcessedDimension(Dimension.Width)}");
Console.WriteLine($"ProcessedDimension Height: {root.ProcessedDimension(Dimension.Height)}");
Console.WriteLine($"HasDefiniteLength Width (NaN): {root.HasDefiniteLength(Dimension.Width, float.NaN)}");
Console.WriteLine($"HasDefiniteLength Height (NaN): {root.HasDefiniteLength(Dimension.Height, float.NaN)}");

var direction = root.ResolveDirection(Direction.LTR);
Console.WriteLine($"Direction: {direction}");

var resolvedDim = root.GetResolvedDimension(direction, FlexDirection.Row.Dimension(), float.NaN, float.NaN);
Console.WriteLine($"ResolvedDimension Width: {resolvedDim}, IsDefined={resolvedDim.IsDefined()}, Unwrap={resolvedDim.Unwrap()}");

try {
    YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
    Console.WriteLine($"Layout Width: {YGNodeLayoutGetWidth(root)}");
    Console.WriteLine($"Layout Height: {YGNodeLayoutGetHeight(root)}");
    Console.WriteLine($"Layout Left: {YGNodeLayoutGetLeft(root)}");
    Console.WriteLine($"Layout Top: {YGNodeLayoutGetTop(root)}");
} catch (Exception e) {
    Console.WriteLine($"Exception: {e.GetType().Name}: {e.Message}");
    Console.WriteLine(e.StackTrace);
}
