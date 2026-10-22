using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// builds the hub cave scene out of basic shapes
// click tools > build hub in the top menu
// it makes assets/scenes/hub.unity and puts it first in build settings
// after that its a normal scene so you can move stuff around and save it
public static class HubBuilder
{
    const string scenePath = "Assets/Scenes/Hub.unity";

    // cave size
    const float caveRadius = 17f;
    const int wallPieces = 40;

    // where things go around the cave, in degrees (0 is the back wall, minus is left)
    const float introAngle = 0f;
    const float level1Angle = -45f;
    const float level2Angle = 45f;
    const float shopAngle = -95f;
    const float settingsAngle = 95f;

    static Font font;
    static Sprite ring;
    static Transform canvas;
    static GameObject lockedMessage;

    [MenuItem("Tools/Build Hub")]
    public static void Build()
    {
        if (System.IO.File.Exists(scenePath))
        {
            if (!EditorUtility.DisplayDialog("Build Hub", "Hub.unity already exists. Replace it?", "Replace", "Cancel"))
                return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/arial/ARIAL.TTF");
        if (font == null)
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        ring = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/circleborder.png");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        Camera cam = Camera.main;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.08f, 0.08f);

        // plain white light so everything is easy to see
        Light sun = Object.FindFirstObjectByType<Light>();
        sun.intensity = 1f;
        sun.color = Color.white;
        sun.transform.rotation = Quaternion.Euler(60f, -30f, 0f);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.4f, 0.4f, 0.4f);

        MakeUI();
        MakeFloor();
        MakeWalls();

        GameObject things = new GameObject("Hub");

        GameObject introMenu = MakeMenu("IntroMenu", "Welcome");
        GameObject shopMenu = MakeMenu("ShopMenu", "Shop");
        GameObject settingsMenu = MakeMenu("SettingsMenu", "Settings");

        MakeIntroWall(things.transform, introMenu);
        MakePortal(things.transform, "Level1Portal", level1Angle, "LEVEL 1\nThe Plains", "Scene 1 Plains", false);
        MakePortal(things.transform, "Level2Portal", level2Angle, "LEVEL 2\nLocked", "", true);
        MakeStation(things.transform, "Shop", shopAngle, "SHOP", shopMenu);
        MakeStation(things.transform, "Settings", settingsAngle, "SETTINGS", settingsMenu);

        MakePlayer(cam);

        GameObject events = new GameObject("EventSystem");
        events.AddComponent<EventSystem>();
        events.AddComponent<StandaloneInputModule>();

        EditorSceneManager.SaveScene(scene, scenePath);
        AddToBuildSettings();

        EditorUtility.DisplayDialog("Build Hub", "Made " + scenePath + " and put it first in build settings. Press play to try it.", "OK");
    }

    // floor

    static void MakeFloor()
    {
        GameObject ground = Shape(PrimitiveType.Cylinder, "Floor", null, new Vector3(0f, -0.25f, 0f),
            new Vector3(caveRadius * 2f + 4f, 0.25f, caveRadius * 2f + 4f), Mat("Colours/Grey"));

        // swap the round collider for a flat box so the floor is flat
        Object.DestroyImmediate(ground.GetComponent<Collider>());
        BoxCollider box = ground.AddComponent<BoxCollider>();
        box.size = new Vector3(1f, 2f, 1f);
    }

    // round cave walls

    static void MakeWalls()
    {
        GameObject walls = new GameObject("CaveWalls");
        walls.AddComponent<WallHider>();

        float pieceWidth = 2f * Mathf.PI * caveRadius / wallPieces * 1.25f;

        for (int i = 0; i < wallPieces; i++)
        {
            float angle = i * 360f / wallPieces;
            Vector3 dir = Dir(angle);

            // each piece faces the middle and leans in a little
            GameObject piece = Shape(PrimitiveType.Cube, "Wall " + i, walls.transform, dir * (caveRadius + 0.8f) + Vector3.up * 3f,
                new Vector3(pieceWidth, 6.5f, 2f), Mat("Colours/DarkGrey"));
            piece.transform.rotation = Quaternion.LookRotation(-dir) * Quaternion.Euler(8f, 0f, 0f);
        }
    }

    // intro message on the wall

    static void MakeIntroWall(Transform parent, GameObject menu)
    {
        GameObject wall = Spot(parent, "IntroWall", introAngle, caveRadius - 0.25f);

        // writing straight on the wall, change this to whatever you want the intro to say
        Quaternion tilt = Quaternion.Euler(8f, 0f, 0f);
        GameObject writing = WorldText(wall.transform, "Writing", new Vector2(650f, 420f), new Vector3(0f, 3f, 0f),
            "WELCOME\n\n<size=30>this cave is the hub\nstep into a portal to start a level\nvisit the shop and settings around the cave</size>",
            48, Color.white);
        writing.transform.localScale = Vector3.one * 0.01f;
        writing.transform.localRotation = tilt * Quaternion.Euler(0f, 180f, 0f);

        // menu pops up when you get close enough to read it
        Zone(wall.transform, new Vector3(0f, 0f, 3.5f), 3.5f, menu);
    }

    // portals

    static void MakePortal(Transform parent, string name, float angle, string label, string sceneName, bool locked)
    {
        GameObject portal = Spot(parent, name, angle, caveRadius - 2.2f);

        // two pillars
        Shape(PrimitiveType.Cube, "PillarLeft", portal.transform, new Vector3(-2.3f, 2.5f, 0f), new Vector3(1f, 5f, 1f), Mat("Colours/LightGrey"));
        Shape(PrimitiveType.Cube, "PillarRight", portal.transform, new Vector3(2.3f, 2.5f, 0f), new Vector3(1f, 5f, 1f), Mat("Colours/LightGrey"));

        // the portal is just a flat circle between them, white if open and dark if locked
        Shape(PrimitiveType.Cylinder, "Circle", portal.transform, new Vector3(0f, 2.5f, 0f), new Vector3(3.6f, 0.05f, 3.6f),
            Mat(locked ? "Colours/Black" : "Colours/White"), new Vector3(90f, 0f, 0f), collider: false);

        Label(portal.transform, label, new Vector3(0f, 6f, 0f), locked ? Color.grey : Color.white);

        // walk in here to go through
        BoxCollider trigger = portal.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.center = new Vector3(0f, 1.5f, 0.3f);
        trigger.size = new Vector3(3.4f, 3f, 1.2f);

        HubPortal hubPortal = portal.AddComponent<HubPortal>();
        hubPortal.sceneName = sceneName;
        hubPortal.locked = locked;
        if (locked)
            hubPortal.lockedMessage = lockedMessage;
    }

    // shop and settings, just a name and a ring for now

    static void MakeStation(Transform parent, string name, float angle, string label, GameObject menu)
    {
        GameObject station = Spot(parent, name, angle, caveRadius - 3f);
        Label(station.transform, label, new Vector3(0f, 2.5f, 0f), Color.white);
        Zone(station.transform, Vector3.zero, 2.5f, menu);
    }

    // player

    static void MakePlayer(Camera cam)
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 0.6f, 0f);

        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Player.mat");
        if (mat != null)
            player.GetComponent<Renderer>().sharedMaterial = mat;

        player.AddComponent<Rigidbody>();
        player.AddComponent<PlayerStats>();
        PlayerController controller = player.AddComponent<PlayerController>();

        // same camera as the plains scene
        controller.cameraSettings.camera = cam;
        controller.cameraSettings.pitch = 50f;
        controller.cameraSettings.yaw = 10f;
        controller.cameraSettings.roll = 0f;
        controller.cameraSettings.distance = 50f;
        controller.cameraSettings.orthographicSize = 10f;
        controller.cameraSettings.startZoom = 10f;

        // put the camera in the right place before pressing play
        Quaternion rot = Quaternion.Euler(50f, 10f, 0f);
        cam.transform.rotation = rot;
        cam.transform.position = player.transform.position - rot * Vector3.forward * 50f;
        cam.orthographic = true;
        cam.orthographicSize = 10f;
    }

    // menus (empty for now)

    static void MakeUI()
    {
        GameObject ui = new GameObject("Canvas");
        Canvas c = ui.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = ui.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        ui.AddComponent<GraphicRaycaster>();
        canvas = ui.transform;

        // message at the top for the locked portal
        lockedMessage = new GameObject("LockedMessage", typeof(RectTransform));
        lockedMessage.transform.SetParent(canvas, false);
        RectTransform rt = lockedMessage.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -60f);
        rt.sizeDelta = new Vector2(900f, 80f);
        Image bg = lockedMessage.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.7f);
        UIText(lockedMessage.transform, "Text", "This portal is locked", 36, Color.white, Vector2.zero, new Vector2(900f, 80f));
        lockedMessage.SetActive(false);
    }

    // empty panel with a title and a close button, ready to be filled in
    static GameObject MakeMenu(string name, string title)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform));
        panel.transform.SetParent(canvas, false);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(1000f, 650f);
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.12f, 0.12f, 0.12f, 0.95f);

        UIText(panel.transform, "Title", title, 56, Color.white, new Vector2(0f, 260f), new Vector2(900f, 90f));

        // close button in the bottom right
        GameObject button = new GameObject("CloseButton", typeof(RectTransform));
        button.transform.SetParent(panel.transform, false);
        RectTransform brt = button.GetComponent<RectTransform>();
        brt.anchoredPosition = new Vector2(370f, -270f);
        brt.sizeDelta = new Vector2(200f, 70f);
        Image bimg = button.AddComponent<Image>();
        bimg.color = new Color(0.3f, 0.3f, 0.3f);
        Button b = button.AddComponent<Button>();
        b.targetGraphic = bimg;
        UIText(button.transform, "Text", "Close", 32, Color.white, Vector2.zero, new Vector2(200f, 70f));

        CloseElemOnClick close = button.AddComponent<CloseElemOnClick>();
        close.elements = new GameObject[] { panel };

        panel.SetActive(false);
        return panel;
    }

    static Text UIText(Transform parent, string name, string text, int size, Color colour, Vector2 pos, Vector2 boxSize)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = boxSize;

        Text t = go.AddComponent<Text>();
        t.text = text;
        t.font = font;
        t.fontSize = size;
        t.color = colour;
        t.alignment = TextAnchor.MiddleCenter;
        t.raycastTarget = false;
        return t;
    }

    // helpers

    // direction on the floor from an angle, 0 is forward (+z)
    static Vector3 Dir(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    }

    // empty object at the edge of the cave facing the middle
    static GameObject Spot(Transform parent, string name, float angle, float distance)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.position = Dir(angle) * distance;
        go.transform.rotation = Quaternion.LookRotation(-Dir(angle));
        return go;
    }

    // the spot you stand on to open a menu, with a ring on the floor
    // (uses the Models/ShowUIonApproach prefab, see RadiusUI)
    static void Zone(Transform parent, Vector3 localPos, float radius, GameObject menu)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/ShowUIonApproach.prefab");
        GameObject zone = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        zone.transform.SetParent(parent, false);
        zone.transform.localPosition = localPos;

        RadiusUI radiusUI = zone.GetComponent<RadiusUI>();
        radiusUI.ui = menu;
        radiusUI.radius = radius;
    }

    // floating name above something, always faces the camera
    static void Label(Transform parent, string text, Vector3 localPos, Color colour)
    {
        GameObject go = WorldText(parent, "Label", new Vector2(600f, 200f), localPos, text, 70, colour);
        go.transform.localScale = Vector3.one * 0.012f;
        go.AddComponent<FaceCamera>();
    }

    static GameObject WorldText(Transform parent, string name, Vector2 size, Vector3 localPos, string text, int fontSize, Color colour)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;

        Canvas c = go.AddComponent<Canvas>();
        c.renderMode = RenderMode.WorldSpace;
        go.AddComponent<CanvasScaler>().dynamicPixelsPerUnit = 4f;
        go.GetComponent<RectTransform>().sizeDelta = size;

        Text t = UIText(go.transform, "Text", text, fontSize, colour, Vector2.zero, size);
        t.fontStyle = FontStyle.Bold;
        t.supportRichText = true;
        return go;
    }

    static GameObject Shape(PrimitiveType type, string name, Transform parent, Vector3 localPos, Vector3 scale, Material mat, Vector3 rotation = default, bool collider = true)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localEulerAngles = rotation;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;

        Collider col = go.GetComponent<Collider>();
        if (!collider)
        {
            Object.DestroyImmediate(col);
        }
        else if (type == PrimitiveType.Cylinder)
        {
            // cylinders come with a capsule collider which is round, use the real shape instead
            Object.DestroyImmediate(col);
            go.AddComponent<MeshCollider>();
        }

        return go;
    }

    // loads a material from the palette folder
    static Dictionary<string, Material> mats = new Dictionary<string, Material>();
    static Material Mat(string name)
    {
        if (mats.TryGetValue(name, out Material m) && m != null)
            return m;

        m = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Palette/" + name + ".mat");
        if (m == null)
        {
            Debug.LogWarning("missing material " + name);
            m = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
        }

        mats[name] = m;
        return m;
    }

    static void AddToBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
        {
            if (s.path != scenePath)
                scenes.Add(s);
        }
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
