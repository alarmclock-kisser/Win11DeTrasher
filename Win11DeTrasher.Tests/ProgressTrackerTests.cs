using Win11DeTrasher.Models;
using Win11DeTrasher.Services;

namespace Win11DeTrasher.Tests;

/// <summary>
/// Unit tests for <see cref="ProgressTracker"/>.
/// </summary>
[TestClass]
public sealed class ProgressTrackerTests : TestBase
{
    [TestInitialize]
    public void Setup()
    {
        // TestContext is automatically injected by MSTest
    }

    [TestMethod]
    public void Start_ShouldResetAllCounters()
    {
        var tracker = new ProgressTracker();
        tracker.Start();

        var progress = tracker.CurrentProgress;

        progress.TotalBytes.ShouldBe(0);
        progress.ProcessedBytes.ShouldBe(0);
        progress.TotalFiles.ShouldBe(0);
        progress.ProcessedFiles.ShouldBe(0);
        progress.ErrorCount.ShouldBe(0);
        progress.IsCompleted.ShouldBeFalse();
        progress.IsCancelled.ShouldBeFalse();
        tracker.IsStarted.ShouldBeTrue();
    }

    [TestMethod]
    public void ReportDiscovered_ShouldIncrementTotalCounters()
    {
        var tracker = new ProgressTracker();
        tracker.Start();

        tracker.ReportDiscovered(10, 1000);
        tracker.ReportDiscovered(20, 2000);

        var progress = tracker.CurrentProgress;

        progress.TotalFiles.ShouldBe(30);
        progress.TotalBytes.ShouldBe(3000);
    }

    [TestMethod]
    public void ReportProcessed_ShouldIncrementProcessedCounters()
    {
        var tracker = new ProgressTracker();
        tracker.Start();

        tracker.ReportDiscovered(100, 100000);
        tracker.ReportProcessed(50, 50000);

        var progress = tracker.CurrentProgress;

        progress.ProcessedFiles.ShouldBe(50);
        progress.ProcessedBytes.ShouldBe(50000);
        progress.ProgressPercent.ShouldBe(50.0, 0.1);
    }

    [TestMethod]
    public void ReportError_ShouldIncrementErrorCount()
    {
        var tracker = new ProgressTracker();
        tracker.Start();

        tracker.ReportError();
        tracker.ReportError();
        tracker.ReportError();

        tracker.CurrentProgress.ErrorCount.ShouldBe(3);
    }

    [TestMethod]
    public void Complete_ShouldSetIsCompletedAndProgressTo100()
    {
        var tracker = new ProgressTracker();
        tracker.Start();
        tracker.ReportDiscovered(100, 1000);
        tracker.ReportProcessed(100, 1000);

        tracker.Complete();

        var progress = tracker.CurrentProgress;
        progress.IsCompleted.ShouldBeTrue();
        progress.ProgressPercent.ShouldBe(100.0, 0.1);
        progress.EstimatedRemainingSeconds.ShouldBe(0, 0.1);
    }

    [TestMethod]
    public void Cancel_ShouldSetIsCancelled()
    {
        var tracker = new ProgressTracker();
        tracker.Start();
        tracker.ReportDiscovered(100, 1000);

        tracker.Cancel();

        tracker.CurrentProgress.IsCancelled.ShouldBeTrue();
    }

    [TestMethod]
    public void Reset_ShouldReturnToInitialState()
    {
        var tracker = new ProgressTracker();
        tracker.Start();
        tracker.ReportDiscovered(100, 1000);
        tracker.Complete();

        tracker.Reset();

        var progress = tracker.CurrentProgress;
        progress.TotalBytes.ShouldBe(0);
        progress.ProcessedBytes.ShouldBe(0);
        progress.IsCompleted.ShouldBeFalse();
        progress.IsCancelled.ShouldBeFalse();
        tracker.IsStarted.ShouldBeFalse();
    }

    [TestMethod]
    public void ProgressUpdated_ShouldBeRaisedOnCompletion()
    {
        var tracker = new ProgressTracker();
        tracker.Start();

        var raised = false;
        tracker.ProgressUpdated += _ => raised = true;

        tracker.Complete();

        raised.ShouldBeTrue();
    }

    [TestMethod]
    public void ReportDiscovered_WhenNotStarted_ShouldBeIgnored()
    {
        var tracker = new ProgressTracker();
        // Not started — no Start() call

        tracker.ReportDiscovered(10, 1000);

        tracker.CurrentProgress.TotalFiles.ShouldBe(0);
        tracker.CurrentProgress.TotalBytes.ShouldBe(0);
    }
}
