using UnityEngine;

namespace Inventory
{
    public class InventoryGridView : MonoBehaviour
    {
        private IReadOnlyInventoryGrid _inventory;
        public void Setup(IReadOnlyInventoryGrid inventory) {
            _inventory = inventory;
            Print();
        }
        public void Print()
        {
            var slots = _inventory.GetSlots();
            var size = _inventory.Size;
            var result = "";
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var slot = slots[x, y];
                    result += $"Slot ({x}:{y}). Item: {slot.ItemId}, amount: {slot.Amount}\n";
                }
            }
            Debug.Log(result);

        }
    }
}
