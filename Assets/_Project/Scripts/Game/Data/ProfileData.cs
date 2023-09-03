using System;

namespace Match_3
{
    [Serializable]
    public class ProfileData
    {
        public int Lives { get; set; }
        public int Level { get; set; }
        public int Gold { get; set; }
        public long LastTimePlay { get; set; }
        public DateTime DailyResetTime { get; set; }
        public PowerUpData PowerUpData { get; set; }
        public LastTimeReceiveLife LastTimeReceiveLife { get; set; }
        public QuestProcessData[] QuestProcessData { get; set; }
        
        public bool IsFirstTimePlay { get; set; }
    }

    [Serializable]
    public class PowerUpData
    {
        public int Undo { get; set; }
        public int Shuffle { get; set; }
        public int Suggests { get; set; }
    }

    [Serializable]
    public class LastTimeReceiveLife
    {
        public int TotalSecond { get; set; }
    }

    [Serializable]
    public class QuestProcessData
    {
        public string ID { get; set; }
        public int Current { get; set; }
        public int Total { get; set; }
        
        public QuestState State { get; set; }
    }
    
    public enum QuestState
    {
        InProgress,
        Completed,
        Claimed
    }
}