﻿namespace AuthEx.Shared.Security
{
    public static class SecurityConstants
    {
        public const string CookieName = ".AuthEx.Identity";
        public const string ApplicationName = "AuthEx";
        public const string AuthenticationScheme = "Identity.Application";
        public const string JwtScheme = "Jwt.Auth";
        public const string OidcScheme = "OpenID.Connect";

        public static readonly string[] Modules = new[]
        {
            "Home",
            "Security",
            "Mvc",
        };
    }
}
