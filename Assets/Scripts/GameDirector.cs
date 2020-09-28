using System.Collections;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField] private IceClimberMovement player;
    [SerializeField] private PlayerInput tetrisPlayer;
    [SerializeField] private Matrix matrix;

    [SerializeField] public int playerLives = 3;

    [SerializeField] private bool atTitleScreen;
    [SerializeField] private CanvasGroup titleScreenGroup;
    [SerializeField] private CanvasGroup gameScreenGroup;

    [SerializeField] private Animator countdownAnimator;
    [SerializeField] private AudioSource music;
    static public bool disableControls = false;
    public int totalPlayers;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<IceClimberMovement>();
    }
    private void Start()
    {
        matrix.Initialize(!atTitleScreen);
        player.Initialize();

        if (!atTitleScreen)
            StartCoroutine(Countdown());
        else if (player.gameObject.activeSelf)
            player.gameObject.SetActive(false);

        SetCanvas(atTitleScreen);
    }

    void OnRestart()
    {       
        player.ResetToStart();
        matrix.OnRestart();
        matrix.ToggleGameController(true);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            QuitGame();
    }
    public int ReturnLife()
    {
        return playerLives;
    }
    public void StartGame(int players)
    {
        totalPlayers = players;
        if (totalPlayers == 1)
        {
            player.input.SetControllerOption(1);
            tetrisPlayer.input.SetControllerOption(2);
        }
        else if (totalPlayers == 2)
        {
            player.input.SetControllerOption(3);
            tetrisPlayer.input.SetControllerOption(4);
        }
        ScoreController.instance.ResetLevel();
        foreach (LevelEntitySpawner levelEntitySpawner in GetComponents<LevelEntitySpawner>())
            levelEntitySpawner.heightLevel = 1;
        if (atTitleScreen)
            StartCoroutine(StartGameSequence());
    }

    public IEnumerator StartGameSequence()
    {
        atTitleScreen = false;
        yield return new WaitForSeconds(0.3f);

        ChangeCanvas(atTitleScreen);

        yield return StartCoroutine(Countdown());

        yield break;
    }

    private IEnumerator Countdown()
    {
        playerLives = 2;
        matrix.UpdateQueue();
        SoundController.instance.SetMusic(0);
        UIController.instance.DefaultValues();
        player.ResetToStart();
        matrix.OnRestart();
        matrix.ToggleGameController(false);
        UIController.instance.DisplayGameUI();

        player.WaitingForCountdown = true;
        disableControls = true;

        player.AddDirectorEvent(StartCountdown);

        SoundController.instance.PlaySound(GetCountdownBarkSFXName(),.75f);
        yield return new WaitForSeconds(2.75f);
        player.RemoveDirectorEvent(StartCountdown);

        player.WaitingForCountdown = false;
        disableControls = false;

        player.AddDirectorEvent(LoseLife);

        matrix.OnNewGame();
        matrix.ToggleGameController(true);

        SoundController.instance.PlaySound("Go", .75f);
        yield break;
    }

    private string GetCountdownBarkSFXName()
    {
        string[] sfxNames = new string[] {
            "IGottaClimbToTheTop",
            "IGottaGetUpThere",
            "GoodThingIGotMyHammer",
            "NowBeginsTheEpicAscemsion"
        };
        return sfxNames[Random.Range(0, sfxNames.Length)];
    }

    private void StartCountdown()
    {
        countdownAnimator.SetTrigger("Count");
    }

    public void ToTitleScreen()
    {
        atTitleScreen = true;
        ChangeCanvas(atTitleScreen);
    }

    void LoseLife()
    {
        playerLives--;
        ScoreController.instance.UpdateMultiplier(-5);
        UIController.instance.UpdateLives(playerLives);
        if (playerLives < 0)
            StartCoroutine(GameOverSequence());
        else
            OnRestart();
    }

    IEnumerator GameOverSequence()
    {
        player.gameObject.SetActive(false);
        player.RemoveDirectorEvent(LoseLife);

        SoundController.instance.FadeMusic();
        matrix.OnGameOver();


        yield return StartCoroutine(UIController.instance.EndScreenAnimation());
        GameRules.instance.SetBlockTypes(0);
        GameRules.instance.SetGameRules(0);
        GameRules.instance.SetMusicIndex(0);

        UIController.instance.EndScreen();
        ToTitleScreen();
    }

    private void ChangeCanvas(bool toTitleScreen)
    {
        CanvasGroup groupToHide = toTitleScreen ? gameScreenGroup : titleScreenGroup;
        CanvasGroup groupToShow = toTitleScreen ? titleScreenGroup : gameScreenGroup;

        StartCoroutine(ChangeAlpha(groupToHide, 0));
        StartCoroutine(ChangeAlpha(groupToShow, 1));
    }

    private IEnumerator ChangeAlpha(CanvasGroup group, float alphaTarget)
    {
        float speed = 0.01f;

        if (alphaTarget > 0)
            group.gameObject.SetActive(true);

        while (group.alpha != alphaTarget)
        {
            if (group.alpha > alphaTarget)
            {
                group.alpha -= speed;
                if (group.alpha < alphaTarget)
                    group.alpha = alphaTarget;
            }
            else if (group.alpha < alphaTarget)
            {
                group.alpha += speed;
                if (group.alpha > alphaTarget)
                    group.alpha = alphaTarget;
            }
            yield return null;
        }
        if (alphaTarget == 0)
            group.gameObject.SetActive(false);
        yield break;
    }

    private void SetCanvas(bool toTitleScreen)
    {
        CanvasGroup groupToHide = toTitleScreen ? gameScreenGroup : titleScreenGroup;
        CanvasGroup groupToShow = toTitleScreen ? titleScreenGroup : gameScreenGroup;

        groupToHide.alpha = 0;
        groupToHide.gameObject.SetActive(false);
        groupToShow.alpha = 1;
        groupToShow.gameObject.SetActive(true);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}