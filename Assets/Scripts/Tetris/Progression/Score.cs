using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Score", menuName = "Tetris/ScoreStorage", order = 1)]
//Stores score for the current instance as well as grade requirements.
public class Score : ScriptableObject
{
    public int[] gradeRequirements;
    public int maxGrade;
    [NonSerialized]
    internal int score;
    [NonSerialized]
    internal int multiplier = 1;
    [NonSerialized]
    public int level;
    [NonSerialized]
    public int heightIncreases;
    [NonSerialized]
    public int grade;

    [NonSerialized]
    public float time;
    [NonSerialized]
    public float sectionTime;
    [NonSerialized]
    public List<float> sectionTimes = new List<float>();

    internal void Reload()
    {
        score = 0;
        multiplier = 1;
        level = 0;
        heightIncreases = 0;
        grade = 0;
        time = 0;
        sectionTime = 0;
    }
}
