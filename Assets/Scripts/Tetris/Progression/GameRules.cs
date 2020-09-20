using UnityEngine;
public class GameRules : MonoBehaviour
{
    public static GameRules instance;

    public Progression gameMode;

    public bool gameStarted;

    public ProgressionNode rules;

    public Sprite[] blockTypes;

    public Sprite[] obsticleTypes;

    public RuntimeAnimatorController animatedTile;

    [System.NonSerialized] public float backgroundPosition;

    int index;
    int musicIndex;
    void Awake()
    {
        if (!instance)
        {
            instance = this;
            index = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Set the gamerules based on the gamemode scriptable object 
    /// </summary>
    public void SetGameRules(int index)
    {
        this.index = index;
        rules = gameMode.progressionNodes[index];
        SetBlockTypes(rules.stage);
    }

    /// <summary>
    /// Set the avaible block types and textures.
    /// </summary>
    public void SetBlockTypes(int index)
    {
        blockTypes = gameMode.stages[index].blockSprites;
        obsticleTypes = gameMode.stages[index].obsticleSprites;
        animatedTile = gameMode.stages[index].animatedTile;
    }

    /// <summary>
    /// Under utalized music progression
    /// </summary>
    public void SetMusicIndex(int index)
    {
        musicIndex = index;
    }

    /// <summary>
    /// Checks if you meet the requirements to progress.
    /// </summary>
    public void UpdateGameRules(int height, int level)
    {
        if (gameMode.progressionNodes.Length > index + 1)
        {
            while (level >= gameMode.progressionNodes[index + 1].internalLevel)
            {
                index++;
                rules = gameMode.progressionNodes[index];
                SetBlockTypes(rules.stage);
            }
        }

        if (gameMode.musicEvents.Length > musicIndex + 1)
        {
            while (level >= gameMode.musicEvents[musicIndex + 1].internalLevel)
            {
                musicIndex++;
                if (gameMode.musicEvents[musicIndex].fadeMusic)
                {
                    SoundController.instance.FadeMusic();
                }
                else
                {
                    SoundController.instance.SetMusic(gameMode.musicEvents[musicIndex].musicIndex);
                }
            }
        }

        backgroundPosition = (float)height / gameMode.maxLevel;
    }
}
