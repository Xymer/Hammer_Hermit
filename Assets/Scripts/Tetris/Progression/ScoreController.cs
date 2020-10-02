using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField] Score score = null;

    public static ScoreController instance;

    [SerializeField] private int maxMultiplier = 5;

    int iterations;
    int sectionCleared;
    bool sectionCheck;

    public int GradeRequirements { get { return score.gradeRequirements.Length > score.grade + 1 ? score.gradeRequirements[score.grade + 1] : 69; } }
    public int Score { get { return score.score; } }
    public int MaxMultiplier { get { return maxMultiplier; } }

    public int Level { get { return score.level; } }

    public int Multiplier { get { return score.multiplier; } }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            score.Reload();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Updates the current high position
    /// </summary>
    public void UpdateHeight(int scrollAmount, int gridHeight)
    {
        score.heightIncreases += scrollAmount;

        if (score.heightIncreases + gridHeight > score.level)
        {
            if (score.heightIncreases + gridHeight - score.level > 1)
            {
                UpdateScore(score.level);
            }
            else
            {
                for (int i = 0; i <= score.heightIncreases + gridHeight - score.level; i++)
                {
                    UpdateScore(score.level - i);
                }
            }
            score.level = score.heightIncreases + gridHeight;
        }
        GameRules.instance.UpdateGameRules(score.heightIncreases, score.level);
    }

    /// <summary>
    /// Updates the score and spawns a score flash on the screen based on position
    /// </summary>
    public void UpdateScore(int amount, Vector3 position)
    {
        score.score += amount * score.multiplier;
        if (score.score > PlayerPrefs.GetInt("TopScore",0))
        {
            PlayerPrefs.SetInt("TopScore", score.score);            
        }
        if (score.grade + 1 >= score.maxGrade)
        {
            return;
        }
        if (score.score >= score.gradeRequirements[score.grade + 1])
        {
            score.grade++;
        }
        UIController.instance.SpawnScoreGain(amount, position);
    }

    /// <summary>
    /// Updates the score
    /// </summary>
    public void UpdateScore(int amount)
    {
        score.score += amount * score.multiplier;
        if (score.grade + 1 >= score.maxGrade)
        {
            return;
        }
        if (score.score >= score.gradeRequirements[score.grade + 1])
        {
            score.grade++;
        }
    }

    /// <summary>
    /// Update the mutliplier
    /// </summary>
    public void UpdateMultiplier(int amount)
    {
        score.multiplier += amount;
        score.multiplier = Mathf.Clamp(score.multiplier, 1, maxMultiplier);
    }

    public bool HasMaxMultiplier()
    {
        return Multiplier >= maxMultiplier;
    }

    /// <summary>
    /// Set level to 0
    /// </summary>
    public void ResetLevel()
    {
        score.level = 0;
    }

    /// <summary>
    /// Update the highet reached
    /// </summary>
    public void UpdateHeight(int scrollAmount)
    {
        score.heightIncreases += scrollAmount;
        GameRules.instance.UpdateGameRules(score.heightIncreases, score.level);
    }

    private void Update()
    {
        if (!GameRules.instance.gameStarted)
        {
            return;
        }
        score.time += Time.deltaTime;
        score.sectionTime += Time.deltaTime;
    }
}
