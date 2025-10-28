using UnityEngine;

public class ReturnToMenuButton : MonoBehaviour
{
    public bool discardProgress = true;

    public void OnReturnToMenuPressed()
    {
        if (GameManager.instance == null) return;

        if (discardProgress)
            GameManager.instance.ReturnToMenuDiscardProgress();
        else
            GameManager.instance.ReturnToMenuFromGameOver();
    }
}
