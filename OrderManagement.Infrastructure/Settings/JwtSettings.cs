using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Infrastructure.Settings
{
    public sealed class JwtSettings
    {
        public const string SectionName = "Jwt";

        public string Key { get; init; } = null!;

        public string Issuer { get; init; } = null!;

        public string Audience { get; init; } = null!;

        public int ExpirationMinutes { get; init; }
    }
}
