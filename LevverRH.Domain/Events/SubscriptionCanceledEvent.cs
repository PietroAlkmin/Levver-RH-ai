using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevverRH.Domain.Events
{
    public class SubscriptionCanceledEvent
    {
        public Guid SubscriptionId { get; }
        public Guid TenantId { get; }
        public DateTime DataCancelamento { get; }

        public SubscriptionCanceledEvent(Guid subscriptionId, Guid tenantId)
        {
            SubscriptionId = subscriptionId;
            TenantId = tenantId;
            DataCancelamento = DateTime.UtcNow;
        }
    }
}
