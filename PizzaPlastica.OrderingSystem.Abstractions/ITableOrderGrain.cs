using Orleans;
using Orleans.Concurrency;
using System.Collections.ObjectModel;

namespace PizzaPlastica.OrderingSystem.Abstractions;

[Alias("PizzaPlastica.OrderingSystem.Abstractions.ITableOrderGrain")]
public interface ITableOrderGrain : IGrainWithGuidCompoundKey
{
    [Alias("OpenTableOrder")]
    Task OpenTableOrder();

    [Alias("CloseTableOrder")]
    Task CloseTableOrder();

    [Alias("AddOrderItem")]
    Task<Guid> AddOrderItem(string name, double cost, int quantity);

    [Alias("RemoveOrderItem")]
    Task RemoveOrderItem(Guid orderItemId);

    [ReadOnly]
    [Alias("GetOrderItemDetails")]
    Task<TableOrderItem> GetOrderItemDetails(Guid orderItemId);

    [ReadOnly]
    [Alias("GetOrderItems")]
    Task<ReadOnlyCollection<TableOrderItem>> GetOrderItems();
}
