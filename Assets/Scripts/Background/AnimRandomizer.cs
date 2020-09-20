using UnityEngine;
using UnityEngine.Assertions;

public class AnimRandomizer : MonoBehaviour
{
    //This is a Script that randomizes how long of a pause there is
    //between animations on the given object.
    //This is to prevent animations to sync with eachother

    #region Variables

    Animator animator;
    float maxTime;
    float timer;

    #endregion Variables

    #region Start and Update

    void Start()
    {
        AnimationStart();
    }
    void Update()
    {
        AnimationLoop();
    }
    #endregion Start and Update


    void AnimationStart()
    {
        animator = GetComponent<Animator>();
        Assert.IsNotNull(animator, "This object does not have a Animator!");

        //Sets a random maxTime at start so all animations don't start at the same time
        maxTime = Random.Range(1f, 7f);

        //A check to see if the Trigger AnimationStart exists on the object
        /////This is something that should've been done in a separate class since in theory
        /////I am going to re-use it for all scripts
        for (int i = 0; i < animator.parameterCount; i++)
        {
            if (animator.GetParameter(i).name == "AnimationStart")
            {
                break;
            }
            else
            {
                Debug.Log("Trigger AnimationStart is Missing");
            }
        }
    }

    void AnimationLoop()
    {
        //Uses a timer that counts up until it reaches the MaxTime,
        //starts the animation, sets a new Random MaxTime and then restarts

        timer += Time.deltaTime;

        if (timer > maxTime)
        {
            animator.SetTrigger("AnimationStart");
            timer = 0;
            maxTime = Random.Range(1f, 7f);
        }
    }
}
