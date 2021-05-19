using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Common.Configurations
{
    public class AuthConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }

        public int? LifeTime { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new(Encoding.UTF8.GetBytes(Secret));
        }
    }
}
