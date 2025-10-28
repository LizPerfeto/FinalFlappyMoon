using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkinManager : MonoBehaviour
{
    [Header("Preview")]
    public SpriteRenderer sr;
    public List<Sprite> skins = new List<Sprite>();
    private int selectedskin = 0;

    [Header("Skins // PrefabsSkins")]
    public List<GameObject> availableSkinPrefabs = new List<GameObject>();
    public List<int> skinCosts = new List<int>();

    [Header("UI")]
    public TextMeshProUGUI skinNameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI statusText;
    public Button playButton;
    public Button buyButton; 

    private const string PrefKeySelected = "SelectedSkinIndex";
    private const string PrefKeyUnlockedPrefix = "SkinUnlocked_";

    void Start()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(PlayGame);
        }

        bool hasAnySkins = (skins != null && skins.Count > 0) || (availableSkinPrefabs != null && availableSkinPrefabs.Count > 0);
        if (!hasAnySkins)
        {
            if (playButton != null) playButton.gameObject.SetActive(false);
            if (buyButton != null) buyButton.gameObject.SetActive(false);
            return;
        }

        EnsureUnlocked(0);

        int maxIndex = 0;
        if (skins != null && skins.Count > 0) maxIndex = skins.Count - 1;
        else if (availableSkinPrefabs != null && availableSkinPrefabs.Count > 0) maxIndex = availableSkinPrefabs.Count - 1;

        int saved = PlayerPrefs.GetInt(PrefKeySelected, 0);
        if (saved < 0 || saved > maxIndex)
        {
            saved = Mathf.Clamp(saved, 0, maxIndex);
            PlayerPrefs.SetInt(PrefKeySelected, saved);
            PlayerPrefs.Save();
        }

        selectedskin = Mathf.Clamp(saved, 0, maxIndex);

        UpdateUIForSelectedSkin();
    }

    public void NextOption()
    {
        if (skins == null || skins.Count == 0) return;
        selectedskin = (selectedskin + 1) % skins.Count;
        UpdateUIForSelectedSkin();
    }

    public void BackOption()
    {
        if (skins == null || skins.Count == 0) return;
        selectedskin = (selectedskin - 1 + skins.Count) % skins.Count;
        UpdateUIForSelectedSkin();
    }

    private void UpdateUIForSelectedSkin()
    {
        if (sr != null && skins != null && skins.Count > 0 && selectedskin >= 0 && selectedskin < skins.Count)
            sr.sprite = skins[selectedskin];

        if (skinNameText != null)
        {
            string nm = (availableSkinPrefabs != null && selectedskin < availableSkinPrefabs.Count && availableSkinPrefabs[selectedskin] != null)
                ? availableSkinPrefabs[selectedskin].name
                : $"Skin {selectedskin}";
            skinNameText.text = nm;
        }

        int cost = GetCost(selectedskin);
        if (costText != null)
            costText.text = cost.ToString();

        bool unlocked = IsUnlocked(selectedskin);
        if (unlocked)
        {
            if (statusText != null) statusText.text = "Unlocked";
            if (buyButton != null) SetButtonText(buyButton, "Select");
        }
        else
        {
            if (statusText != null) statusText.text = "Locked";
            if (buyButton != null) SetButtonText(buyButton, $"Buy ({cost})");
        }
    }

    public void PlayGame()
    {
        if (!IsUnlocked(selectedskin))
        {
            if (statusText != null)
                statusText.text = "Locked";
            
            if (buyButton != null)
                buyButton.interactable = true;
            return;
        }

        PlayerPrefs.SetInt("SelectedSkinIndex", selectedskin);
        PlayerPrefs.Save();

        SceneManager.LoadScene("MainGame");
    }

    public void OpenMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void BuySelectedSkin()
    {
        if (!IsUnlocked(selectedskin))
        {
            int cost = GetCost(selectedskin);
            var mgr = ShellCoinManager.instance;

            if (mgr != null && mgr.TrySpend(cost))
            {
                Unlock(selectedskin);
                UpdateUIForSelectedSkin();
                SelectCurrentSkin();
                mgr.SaveNow();
            }
            else
            {
                if (statusText != null)
                    statusText.text = $"Locked";
            }
        }
        else
        {
            SelectCurrentSkin();
        }
    }

    private void SelectCurrentSkin()
    {
        PlayerPrefs.SetInt(PrefKeySelected, selectedskin);
        PlayerPrefs.Save();
        if (statusText != null) statusText.text = "Selected";
    }

    private bool IsUnlocked(int index)
    {
        if (index == 0) return true;
        return PlayerPrefs.GetInt(PrefKeyUnlockedPrefix + index, 0) == 1;
    }

    private void Unlock(int index)
    {
        PlayerPrefs.SetInt(PrefKeyUnlockedPrefix + index, 1);
        PlayerPrefs.Save();
    }

    private void EnsureUnlocked(int index)
    {
        if (!IsUnlocked(index))
            Unlock(index);
    }

    private int GetCost(int index)
    {
        if (skinCosts != null && index >= 0 && index < skinCosts.Count)
            return Mathf.Max(0, skinCosts[index]);
        return 0;
    }

    private void SetButtonText(Button btn, string text)
    {
        if (btn == null) return;
        var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) { tmp.text = text; return; }
        var uiText = btn.GetComponentInChildren<Text>();
        if (uiText != null) { uiText.text = text; return; }
    }
}