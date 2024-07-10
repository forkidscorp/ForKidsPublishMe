using UnityEngine;
using System.Collections.Generic;

namespace Inventory
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private ScreenView _screenView;

        private const string OWNER_1 = "Publish Me";
        private const string OWNER_2 = "House Flipper";
        private readonly string[] _itemIds = { "Screwdriver", "Hammer", "Trowel", "Knife", "Roller" };

        private InventoryService _inventoryService;
        private ScreenController _screenController;

        private string _cashedOwnerId;

        public void Start()
        {
            var gameStateProvider = new GameStatePlayerPrefsProvider();

            gameStateProvider.LoadGameState();

            _inventoryService = new InventoryService(gameStateProvider);

            var gameState = gameStateProvider.GameState;

            foreach (var inventoryData in gameState.Inventories) 
            {
                _inventoryService.RegistrInventory(inventoryData);
            }

            _screenController = new ScreenController(_inventoryService, _screenView);
            _screenController.OpenInventory(OWNER_1);
            _cashedOwnerId = OWNER_1;
        }

        public void OpenInventory(string ownerId) {
            _screenController.OpenInventory(ownerId);
            _cashedOwnerId = ownerId;
        }
        public void ButtonAddItem(int index) 
        {
            var itemId = _itemIds[index];
            var amount = 1;
            var result = _inventoryService.AddItemsToInventory(_cashedOwnerId, itemId, amount);
        }
        public void ButtonRemoveItem(int index) 
        {
            var itemId = _itemIds[index];
            var amount = 1;
            var result = _inventoryService.RemoveItems(_cashedOwnerId, itemId, amount);
        }
    }
}
