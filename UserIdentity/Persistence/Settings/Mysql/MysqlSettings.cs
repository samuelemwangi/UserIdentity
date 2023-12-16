namespace UserIdentity.Persistence.Settings.Mysql
{
	public class MysqlSettings
	{
		public string? Host { get; set; }

		public string? Port { get; set; }

		public string? Database { get; set; }

		public string? UserName { get; set; }

		public string? Password { get; set; }

		public string ConnectionString(IConfiguration configuration) => ($"Server={Host};Port={Port};Database={Database};User={UserName};Password={Password};")
			.Replace("DB_SERVER", configuration.GetValue<string>("DB_SERVER"))
						.Replace("DB_PORT", configuration.GetValue<string>("DB_PORT"))
						.Replace("DB_NAME", configuration.GetValue<string>("DB_NAME"))
						.Replace("DB_USER", configuration.GetValue<string>("DB_USER"))
			.Replace("DB_PASSWORD", configuration.GetValue<string>("DB_PASSWORD"));
	}
}
