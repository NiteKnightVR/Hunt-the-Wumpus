using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunt_The_Wumpus1
{
    public class GameMap
    {
        //protected int[,] roomContains = new int[20, 4]; //Map of rooms with slot for pits [x,1], bats [x,2], wumpus [x,3]
        protected int[][] fullMap = new int[20][];		//list of adjacent rooms given a room number

        protected int wumpRoom, batRoom, pit1, pit2, currentRoom;

        public void Start()
        {
            fullMap[0] = new int[] { 4, 1, 7 }; fullMap[1] = new int[] { 0, 2, 9 }; fullMap[2] = new int[] { 1, 3, 11 }; fullMap[3] = new int[] { 2, 4, 13 };	//fill array of adjacent rooms
            fullMap[4] = new int[] { 0, 3, 5 }; fullMap[5] = new int[] { 4, 6, 14 }; fullMap[6] = new int[] { 5, 7, 16 }; fullMap[7] = new int[] { 0, 6, 8 };
            fullMap[8] = new int[] { 7, 9, 17 }; fullMap[9] = new int[] { 1, 8, 10 }; fullMap[10] = new int[] { 9, 11, 18 }; fullMap[11] = new int[] { 2, 10, 12 };
            fullMap[12] = new int[] { 11, 13, 19 }; fullMap[13] = new int[] { 3, 12, 14 }; fullMap[14] = new int[] { 5, 13, 15 }; fullMap[15] = new int[] { 14, 16, 19 };
            fullMap[16] = new int[] { 6, 15, 17 }; fullMap[17] = new int[] { 8, 16, 18 }; fullMap[18] = new int[] { 10, 17, 19 }; fullMap[19] = new int[] { 12, 15, 18 };

            /*for (int i = 1; i < 21; i++)
            {
                roomContains[i - 1, 0] = i;	//set room numbers of map
            }*/

            Random random = new Random();

            AssignPlayer(random);	//populate map
            AssignPits(random);
            AssignBats(random);
            AssignWump(random);
        }

        private void AssignPlayer(Random r)
        {
            currentRoom = r.Next(20);		//place player in random room
            Console.WriteLine("player: " + currentRoom);    //debugging
        }

        private void AssignPits(Random r)
        {
            int room = r.Next(20);
            while (room == currentRoom)	//if player room, get another one
            {
                room = r.Next(20);
            }
            pit1 = room;
            room = r.Next(20);

            while (room == currentRoom || room == pit1) //if player room or already has pit
            {
                room = r.Next(20);
            }
            pit2 = room;

            Console.WriteLine("pit1: " + pit1); //debugging
            Console.WriteLine("pit2: " + pit2); //debugging

        }

        private void AssignBats(Random r)
        {
            int room = r.Next(20);
            while (room == currentRoom)	//if player room, get another one
            {
                room = r.Next(20);
            }
            Console.WriteLine("bat: " + room);  //debugging
            batRoom = room;		//place bats
        }

        private void AssignWump(Random r)
        {
            int room = r.Next(20);
            while (room == currentRoom)	//if player room, get another one
            {
                room = r.Next(20);
            }
            Console.WriteLine("wump: " + room); //debugging
            if (room != currentRoom)			//if not player room
                wumpRoom = room;	//place wumpus	
        }

        public int[] getAdjacent(int room)
        {
            return fullMap[room];
        }

        public Boolean isAdjacent(int now, int next)
        {
            for (int i = 0; i < 3; i++)
            {
                if (getAdjacent(now)[i] == next)
                    return true;
            }
            return false;
        }

        public int getWump()    //getters
        { return wumpRoom; }
        public int getBat()
        { return batRoom; }
        public int getPit1()
        { return pit1; }
        public int getPit2()
        { return pit2; }
        public int getCurrent()
        { return currentRoom; }
        public int[][] getMap()
        { return fullMap; }
    }
}
