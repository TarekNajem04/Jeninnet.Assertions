using Jeninnet.Assertions;
using Jeninnet.Assertions.FrameworkZoo.Core;
using NUnit.Framework;

namespace Jeninnet.Assertions.FrameworkZoo.NUnit.Tests;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class BankAccountTests {
    private readonly BankAccount _account = new(100m);

    [Test]
    public void An_account_starts_with_its_opening_balance() =>
        _account.Balance.Should().Be(100m).And.BeGreaterThan(0m);

    [Test]
    public void Deposits_increase_the_balance() {
        _account.Deposit(50m);

        _account.Balance.Should().Be(150m, "a deposit adds its amount to the opening balance");
    }

    [Test]
    public void Withdrawals_decrease_the_balance() {
        _account.Withdraw(40m);

        _account.Balance.Should().Be(60m);
    }

    [Test]
    public void Withdrawing_more_than_the_balance_fails() {
        var withdraw = () => _account.Withdraw(1_000m);

        withdraw.Should().Throw<InvalidOperationException>().WithMessage("*insufficient*");
    }

    [Test]
    public void Independent_checks_are_reported_together() {
        using(new AssertionScope("the opening account")) {
            _account.Balance.Should().Be(100m);
            _account.Balance.Should().BeGreaterThan(0m);
            _account.Balance.Should().BeLessThan(1_000m);
        }
    }

    [Test]
    public void The_balance_is_compared_structurally() =>
        _account.Should().BeEquivalentTo(new { Balance = 100m });
}
