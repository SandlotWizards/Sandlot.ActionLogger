namespace Sandlot.ActionLogger.Models
{
    /// <summary>
    /// Represents the result of a validation operation, including success state and message.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Indicates whether the validation was successful.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Optional message describing the validation result.
        /// </summary>
        public string? Message { get; }

        private ValidationResult(bool isValid, string? message)
        {
            IsValid = isValid;
            Message = message;
        }

        /// <summary>
        /// Creates a successful validation result.
        /// </summary>
        public static ValidationResult Success() => new(true, null);

        /// <summary>
        /// Creates a failed validation result with a descriptive message.
        /// </summary>
        /// <param name="message">The reason the validation failed.</param>
        public static ValidationResult Fail(string message) => new(false, message);
    }
}
