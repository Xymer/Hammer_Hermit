using UnityEngine;

public class ScoreGain : MonoBehaviour
{
    TMPro.TextMeshProUGUI text;
    public int score;
    public int multiplier;
    int frames;
    float alpha = 1f;
    bool flickered = false;
    public int waitFrames;

    public ParticleSystem gainParticle;

    private void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        Destroy(Instantiate(gainParticle.gameObject, gameObject.transform.position, Quaternion.identity), gainParticle.main.duration);
    }

    private void FixedUpdate()
    {
        text.text = $"{score} x {multiplier}";
        if (frames >= waitFrames)
        {
            frames = 0;
            if (flickered)
            {
                text.color = Color.white;
            }
            else
            {
                text.color = Color.yellow;
            }
            flickered = !flickered;
        }
        frames++;
        alpha -= 0.005f;

        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        transform.Translate(Vector3.up * 0.005f);
        if (text.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
