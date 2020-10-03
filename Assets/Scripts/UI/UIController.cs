#pragma warning disable 0649
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    float tempLevel;
    float tempScore;
    float levelH = 0;
    float scoreH = 0;
    int gradeIndex;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    Score score;

    [SerializeField]
    TextMeshProUGUI level;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI multiplier;
    [SerializeField]
    TextMeshProUGUI nextRequirement;
    [SerializeField]
    TextMeshProUGUI lives;
    [SerializeField]
    Canvas worldSpaceCanvas;
    [SerializeField]
    Image grade;
    [SerializeField]
    Sprite[] grades;
    [SerializeField]
    Sprite[] flashGrades;
    [SerializeField]
    Image flash;

    [SerializeField]
    GameObject endScreen;
    [SerializeField]
    TextMeshProUGUI endScreenScore;
    [SerializeField]
    TextMeshProUGUI endScreenHiScore;
    [SerializeField]
    Image endScreenGrade;
    [SerializeField]
    TextMeshProUGUI endScreenPrompt;

    [SerializeField]
    ScoreGain scoreGain;
    [SerializeField]
    Transform playerSpace;

    public void DefaultValues()
    {
        score.Reload();
        levelH = scoreH = gradeIndex = 0;
        level.text = 0.ToString().PadLeft(3, '0') + "M";
        scoreText.text = 0.ToString().PadLeft(7, '0'); ;
        grade.sprite = grades[gradeIndex];
    }

    private void FixedUpdate()
    {
        levelH = Mathf.SmoothDamp(levelH, score.level, ref tempLevel, 0.25f);

        level.text = Mathf.RoundToInt(levelH).ToString().PadLeft(3, '0') + "M";

        scoreH = Mathf.SmoothDamp(scoreH, score.score, ref tempScore, 0.06125f);

        scoreText.text = Mathf.RoundToInt(scoreH).ToString().PadLeft(7, '0');

        multiplier.text = "x" + score.multiplier;

        if (ScoreController.instance.GradeRequirements == 69)
        {
            nextRequirement.text = "Next: \nMax rank";
        }
        else
        {
            nextRequirement.text = "Next: \n" + ScoreController.instance.GradeRequirements;
        }

        if (gradeIndex <= score.grade - 1)
        {
            gradeIndex++;
            //StopCoroutine(FlashGrade());
            //StartCoroutine(FlashGrade());
            grade.sprite = grades[gradeIndex];
            grade.transform.GetChild(0).GetComponent<Image>().sprite = flashGrades[gradeIndex];
            grade.GetComponent<Animator>().SetTrigger("RankUp");
            GetComponent<AudioSource>().Play();
        }
    }

    public void SpawnScoreGain(int amount, Vector3 position)
    {
        var sg = Instantiate(scoreGain, position, Quaternion.identity, playerSpace);
        sg.score = amount;
        sg.multiplier = score.multiplier;
    }

    public IEnumerator EndScreenAnimation()
    {
        DisplayGameUI(false);

        endScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        endScreenScore.text = "Score: \n" + score.score;
        endScreenScore.gameObject.SetActive(true);
        if (score.score>PlayerPrefs.GetInt("TopScore",0))
        {
            endScreenHiScore.text = $"Hi-Score:\n{score.score.ToString().PadLeft(7, '0')}";
        }
        else
        {
            endScreenHiScore.text = $"Hi-Score:\n{PlayerPrefs.GetInt("TopScore", 0).ToString().PadLeft(7, '0')}";
        }      
        endScreenHiScore.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        endScreenGrade.sprite = grade.sprite;
        endScreenGrade.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        float flashDuration = 1f;
        float flashTime = 0f;
        while (!Input.anyKey && !Input.GetButton("Submit"))
        {
            if (flashTime <= 0f)
            {
                endScreenPrompt.gameObject.SetActive(!endScreenPrompt.gameObject.activeSelf);
                flashTime = flashDuration;
            }
            else
                flashTime -= Time.deltaTime;
            yield return null;
        }
    }

    internal void UpdateLives(int playerLives)
    {
        lives.text = "Lives: " + (playerLives + 1);
    }

    public void DisplayGameUI(bool display = true)
    {
        scoreText.gameObject.SetActive(display);
        level.gameObject.SetActive(display);
        nextRequirement.gameObject.SetActive(display);
        grade.transform.parent.gameObject.SetActive(display);
        multiplier.gameObject.SetActive(display);
        lives.gameObject.SetActive(display);
        worldSpaceCanvas.gameObject.SetActive(display);
    }

    public void EndScreen(bool display = true)
    {
        StopCoroutine(EndScreenAnimation());
        endScreenScore.gameObject.SetActive(!display);
        endScreenHiScore.gameObject.SetActive(!display);
        endScreenGrade.gameObject.SetActive(!display);
        endScreenPrompt.gameObject.SetActive(!display);
    }

    IEnumerator FlashGrade()
    {
        flash.color = Color.white;
        float fadeSpeed = 0.01f;
        while (flash.color.a > 0)
        {
            yield return new WaitForFixedUpdate();
            flash.color -= new Color(0, 0, 0, fadeSpeed);
        }
    }
}
