//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class EventDepthTests {
    private sealed class Publisher {
        public event EventHandler? Changed;

        public event EventHandler? ValueSet;

        public void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);

        public void RaiseValueSet() => ValueSet?.Invoke(this, EventArgs.Empty);
    }

    [TestMethod]
    public void RaiseOnce_PassesOnlyForASingleRaise() {
        var publisher = new Publisher();
        using var monitor = publisher.Monitor();
        publisher.RaiseChanged();

        var occurrence = monitor.Should().RaiseOnce("Changed").Which;
        Assert.AreEqual("Changed", occurrence.EventName);

        var empty = new Publisher();
        using var emptyMonitor = empty.Monitor();
        var zero = Assert.ThrowsExactly<AssertionFailedException>(() => emptyMonitor.Should().RaiseOnce("Changed"));
        Assert.AreEqual("Expected emptyMonitor to raise event 'Changed' exactly once, but it was raised 0 time(s).", zero.Message);

        publisher.RaiseChanged();
        var twice = Assert.ThrowsExactly<AssertionFailedException>(() => monitor.Should().RaiseOnce("Changed"));
        Assert.AreEqual("Expected monitor to raise event 'Changed' exactly once, but it was raised 2 time(s).", twice.Message);
    }

    [TestMethod]
    public void RaiseTimes_SupportsEveryOccurrenceConstraint() {
        var publisher = new Publisher();
        using var monitor = publisher.Monitor();
        publisher.RaiseChanged();
        publisher.RaiseChanged();

        monitor.Should().RaiseTimes("Changed", Exactly.Twice());
        monitor.Should().RaiseTimes("Changed", AtLeast.Once());
        monitor.Should().RaiseTimes("Changed", AtMost.Twice());
        monitor.Should().RaiseTimes("Changed", MoreThan.Once());
        monitor.Should().RaiseTimes("Changed", LessThan.Thrice());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => monitor.Should().RaiseTimes("Changed", Exactly.Once()));
        Assert.AreEqual("Expected monitor to raise event 'Changed' exactly once, but it was raised 2 time(s).", ex.Message);
    }

    [TestMethod]
    public void HaveRecordedEvents_ExposesASnapshot() {
        var publisher = new Publisher();
        using var monitor = publisher.Monitor();
        publisher.RaiseChanged();
        publisher.RaiseValueSet();

        var events = monitor.Should().HaveRecordedEvents().Which;
        Assert.HasCount(2, events);
        Assert.AreEqual("Changed", events[0].EventName);
        Assert.AreEqual("ValueSet", events[1].EventName);

        monitor.Clear();
        monitor.Should().HaveNoRecordedEvents();
    }

    [TestMethod]
    public void HaveNoRecordedEvents_FailsWithCount() {
        var publisher = new Publisher();
        using var monitor = publisher.Monitor();
        publisher.RaiseChanged();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => monitor.Should().HaveNoRecordedEvents());

        Assert.AreEqual("Expected monitor to have no recorded events, but found 1.", ex.Message);
    }

    [TestMethod]
    public void RecordedTimestamp_UsesTheAssertionClock() {
        using(AssertionIsolation.Isolate()) {
            AssertionClock.Current = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var publisher = new Publisher();
            using var monitor = publisher.Monitor();

            publisher.RaiseChanged();

            Assert.AreEqual(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), monitor.OccurredEvents[0].Timestamp);
        }
    }

    [TestMethod]
    public void EventMonitorOptions_IncludeAndExcludeAtSubscription() {
        var including = new Publisher();
        using(var monitor = including.Monitor(new EventMonitorOptions().IncludingEvents(nameof(Publisher.Changed)))) {
            including.RaiseChanged();
            including.RaiseValueSet();

            var events = monitor.Should().HaveRecordedEvents().Which;
            Assert.HasCount(1, events);
            Assert.AreEqual("Changed", events[0].EventName);
        }

        var excluding = new Publisher();
        using(var monitor = excluding.Monitor(new EventMonitorOptions().ExcludingEvents(nameof(Publisher.Changed)))) {
            excluding.RaiseChanged();
            excluding.RaiseValueSet();

            var events = monitor.Should().HaveRecordedEvents().Which;
            Assert.HasCount(1, events);
            Assert.AreEqual("ValueSet", events[0].EventName);
        }

        var combined = new Publisher();
        var options = new EventMonitorOptions()
            .IncludingEvents(nameof(Publisher.Changed), nameof(Publisher.ValueSet))
            .ExcludingEvents(nameof(Publisher.Changed));
        using(var monitor = combined.Monitor(options)) {
            combined.RaiseChanged();
            combined.RaiseValueSet();

            var events = monitor.Should().HaveRecordedEvents().Which;
            Assert.HasCount(1, events);
            Assert.AreEqual("ValueSet", events[0].EventName);
        }
    }

    [TestMethod]
    public void EventMonitorOptions_RejectInvalidNames() {
        var options = new EventMonitorOptions();

        Assert.ThrowsExactly<ArgumentNullException>(() => options.IncludingEvents(null!));
        Assert.ThrowsExactly<ArgumentException>(() => options.ExcludingEvents(""));
        Assert.ThrowsExactly<ArgumentException>(() => options.IncludingEvents("Changed", null!));
    }

    [TestMethod]
    public void ConcurrentRaise_RecordsEveryEvent() {
        var publisher = new Publisher();
        using var monitor = publisher.Monitor();

        Parallel.For(0, 500, _ => publisher.RaiseChanged());

        Assert.HasCount(500, monitor.OccurredEvents);
    }
}
