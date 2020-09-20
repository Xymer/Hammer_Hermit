using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] public TMPro.TextMeshProUGUI topScoreText;
    private Animator pointerAnimator;
    private Button[] buttons;
    
    private GameObject lastSelected;

    void Awake()
    {      
        pointerAnimator = GetComponentInChildren<Animator>();
        buttons = GetComponentsInChildren<Button>();
    }
    private void Start()
    {
        UpdateTopScore();        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
            DeleteHighScore();
        UpdateTopScore();
        foreach (Button button in buttons)
        {
            if (button.gameObject == EventSystem.current.currentSelectedGameObject)
            {
                pointerAnimator.transform.position = new Vector3(
                    pointerAnimator.transform.position.x,
                    button.transform.position.y,
                    pointerAnimator.transform.position.z);
                break;
            }
            
        }

        if (!EventSystem.current.currentSelectedGameObject)
            EventSystem.current.SetSelectedGameObject(lastSelected);
        else
            lastSelected = EventSystem.current.currentSelectedGameObject;
    }

    public void UpdateTopScore()
    {
        if (ScoreController.instance.Score > PlayerPrefs.GetInt("TopScore", 0))
        {
            PlayerPrefs.SetInt("TopScore", ScoreController.instance.Score);
        }
        topScoreText.text = "TOP - " + PlayerPrefs.GetInt("TopScore").ToString().PadLeft(7, '0') + " PTS";
    }
    public void ConfirmSelection()
    {
        pointerAnimator.SetTrigger("Confirm");
    }

    private void DeleteHighScore()
    {
        PlayerPrefs.SetInt("TopScore", 0);
    }
}
