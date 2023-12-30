using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Common.ConfigurationHelpers
{
    public class JwtBearerConfig
    {
        public string IssuerSigningKey { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public string ValidAudiences { get; set; }
        public bool ValidateAudience { get; set; }
        public string ValidIssuer {  get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateLifetime { get; set; }
        public int ExpirationTime { get; set; }
        public int RefreshTokenExpirationTime { get; set; }
    }
}
