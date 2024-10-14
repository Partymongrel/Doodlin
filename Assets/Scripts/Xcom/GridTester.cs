using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridTester : MonoBehaviour
{
    [SerializeField] private InputActionReference mouseLeftClickAction, mouseRightClickAction;
    private Pathfinding pathfinding;
    [SerializeField] private float gridSize;

    private void OnEnable()
    {
        mouseLeftClickAction.action.started += MouseLeftClick;
        mouseRightClickAction.action.started += MouseRightClick;
    }

    void Start()
    {

        pathfinding = new Pathfinding(10, 10, transform.position, gridSize);

    }

    private void OnDisable()
    {
        mouseLeftClickAction.action.started -= MouseLeftClick;
        mouseRightClickAction.action.started -= MouseRightClick;
    }

    private void MouseLeftClick(InputAction.CallbackContext context)
    {
               
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pathfinding.GetGrid().GetXY(position, out int x, out int y);

        List<PathNode> path = pathfinding.FindPath(0, 0, x, y);

        //string nodestring = null;

        //foreach (PathNode node in path)
        //{
        //    nodestring += node.ToString() + " | ";
        //}

        //print(nodestring);

        if (path != null)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(new Vector3(path[i].x, path[i].y) * gridSize + Vector3.one * (gridSize / 2), new Vector3(path[i + 1].x, path[i + 1].y) * gridSize + Vector3.one * (gridSize / 2), Color.green, 5f);
            }
        }

    }

    private void MouseRightClick(InputAction.CallbackContext context)
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        PathNode pn = pathfinding.grid.GetGridObject(position);
        //print("got node at: " + pn.ToString());

        pn.isWalkable = false;
        pathfinding.grid.UpdateDebugVisuals();

    }

}
