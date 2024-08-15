namespace ViVaBM.API.Helpers;

public static class AuthSettings
{
    public static string Audience { get; } = "https://localhost:50021/";

    public static string Issuer { get; } = "https://localhost:50021/";

    public static int RefreshTokenValidityIn { get; } = 10;

    public static string PrivateKey { get; } = "asdfhoiuwerhlhQEkjhkjIweOqsdfuqoweucZdsfuerewrjhlH";
}
