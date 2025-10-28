using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager instance;

    public int MaxVida = 3;

    public int Vida { get; private set; }

    public Image[] hearts;
    public Sprite FullHeart;
    public Sprite EmptyHeart;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Vida = MaxVida;
        UpdateHearts();
    }

    public void TakeDamage()
    {
        if (Vida <= 0) return;

        Vida = Mathf.Max(0, Vida - 1);
        UpdateHearts();

        if (GameManager.instance != null)
            GameManager.instance.ApplySpriteByVida(Vida);

        if (Vida <= 0)
        {
            if (GameManager.instance != null)
                GameManager.instance.GameOver();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        Vida = Mathf.Min(MaxVida, Vida + amount);
        UpdateHearts();

        if (GameManager.instance != null)
            GameManager.instance.ApplySpriteByVida(Vida);
    }

    public void ResetLives()
    {
        Vida = MaxVida;
        UpdateHearts();

        if (GameManager.instance != null)
            GameManager.instance.ApplySpriteByVida(Vida);
    }

    private void UpdateHearts()
    {
        if (hearts == null || hearts.Length == 0) return;

        for (int i = 0; i < hearts.Length; i++)
            hearts[i].sprite = EmptyHeart;

        for (int i = 0; i < Vida && i < hearts.Length; i++)
            hearts[i].sprite = FullHeart;
    }
}
