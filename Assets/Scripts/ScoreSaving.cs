using TNSR.Scores;
using UnityEngine;

public class ScoreSaving
{
    static void main()
    {
        Scoring.ScoresPath = Application.persistentDataPath;
        // Scoring.SaveScore();
    }
}