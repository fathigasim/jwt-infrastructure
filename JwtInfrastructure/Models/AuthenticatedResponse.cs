﻿namespace JwtInfrastructure.Models
{
    public class AuthenticatedResponse
    {
        //public User User { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
