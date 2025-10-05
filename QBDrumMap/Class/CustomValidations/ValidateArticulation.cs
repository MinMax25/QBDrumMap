using System.ComponentModel.DataAnnotations;
using QBDrumMap.Class.Extentions;
using QBDrumMap.Class.MapModels;

namespace QBDrumMap.Class.CustomValidations
{
    public class ValidateArticulation
        : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext?.ObjectInstance is not Articulation articulation) return ValidationResult.Success;

            string str = $"{value}";

            if (str.Length < 1 || str.Length > ExModel.NAME_MAX_LENGTH)
            {
                return new ValidationResult(string.Format(Properties.Resources.LengthBetween, Properties.Name.Articulation, 1, ExModel.NAME_MAX_LENGTH));
            }

            if (UtilMap.MapData.Parts.SelectMany(x => x.Articulations).Where(x => x.ID != articulation.ID && x.Name == articulation.Name).Any())
            {
                return new ValidationResult(string.Format(Properties.Resources.DuplicateNaem, $"{value}"));
            }

            return ValidationResult.Success;
        }
    }
}
