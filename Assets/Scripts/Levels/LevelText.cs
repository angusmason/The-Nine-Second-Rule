using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TNSR.Levels
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LevelText : MonoBehaviour
    {
        TextMeshProUGUI levelText;

        void Start()
        {
            levelText = GetComponent<TextMeshProUGUI>();
        }
        void Update()
        {
            var buildIndex = SceneManager.GetActiveScene().buildIndex;
            var timeCompleted = LevelSaver.GetLevel(buildIndex - 1)?.TimeMilliseconds;
            levelText.text = $@"Level {buildIndex}
                {(timeCompleted == null
                    ? "Not completed"
                    : $@"Best Time: {TimeSpan.FromMilliseconds
                        ((double)timeCompleted):s\.ff\s}")}";
        }
    }
}
