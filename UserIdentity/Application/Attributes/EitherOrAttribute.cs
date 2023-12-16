using System.ComponentModel.DataAnnotations;

namespace UserIdentity.Application.Attributes
{
	public class EitherOrAttribute : ValidationAttribute
	{
		private readonly string _firstProperty;
		private readonly string _secondProperty;

		public EitherOrAttribute(string firstProperty, string secondProperty)
		{
			_firstProperty = firstProperty;
			_secondProperty = secondProperty;
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			var firstPropertyValue = validationContext.ObjectType.GetProperty(_firstProperty)?.GetValue(validationContext.ObjectInstance);
			var secondPropertyValue = validationContext.ObjectType.GetProperty(_secondProperty)?.GetValue(validationContext.ObjectInstance);

			if ((firstPropertyValue == null || string.IsNullOrWhiteSpace(firstPropertyValue.ToString())) &&
					(secondPropertyValue == null || string.IsNullOrWhiteSpace(secondPropertyValue.ToString())))
			{
				return new ValidationResult($"Either {_firstProperty} or {_secondProperty} must be provided.");
			}

			return ValidationResult.Success;
		}
	}
}
