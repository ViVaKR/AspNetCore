namespace ViVaBM.API.Helpers;

public static class AuthSettings
{
    public static string Audience { get; } = "https://localhost:55521/";

    public static string Issuer { get; } = "https://localhost:55521/";

    public static int RefreshTokenValidityIn { get; } = 60;

    public static string PrivateKey { get; } = "-";
}
