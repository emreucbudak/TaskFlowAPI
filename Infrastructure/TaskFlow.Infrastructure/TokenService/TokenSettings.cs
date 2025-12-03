using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Infrastructure.TokenService
{
    public class TokenSettings
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string SecretKey { get; set; }
        public int AccessTokenExpiryMinutes { get; set; }
    }
}
