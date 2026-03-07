namespace DeadCellsArchipelago {
    public class ArchipelagoSaveData
    {
        public HashSet<string> SentChecks { get; set; } = [];
        public HashSet<string> RecievedItem { get; set; } = [];

        public void SaveCheckSent(string checkName)
        {
            SentChecks.Add(checkName);
        }

        public void SaveItemRecieved(string itemName)
        {
            RecievedItem.Add(itemName);
        }

        public bool IsCheckSent(string checkName)
        {
            return SentChecks.Contains(checkName);
        }

        public bool IsItemRecieved(string itemName)
        {
            return RecievedItem.Contains(itemName);
        }
    }
}