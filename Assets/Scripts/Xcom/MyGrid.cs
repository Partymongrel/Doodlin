using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class MyGrid<TGridObject>
{

    public event Action<Vector2> OnGridValueChangedEvent;

    public const int MAX_HEATMAP_VALUE = 100, MIN_HEATMAP_VALUE = 0;

    public int width, height;
    public float cellSize;
    private Vector3 originPos;
    private TGridObject[,] gridArray;
    private TextMeshPro[,] debugTextArray;

    private bool showDebug = true;

    public MyGrid (int width, int height, float cellSize, Vector3 originPos, Func<MyGrid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPos = originPos;

        gridArray = new TGridObject[width, height];
        debugTextArray = new TextMeshPro[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        if (showDebug)
        {
            UpdateDebugVisuals();
        }

    }

    public void UpdateDebugVisuals()
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                if (debugTextArray[x, y] != null)
                {
                    GameObject.Destroy(debugTextArray[x, y]);
                }

                debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize, -1) * .5f, 20, Color.white, new Vector2(cellSize, cellSize));
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    public Vector3 GetWorldPosition (int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPos;
    }

    public void GetXY(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos - originPos).x / cellSize);
        y = Mathf.FloorToInt((worldPos - originPos).y / cellSize);
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        if (OnGridValueChangedEvent != null) OnGridValueChangedEvent(new Vector2(x, y));

    }

    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            if (showDebug) debugTextArray[x, y].text = gridArray[x, y].ToString();

            TriggerGridObjectChanged(x, y);

        }
    }

    public void SetGridObject(Vector3 worldPos, TGridObject value)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPos)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        return GetGridObject(x, y);
    }

    #region Old add value methods
    //public void AddValue(int x, int y, int value)
    //{
    //    SetValue(x, y, GetValue(x,y) + value);
    //}

    //public void AddValue(Vector3 worldPos, int value, int fullValueRange, int totalRange)
    //{
    //    int lowerValueAmount = Mathf.RoundToInt((float)value / (totalRange - fullValueRange));

    //    GetXY(worldPos, out int originX, out int originY);

    //    for (int x = 0; x < totalRange; x++) {

    //        for (int y = 0; y < totalRange - x; y++) {

    //            int radius = x + y;
    //            int addValueAmount = value;
    //            if (radius > fullValueRange)
    //            {
    //                addValueAmount -= lowerValueAmount * (radius - fullValueRange);
    //            }

    //            AddValue(originX + x, originY + y, addValueAmount); //top right
    //            if (x != 0) 
    //                AddValue(originX - x, originY + y, addValueAmount); //top left
    //            if (y != 0)
    //            {
    //                AddValue(originX + x, originY - y, addValueAmount); //bottom right

    //                if (x != 0)
    //                    AddValue(originX - x, originY - y, addValueAmount); //bottom left

    //            }

    //        }
    //    }
    //}
    #endregion

    public Vector2 GetWidthAndHeight()
    {
        return new Vector2 (width, height);
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector2 GetCenterCoords()
    {
        int x = (int)MathF.Floor(width / 2);
        int y = (int)MathF.Floor(height / 2);
        return new Vector2(x, y);
    }

}
