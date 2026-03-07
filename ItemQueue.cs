using Serilog;

using static DeadCellsArchipelago.ItemManager;

namespace DeadCellsArchipelago
{
    public static class ItemQueue
    {
        private static List<string> pendingItems = [];

        public static void AddItemToQueue(string itemName)
        {
            Log.Information($"=== Item reçu d'Archipelago: {itemName} ===");
            pendingItems.Add(itemName);
        }

        public static void GiveItemInQueue()
        {
            if(IsQueueEmpty()) { return; }
            if(SAVED_DATA == null) { return; }

            var itemName = pendingItems[0];

            if(!SAVED_DATA.IsItemRecieved(itemName))
            {
                GiveItemFromArchipelago(itemName);
                Log.Information($"=== Item {itemName} given ===");
                SAVED_DATA.SaveItemRecieved(itemName);
            }
            else
            {
                Log.Error($"=== Item {itemName} already given ===");
            }
            pendingItems.RemoveAt(0);
        }

        public static bool IsQueueEmpty()
        {
            return pendingItems.Count == 0;
        }
    }
}