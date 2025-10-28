using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShellCoinUI : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public Text uiText;

    void OnEnable()
    {
        var mgr = ShellCoinManager.instance;
        if (mgr != null)
        {
            mgr.OnCoinsChanged += UpdateUI;
            UpdateUI(mgr.Coins);
        }
    }

    void OnDisable()
    {
        if (ShellCoinManager.instance != null)
            ShellCoinManager.instance.OnCoinsChanged -= UpdateUI;
    }

    private void UpdateUI(int coins)
    {
        string text = coins.ToString();
        if (tmpText != null) tmpText.text = text;
        if (uiText != null) uiText.text = text;
    }
}