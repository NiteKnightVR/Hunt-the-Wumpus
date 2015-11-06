using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour
{
    public bool player = true;
    public bool arrow = true;
    public int roomNum;
    public float xPos;
    public float yPos;
    public Transform target;
    private GUIStyle numStyle = new GUIStyle();

    void Start()
    {
        xPos = this.transform.position.x;   //learning experience
        yPos = this.transform.position.y;   //not necessary at all
        target = this.transform;
    }

    void Update()
    {
        if (player)     //Player takes priority
            GetComponent<SpriteRenderer>().color = Color.cyan;
        else if (arrow) //Then arrow
            GetComponent<SpriteRenderer>().color = Color.yellow;
        else            //Empty room is red
            GetComponent<SpriteRenderer>().color = Color.red;
    }

    void OnGUI()    //Draw gui on top of room texture
    {
        numStyle.fontSize = 20;
        Vector3 getPixelPos = Camera.main.WorldToScreenPoint(target.position);
        getPixelPos.y = Screen.height - getPixelPos.y;
        if (GUI.Button(new Rect(getPixelPos.x, getPixelPos.y, 20f, 20f), roomNum.ToString(), numStyle)) //put button in lower right, so other sprites can use other corners
            GM.instance.MovePlayer(roomNum);
    }
}
