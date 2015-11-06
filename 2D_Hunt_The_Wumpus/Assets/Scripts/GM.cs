using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Hunt_The_Wumpus1;
using System.Linq;


public class GM : MonoBehaviour {

    public Text warning;
    public GameObject gameOver;
    public GameObject announce;
    public GameObject wumpSprite;
    public GameObject batSprite;
    public GameObject pitSprite;
    public GameMap map;
    public Wumpus wump;
    public Bat bats;
    public Player player;
    public int shotResult;
    public bool keepSame = false;
    public bool playAgain = false;
    public bool endGame;
    public GameObject[] rooms;
    public static GM instance = null;   
    
	// Use this for initialization
	void Start () {
        if (instance == null)   //make sure only 1 instance
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
        Setup(keepSame);    //call setup
	}

    void Update()
    {
        wumpSprite.SetActive(false);    //don't show sprites if not in room
        batSprite.SetActive(false);
        pitSprite.SetActive(false);

        if (!player.alive || !wump.alive)   //if game is over
        {
            if (!endGame)
            {
                endGame = true;
                StartCoroutine(GameOver()); //start end game routine
            }
        }

        if(player.room == wump.room)    //If enter wump room, show the sprite (brown square)
        {
            wumpSprite.transform.position = new Vector3(rooms[player.room].transform.position.x - .35f, rooms[player.room].transform.position.y + .35f, 
                rooms[player.room].transform.position.z);  //sprite is in top left
            wumpSprite.SetActive(true);
        }
        if (player.room == bats.room)   //if bat room, show blue square
        {
            batSprite.transform.position = new Vector3(rooms[player.room].transform.position.x + .35f, rooms[player.room].transform.position.y + .35f,
                rooms[player.room].transform.position.z);   //sprite is in top right (though bats transport you immediately so this will only show in the 1/20 chance they transport you to same room)
            batSprite.SetActive(true);
        }
        if (player.room == map.getPit1())   //if pit room, show black square
        {
            pitSprite.transform.position = new Vector3(rooms[player.room].transform.position.x - .35f, rooms[player.room].transform.position.y - .35f,
                rooms[player.room].transform.position.z);   //sprite is in bottom left
            pitSprite.SetActive(true);
        }
        if (player.room == map.getPit2())
        {
            pitSprite.transform.position = new Vector3(rooms[player.room].transform.position.x - .35f, rooms[player.room].transform.position.y - .35f,
                rooms[player.room].transform.position.z);   //sprite is in bottom left
            pitSprite.SetActive(true);
        }
    }
    public void Setup(bool same)
    {
        if (!same)  //If new map
        {
            map = new GameMap();
            map.Start();
        }
        endGame = false;    //setup
        wump = new Wumpus();
        player = new Player();
        bats = new Bat();

        wump.room = map.getWump();
        player.room = map.getCurrent();
        bats.room = map.getBat();
        for (int i = 0; i < 20; i++)
        {
            rooms[i] = GameObject.Find("Room (" + i + ")");
            rooms[i].GetComponent<Room>().player = false;
        }
        Warnings(); //check for initial warnings
        rooms[player.room].GetComponent<Room>().player = true;     //show player room
    }

    public void MovePlayer(int dest)    //function to move player
    {
        if(map.isAdjacent(player.room, dest))   //check adjacent
        {
            wump.move(map.getAdjacent(wump.room));  //move wump at start of turn
            rooms[player.room].GetComponent<Room>().player = false; //move player
            rooms[dest].GetComponent<Room>().player = true;
            player.room = dest;
            CheckRoom();    //check if stepped in hazards
            Warnings();     //show warnings
        }
        
        else
        {
            StartCoroutine(Announce("Please choose an adjacent room"));
        }
    }

    IEnumerator Announce(string msg)    //announce, only show for 2 seconds
    {
        announce.GetComponent<Text>().text = msg;
        announce.SetActive(true);
        yield return new WaitForSeconds(2);
        announce.GetComponent<Text>().text = "";
        announce.SetActive(false);
        StopCoroutine("Announce");
    }
    
    void Warnings() //function to update warnings
    {
        warning.text = "";
        if (map.isAdjacent(player.room, wump.room))
            warning.text = (warning.text + "I smell a Wumpus\n");
        if (map.isAdjacent(player.room, bats.room))
            warning.text = (warning.text + "Bats nearby\n");
        if (map.isAdjacent(player.room, map.getPit1()))
            warning.text = (warning.text + "I feel a draft\n");
        if (map.isAdjacent(player.room, map.getPit2()))
            warning.text = (warning.text + "I feel a draft");
    }
    
    void CheckRoom()    //function to check room for hazards and execute
    {
        if (player.room == bats.room)    // check bats first, b/c they might drop player into pits or wumpus, and "logic" says bats can also save you from pits and wumpus
        {
            System.Random r = new System.Random();
            StartCoroutine(Announce("Zap--Super Bat snatch! Elsewhereville for you!"));
            rooms[player.room].GetComponent<Room>().player = false;
            player.room = r.Next(20);
            rooms[player.room].GetComponent<Room>().player = true;
        }
        if (player.room == map.getPit1() || player.room == map.getPit2())   // check pits next, even if wumpus, player dies anyway
        {
            player.alive = false;
            StartCoroutine(Announce("YYYIIIIEEEE...fell in a pit"));
        }
        else if (player.room == wump.room)   // check for wumpus
        {
            if (wump.isAwake)
            {
                player.alive = false;
                StartCoroutine(Announce("Killed by Wumpus"));
            }
            else
            {
                StartCoroutine(Announce("...Ooops! Bumped a Wumpus"));
                wump.wakeup();
            }
        }   
    }

    IEnumerator GameOver()  //Coroutine for Gameover, needed to wait for user input
    {
        if (!wump.alive)
        {
            StartCoroutine(Announce("Hee hee hee - the Wumpus'll getcha next time!!")); //if the player won
            gameOver.GetComponent<Text>().text = "You Won! Play Again? (T/F)";
        }
        else
            gameOver.GetComponent<Text>().text = "Game Over! Play Again? (T/F)";
        gameOver.SetActive(true);
        while (!Input.GetKeyDown(KeyCode.T) && !Input.GetKeyDown(KeyCode.F))    //wait for play again
        {
            yield return null;
        }
        if(Input.GetKeyDown(KeyCode.T)) //if play again
        {
            gameOver.GetComponent<Text>().text = "Keep same starting positions? (Y/N)";
            while (!Input.GetKeyDown(KeyCode.Y) && !Input.GetKeyDown(KeyCode.N))    //wait for keep same
            {
                yield return null;
            }
            if (Input.GetKeyDown(KeyCode.Y))    //if keep same
            {
                keepSame = true;
                Setup(keepSame);
                gameOver.SetActive(false);
                StartCoroutine(Announce("Keep track this time!"));
            }
            else if (Input.GetKeyDown(KeyCode.N))   //if don't keep same
            {
                keepSame = false;
                Setup(keepSame);
                gameOver.SetActive(false);
                StartCoroutine(Announce("Good Luck!"));
            }
        }
        else if(Input.GetKeyDown(KeyCode.F))    //if don't play again
        {
            gameOver.GetComponent<Text>().text = "Thanks for Playing!";
            Destroy(this);  //destroy object and quit won't do anything in debugging
            Application.Quit();
        }
    }
	
    public void Shoot(string path)  //if user chooses to shoot
    {
        wump.move(map.getAdjacent(wump.room));  //move wumpus at start of turn
        List<int> arrowPath = new List<int>();
        arrowPath.Clear();  //Just in case
        int[] temp = path.Split(' ').Select(s => int.Parse(s)).ToArray();   //convert string to int array
        if(temp.Length > 5)
        {
            StartCoroutine(Announce("Can't Shoot that far"));   //if more than 5 numbers
            arrowPath.Clear();
            return;
        }

        for (int i = 0; i < temp.Length; i++)   //add to list
        {
            arrowPath.Add(temp[i]);
        }
        shotResult = player.shoot(map, player.room, wump.room, ref arrowPath);  //call shoot
        
        if (shotResult == 3)    //if invalid
        {
            StartCoroutine(Announce("Arrows aren't that crooked, try again."));
            arrowPath.Clear();
        }

        ShowPath(true, arrowPath);  //if path is ok, show it

        switch (shotResult)  //0 = miss, 1 = hit wumpus, 2 = hit player
        {
            case 0:
                StartCoroutine(Announce("Missed!"));
                ShowPath(false, arrowPath); //hide path
                if (!wump.isAwake)  //wake up wumpus if not already awake
                    wump.wakeup();
                break;

            case 1:
                StartCoroutine(Announce("Aha! You got the Wumpus"));
                ShowPath(false, arrowPath);
                wump.kill();
                break;

            case 2:
                StartCoroutine(Announce("Ouch! Arrow got you!"));
                ShowPath(false, arrowPath);
                if (!wump.isAwake)
                    wump.wakeup();
                break;
        }
    }

    void ShowPath(bool on, List<int> arrow) //function to show arrow path
    {
        Hall hall = new Hall();
        if (on)
        {
            for (int i = 0; i < arrow.Count; i++)
            {
                rooms[arrow[i]].GetComponent<Room>().arrow = on;
                if (i == 0)
                {
                    hall.DrawLine(rooms[player.room], rooms[arrow[i]]);
                    if(arrow.Count > 1)
                        hall.DrawLine(rooms[arrow[i]], rooms[arrow[i+1]]);
                }
                else if (i + 1 < arrow.Count)
                    hall.DrawLine(rooms[arrow[i]], rooms[arrow[i + 1]]);
            }
        }

        else
        {
            for (int i = 0; i < arrow.Count; i++)
            {
                if (i == 0)
                {
                    StartCoroutine(HidePath(rooms[player.room]));   //needed to not hide path immediately
                    StartCoroutine(HidePath(rooms[arrow[i]]));
                }
                else
                    StartCoroutine(HidePath(rooms[arrow[i]]));
            }
        }
    }

    IEnumerator HidePath(GameObject room)   //wait before hiding the arrow path
    {
        yield return new WaitForSeconds(2);
        Destroy(room.GetComponent<LineRenderer>());
        room.GetComponent<Room>().arrow = false;
        StopCoroutine("HidePath");
    }

    void OnRenderObject()   //Draws halls
    {
        Material mat = new Material(Shader.Find("Hidden/Internal-Colored"));
        mat.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GL.Vertex(rooms[i].transform.position);
                GL.Vertex(rooms[map.getAdjacent(i)[j]].transform.position);
            }
        }
        GL.End();
        GL.PopMatrix();
    }
}
