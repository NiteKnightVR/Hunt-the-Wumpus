using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public int roomNum;
    public int doorNum;
    private Vector2 pivotPoint;
    public Transform target;
    private GUIStyle numStyle = new GUIStyle();

    void Start()
    {
       
    }

	/*void OnTriggerEnter ()
    {
        GM.instance.MovePlayer(roomNum);
    }*/

    void OnGUI()    //Draw gui on top of door texture
    {
        if (roomNum < 20)   //if room is active, show GUI
        {
            numStyle.fontSize = 60;
            Vector3 getPixelPos = Camera.main.WorldToScreenPoint(this.transform.position);
            getPixelPos.y = Screen.height - getPixelPos.y;
            if (GUI.Button(new Rect(getPixelPos.x - 60, getPixelPos.y - 100, 120f, 200f), doorNum.ToString(), numStyle))
                GM.instance.MovePlayer(doorNum);
        }
        
    }

}
