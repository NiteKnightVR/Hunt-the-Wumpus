using UnityEngine;
using System.Collections;

public class Hall
{
    public void DrawLine(GameObject start, GameObject stop) //LineRenderer attached to first object, connects the two
    {
        if (start.GetComponent<LineRenderer>() == null)     //Create LineRenderer to draw line for arrow path
        {
            LineRenderer lineRenderer = start.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));
            lineRenderer.SetColors(Color.yellow, Color.yellow);
            lineRenderer.SetWidth(0.2F, 0.2F);
            lineRenderer.SetPosition(0, new Vector3(start.transform.position.x, start.transform.position.y, start.transform.position.z));
            lineRenderer.SetPosition(1, new Vector3(stop.transform.position.x, stop.transform.position.y, stop.transform.position.z));
        }
    }
}