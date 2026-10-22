using UnityEngine;

// ai use: written with claude (anthropic), owner: cthullu96
//
// draws a flat circle with a line renderer, used for the radius rings on the floor
// the circle is 1 unit wide like the old ring picture, so scripts that scale the ring to the radius still work
public static class RingLine
{
    const int points = 64;

    public static LineRenderer Add(GameObject go, Color colour, float width = 0.3f)
    {
        LineRenderer line = go.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.alignment = LineAlignment.TransformZ;   // lies flat in the ring's own plane instead of facing the camera
        line.widthMultiplier = width;
        line.startColor = colour;
        line.endColor = colour;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.sharedMaterial = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");

        line.positionCount = points;
        for (int i = 0; i < points; i++)
        {
            float a = i * Mathf.PI * 2f / points;
            line.SetPosition(i, new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * 0.5f);
        }
        return line;
    }
}
