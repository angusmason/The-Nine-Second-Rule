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
        static LevelData LevelFile
        {
            get => JsonUtility.FromJson<LevelData>(File.ReadAllText(_dataPath));
            set => File.WriteAllText(_dataPath, JsonUtility.ToJson(value));
        }
        static LevelSaver()
        {
            _dataPath = Application.persistentDataPath + "/levels.json";
            if (!File.Exists(_dataPath))
                using (var stream = File.Create(_dataPath))
                    stream.Write(Encoding.ASCII.GetBytes("{}"));
        }
        public static LevelDatum GetLevel(int levelIndex)
        {
            LevelDatum levelData = LevelFile.Levels.FirstOrDefault(levelData => levelData.LevelIndex == levelIndex);
            if (levelData == null) throw new Exception("Level not completed");
            return levelData;
        }
        public static bool LevelCompleted(int levelIndex)
        {
            return LevelFile.Levels.Any(levelData => levelData.LevelIndex == levelIndex);
        }
        public static void Update(LevelDatum newLevelDatum)
        {
            var matchingLevel = LevelFile.Levels
                .FirstOrDefault(levelData => newLevelDatum.LevelIndex == levelData.LevelIndex);
            if (matchingLevel == null)
            {
                LevelFile.Levels = LevelFile.Levels.Union(new LevelDatum[] { newLevelDatum }).ToArray();
                foreach (var level in LevelFile.Levels)
                {
                    Debug.Log(level.LevelIndex);
                }
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
