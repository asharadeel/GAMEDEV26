// ============================================================
// Title:       SkillNodeUI
// Version:     1.0
// Written by:  Ali
// Description: One circle in the skill tree (the SkillNode prefab).
//              SkillTreeUI places and fills these in, nothing to
//              set up by hand apart from the prefab's parts.
// ============================================================

using UnityEngine;
using UnityEngine.UI;

public class SkillNodeUI : MonoBehaviour
{
    public Button button;
    public Image circle;
    public Text title;      // under the circle: name, effect, cost
    public Text icon;       // inside the circle: short text, e.g. a tick when unlocked
}
