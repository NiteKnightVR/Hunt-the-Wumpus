using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Hunt_The_Wumpus1;
using System.Linq;


public class GM : MonoBehaviour {

    public Text warning;
    public Text currentNum;
    public GameObject gameOver;
    public GameObject announce;
    public GameObject wumpSprite;
    public GameObject batSprite;
    public GameObject pitSprite;
    public GameMap map;
    public Wumpus wump;
    public Bat bats;
    public Player player;
    public GameObject view;
    public int shotResult;
    public bool keepSame = false;
    public bool playAgain = false;
    public bool endGame;
    public GameObject[] rooms;
    public GameObject[] doors;
    System.Random r = new System.Random();
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
            wumpSprite.SetActive(true);
        }
        if (player.room == bats.room)   //if bat room, show blue square
        {
            batSprite.SetActive(true);
        }
        if (player.room == map.getPit1())   //if pit room, show black square
        {
            pitSprite.SetActive(true);
        }
        if (player.room == map.getPit2())
        {
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
        int[] temp = map.getAdjacent(player.room);
        doors[0] = GameObject.Find("Left Door");
        doors[1] = GameObject.Find("Middle Door");
        doors[2] = GameObject.Find("Right Door");
        for (int i = 0; i < 3; i++)
        {
            doors[i].GetComponent<Door>().doorNum = temp[i];
        }
        bats.room = map.getBat();
        currentNum.text = "Current Room: " + player.room;
        //GameObject[] doors = FindObjectsOfType(typeof(Door)) as GameObject[];
        for (int i = 0; i < 20; i++)
        {
            rooms[i] = GameObject.Find("/Rooms/Room (" + i + ")");
        }
        Warnings(); //check for initial warnings      
    }

    public void MovePlayer(int dest)    //function to move player
    {
        
        
            wump.move(map.getAdjacent(wump.room));  //move wump at start of turn
            view.transform.position = new Vector3 (9,2,0);
            player.room = dest;
            int[] temp = map.getAdjacent(dest);
            for (int i = 0; i < temp.Length; i++ )
            {
                doors[i].GetComponent<Door>().doorNum = temp[i];
            }
            CheckRoom();    //check if stepped in hazards
            Warnings();     //show warnings
            currentNum.text = "Current Room: " + player.room;
        
    }

    IEnumerator Announce(string msg, int room)    //announce, only show for 2 seconds
    {
        announce.GetComponent<Text>().text = msg;
        announce.SetActive(true);
        yield return new WaitForSeconds(2);
        if(room < 20)
        {
            MovePlayer(room);
        }
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
            StartCoroutine(Announce("Zap--Super Bat snatch! Elsewhereville for you!", r.Next(20)));
        }
        if (player.room == map.getPit1() || player.room == map.getPit2())   // check pits next, even if wumpus, player dies anyway
        {
            player.alive = false;
            StartCoroutine(Announce("YYYIIIIEEEE...fell in a pit", 20));
        }
        else if (player.room == wump.room)   // check for wumpus
        {
            if (wump.isAwake)
            {
                player.alive = false;
                StartCoroutine(Announce("Killed by Wumpus", 20));
            }
            else
            {
                StartCoroutine(Announce("...Ooops! Bumped a Wumpus", 20));
                wump.wakeup();
            }
        }   
    }

    IEnumerator GameOver()  //Coroutine for Gameover, needed to wait for user input
    {
        if (!wump.alive)
        {
            StartCoroutine(Announce("Hee hee hee - the Wumpus'll getcha next time!!", 20)); //if the player won
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
                StartCoroutine(Announce("Keep track this time!", 20));
            }
            else if (Input.GetKeyDown(KeyCode.N))   //if don't keep same
            {
                keepSame = false;
                Setup(keepSame);
                gameOver.SetActive(false);
                StartCoroutine(Announce("Good Luck!", 20));
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
            StartCoroutine(Announce("Can't Shoot that far", 20));   //if more than 5 numbers
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
            StartCoroutine(Announce("Arrows aren't that crooked, try again.", 20));
            arrowPath.Clear();
        }

        //ShowPath(true, arrowPath);  //if path is ok, show it

        switch (shotResult)  //0 = miss, 1 = hit wumpus, 2 = hit player
        {
            case 0:
                StartCoroutine(Announce("Missed!", 20));
                //ShowPath(false, arrowPath); //hide path
                if (!wump.isAwake)  //wake up wumpus if not already awake
                    wump.wakeup();
                break;

            case 1:
                StartCoroutine(Announce("Aha! You got the Wumpus", 20));
                //ShowPath(false, arrowPath);
                wump.kill();
                break;

            case 2:
                StartCoroutine(Announce("Ouch! Arrow got you!", 20));
                //ShowPath(false, arrowPath);
                if (!wump.isAwake)
                    wump.wakeup();
                break;
        }
    }
}
