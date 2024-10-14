using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PathfindingVisual : MonoBehaviour
{

    private MyGrid<PathNode> grid;
    private Mesh mesh;

    private bool updateMesh;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetGrid(MyGrid<PathNode> grid)
    {
        this.grid = grid;
        UpdatePathfindingVisuals();

        grid.OnGridValueChangedEvent += Grid_UpdateHeatmapVisual;

    }

    private void LateUpdate()
    {
        if (updateMesh) { UpdatePathfindingVisuals(); }
    }

    public void UpdatePathfindingVisuals()
    {
        grid.UpdateDebugVisuals();
        updateMesh = false;
    }

    private void Grid_UpdateHeatmapVisual(Vector2 pos)
    {
        updateMesh = true;
    }


}
