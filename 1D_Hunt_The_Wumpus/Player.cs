using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunt_The_Wumpus1
{
    class Player
    {
        public bool alive = true;
        private int arrows = 5;
        public int room;

        public int shoot(GameMap map, int current, int wump, List<int> list)
        {   //0 = miss, 1 = hit wumpus, 2 = hit player, 3 = invalid input
            int[] path = list.ToArray();
            for (int i = 0; i < path.Length; i++)
            {
                for (int j = 1; j < i-1; j++)
                {
                    if (path[i] == path[j] && i != j)
                        return 3;
                }
            }
            for (int i = 0; i < path.Length; i++) //fix path if rooms are not adjacent
            {
                if (i == 0)
                    path[i] = checkPath(current, path[i], map);
                else
                    path[i] = checkPath(path[i - 1], path[i], map);
            }

            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == current)   //if room player is in is on path
                {
                    arrows--;       //consume an arrow
                    if (arrows == 0)    //out of arrows
                        kill(2);
                    return 2;       //return hit player
                }

                else if (path[i] == wump)   //if wumpus is is one of the rooms
                    return 1;   //return killed wumpus
            }

            arrows--;       //if nothing hits, consume an arrow
            if (arrows == 0)    //out of arrows
                kill(2);
            return 0;   //return miss               
        }

        public int checkPath(int start, int stop, GameMap m) //function to check path for shoot()
        {
            bool goodPath = false;
            if (m.isAdjacent(start, stop))    //if the next room is adjacent
            {
                goodPath = true;        //path is good
            }

            if (goodPath)   //if next room is adjacent
                return stop;//don't change it
            else
            {
                Random r = new Random();    //room wasn't adjacent
                int[] choose = m.getAdjacent(start);
                return choose[r.Next(3)]; //so pick a random room that is adjacent
            }
        }

        public void kill(int n)  //function to kill player--- 0 = wumpus, 1 = pit, 2 = out of arrows
        {                        //where n represents different ways to die
            alive = false;
            if (n == 0)         //death by wumpus
                Console.WriteLine("Killed by Wumpus");
            else if (n == 1)    //death by pit
                Console.WriteLine("YYYIIIIEEEE...fell in a pit");
            else if (n == 2)    //out of arrows
                Console.WriteLine("Out of arrows! Game Over");
        }
    }
}
