using System.Collections.Generic;
using UnityEngine;

public class PathfindingTest : MonoBehaviour
{
    private Pathfinding pathfinding;

    void Start()
    {
        pathfinding = new Pathfinding(10, 5);
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

            if (x >= 0 && y >= 0 && x < pathfinding.GetGrid().GetWidth() && y < pathfinding.GetGrid().GetHeight())
            {
                List<PathNode> path = pathfinding.FindPath(0, 2, x, y);
                if (path != null)
                {
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        //Debug.Log(path[i].x + "," + path[i].y);
                        Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f, new Vector3(path[i + 1].x, path[i + 1].y) * 10f + Vector3.one * 5f);
                    }
                }
            }
        }        
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vector = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vector.z = 0f;
        return vector;
    }

    public static Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }

    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPoint, Camera worldCamera)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // vzdálenost od kamery
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
