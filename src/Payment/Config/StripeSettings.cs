using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payment.Config
{
    public class StripeSettings
    {
        public string PublishableKey { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public string WebhookSecret { get; set; } = default!;
    }
}