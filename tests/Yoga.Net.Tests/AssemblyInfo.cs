// Disable parallel test execution because the Event system uses static state
// that would cause cross-test interference when tests run concurrently.
// This mirrors the C++ test behavior where tests run sequentially.
[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]
