using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {

    public string path = "1 2 3 4"; //default text field value

    void OnGUI()    //Shoot Button
    {
        path = GUI.TextField(new Rect(10, 85, 120, 20), path, 15);  //read the input from text field
        if (GUI.Button(new Rect(7, 16, 64, 64), "Shoot"))
            GM.instance.Shoot(path);
    }
}
