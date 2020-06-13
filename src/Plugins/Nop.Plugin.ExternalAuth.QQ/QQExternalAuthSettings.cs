using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.QQ
{
    /// <summary>
    /// Represents settings of the Facebook authentication method
    /// </summary>
    public class QQExternalAuthSettings : ISettings
    {
        /// <summary>
        /// Gets or sets OAuth2 client identifier
        /// </summary>
        public string ClientKeyIdentifier { get; set; }

        /// <summary>
        /// Gets or sets OAuth2 client secret
        /// </summary>
        public string ClientSecret { get; set; }
    }
}
