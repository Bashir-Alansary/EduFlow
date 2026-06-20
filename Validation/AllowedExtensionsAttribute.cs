using System.ComponentModel.DataAnnotations;

namespace EduFlow.Validation
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var file = value as IFormFile;

            if (file == null)
                return new ValidationResult("Invalid file.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!_extensions.Contains(extension))
            {
                return new ValidationResult(
                    $"Only the following extensions are allowed: {string.Join(", ", _extensions)}"
                );
            }

            return ValidationResult.Success;
        }
    }
}
