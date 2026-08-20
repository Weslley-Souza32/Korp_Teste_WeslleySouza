namespace Korp.Billing.Tests.Infrastructure
{
    [CollectionDefinition("BillingDatabase")]
    public class BillingDatabaseCollection
        : ICollectionFixture<BillingDatabaseFixture>
    {
    }
}