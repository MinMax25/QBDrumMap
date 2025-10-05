using System.ComponentModel.DataAnnotations;
using QBDrumMap.Class.MapModels;

namespace QBDrumMap.Class.CustomValidations
{
    public class ValidateKitPitchAttribute
        : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext?.ObjectInstance is not KitPitch kitPitch) return ValidationResult.Success;

            string str = $"{value}";

            if (str.Length < 0 || str.Length > ExModel.NAME_MAX_LENGTH)
            {
                return new ValidationResult(string.Format(Properties.Resources.LenghtLessLThan, Properties.Name.KitPitch, ExModel.NAME_MAX_LENGTH));
            }

            return ValidationResult.Success;
        }
    }
}
