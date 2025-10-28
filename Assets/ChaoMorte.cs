using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InstantDeathPlatform : MonoBehaviour
{
    public string playerTag = "Player";
    public bool useTrigger = true;
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!useTrigger) return;
        if (!other.CompareTag(playerTag)) return;
        TriggerGameOver();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (useTrigger) return;
        if (!collision.collider.CompareTag(playerTag)) return;
        TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        if (GameManager.instance != null)
            GameManager.instance.GameOver();
    }
}