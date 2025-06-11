namespace Sandlot.ActionLogger.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string? Message { get; }

        private ValidationResult(bool isValid, string? message)
        {
            IsValid = isValid;
            Message = message;
        }

        public static ValidationResult Success() => new(true, null);
        public static ValidationResult Fail(string message) => new(false, message);
    }
}
