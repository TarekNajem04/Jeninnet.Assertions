namespace Jeninnet.Assertions.Consumer;

/// <summary>The exception raised when the payment gateway declines a charge.</summary>
public sealed class PaymentDeclinedException : Exception {
    /// <summary>Initializes a new instance of the exception.</summary>
    public PaymentDeclinedException() {
    }

    /// <summary>Initializes a new instance of the exception with a message.</summary>
    /// <param name="message">The decline reason.</param>
    public PaymentDeclinedException(string message)
        : base(message) {
    }

    /// <summary>Initializes a new instance of the exception with a message and an inner exception.</summary>
    /// <param name="message">The decline reason.</param>
    /// <param name="innerException">The exception that caused the decline.</param>
    public PaymentDeclinedException(string message, Exception innerException)
        : base(message, innerException) {
    }
}
