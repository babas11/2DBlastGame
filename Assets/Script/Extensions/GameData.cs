using Script.Data.ValueObjects;

namespace Script.Extensions
{
    public static class GameData
    {
        public static SaveFileData SaveData;
        public static CustomGridData CurrentLevelData => SaveData.gridData;
        public static byte CurrentLevel => SaveData.currentLevel;
    }
}