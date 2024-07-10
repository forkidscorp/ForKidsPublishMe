using UnityEditor;
using UnityEngine;

namespace GoogleImporter
{
    public class ConfigImportsMenu
    {
        private const string SPREADSHEET_ID = "16er4ldbNUC_X2fJ3O_Nn4HJs3-ZYuXjzZCuDQO9pDFA";
        private const string ITEMS_SHEETS_NAME = "InventoryItems";
        private const string CREDENTIAL_PATH = "publish-me-428815-32bdc56f729b.json";
        private const string SETTINGS_FILE_NAME = "GameSettings";

        [MenuItem("PublishMe/Import Items Settings")]
        private static async void LoadItemsSettings() 
        {
            var sheetsImporter = new GoogleSheetsImporter(CREDENTIAL_PATH, SPREADSHEET_ID);
            var gameSettings = LoadSettings();

            var itemsParser = new ItemsSettingsParser(gameSettings);
            await sheetsImporter.DownloadAndParseSheet(ITEMS_SHEETS_NAME, itemsParser);

            var jsonForSaving = JsonUtility.ToJson(gameSettings);
            PlayerPrefs.SetString(SETTINGS_FILE_NAME, jsonForSaving);

            Debug.Log(jsonForSaving);
        }

        private static GameSettings LoadSettings()
        {
            var jsonLoaded = PlayerPrefs.GetString(SETTINGS_FILE_NAME);
            var gameSettings = !string.IsNullOrEmpty(jsonLoaded)
                ? JsonUtility.FromJson<GameSettings>(jsonLoaded)
                : new GameSettings();
            return gameSettings;
        }
    }
}
