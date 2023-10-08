using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Validations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfMatchConditionAttribute : RequiredAttribute
    {
        private string PropertyName { get; set; }
        private bool Condition { get; set; }

        public RequiredIfMatchConditionAttribute(string propertyName,bool condition)
        {
            PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            object instance = context.ObjectInstance;
            Type type = instance.GetType();

            bool.TryParse(type.GetProperty(PropertyName).GetValue(instance)?.ToString(), out bool propertyValue);

            if (propertyValue==Condition && string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
