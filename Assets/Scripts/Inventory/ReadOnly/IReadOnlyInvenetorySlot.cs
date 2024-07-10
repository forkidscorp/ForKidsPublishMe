using System;

namespace Inventory
{
    public interface IReadOnlyInvenetorySlot
    {
        event Action<string> ItemIdChanged;
        event Action<int> ItemAmountChanged;

        string ItemId { get; }
        int Amount { get; }
        bool IsEmpty { get; }
    }
}
