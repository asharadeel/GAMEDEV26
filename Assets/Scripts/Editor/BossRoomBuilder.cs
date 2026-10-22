using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// adds the boss fight to the hex room in scene 1 plains
// open scene 1 plains then click tools > build boss room
// it puts everything in the hex room and adds the portal piece pop up to the canvas
// running it again replaces the old boss arena
// save the scene after (ctrl + s)
// ai use: written with claude (anthropic), owner: cthullu96
public static class BossRoomBuilder
{
    const float pillarDistance = 28f;   // how far the pillars are from the middle

    static Material bodyMat;

    [MenuItem("Tools/Build Boss Room")]
    public static void Build()
    {
        GameObject hexRoom = GameObject.Find("HexRoom");
        GameObject canvas = GameObject.Find("Canvas");
        if (hexRoom == null || canvas == null)
        {
            EditorUtility.DisplayDialog("Build Boss Room", "Open Scene 1 Plains first (it needs HexRoom and Canvas).", "OK");
            return;
        }

        // remove the old one if its already there
        Transform old = hexRoom.transform.Find("BossArena");
        if (old != null)
            Undo.DestroyObjectImmediate(old.gameObject);
        Transform oldMenu = canvas.transform.Find("PortalPieceMenu");
        if (oldMenu != null)
            Undo.DestroyObjectImmediate(oldMenu.gameObject);
        GameObject oldSkip = GameObject.Find("PrototypeSkip");
        if (oldSkip != null)
            Undo.DestroyObjectImmediate(oldSkip);

        // find the middle of the room and the top of the floor
        Bounds floor = FloorBounds(hexRoom.transform.Find("Floor"));
        Vector3 centre = new Vector3(floor.center.x, floor.max.y, floor.center.z);

        GameObject arena = new GameObject("BossArena");
        Undo.RegisterCreatedObjectUndo(arena, "Build Boss Room");
        arena.transform.SetParent(hexRoom.transform, false);
        arena.transform.position = centre;

        GameObject middle = new GameObject("ArenaCentre");
        middle.transform.SetParent(arena.transform, false);

        GameObject popupFrame = MakePopup(canvas.transform);
        GameObject piece = MakePortalPiece(arena.transform, popupFrame);
        GameObject warning = MakeWarningRing(arena.transform);

        // six pillars on the corners of the hexagon
        List<BossPillar> pillars = new List<BossPillar>();
        for (int i = 0; i < 6; i++)
        {
            float angle = (30f + i * 60f) * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * pillarDistance;
            pillars.Add(MakePillar(arena.transform, "Pillar " + (i + 1), pos));
        }

        BossMob boss = MakeBoss(arena.transform);
        boss.bossData = BossData();
        boss.healthBarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UI/MobHealthBar.prefab");
        boss.arenaCentre = middle.transform;
        boss.pillars = pillars.ToArray();
        boss.portalPiece = piece;
        boss.slamWarning = warning;
        boss.boulderModel = BoulderPrefab();

        MakeSkipPad(hexRoom.transform.Find("DoorSpawn"));

        EditorSceneManager.MarkSceneDirty(hexRoom.scene);
        Selection.activeGameObject = boss.gameObject;
        EditorUtility.DisplayDialog("Build Boss Room", "Boss room added to HexRoom. Save the scene (Ctrl + S) to keep it.", "OK");
    }

    // the boss

    static BossMob MakeBoss(Transform parent)
    {
        GameObject root = new GameObject("Boss");
        root.transform.SetParent(parent, false);

        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.mass = 50f;
        rb.freezeRotation = true;

        CapsuleCollider col = root.AddComponent<CapsuleCollider>();
        col.radius = 1.5f;
        col.height = 6f;
        col.center = new Vector3(0f, 3f, 0f);

        // pill shaped body
        bodyMat = Mat("Colours/Purple");
        GameObject body = Shape(PrimitiveType.Capsule, "Body", root.transform, new Vector3(0f, 3f, 0f), new Vector3(3f, 3f, 3f), bodyMat);

        // eyes so you can tell which way its facing
        for (int side = -1; side <= 1; side += 2)
        {
            Shape(PrimitiveType.Sphere, "Eye", root.transform, new Vector3(side * 0.55f, 4.6f, 1.3f), Vector3.one * 0.6f, Mat("Colours/White"));
            Shape(PrimitiveType.Sphere, "Pupil", root.transform, new Vector3(side * 0.55f, 4.6f, 1.58f), Vector3.one * 0.3f, Mat("Colours/Black"));
        }

        // club on the right side, swings around this pivot
        GameObject pivot = new GameObject("ClubPivot");
        pivot.transform.SetParent(root.transform, false);
        pivot.transform.localPosition = new Vector3(1.4f, 2.8f, 0f);
        pivot.transform.localRotation = Quaternion.Euler(0f, 60f, 0f);
        Shape(PrimitiveType.Capsule, "Club", pivot.transform, new Vector3(0f, 0f, 2.2f), new Vector3(0.7f, 2f, 0.7f), Mat("Surfaces/DarkWood"), new Vector3(90f, 0f, 0f));

        // left hand holds the boulder before throwing
        GameObject hand = new GameObject("Hand");
        hand.transform.SetParent(root.transform, false);
        hand.transform.localPosition = new Vector3(-1.9f, 1.8f, 0.8f);
        GameObject held = Shape(PrimitiveType.Sphere, "HeldBoulder", hand.transform, Vector3.zero, Vector3.one * 1.2f, Mat("Surfaces/Stone"));

        // see through bubble while its shielded
        GameObject shield = Shape(PrimitiveType.Sphere, "Shield", root.transform, new Vector3(0f, 3f, 0f), new Vector3(5f, 7f, 5f), Mat("Glass/GlassBlue"));

        BossMob boss = root.AddComponent<BossMob>();
        boss.body = body.GetComponent<Renderer>();
        boss.shield = shield;
        boss.clubPivot = pivot.transform;
        boss.hand = hand.transform;
        boss.heldBoulder = held;

        // face the door on the west side
        root.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        return boss;
    }

    static MobData BossData()
    {
        const string path = "Assets/Data/Mobs/Boss.asset";
        MobData data = AssetDatabase.LoadAssetAtPath<MobData>(path);
        if (data != null)
            return data;

        data = ScriptableObject.CreateInstance<MobData>();
        data.id = "boss_level_1";
        data.displayName = "Plains Guardian";
        data.shortName = "Boss";
        data.type = MobType.Follow;
        data.health = 1500f;
        data.attackDamage = 25f;
        data.moveSpeed = 2.5f;
        data.chaseRange = 60f;
        data.attackRange = 2f;
        data.attackCooldown = 1.5f;
        data.drops = new LootEntry[0];
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
        return data;
    }

    static GameObject BoulderPrefab()
    {
        const string path = "Assets/Models/Boss/Boulder.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab != null)
            return prefab;

        if (!AssetDatabase.IsValidFolder("Assets/Models/Boss"))
            AssetDatabase.CreateFolder("Assets/Models", "Boss");

        GameObject boulder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        boulder.name = "Boulder";
        boulder.transform.localScale = Vector3.one * 1.2f;
        boulder.GetComponent<Renderer>().sharedMaterial = Mat("Surfaces/Stone");
        prefab = PrefabUtility.SaveAsPrefabAsset(boulder, path);
        Object.DestroyImmediate(boulder);
        return prefab;
    }

    // pillars

    static BossPillar MakePillar(Transform parent, string name, Vector3 localPos)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);
        root.transform.localPosition = localPos;

        Shape(PrimitiveType.Cube, "Base", root.transform, new Vector3(0f, 0.4f, 0f), new Vector3(2.2f, 0.8f, 2.2f), Mat("Surfaces/Stone"), collider: true);
        GameObject crystal = Shape(PrimitiveType.Cube, "Crystal", root.transform, new Vector3(0f, 2.3f, 0f), new Vector3(1.2f, 3f, 1.2f), Mat("Glow/GlowPurple"), new Vector3(0f, 45f, 0f), collider: true);

        LineRenderer beam = root.AddComponent<LineRenderer>();
        beam.sharedMaterial = Mat("Glow/GlowPurple");
        beam.startWidth = 0.15f;
        beam.endWidth = 0.15f;
        beam.positionCount = 2;
        beam.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        BossPillar pillar = root.AddComponent<BossPillar>();
        pillar.crystal = crystal;
        pillar.beam = beam;
        pillar.healthBarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UI/MobHealthBar.prefab");
        return pillar;
    }

    // testing shortcut next to the path before the maze, press e in the circle to go straight to the boss
    // delete the prototype skip object when its not needed any more

    static void MakeSkipPad(Transform destination)
    {
        GameObject spawn = GameObject.Find("PlayerSpawn");
        if (spawn == null || destination == null)
            return;

        GameObject pad = new GameObject("PrototypeSkip");
        Undo.RegisterCreatedObjectUndo(pad, "Build Boss Room");
        pad.transform.position = new Vector3(spawn.transform.position.x + 8f, 0.55f, spawn.transform.position.z + 20f);

        Shape(PrimitiveType.Cylinder, "Pad", pad.transform, Vector3.zero, new Vector3(3f, 0.03f, 3f), Mat("Colours/Yellow"));

        GameObject ring = new GameObject("RadiusRing");
        ring.transform.SetParent(pad.transform, false);
        ring.transform.localPosition = new Vector3(0f, 0.05f, 0f);
        ring.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        RingLine.Add(ring, Color.yellow);

        // same door script the maze end room uses
        DoorInteract door = pad.AddComponent<DoorInteract>();
        door.destination = destination;
        door.radius = 2.5f;
        door.showRingWithin = 12f;
        door.radiusRing = ring;

        // floating label
        GameObject label = new GameObject("Label", typeof(RectTransform));
        label.transform.SetParent(pad.transform, false);
        label.transform.localPosition = new Vector3(0f, 3f, 0f);
        label.transform.localScale = Vector3.one * 0.012f;
        Canvas c = label.AddComponent<Canvas>();
        c.renderMode = RenderMode.WorldSpace;
        label.AddComponent<CanvasScaler>().dynamicPixelsPerUnit = 4f;
        label.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, 200f);
        label.AddComponent<FaceCamera>();

        GameObject textObj = new GameObject("Text", typeof(RectTransform));
        textObj.transform.SetParent(label.transform, false);
        textObj.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, 200f);
        Text text = textObj.AddComponent<Text>();
        text.text = "PROTOTYPE SKIP\n<size=40>press E to go to the boss</size>";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 60;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.yellow;
        text.raycastTarget = false;
    }

    // red ring on the floor showing where the boss will land

    static GameObject MakeWarningRing(Transform parent)
    {
        GameObject ring = new GameObject("SlamWarning");
        ring.transform.SetParent(parent, false);
        ring.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        RingLine.Add(ring, new Color(1f, 0.2f, 0.2f), 0.4f);
        ring.SetActive(false);
        return ring;
    }

    // portal piece and its pop up

    static GameObject MakePortalPiece(Transform parent, GameObject popupFrame)
    {
        GameObject piece = new GameObject("PortalPiece");
        piece.transform.SetParent(parent, false);

        GameObject model = Shape(PrimitiveType.Cube, "Model", piece.transform, new Vector3(0f, 1.5f, 0f), Vector3.one * 1.2f, Mat("Glow/GlowPurple"), new Vector3(45f, 0f, 45f));
        Light light = model.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(0.7f, 0.4f, 1f);
        light.range = 8f;
        light.intensity = 2f;

        piece.AddComponent<PortalPiece>().model = model.transform;

        // opens the pop up when the player walks onto it (see ui policy rule 9)
        GameObject approachPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/ShowUIonApproach.prefab");
        GameObject approach = (GameObject)PrefabUtility.InstantiatePrefab(approachPrefab, piece.transform);
        RadiusUI radius = approach.GetComponent<RadiusUI>();
        radius.ui = popupFrame;
        radius.radius = 2.5f;
        radius.showRingWithin = 10f;
        PrefabUtility.RecordPrefabInstancePropertyModifications(radius);

        piece.SetActive(false);
        return piece;
    }

    // container > frame, same as every other menu (ui policy)
    static GameObject MakePopup(Transform canvas)
    {
        GameObject container = new GameObject("PortalPieceMenu", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(container, "Build Boss Room");
        container.transform.SetParent(canvas, false);
        RectTransform rt = container.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UI/GeneralPopUpWindow.prefab");
        GameObject frame = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container.transform);
        frame.name = "Frame";

        SetText(frame.transform.Find("Title/Label"), "You found a Portal Piece!\nTake it back to the Hub");
        SetText(frame.transform.Find("Button/Text (TMP)"), "Back to Hub");

        // both buttons go back to the hub
        AddOpenScene(frame.transform.Find("Button"));
        AddOpenScene(frame.transform.Find("CloseButton"));

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

    static void AddOpenScene(Transform button)
    {
        if (button == null)
            return;
        OpenScene open = button.gameObject.AddComponent<OpenScene>();
        open.sceneName = "Hub";
    }

    // helpers

    static Bounds FloorBounds(Transform floor)
    {
        Renderer[] renderers = floor.GetComponentsInChildren<Renderer>();
        Bounds b = renderers[0].bounds;
        foreach (Renderer r in renderers)
            b.Encapsulate(r.bounds);
        return b;
    }

    static GameObject Shape(PrimitiveType type, string name, Transform parent, Vector3 localPos, Vector3 scale, Material mat, Vector3 rotation = default, bool collider = false)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localEulerAngles = rotation;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        if (!collider)
            Object.DestroyImmediate(go.GetComponent<Collider>());
        return go;
    }

    // loads a material from the palette folder
    static Material Mat(string name)
    {
        Material m = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Palette/" + name + ".mat");
        if (m == null)
        {
            Debug.LogWarning("missing material " + name);
            m = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
        }
        return m;
    }
}
