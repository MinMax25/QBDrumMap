using System.ComponentModel.DataAnnotations;
using QBDrumMap.Class.Extentions;
using QBDrumMap.Class.MapModels;

namespace QBDrumMap.Class.CustomValidations
{
    public class ValidatePartAttribute
        : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext?.ObjectInstance is not Part part) return ValidationResult.Success;

            string str = $"{value}";

            if (str.Length < 1 || str.Length > ExModel.NAME_MAX_LENGTH)
            {
                return new ValidationResult(string.Format(Properties.Resources.LengthBetween, Properties.Name.Part, 1, ExModel.NAME_MAX_LENGTH));
            }

            if (UtilMap.MapData.Parts.Where(x => x.ID != part.ID && x.Name == part.Name).Any())
            {
                return new ValidationResult(string.Format(Properties.Resources.DuplicateNaem, $"{value}"));
            }

            return ValidationResult.Success;
        }
    }
}
