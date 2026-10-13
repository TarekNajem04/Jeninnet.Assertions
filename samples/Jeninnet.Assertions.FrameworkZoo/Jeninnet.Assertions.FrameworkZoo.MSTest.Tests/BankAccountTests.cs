using Jeninnet.Assertions;
using Jeninnet.Assertions.FrameworkZoo.Core;
using Jeninnet.Assertions.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jeninnet.Assertions.FrameworkZoo.MSTest.Tests;

[TestClass]
public class BankAccountTests {
    private readonly BankAccount _account = new(100m);

    [TestMethod]
    public void An_account_starts_with_its_opening_balance() =>
        MSTestAssertions.That(() => _account.Balance.Should().Be(100m).And.BeGreaterThan(0m));

    [TestMethod]
    public void Deposits_increase_the_balance() {
        _account.Deposit(50m);

        MSTestAssertions.That(() => _account.Balance.Should().Be(150m, "a deposit adds its amount to the opening balance"));
    }

    [TestMethod]
    public void Withdrawals_decrease_the_balance() {
        _account.Withdraw(40m);

        MSTestAssertions.That(() => _account.Balance.Should().Be(60m));
    }

    [TestMethod]
    public void Withdrawing_more_than_the_balance_fails() {
        var withdraw = () => _account.Withdraw(1_000m);

        MSTestAssertions.That(() => withdraw.Should().Throw<InvalidOperationException>().WithMessage("*insufficient*"));
    }

    [TestMethod]
    public void Independent_checks_are_reported_together() =>
        MSTestAssertions.That(() => {
            using(new AssertionScope("the opening account")) {
                _account.Balance.Should().Be(100m);
                _account.Balance.Should().BeGreaterThan(0m);
                _account.Balance.Should().BeLessThan(1_000m);
            }
        });

    [TestMethod]
    public void The_balance_is_compared_structurally() =>
        MSTestAssertions.That(() => _account.Should().BeEquivalentTo(new { Balance = 100m }));
}
