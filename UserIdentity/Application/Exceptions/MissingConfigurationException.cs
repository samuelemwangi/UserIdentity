namespace UserIdentity.Application.Exceptions
{
	public class MissingConfigurationException : Exception
	{
		public MissingConfigurationException(string configItem) : base(configItem + ": Configuration for item - " + configItem + " - is invalid")
		{
		}
	}
}
