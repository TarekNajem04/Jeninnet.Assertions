using Jeninnet.Assertions;
using Jeninnet.Assertions.FrameworkZoo.Core;
using Xunit;

namespace Jeninnet.Assertions.FrameworkZoo.Xunit.Tests;

public class BankAccountTests {
    private readonly BankAccount _account = new(100m);

    [Fact]
    public void An_account_starts_with_its_opening_balance() =>
        _account.Balance.Should().Be(100m).And.BeGreaterThan(0m);

    [Fact]
    public void Deposits_increase_the_balance() {
        _account.Deposit(50m);

        _account.Balance.Should().Be(150m, "a deposit adds its amount to the opening balance");
    }

    [Fact]
    public void Withdrawals_decrease_the_balance() {
        _account.Withdraw(40m);

        _account.Balance.Should().Be(60m);
    }

    [Fact]
    public void Withdrawing_more_than_the_balance_fails() {
        var withdraw = () => _account.Withdraw(1_000m);

        withdraw.Should().Throw<InvalidOperationException>().WithMessage("*insufficient*");
    }

    [Fact]
    public void Independent_checks_are_reported_together() {
        using(new AssertionScope("the opening account")) {
            _account.Balance.Should().Be(100m);
            _account.Balance.Should().BeGreaterThan(0m);
            _account.Balance.Should().BeLessThan(1_000m);
        }
    }

    [Fact]
    public void The_balance_is_compared_structurally() =>
        _account.Should().BeEquivalentTo(new { Balance = 100m });
}
