using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class TilemapVisual : MonoBehaviour
{
    private MyGrid<HFBoardTile> grid;
    private Mesh mesh;

    private bool updateMesh;

    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;

    private void Awake()
    {
        mesh = new Mesh(); 
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDisable()
    {
        if (grid != null) grid.OnGridValueChangedEvent -= Grid_UpdateBoardVisual;
    }

    public void SetGrid(MyGrid<HFBoardTile> grid)
    {
        this.grid = grid;
        //DrawTiles();

        grid.OnGridValueChangedEvent += Grid_UpdateBoardVisual;
        DrawTiles();

    }

    private void Grid_UpdateBoardVisual(Vector2 e)
    {
        //UpdateHeatmapVisual();
        updateMesh = true;
        //print("Event fired on grid " + e);
    }

    private void LateUpdate()
    {
        if (updateMesh) DrawTiles();
    }

    private void DrawTiles()
    {
        UtilsClass.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out vertices, out uvs, out triangles);

        Texture tileTexture = GetComponent<MeshRenderer>().material.mainTexture;
        float textureWidth = tileTexture.width;

        for (int x = 0; x < grid.GetWidth(); x++) { 
            for (int y = 0; y < grid.GetHeight(); y++) {

                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                HFBoardTile tileToDraw = grid.GetGridObject(x, y);
                int tilemapSpriteIndex = tileToDraw.GetSpriteIndex();

                Vector2 gridValueUV00 = new Vector2((64 * tilemapSpriteIndex) / textureWidth, 0);
                Vector2 gridValueUV11 = new Vector2((64 * (tilemapSpriteIndex + 1)) / textureWidth, 1);

                MeshUtils.AddToMeshArrays(vertices, uvs, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridValueUV00, gridValueUV11);

            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        updateMesh = false;

    }

}
