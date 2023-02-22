using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TNSR.Levels
{
    public static class LevelSaver
    {
        static readonly string _dataPath;
        static LevelData LevelData;
        static void Load() => LevelData = JsonUtility.FromJson<LevelData>(File.ReadAllText(_dataPath));
        static void Save() => File.WriteAllText(_dataPath, JsonUtility.ToJson(LevelData));
        static LevelSaver()
        {
            _dataPath = Application.persistentDataPath + "/levels.json";
            if (!File.Exists(_dataPath))
                using (var stream = File.Create(_dataPath))
                    stream.Write(Encoding.ASCII.GetBytes("{}"));
            Load();
        }
        public static LevelDatum GetLevel(int levelIndex)
        {
            Load();
            LevelDatum levelData = LevelData.Levels.FirstOrDefault(levelData => levelData.LevelIndex == levelIndex);
            if (levelData == null) throw new Exception("Level not completed");
            return levelData;
        }
        public static bool LevelCompleted(int levelIndex)
        {
            Load();
            return LevelData.Levels.Any(levelData => levelData.LevelIndex == levelIndex);
        }
        public static void UpdateData(LevelDatum newLevelDatum)
        {
            var matchingLevel = LevelData.Levels
                .FirstOrDefault(levelData => newLevelDatum.LevelIndex == levelData.LevelIndex);
            if (matchingLevel == null)
            {
                LevelData.Levels = LevelData.Levels.Concat(new LevelDatum[] { newLevelDatum }).ToArray();
                Save();
                return;
            }
            matchingLevel = newLevelDatum;
        }
    }

    [Serializable]
    public class LevelData
    {
        public LevelDatum[] Levels = Array.Empty<LevelDatum>();
    }

    [Serializable]
    public class LevelDatum
    {
        public int LevelIndex;

        public LevelDatum(int levelIndex)
        {
            LevelIndex = levelIndex;
        }
    }
}
