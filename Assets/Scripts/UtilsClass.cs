using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;

public class UtilsClass
{

    public static TextMeshPro CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color color = default(Color), Vector2 rectSize = default(Vector2), TextAlignmentOptions alignment = TextAlignmentOptions.Center, TextContainerAnchors anchor = TextContainerAnchors.Middle)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, color, rectSize, alignment, anchor);
    }

    public static TextMeshPro CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, Vector2 rectSize, TextAlignmentOptions alignment, TextContainerAnchors anchor)
    {
        GameObject gO = new GameObject("World_Text", typeof(TextMeshPro));
        Transform transform = gO.transform;
        transform.SetParent(parent);
        transform.localPosition = localPosition;

        TextMeshPro textMesh = gO.GetComponent<TextMeshPro>();
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        RectTransform textRect = textMesh.GetComponent<RectTransform>();
        textRect.sizeDelta = rectSize;
        textMesh.alignment = alignment;

        return textMesh;
    }

    public static Mesh CreateEmptyMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[0];
        mesh.uv = new Vector2[0];
        mesh.triangles = new int[0];

        return mesh;
    }

    public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles)
    {
        vertices = new Vector3[4 * quadCount];
        uvs = new Vector2[4 * quadCount];
        triangles = new int[6 * quadCount];
    }

}
