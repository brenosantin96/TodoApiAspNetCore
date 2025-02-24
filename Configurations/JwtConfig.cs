﻿namespace ApiBreno01.Configurations
{
    public class JwtConfig
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int TokenValidityMins { get; set; }

    }
}
