using UnityEngine;
using System.Collections;

public class View : MonoBehaviour {

    float x, y, z;
    public float speed = 1f;
    public Vector3 playerPos = new Vector3(0,2,0);
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        x = this.transform.position.x - (Input.GetAxis("Vertical") * speed);    //forward and backward
        z = this.transform.position.z + (Input.GetAxis("Horizontal") * speed);  //left and Right            
        //Don't need y movement
        playerPos = new Vector3(Mathf.Clamp(x, -9.5f, 9.5f), 2, Mathf.Clamp(z, -9.5f, 9.5f));
        //playerPos = new Vector3(Mathf.Clamp(x, playerPos.x-9.5f, playerPos.x+9.5f), 2, Mathf.Clamp(z, playerPos.z-9.5f, playerPos.z+9.5f));
        transform.position = playerPos;
	}
}
