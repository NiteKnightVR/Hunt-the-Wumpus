using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunt_The_Wumpus1
{
    class Wumpus
    {
        public bool isAwake = false;
        public bool alive = true;
        public int room;

        public void move(int[] map)   //function to move wumpus
        {
            if (!isAwake)   //if wumpus is not awake, don't move
                return;

            Random r = new Random();
            int moving = r.Next(4); //random for 25% chance of staying
            if (moving == 0)    //if not moving
                return; //don't move
            else
            {
                room = map[r.Next(3)];  //return an adjacent room
                return;
            }

        }

        public void wakeup()    //function to wakeup the wumpus
        {
            isAwake = true;
        }

        public void kill()  //function to kill the wumpus
        {
            alive = false;
        }
    }
}
