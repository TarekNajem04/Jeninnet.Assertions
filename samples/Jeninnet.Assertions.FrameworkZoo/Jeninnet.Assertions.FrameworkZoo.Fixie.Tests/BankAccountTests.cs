using Jeninnet.Assertions;
using Jeninnet.Assertions.FrameworkZoo.Core;

namespace Jeninnet.Assertions.FrameworkZoo.Fixie.Tests;

public class BankAccountTests {
    private readonly BankAccount _account = new(100m);

    public void An_account_starts_with_its_opening_balance() =>
        _account.Balance.Should().Be(100m).And.BeGreaterThan(0m);

    public void Deposits_increase_the_balance() {
        _account.Deposit(50m);

        _account.Balance.Should().Be(150m, "a deposit adds its amount to the opening balance");
    }

    public void Withdrawals_decrease_the_balance() {
        _account.Withdraw(40m);

        _account.Balance.Should().Be(60m);
    }

    public void Withdrawing_more_than_the_balance_fails() {
        var withdraw = () => _account.Withdraw(1_000m);

        withdraw.Should().Throw<InvalidOperationException>().WithMessage("*insufficient*");
    }

    public void Independent_checks_are_reported_together() {
        using(new AssertionScope("the opening account")) {
            _account.Balance.Should().Be(100m);
            _account.Balance.Should().BeGreaterThan(0m);
            _account.Balance.Should().BeLessThan(1_000m);
        }
    }

    public void The_balance_is_compared_structurally() =>
        _account.Should().BeEquivalentTo(new { Balance = 100m });
}
