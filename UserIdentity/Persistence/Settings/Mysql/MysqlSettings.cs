namespace UserIdentity.Persistence.Settings.Mysql
{
	public class MysqlSettings
	{
		public String? Host { get; set; }

		public String? Port { get; set; }

		public String? Database { get; set; }

		public String? UserName { get; set; }

		public String? Password { get; set; }

		public String ConnectionString(IConfiguration configuration) => ($"Server={Host};Database={Database};User={UserName};Password={Password};")
			.Replace("DB_SERVER", configuration.GetValue<String>("DB_SERVER"))
            .Replace("DB_PORT", configuration.GetValue<String>("DB_PORT"))
            .Replace("DB_NAME", configuration.GetValue<String>("DB_NAME"))
            .Replace("DB_USER", configuration.GetValue<String>("DB_USER"))
			.Replace("DB_PASSWORD", configuration.GetValue<String>("DB_PASSWORD"));
	}
}
