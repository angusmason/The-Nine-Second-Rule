using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TNSR.Levels
{
    public class LevelSelectManager : MonoBehaviour
    {
        [SerializeField] Transform player;
        [SerializeField] string[] nonLevelScenes;
        [SerializeField] GameObject levelPrefab;
        [SerializeField] float spacing;

        void Start()
        {
            var scenes = Enumerable
                .Range(0, SceneManager.sceneCountInBuildSettings)
                .Select(index => SceneManager.GetSceneByBuildIndex(index))
                .Where(scene => !nonLevelScenes.Contains(scene.name))
                .ToList();

            foreach (var (scene, index) in scenes.Select((scene, index) => (scene, index)))
            {
                var level = Instantiate(levelPrefab, new(
                        transform.position.x + index * spacing,
                        transform.position.y
                    ),
                    Quaternion.identity,
                    transform
                ).GetComponent<Level>();
                level.player = player;
                level.buildIndex = index;
                level.colour = Resources
                    .Load<LevelColours>("LevelColours")
                    .levelColours[index];
            }
        }
    }
}
