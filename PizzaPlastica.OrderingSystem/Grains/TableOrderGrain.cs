using Orleans;
using Orleans.Runtime;
using PizzaPlastica.OrderingSystem.Abstractions;
using PizzaPlastica.OrderingSystem.Exceptions;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PizzaPlastica.OrderingSystem.Grains;

public class TableOrderGrain : Grain, ITableOrderGrain
{
    private bool IsOpen { get; set; }
    private List<TableOrderItem> OrderItems { get; set; } = new();

    public Task OpenTableOrder()
    {
        if (IsOpen)
        {
            throw new InvalidStateException("Table has already opened.");
        }

        this.IsOpen = true;

        OrderItems = new List<TableOrderItem>();

        return Task.CompletedTask;
    }

    public Task CloseTableOrder()
    {
        if (!IsOpen)
        {
            throw new InvalidStateException("Table has already closed.");
        }

        IsOpen = false;
        OrderItems = new List<TableOrderItem>();

        return Task.CompletedTask;
    }

    public Task<Guid> AddOrderItem(string name, double cost, int quantity)
    {
        if (!IsOpen)
        {
            throw new InvalidStateException("Table should be opened.");
        }

        var orderItemId = Guid.NewGuid();
        OrderItems.Add(new TableOrderItem
        {
            Id = orderItemId,
            Name = name,
            Cost = cost,
            Quantity = quantity
        });

        return Task.FromResult(orderItemId);
    }

    public Task<TableOrderItem> GetOrderItemDetails(Guid orderItemId)
    {
        var orderItemDetails = OrderItems.Single(x => x.Id == orderItemId);
        return Task.FromResult(orderItemDetails);
    }

    public Task<ReadOnlyCollection<TableOrderItem>> GetOrderItems()
    {
        return Task.FromResult(OrderItems.AsReadOnly());
    }

    public Task RemoveOrderItem(Guid orderItemId)
    {
        var orderItem = OrderItems.Single(x => x.Id == orderItemId);
        OrderItems.Remove(orderItem);

        return Task.CompletedTask;
    }

}
