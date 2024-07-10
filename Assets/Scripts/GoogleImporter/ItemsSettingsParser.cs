using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoogleImporter
{
    public class ItemsSettingsParser : IGoogleSheetParser
    {
        private readonly GameSettings _gameSettings;
        private ItemSettings _currentItemSettings;

        public ItemsSettingsParser(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
            _gameSettings.Items = new List<ItemSettings>();
        }
        public void Parse(string header, string token)
        {
            switch (header) 
            {
                case "ID":
                    _currentItemSettings = new ItemSettings()
                    {
                        Id = token
                    };
                    _gameSettings.Items.Add(_currentItemSettings);
                    break;
                case "CellCapacity":
                    _currentItemSettings.CellCapacity = Convert.ToInt32(token);
                    break;
                case "Title":
                    _currentItemSettings.Title = token;
                    break;
                case "Description":
                    _currentItemSettings.Description = token;
                    break;
                case "IconItem":
                    _currentItemSettings.IconName = token;
                    break;
                default:
                    throw new Exception($"Invalid Header: {header}");
            }
        }
    }
}
