using UnityEngine;
using UnityEngine.Assertions;

public class Background : MonoBehaviour
{
    //This script is used to move both the lights and background when the palyer climbs higher
    //Right now it is on the background object itself, but it was meant to be on a independent object,
    // So you can change the background easily.

    #region Variables

    [SerializeField] GameObject indirectLightObject = null;
    [SerializeField] GameObject rimLightObject = null;
    [SerializeField] GameObject background = null;
    [SerializeField] GameObject pillars = null;

    Vector3 tempVelocity;

    Light indirectLight;
    Light rimLight;

    public double highestYPos;
    public double highestPillarsYPos;

    float indirectLightIntensity;
    float rimLightIntensity;

    [SerializeField] float indirectLightIntensityMultiplier = 0.01f;
    [SerializeField] float rimLightIntensityMultiplier = 0.15f;

    bool getBackgroundPos = true;
    float startPos = -50f;
    float endPos = -60f;

    #endregion Variables

    #region Start and Update
    private void Awake()
    {
        //Adds components and intensity into variables jsut for readability later on
        indirectLight = indirectLightObject.GetComponent<Light>();
        Assert.IsNotNull(indirectLightObject, "Pipe in a indirect light!");
        rimLight = rimLightObject.GetComponent<Light>();
        Assert.IsNotNull(indirectLightObject, "Pipe in a direct/rim light!");

        indirectLightIntensity = indirectLight.intensity;
        rimLightIntensity = rimLight.intensity;
    }

    private void Update()
    {
        //Makes the backgronud only scroll to the highest point and then it stops.
        if (background.transform.position.y > -highestYPos)
        {
            //Moves the background and the pillars on the side.
            //THey are separate to give some depth and paralax, since the pillars move faster.
            BackgroundMovement(background, highestYPos);
            BackgroundMovement(pillars, highestPillarsYPos);

            LightControl();
        }
    }
    #endregion Start and Update

    #region BackgroundControl
    //Moves the background depending on how high up you are in the game
    //Which you get from the GameRules class
    void BackgroundMovement(GameObject backgroundObject, double highestPos)
    {
        //Since the number you get from GameRules is stuttering,
        //I needed a check to smoothen that movement out to remove the stutter
        if (Vector3.Distance(backgroundObject.transform.position, (new Vector3(0, (float)(-GameRules.instance.backgroundPosition * highestPos)))) < .001f)
        {
            backgroundObject.transform.position = new Vector3(0, (float)(-GameRules.instance.backgroundPosition * highestPos));
        }
        else
        {
            backgroundObject.transform.position = Vector3.SmoothDamp(backgroundObject.transform.position, new Vector3(0, (float)(-GameRules.instance.backgroundPosition * highestPos)), ref tempVelocity, 0.1f);
        }
    }
    #endregion BackgroundControl

    #region LightControl
    void LightControl()
    {
        //The light Intensity is dependant on the background position

        float backgroundPosY = background.transform.position.y;

        if (getBackgroundPos && backgroundPosY < startPos)
        {
            startPos = backgroundPosY;
            getBackgroundPos = false;
        }

        //The light intensity will start getting higher on given positions of the background
        if (backgroundPosY > -25f)
        {
            IndirectLightIntensityAnimation(indirectLight, indirectLightIntensity, indirectLightIntensityMultiplier);
        }

        if (backgroundPosY < startPos && backgroundPosY > endPos)
        {
            RimLightIntensityAnimation(rimLight, rimLightIntensity, rimLightIntensityMultiplier, startPos);
        }

    }

    //Methods for indirect and direct light are separate since they are slightly different
    //However, there is probably a way to make these into one.
    void IndirectLightIntensityAnimation(Light light, float lightIntensity, float multiplier)
    {
        light.intensity = lightIntensity - background.transform.position.y * multiplier;
    }

    void RimLightIntensityAnimation(Light light, float lightIntensity, float multiplier, float startPos)
    {
        light.intensity = lightIntensity - (background.transform.position.y - startPos) * multiplier;
    }
    #endregion LightControl
}
