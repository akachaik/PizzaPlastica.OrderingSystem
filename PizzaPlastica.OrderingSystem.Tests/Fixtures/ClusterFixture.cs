﻿using Orleans.Serialization;
using Orleans.TestingHost;

namespace PizzaPlastica.OrderingSystem.Tests.Fixtures;

public class ClusterFixture : IDisposable
{
    public ClusterFixture()
    {
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurations>();

        Cluster = builder.Build();
        Cluster.Deploy();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Cleanup
        Cluster.StopAllSilos();
    }

    public TestCluster Cluster { get; }
}

public class TestSiloConfigurations : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        // PUT HERE SERVICE DEPENDENCIES...

        siloBuilder.Services.AddSerializer(serializerBuilder =>
        {
            serializerBuilder.AddNewtonsoftJsonSerializer(
                isSupported: type => type.Namespace?.StartsWith("PizzaPlastica.OrderingSystem.Abstractions") ?? false);
        });
        siloBuilder.AddMemoryGrainStorage("table-order-storage");
        siloBuilder.UseInMemoryReminderService();  // add this line to support reminders
    }
}