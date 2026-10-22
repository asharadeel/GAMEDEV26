using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// adds the portal piece slots to the hub portals
// open the hub scene then click tools > add portal slots
// level 1's slot is purple (already open), level 2's is dark grey until the piece is placed
// running it again replaces them
// save the scene after (ctrl + s)
public static class PortalSlotBuilder
{
    [MenuItem("Tools/Add Portal Slots")]
    public static void Build()
    {
        GameObject portal1 = GameObject.Find("Level1Portal");
        GameObject portal2 = GameObject.Find("Level2Portal");
        GameObject canvas = GameObject.Find("Canvas");
        if (portal1 == null || portal2 == null || canvas == null)
        {
            EditorUtility.DisplayDialog("Add Portal Slots", "Open the Hub scene first (it needs Level1Portal, Level2Portal and Canvas).", "OK");
            return;
        }

        // remove old ones
        RemoveChild(portal1.transform, "PortalSlot");
        RemoveChild(portal2.transform, "PortalSlot");
        RemoveChild(canvas.transform, "PlacePiecePrompt");
        PortalSlot oldSlot = portal2.GetComponent<PortalSlot>();
        if (oldSlot != null)
            Undo.DestroyObjectImmediate(oldSlot);

        Material purple = Mat("Glow/GlowPurple");

        // the square on the inner pillar of each portal
        MakeSlot(portal1.transform, "PillarRight", purple);
        Renderer slot2 = MakeSlot(portal2.transform, "PillarLeft", Mat("Colours/DarkGrey"));

        GameObject prompt = MakePrompt(canvas.transform);

        PortalSlot slot = Undo.AddComponent<PortalSlot>(portal2);
        slot.slot = slot2;
        Transform circle = portal2.transform.Find("Circle");
        if (circle != null)
            slot.circle = circle.GetComponent<Renderer>();
        slot.openMaterial = purple;
        slot.approach = portal2.GetComponentInChildren<RadiusUI>(true);
        slot.placePrompt = prompt;
        slot.hubPortal = portal2.GetComponent<HubPortal>();

        // the place button runs place on the portal slot
        Button place = prompt.transform.Find("Button").GetComponent<Button>();
        UnityEventTools.AddPersistentListener(place.onClick, slot.Place);
        PrefabUtility.RecordPrefabInstancePropertyModifications(place);

        EditorSceneManager.MarkSceneDirty(portal2.scene);
        EditorUtility.DisplayDialog("Add Portal Slots", "Slots added. Save the scene (Ctrl + S) to keep them.", "OK");
    }

    static Renderer MakeSlot(Transform portal, string pillarName, Material mat)
    {
        Transform pillar = portal.Find(pillarName);
        Vector3 pos = pillar != null ? pillar.localPosition : Vector3.zero;

        // on the front of the pillar facing into the cave, near the top
        GameObject slot = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Undo.RegisterCreatedObjectUndo(slot, "Add Portal Slots");
        slot.name = "PortalSlot";
        Object.DestroyImmediate(slot.GetComponent<Collider>());
        slot.transform.SetParent(portal, false);
        slot.transform.localPosition = new Vector3(pos.x, 3.8f, 0.55f);
        slot.transform.localScale = new Vector3(0.7f, 0.7f, 0.1f);
        Renderer r = slot.GetComponent<Renderer>();
        r.sharedMaterial = mat;
        return r;
    }

    // container > frame, same as every other menu (ui policy)
    static GameObject MakePrompt(Transform canvas)
    {
        GameObject container = new GameObject("PlacePiecePrompt", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(container, "Add Portal Slots");
        container.transform.SetParent(canvas, false);
        RectTransform rt = container.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UI/GeneralPopUpWindow.prefab");
        GameObject frame = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container.transform);
        frame.name = "Frame";

        SetText(frame.transform.Find("Title/Label"), "You have a Portal Piece\nPlace it to open the portal?");
        SetText(frame.transform.Find("Button/Text (TMP)"), "Place");
        return frame;
    }

    static void SetText(Transform t, string text)
    {
        if (t == null)
            return;
        Text label = t.GetComponent<Text>();
        label.text = text;
        PrefabUtility.RecordPrefabInstancePropertyModifications(label);
    }

    static void RemoveChild(Transform parent, string name)
    {
        Transform old = parent.Find(name);
        if (old != null)
            Undo.DestroyObjectImmediate(old.gameObject);
    }

    static Material Mat(string name)
    {
        Material m = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Palette/" + name + ".mat");
        if (m == null)
            m = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
        return m;
    }
}
