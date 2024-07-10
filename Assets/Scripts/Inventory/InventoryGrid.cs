using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class InventoryGrid : IReadOnlyInventoryGrid
    {
        public Vector2Int Size 
        {
            get => _data.Size;
            set 
            { 
                if (_data.Size != value)
                {
                    _data.Size = value;
                    SizeChanged?.Invoke(value);
                }
            }
        }

        public string OwnerId => _data.OwnreId;

        public event Action<Vector2Int> SizeChanged;
        public event Action<string, int> ItemsAdded;
        public event Action<string, int> ItemsRemoved;

        private readonly InventoryGridData _data;

        private readonly Dictionary<Vector2Int, InventorySlot> _slotsMap = new();
        public InventoryGrid(InventoryGridData data) 
        {
            _data = data;
            var size = data.Size;

            for (var x = 0; x < size.x; x++) {
                for (var y = 0; y < size.y; y++) {
                    var index = x * size.y + y;
                    var slotData = data.Slots[index];
                    var slot = new InventorySlot(slotData);
                    var position = new Vector2Int(x, y);

                    _slotsMap[position] = slot;
                }
            }
        }

        public AddItemsToInventoryGridResult AddItems(string itemId, int amount = 1) 
        {
            var remainingAmount = amount;
            var itemsAddedToSlotsWithSameItemsAmount = AddToSlotsWithSameItem(itemId, remainingAmount, out remainingAmount);

            if (remainingAmount <= 0)
            {
                return new AddItemsToInventoryGridResult(OwnerId, amount, itemsAddedToSlotsWithSameItemsAmount);
            }

            var itemsAddedToAvailableSlotsAmount = AddToFirstAvailableSlots(itemId, remainingAmount, out remainingAmount);
            var totalAddedItemsAmount = itemsAddedToSlotsWithSameItemsAmount + itemsAddedToAvailableSlotsAmount;

            return new AddItemsToInventoryGridResult(OwnerId, amount, totalAddedItemsAmount);
        }
        public AddItemsToInventoryGridResult AddItems(Vector2Int slotCoords, string itemId, int amount = 1) 
        {
            var slot = _slotsMap[slotCoords];
            var newValue = slot.Amount + amount;
            int itemsAddedAmount = 0;

            if (slot.IsEmpty) 
            {
                slot.ItemId = itemId;
            }

            var itemSlotCapasity = GetItemSlotCapasity(itemId);

            if (newValue > itemSlotCapasity)
            {
                var remainingItems = newValue - itemSlotCapasity;
                var itemsToAddAmount = itemSlotCapasity - slot.Amount;
                itemsAddedAmount += itemsToAddAmount;
                slot.Amount = itemSlotCapasity;

                var result = AddItems(itemId, remainingItems);
                itemsAddedAmount += result.ItemsAddedAmount;
            }
            else 
            {
                itemsAddedAmount = amount;
                slot.Amount = newValue;
            }
            return new AddItemsToInventoryGridResult(OwnerId, amount, itemsAddedAmount);
        }
        public RemoveItemsFromInventoryGridResult RemoveItems(string itemId, int amount = 1) 
        {
            if (!Has(itemId, amount))
            {
                return new RemoveItemsFromInventoryGridResult(OwnerId, amount, false);
            }

            var amountToRemove = amount;

            for (var x = 0; x < Size.x; x++)
            {
                for (var y = 0; y < Size.y; y++)
                {
                    var slotCoords = new Vector2Int(x, y);
                    var slot = _slotsMap[slotCoords];

                    if (slot.ItemId != itemId)
                    {
                        continue;
                    }

                    if (amountToRemove > slot.Amount)
                    {
                        amountToRemove = slot.Amount;

                        RemoveItems(slotCoords, itemId, slot.Amount);
                    }
                    else
                    {
                        RemoveItems(slotCoords, itemId, amountToRemove);

                        return new RemoveItemsFromInventoryGridResult(OwnerId, amount, true);
                    }
                    
                }
            }

            throw new Exception("couldn't remove some items");
        }
        public RemoveItemsFromInventoryGridResult RemoveItems(Vector2Int slotCoords, string itemId, int amount = 1)
        { 
            var slot = _slotsMap[slotCoords];

            if (slot.IsEmpty || slot.ItemId != itemId || slot.Amount < amount) 
            {
                return new RemoveItemsFromInventoryGridResult(OwnerId, amount, false);
            }
            slot.Amount -= amount;

            if (slot.Amount == 0) 
            {
                slot.ItemId = null;
            }
            return new RemoveItemsFromInventoryGridResult(OwnerId, amount, true);
        }


        public int GetAmount(string itemId)
        {
            var amount = 0;
            var slots = _data.Slots;

            foreach (var slot in slots) {
                if (slot.ItemId == itemId)
                {
                    amount = slot.Amount;
                }
            }

            return amount;
        }

        public IReadOnlyInvenetorySlot[,] GetSlots()
        {
            var array = new IReadOnlyInvenetorySlot[Size.x, Size.y];
            for (var x = 0; x < Size.x; x++)
            {
                for (var y = 0; y < Size.y; y++)
                {
                    var position = new Vector2Int(x, y);
                    array[x, y] = _slotsMap[position];
                }
            }
            return array;
        }
        public bool Has(string itemId, int amount)
        {
            var amountExist = GetAmount(itemId);
            return amountExist >= amount;
        }

        public void SwitchSlots(Vector2Int slotCoordsA, Vector2Int slotCoordsB) 
        {
            var slotA = _slotsMap[slotCoordsA];
            var slotB = _slotsMap[slotCoordsB];
            var tempSlotItemId = slotA.ItemId;
            var tempSlotItemAmount = slotA.Amount;

            slotA.ItemId = slotB.ItemId;
            slotA.Amount = slotB.Amount;
            slotB.ItemId = tempSlotItemId;
            slotB.Amount = tempSlotItemAmount;
        }

        public void SetSize(Vector2Int newSize) { 
            throw new NotImplementedException();
        }

        private int AddToSlotsWithSameItem(string itemId, int amount, out int remainingAmount) {
            var itemsAddedAmount = 0;
            remainingAmount = amount;
            for (var x = 0; x < Size.x; x++)
            {
                for (var y = 0; y < Size.y; y++)
                {
                    var coords = new Vector2Int(x, y);
                    var slot = _slotsMap[coords];

                    if (slot.IsEmpty)
                    {
                        continue;
                    }

                    var slotItemCapacity = GetItemSlotCapasity(slot.ItemId);
                    if (slot.Amount >= slotItemCapacity) 
                    {
                        continue;
                    }

                    if (slot.ItemId != itemId)
                    {
                        continue;
                    }

                    var newValue = slot.Amount + remainingAmount;

                    if (newValue > slotItemCapacity)
                    {
                        remainingAmount = newValue - slotItemCapacity;
                        var itemsToAddAmount = slotItemCapacity - slot.Amount;
                        itemsAddedAmount += itemsToAddAmount;
                        slot.Amount = slotItemCapacity;

                        if (remainingAmount == 0)
                        {
                            return itemsAddedAmount;
                        }
                    }
                    else
                    {
                        itemsAddedAmount += remainingAmount;
                        slot.Amount = newValue;
                        remainingAmount = 0;

                        return itemsAddedAmount;
                    }
                }
            }
            return itemsAddedAmount;
        }

        private int AddToFirstAvailableSlots(string itemId, int amount, out int remainingAmount) 
        {
            var itemsAddedAmount = 0;
            remainingAmount = amount;

            for (var x = 0; x < Size.x; x++)
            {
                for (var y = 0; y < Size.y; y++)
                {
                    var coords = new Vector2Int(x, y);
                    var slot = _slotsMap[coords];

                    if (!slot.IsEmpty)
                    {
                        continue;
                    }

                    slot.ItemId = itemId;
                    var newValue = remainingAmount;
                    var slotItemCapacity = GetItemSlotCapasity(slot.ItemId);

                    if (newValue > slotItemCapacity)
                    {
                        remainingAmount = newValue - slotItemCapacity;
                        var itemsToAddAmount = slotItemCapacity;
                        itemsAddedAmount += itemsToAddAmount;
                        slot.Amount = slotItemCapacity;
                    }
                    else
                    {
                        itemsAddedAmount += remainingAmount;
                        slot.Amount = newValue;
                        remainingAmount = 0;

                        return itemsAddedAmount;
                    }
                }
            }
            return itemsAddedAmount;
        }
        private int GetItemSlotCapasity(string itemId) {
            return 99;
        }
    }
}
