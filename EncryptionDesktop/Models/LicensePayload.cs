using System;

namespace EncryptionDesktop.Models
{
    public class LicensePayload
    {
        public string Product { get; set; } = string.Empty;
        public string LicenseId { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public DateTime IssuedAtUtc { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
    }
}
