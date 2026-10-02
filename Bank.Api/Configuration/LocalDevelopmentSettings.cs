namespace Bank.Api.Configuration;

public static class LocalDevelopmentSettings
{
    public const string Url = "http://localhost:5176";
    public const string ConnectionString =
        "Server=.\\SQLEXPRESS;Database=BankFormationDb;Trusted_Connection=True;TrustServerCertificate=True";

    public const string JwtSecret =
        "zvyy97M7Jj/JIJq7lW9bj9XjvSkqoiwSy6dpLNCUB5T2ZWo4L3GbsDiaKae3gT+mGC2MtV2JZWfApaBf001nrA==";
    public const string JwtIssuer = "Bank.Api";
    public const string JwtAudience = "Bank.Api.Client";
    public const int JwtExpirationMinutes = 120;

    public const string AdminEmail = "admin@bank.local";
    public const string AdminPassword = "Admin123!";
    public const string AdminLastName = "Administrateur";
    public const string AdminFirstName = "Principal";
}
