using System;

namespace Rewind.One.AuthServer.Entities
{
    public class AppUserLogin
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string ProviderName { get; set; }
        public string ProviderKey { get; set; }
    }
}
