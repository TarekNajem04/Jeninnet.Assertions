//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class EventTests {
    private sealed class ValueEventArgs(int value) : EventArgs {
        public int Value { get; } = value;
    }

    private sealed class Publisher {
        public event EventHandler? Changed;

        public event EventHandler<ValueEventArgs>? ValueSet;

        public void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);

        public void SetValue(int value) => ValueSet?.Invoke(this, new ValueEventArgs(value));
    }

    private sealed class Notifier : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _name = "";

        public string Name {
            get => _name;
            set {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
    }

#pragma warning disable S1144, S3237, CA1822
    // Fixture for the options test below: subscription happens through EventMonitor reflection,
    // so this event has no direct subscribers in test code. The throwing add documents the failure mode.
    private sealed class ThrowingAddPublisher {
        public event EventHandler? Broken {
            add => throw new InvalidOperationException("add");
            remove {
                // Detach is a no-op: the subscription never succeeds.
            }
        }

        public event EventHandler? Fine;

        public void RaiseFine() => Fine?.Invoke(this, EventArgs.Empty);
    }
#pragma warning restore S1144, S3237, CA1822

    [TestMethod]
    public void Raise_PassAndExposeOccurrence() {
        var publisher = new Publisher();
        using var monitor = publisher.Monitor();
        publisher.RaiseChanged();

        var occurrence = monitor.Should().Raise("Changed").Which;
        occurrence.Sender.Should().BeSameAs(publisher);
        Assert.AreEqual("Changed", occurrence.EventName);
    }

    [TestMethod]
    public void Raise_FailsWhenMissing() {
        var publisher = new Publisher();
        using var monitor = publisher.Monitor();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => monitor.Should().Raise("Changed"));
        Assert.AreEqual("Expected monitor to raise event 'Changed', but it was not raised.", ex.Message);
    }

    [TestMethod]
    public void NotRaise_PassAndFail() {
        var publisher = new Publisher();
        using var monitor = publisher.Monitor();
        monitor.Should().NotRaise("Changed");

        publisher.RaiseChanged();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => monitor.Should().NotRaise("Changed"));
        Assert.AreEqual("Expected monitor not to raise event 'Changed', but it was raised 1 time(s).", ex.Message);
    }

    [TestMethod]
    public void Raise_ExposesEventArgs() {
        var publisher = new Publisher();
        using var monitor = publisher.Monitor();
        publisher.SetValue(5);

        var occurrence = monitor.Should().Raise("ValueSet").Which;
        Assert.HasCount(1, occurrence.Args);
        occurrence.Args[0].Should().BeOfType<ValueEventArgs>();
        Assert.AreEqual(5, ((ValueEventArgs)occurrence.Args[0]!).Value);
    }

    [TestMethod]
    public void Monitor_NullSource_ThrowsArgumentNull() {
        object? source = null;
        Assert.ThrowsExactly<ArgumentNullException>(() => source.Monitor());
    }

    [TestMethod]
    public void Monitor_GenericEntry_MonitorsSource() {
        var publisher = new Publisher();
        using var monitor = publisher.Monitor<Publisher>();
        publisher.RaiseChanged();

        monitor.Should().Raise("Changed").WithSender(publisher);
    }

    [TestMethod]
    public void WithSender_PassAndFail() {
        var publisher = new Publisher();
        var other = new Publisher();
        using var monitor = publisher.Monitor();
        publisher.RaiseChanged();

        monitor.Should().Raise("Changed").WithSender(publisher);

        var occurrence = monitor.Should().Raise("Changed").WithSender(publisher).And.Which;
        Assert.AreSame(publisher, occurrence.Sender);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => monitor.Should().Raise("Changed").WithSender(other));
        Assert.Contains("to have sender", ex.Message);
        Assert.Contains("but found", ex.Message);
    }

    [TestMethod]
    public void WithArgs_SinglePredicate_PassAndFail() {
        var publisher = new Publisher();
        using var monitor = publisher.Monitor();
        publisher.SetValue(5);

        monitor.Should().Raise("ValueSet").WithArgs<ValueEventArgs>(static args => args.Value == 5);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => monitor.Should().Raise("ValueSet").WithArgs<ValueEventArgs>(static args => args.Value == 6));
        Assert.Contains("argument at index 0 to satisfy the predicate", ex.Message);
    }

    [TestMethod]
    public void WithArgs_MultiplePredicates_PassAndFail() {
        var publisher = new Publisher();
        using var monitor = publisher.Monitor();
        publisher.SetValue(5);

        monitor.Should().Raise("ValueSet").WithArgs(static arg => ((ValueEventArgs)arg!).Value == 5);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => monitor.Should().Raise("ValueSet").WithArgs(static _ => true, static _ => false));
        Assert.Contains("to have 2 argument(s), but found 1.", ex.Message);

        var mismatch = Assert.ThrowsExactly<AssertionFailedException>(
            () => monitor.Should().Raise("ValueSet").WithArgs(static arg => ((ValueEventArgs)arg!).Value == 6));
        Assert.Contains("argument at index 0 to satisfy the predicate", mismatch.Message);
    }

    [TestMethod]
    public void RaisePropertyChangeFor_PassAndFail() {
        var notifier = new Notifier();
        using var monitor = notifier.Monitor();
        notifier.Name = "a";

        monitor.Should().RaisePropertyChangeFor("Name").WithSender(notifier);
        monitor.Should().NotRaisePropertyChangeFor("Missing");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => monitor.Should().RaisePropertyChangeFor("Missing"));
        Assert.AreEqual("Expected monitor to raise property change for 'Missing', but it was not raised.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => monitor.Should().NotRaisePropertyChangeFor("Name"));
        Assert.AreEqual("Expected monitor not to raise property change for 'Name', but it was raised 1 time(s).", notEx.Message);
    }

    [TestMethod]
    public void Monitor_ThrowingAccessor_FailsWithoutOption() {
        var publisher = new ThrowingAddPublisher();
        var ex = Assert.ThrowsExactly<TargetInvocationException>(() => publisher.Monitor());
        Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
        Assert.ThrowsExactly<TargetInvocationException>(() => new EventMonitor(publisher));
    }

    [TestMethod]
    public void Monitor_IgnoringAccessorExceptions_SkipsBrokenEvent() {
        var publisher = new ThrowingAddPublisher();
        var options = new EventMonitorOptions().IgnoringEventAccessorExceptions();
        using var monitor = publisher.Monitor(options);
        publisher.RaiseFine();

        monitor.Should().Raise("Fine");
        monitor.Should().NotRaise("Broken");
    }

    [TestMethod]
    public void Monitor_Recordings_CanBeQueriedAndCleared() {
        var publisher = new Publisher();
        using var monitor = publisher.Monitor();
        publisher.RaiseChanged();
        publisher.SetValue(5);

        var recordings = monitor;
        AssertMonitorContract(recordings, publisher);
        monitor.Should().NotRaise("Changed");

        Assert.ThrowsExactly<ArgumentNullException>(() => recordings.GetRecordingFor(null!));
    }

    private static void AssertMonitorContract(EventMonitor recordings, Publisher publisher) {
        Assert.AreSame(publisher, recordings.Subject);
        Assert.HasCount(2, recordings.MonitoredEvents);
        Assert.HasCount(1, recordings.GetRecordingFor("Changed"));
        Assert.IsEmpty(recordings.GetRecordingFor("Missing"));

        recordings.Clear();
        Assert.IsEmpty(recordings.MonitoredEvents);
    }
}
