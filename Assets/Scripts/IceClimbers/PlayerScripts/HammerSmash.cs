using UnityEngine;

public class HammerSmash : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Npc"))
        {
            Destroy(collision.gameObject);
        }
    }
  
}
