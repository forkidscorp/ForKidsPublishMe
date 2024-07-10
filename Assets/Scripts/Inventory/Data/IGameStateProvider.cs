namespace Inventory
{
    public interface IGameStateProvider
    {
        void SaveGameState();
        void LoadGameState();
    }
}
