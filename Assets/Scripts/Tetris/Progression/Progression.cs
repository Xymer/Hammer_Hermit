using UnityEngine;

[CreateAssetMenu(fileName = "Score", menuName = "Tetris/Progression", order = 1)]
public class Progression : ScriptableObject
{
    public int maxLevel;
    public Stage[] stages;
    public ProgressionNode[] progressionNodes;
    public MusicEvent[] musicEvents = new MusicEvent[1];
}

//Scriptable object that stores progression data. Somewhat modular but not very designer friendly. I don't know tools programming :(
[System.Serializable]
public class ProgressionNode
{
    public int internalLevel;
    [Range(2, 128)] public int dropFrames;
    [Range(30, 60)] public int lockFrames;
    [Range(8, 50)] public int areFrames;
    [Range(8, 50)] public int areLineFrames;
    [Range(12, 80)] public int lineClearFrames;
    [Range(12, 28)] public int dasFrames;
    [Range(1, 20)] public int gravity;
    [Range(1, 4)] public int scrollRequirement;
    [Range(8, 14)] public int maxHeight;
    [Range(2, 8)] public int blockDifficulty;
    [Range(0, 3)] public int stage;
}

[System.Serializable] 
public class MusicEvent
{
    public int internalLevel;
    public bool fadeMusic;
    public int musicIndex;
}

[System.Serializable]
public class Stage
{
    public Sprite[] blockSprites;
    public Sprite[] obsticleSprites;
    public RuntimeAnimatorController animatedTile;
}