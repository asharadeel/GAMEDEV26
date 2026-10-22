using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// builds the shooter upgrade window in the hub and the ammo counter in scene 1
// click tools > build shooter upgrades (it opens and saves both scenes itself)
// hub: shop title set to "Shop", Canvas > ShooterUpgradePopup (container + ShooterUpgradeUI) > Frame (GeneralPopUpWindow copy)
//      with one bar per ShooterStat: label, 5 blocks, value, Buy button
//      placed just after StoreUI so it covers the shop but BuyPopup still shows on top of it
// scene 1: PlayerUI > Frame > AmmoCounter, wired into PlayerHUD
// running it again replaces them
public static class ShooterUpgradeBuilder
{
    const string HubPath = "Assets/Scenes/Hub.unity";
    const string PlainsPath = "Assets/Scenes/Scene 1 Plains.unity";
    const string FontPath = "Assets/Fonts/arial/ARIAL.TTF";

    static readonly string[] BarNames = ShooterUpgradeUI.BarNames;

    [MenuItem("Tools/Build Shooter Upgrades")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorUtility.DisplayDialog("Build Shooter Upgrades", "Stop Play mode first, then run this again.", "OK");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        Scene hub = EditorSceneManager.OpenScene(HubPath);
        if (!BuildUpgradeWindow())
            return;
        EditorSceneManager.SaveScene(hub);

        Scene plains = EditorSceneManager.OpenScene(PlainsPath);
        if (!BuildAmmoCounter())
            return;
        EditorSceneManager.SaveScene(plains);

        EditorSceneManager.OpenScene(HubPath);
        EditorUtility.DisplayDialog("Build Shooter Upgrades", "Done. The Hub has the upgrade window and Scene 1 has the ammo counter (both saved).", "OK");
    }

    // ---- Hub ----

    static bool BuildUpgradeWindow()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null || canvas.transform.Find("StoreUI") == null)
        {
            EditorUtility.DisplayDialog("Build Shooter Upgrades", "The Hub needs a Canvas with StoreUI in it.", "OK");
            return false;
        }

        Transform shopTitle = canvas.transform.Find("StoreUI/GeneralSelectorUI/Title/Label");
        if (shopTitle != null)
        {
            Text shopLabel = shopTitle.GetComponent<Text>();
            Undo.RecordObject(shopLabel, "Build Shooter Upgrades");
            shopLabel.text = "Shop";
        }

        RemoveChild(canvas.transform, "ShooterUpgradePopup");

        // container > frame, same as every other menu (ui policy). Right after StoreUI: draws over the shop, under BuyPopup
        Transform store = canvas.transform.Find("StoreUI");
        GameObject container = Stretched("ShooterUpgradePopup", canvas.transform);
        container.transform.SetSiblingIndex(store.GetSiblingIndex() + 1);

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UI/GeneralPopUpWindow.prefab");
        GameObject frame = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container.transform);
        frame.name = "Frame";

        // bigger than a normal pop-up so the 5 bars fit (1200 x 800)
        RectTransform frameRect = frame.GetComponent<RectTransform>();
        frameRect.offsetMin = new Vector2(360f, 140f);
        frameRect.offsetMax = new Vector2(-360f, -140f);
        PrefabUtility.RecordPrefabInstancePropertyModifications(frameRect);

        // no single action button, every bar has its own
        GameObject action = frame.transform.Find("Button").gameObject;
        action.SetActive(false);
        PrefabUtility.RecordPrefabInstancePropertyModifications(action);

        // copy the pop-up's look for the new parts
        Text titleLabel = frame.transform.Find("Title/Label").GetComponent<Text>();
        Image buttonLook = action.GetComponent<Image>();
        Text buttonText = action.transform.Find("Text (TMP)").GetComponent<Text>();

        Text message = MakeText("Coins", frame.transform, "You have 0 coins", 32, TextAnchor.MiddleCenter, titleLabel.color);
        Place(message.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(40f, -160f), new Vector2(-40f, -100f));

        GameObject barsObj = new GameObject("Bars", typeof(RectTransform));
        barsObj.transform.SetParent(frame.transform, false);
        Place((RectTransform)barsObj.transform, Vector2.zero, Vector2.one, new Vector2(40f, 110f), new Vector2(-40f, -170f));

        ShooterUpgradeUI ui = Undo.AddComponent<ShooterUpgradeUI>(container);
        ui.window = frame;
        Transform shopWindow = store.Find("GeneralSelectorUI");
        ui.shop = shopWindow != null ? shopWindow.gameObject : null;
        ui.title = titleLabel;
        ui.message = message;
        ui.bars = new UpgradeBar[BarNames.Length];

        for (int i = 0; i < BarNames.Length; i++)
            ui.bars[i] = MakeBar(barsObj.transform, i, titleLabel.color, buttonLook, buttonText.color, ui.empty);

        EditorUtility.SetDirty(ui);
        EditorSceneManager.MarkSceneDirty(canvas.scene);
        return true;
    }

    // Label | 5 blocks | value | Buy button, one row out of 5
    static UpgradeBar MakeBar(Transform parent, int index, Color textColour, Image buttonLook, Color buttonTextColour, Color emptyColour)
    {
        float rows = BarNames.Length;
        GameObject row = new GameObject(BarNames[index].Replace(" ", "") + "Bar", typeof(RectTransform));
        row.transform.SetParent(parent, false);
        Place((RectTransform)row.transform, new Vector2(0f, 1f - (index + 1) / rows), new Vector2(1f, 1f - index / rows),
              new Vector2(0f, 6f), new Vector2(0f, -6f));

        UpgradeBar bar = new UpgradeBar();
        bar.stat = (ShooterStat)index;

        Text label = MakeText("Label", row.transform, BarNames[index], 30, TextAnchor.MiddleLeft, textColour);
        Place(label.rectTransform, Vector2.zero, new Vector2(0f, 1f), Vector2.zero, new Vector2(220f, 0f));

        bar.blocks = new RawImage[ShooterData.MaxUpgrades];
        for (int b = 0; b < bar.blocks.Length; b++)
        {
            GameObject block = new GameObject("Block" + (b + 1), typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            block.transform.SetParent(row.transform, false);
            RectTransform rt = (RectTransform)block.transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.sizeDelta = new Vector2(52f, 40f);
            rt.anchoredPosition = new Vector2(230f + b * 62f, 0f);
            RawImage img = block.GetComponent<RawImage>();
            img.color = emptyColour;
            img.raycastTarget = false;
            bar.blocks[b] = img;
        }

        bar.value = MakeText("Value", row.transform, "", 26, TextAnchor.MiddleLeft, textColour);
        Place(bar.value.rectTransform, Vector2.zero, Vector2.one, new Vector2(550f, 0f), new Vector2(-190f, 0f));

        GameObject buy = new GameObject("Buy", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buy.transform.SetParent(row.transform, false);
        RectTransform buyRect = (RectTransform)buy.transform;
        buyRect.anchorMin = buyRect.anchorMax = new Vector2(1f, 0.5f);
        buyRect.pivot = new Vector2(1f, 0.5f);
        buyRect.sizeDelta = new Vector2(170f, 64f);
        Image buyImage = buy.GetComponent<Image>();
        buyImage.sprite = buttonLook.sprite;
        buyImage.type = buttonLook.type;
        buyImage.color = buttonLook.color;
        bar.buy = buy.GetComponent<Button>();
        bar.buy.targetGraphic = buyImage;

        bar.buyLabel = MakeText("Text", buy.transform, "Buy", 30, TextAnchor.MiddleCenter, buttonTextColour);
        Place(bar.buyLabel.rectTransform, Vector2.zero, Vector2.one, new Vector2(8f, 4f), new Vector2(-8f, -4f));

        return bar;
    }

    // ---- Scene 1 ----

    static bool BuildAmmoCounter()
    {
        PlayerHUD hud = Object.FindFirstObjectByType<PlayerHUD>();
        Transform frame = hud != null ? hud.transform.Find("Frame") : null;
        if (frame == null)
        {
            EditorUtility.DisplayDialog("Build Shooter Upgrades", "Scene 1 needs PlayerUI (with PlayerHUD) and a Frame inside it.", "OK");
            return false;
        }

        RemoveChild(frame, "AmmoCounter");

        // bottom right, above the thirst bar
        Text ammo = MakeText("AmmoCounter", frame, "20 / 20", 40, TextAnchor.MiddleRight, Color.white);
        RectTransform rt = ammo.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(1f, 0f);
        rt.sizeDelta = new Vector2(320f, 60f);
        rt.anchoredPosition = new Vector2(-30f, 100f);
        ammo.gameObject.AddComponent<Outline>();

        Undo.RecordObject(hud, "Build Shooter Upgrades");
        hud.ammoText = ammo;
        hud.combat = Object.FindFirstObjectByType<PlayerCombat>();
        EditorUtility.SetDirty(hud);
        EditorSceneManager.MarkSceneDirty(hud.gameObject.scene);
        return true;
    }

    // ---- Helpers ----

    static GameObject Stretched(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(obj, "Build Shooter Upgrades");
        obj.transform.SetParent(parent, false);
        Place((RectTransform)obj.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        return obj;
    }

    // ui policy: legacy text, arial, best fit on
    static Text MakeText(string name, Transform parent, string text, int size, TextAnchor align, Color colour)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        obj.transform.SetParent(parent, false);
        Text t = obj.GetComponent<Text>();
        t.font = AssetDatabase.LoadAssetAtPath<Font>(FontPath);
        t.text = text;
        t.fontSize = size;
        t.resizeTextForBestFit = true;
        t.resizeTextMinSize = 10;
        t.resizeTextMaxSize = size;
        t.alignment = align;
        t.color = colour;
        t.raycastTarget = false;
        return t;
    }

    static void Place(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
    }

    static void RemoveChild(Transform parent, string name)
    {
        Transform old = parent.Find(name);
        if (old != null)
            Undo.DestroyObjectImmediate(old.gameObject);
    }
}
