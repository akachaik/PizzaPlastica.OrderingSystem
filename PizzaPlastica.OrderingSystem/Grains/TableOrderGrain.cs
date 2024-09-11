using PizzaPlastica.OrderingSystem.Abstractions;
using PizzaPlastica.OrderingSystem.Exceptions;
using System.Collections.ObjectModel;

namespace PizzaPlastica.OrderingSystem.Grains;

public class TableOrderState
{
    [Id(0)]
    public bool IsOpen { get; set; }

    [Id(1)]
    public List<TableOrderItem> OrderItems { get; set; } = [];
}

public class TableOrderGrain : Grain, ITableOrderGrain, IRemindable
{

    private readonly IPersistentState<TableOrderState> _state;

    private IGrainReminder? _reminder = null;

#pragma warning disable IDE0290 // Use primary constructor
    public TableOrderGrain(
#pragma warning restore IDE0290 // Use primary constructor
        [PersistentState("table-order", "table-order-storage")]
        IPersistentState<TableOrderState> state)
    {
        _state = state;
    }

    public async Task OpenTableOrder()
    {
        if (_state.State.IsOpen)
        {
            throw new InvalidStateException("Table has already opened.");
        }

        this._state.State.IsOpen = true;

        _state.State.OrderItems = [];

        _reminder = await this.RegisterOrUpdateReminder(reminderName: "TableOrderExpired",
            dueTime: TimeSpan.FromMinutes(2),
            period: TimeSpan.FromMinutes(3));

        await _state.WriteStateAsync();

    }

    public async Task CloseTableOrder()
    {
        if (!_state.State.IsOpen)
        {
            throw new InvalidStateException("Table has already closed.");
        }

        _state.State.IsOpen = false;
        _state.State.OrderItems = [];

        if (_reminder is not null)
        {
            await this.UnregisterReminder(_reminder);
            _reminder = null;
        }

        await _state.WriteStateAsync();
    }

    public async Task<Guid> AddOrderItem(string name, double cost, int quantity)
    {
        if (!_state.State.IsOpen)
        {
            throw new InvalidStateException("Table should be opened.");
        }

        var orderItemId = Guid.NewGuid();
        _state.State.OrderItems.Add(new TableOrderItem
        {
            Id = orderItemId,
            Name = name,
            Cost = cost,
            Quantity = quantity
        });

        await _state.WriteStateAsync();

        return orderItemId;
    }

    public Task<TableOrderItem> GetOrderItemDetails(Guid orderItemId)
    {
        var orderItemDetails = _state.State.OrderItems.Single(x => x.Id == orderItemId);
        return Task.FromResult(orderItemDetails);
    }

    public Task<ReadOnlyCollection<TableOrderItem>> GetOrderItems()
    {
        return Task.FromResult(_state.State.OrderItems.AsReadOnly());
    }

    public async Task RemoveOrderItem(Guid orderItemId)
    {
        var orderItem = _state.State.OrderItems.Single(x => x.Id == orderItemId);
        _state.State.OrderItems.Remove(orderItem);

        await _state.WriteStateAsync();

    }

    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        return reminderName switch
        {
            "TableOrderExpired" => CloseTableOrder(),
            _ => Task.CompletedTask
        };
    }
}
