using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatmapVisual : MonoBehaviour
{

    private MyGrid<int> grid;
    private Mesh mesh;

    private bool updateMesh;

    private void Awake()
    {
        mesh = new Mesh(); 
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDisable()
    {
        if (grid != null) grid.OnGridValueChangedEvent -= Grid_UpdateHeatmapVisual;
    }

    public void SetGrid(MyGrid<int> grid)
    {
        this.grid = grid;
        UpdateHeatmapVisual();

        grid.OnGridValueChangedEvent += Grid_UpdateHeatmapVisual;

    }

    private void Grid_UpdateHeatmapVisual(Vector2 e)
    {
        //UpdateHeatmapVisual();
        updateMesh = true;
        //print("Event fired on grid " + e);
    }

    private void LateUpdate()
    {
        if (updateMesh) UpdateHeatmapVisual();
    }

    private void UpdateHeatmapVisual()
    {
        UtilsClass.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uvs, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++) { 
            for (int y = 0; y < grid.GetHeight(); y++) {

                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                int gridValue = grid.GetGridObject(x, y);
                float gridValueNormalized = (float)gridValue / MyGrid<bool>.MAX_HEATMAP_VALUE;
                Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);
                MeshUtils.AddToMeshArrays(vertices, uvs, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridValueUV, gridValueUV);

            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

    }

}



public class HeatmapGridObject
{
    private const int MIN = 0;
    private const int MAX = 100;

    private MyGrid<HeatmapGridObject> grid;
    private int x, y;
    public int value;

    public HeatmapGridObject(MyGrid<HeatmapGridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void AddValue(int addValue)
    {
        value += addValue;
        Mathf.Clamp(value, MIN, MAX);
        grid.SetGridObject(x, y, this);
    }

    public float GetValueNormalized()
    {
        return (float)value / MAX;
    }

    public override string ToString()
    {
        return value.ToString();
    }

}
