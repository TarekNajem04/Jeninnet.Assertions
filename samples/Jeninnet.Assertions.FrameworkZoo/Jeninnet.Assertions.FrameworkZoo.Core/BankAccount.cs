namespace Jeninnet.Assertions.FrameworkZoo.Core;

/// <summary>
/// A deliberately small bank account that gives the sample tests something
/// stateful to exercise: opening balances, deposits, withdrawals, and the
/// failure raised when a withdrawal exceeds the available balance.
/// </summary>
public sealed class BankAccount {
    /// <summary>Initializes a new account with the specified opening balance.</summary>
    /// <param name="openingBalance">The starting balance; must not be negative.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the opening balance is negative.</exception>
    public BankAccount(decimal openingBalance) {
        if(openingBalance < 0m) {
            throw new ArgumentOutOfRangeException(nameof(openingBalance), "The opening balance must not be negative.");
        }

        Balance = openingBalance;
    }

    /// <summary>Gets the current balance of the account.</summary>
    public decimal Balance { get; private set; }

    /// <summary>Adds an amount to the current balance.</summary>
    /// <param name="amount">The amount to deposit; must be positive.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the amount is not positive.</exception>
    public void Deposit(decimal amount) {
        if(amount <= 0m) {
            throw new ArgumentOutOfRangeException(nameof(amount), "The deposit amount must be positive.");
        }

        Balance += amount;
    }

    /// <summary>Subtracts an amount from the current balance.</summary>
    /// <param name="amount">The amount to withdraw; must be positive and must not exceed the balance.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the amount is not positive.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the amount exceeds the current balance.</exception>
    public void Withdraw(decimal amount) {
        if(amount <= 0m) {
            throw new ArgumentOutOfRangeException(nameof(amount), "The withdrawal amount must be positive.");
        }

        if(amount > Balance) {
            throw new InvalidOperationException("The account balance is insufficient for this withdrawal.");
        }

        Balance -= amount;
    }
}
