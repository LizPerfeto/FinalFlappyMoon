using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Text.RegularExpressions;
#endif

public class SkinSprites : MonoBehaviour
{
    public Sprite fullLife;
    public Sprite dmg1;
    public Sprite dmg2;
    public Sprite dead;

    public Sprite GetSpriteForVida(int vida)
    {
        switch (vida)
        {
            case 3: return fullLife;
            case 2: return dmg1;
            case 1: return dmg2;
            default: return dead;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Auto Fill From fullLife name")]
    private void AutoFillFromFullLifeName()
    {
        if (fullLife == null)
            return;

        string name = fullLife.name;
        var m = Regex.Match(name, @"^(.*?[_\-]?)(\d+)$");
        if (!m.Success)
            return;

        string prefix = m.Groups[1].Value;
        if (!int.TryParse(m.Groups[2].Value, out int baseNum))
            return;

        Sprite FindSpriteByExactName(string targetName)
        {
            var guids = AssetDatabase.FindAssets($"t:Sprite {targetName}");
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                var assetName = System.IO.Path.GetFileNameWithoutExtension(path);
                if (assetName == targetName)
                {
                    var sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    if (sp != null) return sp;
                }
            }
            return null;
        }

        dmg1 = FindSpriteByExactName(prefix + (baseNum + 1)) ?? dmg1;
        dmg2 = FindSpriteByExactName(prefix + (baseNum + 2)) ?? dmg2;
        dead = FindSpriteByExactName(prefix + (baseNum + 3)) ?? dead;

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
