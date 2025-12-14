namespace PetCafe.Application;

public class AppSettings
{
    public ConnectionStrings ConnectionStrings { get; set; } = default!;
    public JWTOptions JWTOptions { get; set; } = default!;
    public FirebaseSettings FirebaseSettings { get; set; } = default!;
    public SepayOptions SepayOptions { get; set; } = default!;
    public string GoongAPIKey { get; set; } = default!;
    public PayOSConfig PayOSConfig { get; set; } = default!;
    public SmtpSettings SmtpSettings { get; set; } = default!;


}

public class ConnectionStrings
{
    public string PostgreConnection { get; set; } = default!;
    public string MongoDbConnection { get; set; } = default!;
}

public class SepayOptions
{
    public string BankId { get; set; } = default!;
    public string AccountNo { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
}

public class JWTOptions
{
    public string SecretKey { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
}

public class FirebaseSettings
{
    public string SenderId { get; set; } = default!;
    public string ServerKey { get; set; } = default!;
    public string ApiKeY { get; set; } = default!;
    public string Bucket { get; set; } = default!;
    public string AuthEmail { get; set; } = default!;
    public string AuthPassword { get; set; } = default!;
}


public class PayOSConfig
{
    public string ClientId { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
    public string ChecksumKey { get; set; } = default!;
    public string SuccessURL { get; set; } = default!;
    public string CancelURL { get; set; } = default!;
}

public class SmtpSettings
{
    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string FromEmail { get; set; } = default!;
    public string FromName { get; set; } = default!;
    public bool EnableSsl { get; set; } = true;
}