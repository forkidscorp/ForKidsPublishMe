using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class GameStatePlayerPrefsProvider : IGameStateProvider, IGameStateSaver
    {
        private const string KEY = "GAME STATE";

        public GameStateData GameState { get; private set; }
        
        public void SaveGameState()
        {
            var json = JsonUtility.ToJson(GameState);
            PlayerPrefs.SetString(KEY, json);
        }

        public void LoadGameState()
        {
            if (PlayerPrefs.HasKey(KEY))
            {
                var json = PlayerPrefs.GetString(KEY);
                GameState = JsonUtility.FromJson<GameStateData>(json);
            }
            else 
            {
                GameState = InitFromSettings();
                SaveGameState();
            }
        }

        private GameStateData InitFromSettings()
        {
            var gameState = new GameStateData
            {
                Inventories = new List<InventoryGridData>
                {
                    CreateInventory("Publish Me"),
                    CreateInventory("House Flipper")
                }
            };
            return gameState;
        }

        private InventoryGridData CreateInventory(string ownerId)
        {
            var size = new Vector2Int(5, 5);
            var createdInventorySlots = new List<InventorySlotData>();
            var length = size.x * size.y;

            for (var i = 0; i < length; i++)
            {
                createdInventorySlots.Add(new InventorySlotData());
            }

            var createdInventoryData = new InventoryGridData
            {
                OwnreId = ownerId,
                Size = size,
                Slots = createdInventorySlots
            };

            return createdInventoryData;
        }
    }
}
