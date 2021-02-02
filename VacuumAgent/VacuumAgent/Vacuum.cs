using System;
using System.Collections.Generic;
using System.Text;

namespace VacuumAgent
{
    class Vacuum
    {
        enum TreeState
        {
            goRight,
            goLeft,
            goUp, 
            goDown,
            Clean,
            Grab
        }


        public static void VaccumProc(){

            // Choose random location in the grid as  starting point
            

            List<TreeState> intent = new List<TreeState>();

            while (true)
            {
                if(intent.Count == 0)
                {
                    //Explore
                }
                else
                {
                    //Execute one step of the plan of action
                    //Remove action just executed
                }
            }
        }


    }
}
