using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunt_The_Wumpus1
{
    class Program
    {
        static void Main(string[] args)
        {
            Boolean tryAgain = false;
            Boolean keepSame = false;
            GameMap map = new GameMap();
            do
            {
                tryAgain = false;
                map = new GameMap();
                if (!keepSame)
                {
                    map.Start();        //will run at least once, sets initial values for bats, pits, wumpus, player
                }

                Wumpus wump = new Wumpus();
                Player player = new Player();
                Bat superBat = new Bat();

                wump.room = map.getWump();
                player.room = map.getCurrent();
                superBat.room = map.getBat();

                int nextRoom;
                int shotResult = 3;
                List<int> arrowPath = new List<int>();

                while (wump.alive && player.alive)   //while wumpus and player are alive
                {
                    wump.move(map.getAdjacent(wump.room));

                    if (map.isAdjacent(player.room, wump.room))
                        Console.WriteLine("I smell a Wumpus");
                    else if (map.isAdjacent(player.room, superBat.room))
                        Console.WriteLine("Bats nearby");
                    else if (map.isAdjacent(player.room, map.getPit1()))
                        Console.WriteLine("I feel a draft");
                    else if (map.isAdjacent(player.room, map.getPit2()))
                        Console.WriteLine("I feel a draft");


                    Console.WriteLine("\nYou are in room: " + player.room);
                    Console.WriteLine("Adjacent rooms are: ");
                    for (int i = 0; i < 3; i++)
                    {
                        Console.WriteLine("\tRoom:" + map.getAdjacent(player.room)[i]);
                    }
                    Boolean turn = false;
                    do
                    {
                        int action = 0;
                        Console.Write("SHOOT an arrow or MOVE to another room: ");
                        String moveShoot = Console.ReadLine();
                        if (moveShoot.Equals("SHOOT"))
                            action = 2;
                        if (moveShoot.Equals("MOVE"))
                            action = 1;
                        else
                        {
                            while (action == 0)
                            {
                                Console.WriteLine("Please enter either SHOOT or MOVE");
                                Console.Write("SHOOT an arrow or MOVE to another room: ");
                                moveShoot = Console.ReadLine();
                                if (moveShoot.Equals("SHOOT"))
                                    action = 2;
                                else if (moveShoot.Equals("MOVE"))
                                    action = 1;
                            }
                        }

                        switch (action)
                        {
                            case 1:
                                Boolean isNotAdjacent = true;
                                do
                                {
                                    Console.Write("Enter destination room #: ");
                                    nextRoom = Convert.ToInt32(Console.ReadLine());
                                    if (map.isAdjacent(player.room, nextRoom))
                                        isNotAdjacent = false;
                                    else
                                        Console.WriteLine("That room is not adjacent, check the list above.");
                                }
                                while (isNotAdjacent);
                                player.room = nextRoom;     //move player
                                checkHazards(player, wump, superBat, map);      // Check room for hazards
                                turn = false;
                                break;

                            case 2:
                                do
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        Console.Write("Enter next room on arrow path (0-19) or type END: ");
                                        String pathInput = Console.ReadLine();
                                        
                                        if (pathInput.Equals("END"))
                                            i = 5;
                                        else
                                        {
                                            int tempRoom = Convert.ToInt32(pathInput);
                                            while (tempRoom < 0 || tempRoom > 19)
                                            {
                                                Console.Write("Please enter a room number (0-19): ");
                                                pathInput = Console.ReadLine();
                                                tempRoom = Convert.ToInt32(pathInput);
                                            }
                                            arrowPath.Add(tempRoom);
                                        }
                                    }

                                    shotResult = player.shoot(map, player.room, wump.room, arrowPath);
                                    if (shotResult == 3)
                                    {
                                        Console.WriteLine("Arrows aren't that crooked, try again.");
                                        arrowPath.Clear;
                                    }
                                } while (shotResult == 3);
                                turn = false;
                                break;
                        }
                    }
                    while (turn);

                    switch (shotResult)  //0 = miss, 1 = hit wumpus, 2 = hit player, 3 = didn't shoot
                    {
                        case 0:
                            Console.WriteLine("Missed!");
                            if (!wump.isAwake)  //wake up wumpus if not already awake
                                wump.wakeup();
                            break;

                        case 1:
                            Console.WriteLine("Aha! You got the Wumpus");
                            wump.kill();
                            break;

                        case 2:
                            Console.WriteLine("Ouch! Arrow got you!");
                            if (!wump.isAwake)
                                wump.wakeup();
                            break;
                        case 3:
                            break;
                    }

                    Console.WriteLine("\n-----------------------------------\n");
                }

                if (!wump.alive)
                    Console.WriteLine("Hee hee hee - the Wumpus'll getcha next time!!");
                Console.Write("Play again? (Y/N): ");
                String again = Console.ReadLine();
                while (!again.Equals("Y") && !again.Equals("N"))
                {
                    Console.Write("Please enter Y or N: ");
                    again = Console.ReadLine();
                }
                if (again.Equals("Y"))
                {
                    tryAgain = true;
                    Console.Write("Keep same starting positions? (Y/N): ");
                    String keep = Console.ReadLine();
                    while (!keep.Equals("Y") && !keep.Equals("N"))
                    {
                        Console.Write("Please enter Y or N: ");
                        keep = Console.ReadLine();
                    }
                    if (keep.Equals("Y"))
                    {
                        keepSame = true;
                    }
                }
            } while (tryAgain);
        }

        private static void checkHazards(Player player, Wumpus wump, Bat bat, GameMap map)
        {
            if (player.room == bat.room)    // check bats first, b/c they might drop player into pits or wumpus, and "logic" says bats can also save you from pits and wumpus
            {
                Random r = new Random();
                Console.WriteLine("Zap--Super Bat snatch! Elsewhereville for you!");
                player.room = r.Next(20);
            }
            if (player.room == map.getPit1() || player.room == map.getPit2())   // check pits next, even if wumpus, player dies anyway
            {
                player.kill(1);
            }
            else if (player.room == wump.room)   // check for wumpus
            {
                if (wump.isAwake)
                    player.kill(0);
                else
                {
                    Console.WriteLine("...Ooops! Bumped a Wumpus");
                    wump.wakeup();
                }
            }
        }

    }
}
