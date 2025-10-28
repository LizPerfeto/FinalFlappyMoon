using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class ShellCollect : MonoBehaviour
{
    public int coinValue = 1;
    public bool destroyOnCollect = true;
    public bool grantLifeOnCollect = true;
    public int lifeAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        try
        {
            ShellCoinManager.instance?.AddCoins(coinValue);
        }
        catch (System.Exception)
        {
        }

        if (grantLifeOnCollect && lifeAmount > 0)
        {
            try
            {
                HealthManager.instance?.Heal(lifeAmount);
            }
            catch (System.Exception)
            {
            }
        }

        if (destroyOnCollect)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}