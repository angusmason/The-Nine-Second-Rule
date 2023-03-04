using UnityEngine;

[
    CreateAssetMenu(
        fileName = "Data",
        menuName = "TNSR/LevelColours",
        order = 1
    )
]
public class LevelColours : ScriptableObject
{
    public Color[] levelColours;
}
