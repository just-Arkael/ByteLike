using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteLike
{

    // Generator
    static class Generator
    {
        static public int size = 30;
        static Random rand = new Random();
        static public int[] position = new int[] { 0, 0 };
        static public volatile int[,] level;
        static int[] room;
        static List<int[]> remrooms = new List<int[]>();
        static int floor = 0;
        static bool GeneratedSecret = false;

        static double DistanceBetween(int[] x, int[] y)
        {
            int disx = 0;
            int disy = 0;
            if (x[0] >= y[0]) { disx = x[0] - y[0]; }
            else { disx = y[0] - x[0]; }

            if (x[1] >= y[1]) { disy = x[1] - y[1]; }
            else { disy = y[1] - x[1]; }

            return Math.Sqrt(Math.Pow(disx, 2) + Math.Pow(disy, 2));
        }

        static public void Reset()
        {
            remrooms = new List<int[]>();
            GeneratedSecret = false;
        }

        // Room creator
        static void CreateRoom(bool remember, out Chest chest, int currentFloor)
        {
            int water = 0;
            int[] waterCords = new int[] { 0, 0 };
            bool lava = false;
            chest = null;
            // If we're generating a rock, reduce it's size
            if (!remember)
            {
                room[0] -= 4;
                room[1] -= 4;
            }
            // Decide to generate water/lava if not rock
            else
            {
                if (rand.Next(10) == 0)
                {
                    water = rand.Next(2, 6);
                    waterCords = new int[] { rand.Next(-(room[0] / 2), room[0] / 2), rand.Next(-(room[1] / 2), room[1] / 2) };
                    if (rand.Next(10) < floor - 10)
                    {
                        lava = true;
                    }
                }
            }

            int[] chestPosition = new int[] { -100, -100 };

            if (rand.Next(3) != 0)
            {
                chestPosition[0] = rand.Next(-(room[0] / 2), room[0] / 2);
                chestPosition[1] = rand.Next(-(room[1] / 2), room[1] / 2);
            }

            // Generate tiles
            for (int i = -(room[1] / 2); i < room[1] / 2; i++)
            {
                for (int j = -(room[0] / 2); j < room[0] / 2; j++)
                {
                    if ((j == chestPosition[0] && i == chestPosition[1]) || (rand.Next(10) == 0 && !remember))
                        chest = new Chest(new int[] { position[0] + j, position[1] + i }, currentFloor, null);
                    // If we're in boundaries
                    if (position[0] + j >= 0 && position[0] + j < level.GetLength(0) && position[1] + i >= 0 && position[1] + i < level.GetLength(1))
                    {
                        // If we're on room's edges or we're generating a rock -> set to wall
                        if (i == -(room[1] / 2) || i + 1 >= (room[1] / 2) || j == -(room[0] / 2) || j + 1 >= (room[0] / 2) || remember == false)
                        {
                            level[position[0] + j, position[1] + i] = 2;
                        }
                        // Else, set to floor
                        else
                        {
                            level[position[0] + j, position[1] + i] = 1;
                            if (rand.Next(5) == 0)
                                level[position[0] + j, position[1] + i] = 3;
                            if (rand.Next(5) == 0)
                            {
                                level[position[0] + j, position[1] + i] = 8;
                                level[position[0] + j - 1, position[1] + i] = 8;
                                level[position[0] + j + 1, position[1] + i] = 8;
                                level[position[0] + j, position[1] + i + 1] = 8;
                                level[position[0] + j, position[1] + i - 1] = 8;
                            }
                            else if (rand.Next(20) == 0)
                            {
                                level[position[0] + j, position[1] + i] = 9;
                                level[position[0] + j - 1, position[1] + i] = 8;
                                level[position[0] + j + 1, position[1] + i] = 8;
                                level[position[0] + j, position[1] + i + 1] = 8;
                                level[position[0] + j, position[1] + i - 1] = 8;
                            }
                            else if (rand.Next(15) == 0)
                            {
                                level[position[0] + j, position[1] + i] = 10;
                                level[position[0] + j - 1, position[1] + i] = 8;
                                level[position[0] + j + 1, position[1] + i] = 8;
                                level[position[0] + j, position[1] + i + 1] = 8;
                                level[position[0] + j, position[1] + i - 1] = 8;
                            }
                            else if (rand.Next(15) == 0 && GeneratedSecret == false)
                            {
                                GeneratedSecret = true;
                                level[position[0] + j, position[1] + i] = 16;
                                level[position[0] + j - 1, position[1] + i] = 8;
                                level[position[0] + j + 1, position[1] + i] = 8;
                                level[position[0] + j, position[1] + i + 1] = 8;
                                level[position[0] + j, position[1] + i - 1] = 8;
                            }
                            // Check for water/lava
                            if (DistanceBetween(new int[] { j, i }, waterCords) < water)
                            {
                                level[position[0] + j, position[1] + i] = 6;
                                if (lava) { level[position[0] + j, position[1] + i] = 7; }
                            }
                        }
                    }
                }
            }
            //

            // If not a rock, add to rooms list
            if (remember)
            {
                remrooms.Add(new int[] { position[0], position[1], room[0], room[1] });
            }
        }
        //

        // Position randomizer
        static bool Randomize()
        {
            bool check = true;
            int counter = 0;
            // Attempt to create a room 100 times
            while (check && counter < 100)
            {
                // Choose a size
                room = new int[] { rand.Next(6, 16), rand.Next(6, 16) };

                // Set to a random position
                position[0] = rand.Next(room[0] / 2, level.GetLength(0) - room[0] / 2);
                position[1] = rand.Next(room[1] / 2, level.GetLength(1) - room[1] / 2);

                // Prepare to get out of the cycle
                check = false;

                // If at any tiles there's not a Void Wall (aka, already a generated tile), repeat the cycle
                for (int i = -(room[1] / 2); i < room[1] / 2 && check == false; i++)
                {
                    for (int j = -(room[0] / 2); j < room[0] / 2 && check == false; j++)
                    {
                        // Check if in boundaries
                        if (position[0] + j >= 0 && position[0] + j < level.GetLength(0) && position[1] + i >= 0 && position[1] + i < level.GetLength(1))
                        {
                            // If not a Void Wall, try a new cycle
                            if (level[position[0] + j, position[1] + i] != 0)
                            {
                                check = true;
                            }
                        }
                    }
                }
                counter++;
            }
            //

            // Return the success of creating a room
            if (counter == 100) { return false; }
            else { return true; }
        }
        //

        // Level Generator
        static public int[,] Generate(int[,] game, out int[] player, int currentfloor, ref List<Chest> chests)
        {
            floor = currentfloor;
            level = game;
            int[] startingpoint = new int[] { level.GetLength(0) / 2, level.GetLength(1) / 2 };

            // Create a bunch of rooms
            for (int i = 0; i < size; i++)
            {
                // If managed to find a possible position, create a room there
                if (Randomize())
                {
                    // If it's the first generated room - make it the starting point
                    if (remrooms.Count == 0)
                    {
                        startingpoint = position;
                    }
                    // Create room, tell it not to be a rock
                    Chest temp;
                    CreateRoom(true, out temp, currentfloor);
                    if (temp != null) { chests.Add(temp); }
                }
            }
            //

            // Create a bunch of rocks
            for (int i = 0; i < size; i++)
            {
                if (Randomize())
                {
                    Chest temp2;
                    CreateRoom(false, out temp2, currentfloor);
                }
            }
            //




            // If we managed to generate any rooms
            if (remrooms.Count > 0)
            {

                for (int f = 0; f < remrooms.Count; f++)
                {
                    // Set position to room's center
                    position[0] = remrooms[f][0];
                    position[1] = remrooms[f][1];

                    // Set current room to the one we're working with
                    room[0] = remrooms[f][2];
                    room[1] = remrooms[f][3];

                    bool generateAll = false;

                    if (f == 0 || rand.Next(2) == 0 || f + 1 == remrooms.Count) { generateAll = true; }

                    // If the last room - generate exit
                    if (f == 0)
                        level[position[0], position[1]] = 15;

                    // Create a hallway for each direction
                    for (int i = 0; i < 360; i += 90)
                    {

                        int width = rand.Next(4, 9);
                        if (width < 6) { width = rand.Next(4, 9); }
                        int[] beampos = new int[2];

                        if (!generateAll)
                        {

                            if (DistanceBetween(new int[] { position[0], level.GetLength(1) / 2 }, new int[] { level.GetLength(0) / 2, level.GetLength(1) / 2 }) < DistanceBetween(new int[] { level.GetLength(0) / 2, position[1] }, new int[] { level.GetLength(0) / 2, level.GetLength(1) / 2 }))
                            {
                                if (position[0] - level.GetLength(0) / 2 >= 0)
                                {
                                    i = 0;
                                }
                                else { i = 180; }
                            }
                            else
                            {
                                if (position[1] - level.GetLength(1) / 2 >= 0)
                                {
                                    i = 90;
                                }
                                else { i = 270; }
                            }
                        }
                        switch (i)
                        {
                            // Right
                            case 0:
                                // Set hallways position to edge of the room
                                beampos[0] = position[0] + (room[0] / 2);
                                beampos[1] = position[1] - (room[1] / 2 - 2) + rand.Next(room[1] / 2 - 2);
                                // Create doors
                                for (int j = -(width / 2) + 1; j < width / 2 - 1; j++)
                                {
                                    if (beampos[0] - 1 >= 0 && beampos[0] - 1 < level.GetLength(0) && beampos[1] + j >= 0 && beampos[1] + j < level.GetLength(1))
                                        level[beampos[0] - 1, beampos[1] + j] = 4;
                                }
                                break;

                            // Left
                            case 180:
                                // Set hallways position to edge of the room
                                beampos[0] = position[0] - (room[0] / 2);
                                beampos[1] = position[1] - (room[1] / 2 - 2) + rand.Next(room[1] / 2 - 2);
                                // Create doors
                                for (int j = -(width / 2) + 1; j < width / 2 - 1; j++)
                                {
                                    if (beampos[0] >= 0 && beampos[0] < level.GetLength(0) && beampos[1] + j >= 0 && beampos[1] + j < level.GetLength(1))
                                        level[beampos[0], beampos[1] + j] = 4;
                                }
                                // Adjust hallway position due to calculation error
                                beampos[0]--;
                                break;

                            // Up
                            case 90:
                                // Set hallways position to edge of the room
                                beampos[1] = position[1] - (room[1] / 2);
                                beampos[0] = position[0] - (room[0] / 2 - 2) + rand.Next(room[0] / 2 - 2);
                                // Create doors
                                for (int j = -(width / 2) + 1; j < width / 2 - 1; j++)
                                {
                                    if (beampos[0] + j >= 0 && beampos[0] + j < level.GetLength(0) && beampos[1] >= 0 && beampos[1] < level.GetLength(1))
                                        level[beampos[0] + j, beampos[1]] = 4;
                                }
                                // Adjust hallway position due to calculation error
                                beampos[1]--;
                                break;

                            // Down
                            case 270:
                                // Set hallways position to edge of the room
                                beampos[1] = position[1] + (room[1] / 2);
                                beampos[0] = position[0] - (room[0] / 2 - 2) + rand.Next(room[0] / 2 - 2);
                                // Create doors
                                for (int j = -(width / 2) + 1; j < width / 2 - 1; j++)
                                {
                                    if (beampos[0] + j >= 0 && beampos[0] + j < level.GetLength(0) && beampos[1] - 1 >= 0 && beampos[1] - 1 < level.GetLength(1))
                                        level[beampos[0] + j, beampos[1] - 1] = 4;
                                }
                                break;
                        }
                        // Launch a new hallway, wait for it a bit
                        Task beam = new Task(new Action(new Corridor(beampos, i, width).Generate));
                        beam.Start();
                        beam.Wait(100);

                        if (!generateAll) { i = 360; }
                    }
                }

            }
            //

            // Create boundaries
            for (int i = 0; i < level.GetLength(0); i++)
            {
                level[i, 0] = 5;
                level[i, level.GetLength(1) - 1] = 5;
            }
            for (int i = 0; i < level.GetLength(1); i++)
            {
                level[0, i] = 5;
                level[level.GetLength(0) - 1, i] = 5;
            }

            for (int i = 1; i < level.GetLength(0) - 1; i++)
            {
                level[i, 1] = 0;
                level[i, level.GetLength(1) - 2] = 0;
            }
            for (int i = 1; i < level.GetLength(1) - 1; i++)
            {
                level[1, i] = 0;
                level[level.GetLength(0) - 2, i] = 0;
            }
            //


            // Set default spawnpoint, return newly generated map
            player = startingpoint;
            return level;
        }
        //


    }
    //


    public class Corridor
    {
        static Random rand = new Random();
        int[] position = new int[2];
        int direction = 0;
        public int width = rand.Next(4, 7);
        bool doNext = true;
        int type = 0;

        // Literally the only thing hallways do
        public void Generate()
        {
            type = rand.Next(9);
            // While we have where to go
            while (doNext)
            {
                // Check if in boundaries
                if (position[0] >= 0 && position[0] < Generator.level.GetLength(0) && position[1] >= 0 && position[1] < Generator.level.GetLength(1))
                {
                    // Generate walls/floor and move

                    // Check if the position we're at is empty
                    if (Generator.level[position[0], position[1]] == 0)
                    {
                        // Generate walls/floor
                        for (int i = -(width / 2); i < width / 2; i++)
                        {
                            switch (direction)
                            {
                                // Horizontal
                                case 0:
                                case 180:
                                    // Check whether where we're trying to generate is in bounds
                                    if (position[1] + i < Generator.level.GetLength(1) && position[1] + i > 0)
                                    {
                                        // If at edges, set to wall, else set to floor
                                        if (i == -(width / 2) || i + 1 >= width / 2) { Generator.level[position[0], position[1] + i] = 2; }
                                        else
                                        {
                                            Generator.level[position[0], position[1] + i] = 1;
                                            if (rand.Next(5) == 0)
                                                Generator.level[position[0], position[1] + i] = 3;
                                            if (rand.Next(10) == 0) { Generator.level[position[0], position[1] + i] = 8; }
                                            else if (rand.Next(25) == 0) { Generator.level[position[0], position[1] + i] = 9; }
                                            else if (rand.Next(20) == 0) { Generator.level[position[0], position[1] + i] = 10; }
                                        }
                                    }
                                    break;
                                // Vertical
                                case 90:
                                case 270:
                                    // Check whether where we're trying to generate is in bounds
                                    if (position[0] + i < Generator.level.GetLength(0) && position[0] + i > 0)
                                    {
                                        // If at edges, set to wall, else set to floor
                                        if (i == -(width / 2) || i + 1 >= width / 2) { Generator.level[position[0] + i, position[1]] = 2; }
                                        else
                                        {
                                            Generator.level[position[0] + i, position[1]] = 1;
                                            if (rand.Next(5) == 0)
                                                Generator.level[position[0] + i, position[1]] = 3;
                                            if (rand.Next(10) == 0) { Generator.level[position[0] + i, position[1]] = 8; }
                                            else if (rand.Next(25) == 0) { Generator.level[position[0] + i, position[1]] = 9; }
                                            else if (rand.Next(20) == 0) { Generator.level[position[0] + i, position[1]] = 10; }
                                        }
                                    }
                                    break;
                            }
                        }
                        // Move by one, depending on direction
                        switch (direction)
                        {
                            case 0:
                                position[0]++;
                                if (type == 1) { position[1]++; }
                                if (type == 2) { position[1]--; }
                                if (type == 3 && position[0] % 2 == 0) { position[1]--; }
                                if (type == 4 && position[0] % 3 == 0) { position[1]--; }
                                if (type == 5 && position[0] % 2 == 0) { position[1]++; }
                                if (type == 6 && position[0] % 3 == 0) { position[1]++; }
                                break;
                            case 180:
                                position[0]--;
                                if (type == 1) { position[1]++; }
                                if (type == 2) { position[1]--; }
                                if (type == 3 && position[0] % 2 == 0) { position[1]--; }
                                if (type == 4 && position[0] % 3 == 0) { position[1]--; }
                                if (type == 5 && position[0] % 2 == 0) { position[1]++; }
                                if (type == 6 && position[0] % 3 == 0) { position[1]++; }
                                break;
                            case 90:
                                position[1]--;
                                if (type == 1) { position[0]++; }
                                if (type == 2) { position[0]--; }
                                if (type == 3 && position[1] % 2 == 0) { position[0]--; }
                                if (type == 4 && position[1] % 3 == 0) { position[0]--; }
                                if (type == 5 && position[1] % 2 == 0) { position[0]++; }
                                if (type == 6 && position[1] % 3 == 0) { position[0]++; }
                                break;
                            case 270:
                                position[1]++;
                                if (type == 1) { position[0]++; }
                                if (type == 2) { position[0]--; }
                                if (type == 3 && position[1] % 2 == 0) { position[0]--; }
                                if (type == 4 && position[1] % 3 == 0) { position[0]--; }
                                if (type == 5 && position[1] % 2 == 0) { position[0]++; }
                                if (type == 6 && position[1] % 3 == 0) { position[0]++; }
                                break;
                        }

                    }
                    //


                    // Check for collision with a wall

                    // Check if in boundaries again, due to movement earlier
                    if (position[0] >= 0 && position[0] < Generator.level.GetLength(0) && position[1] >= 0 && position[1] < Generator.level.GetLength(1))
                    {
                        // if we colided with a wall/door
                        if (Generator.level[position[0], position[1]] == 2 || Generator.level[position[0], position[1]] == 4)
                        {
                            // Create doors
                            switch (direction)
                            {
                                case 0:
                                    for (int j = -(width / 2) + 1; j < width / 2 - 1; j++)
                                    {
                                        if (position[1] + j >= 0 && position[1] + j < Generator.level.GetLength(1))
                                            Generator.level[position[0], position[1] + j] = 4;
                                    }
                                    break;
                                case 180:
                                    for (int j = -(width / 2) + 1; j < width / 2 - 1; j++)
                                    {
                                        if (position[1] + j >= 0 && position[1] + j < Generator.level.GetLength(1))
                                            Generator.level[position[0], position[1] + j] = 4;
                                    }
                                    break;
                                case 90:
                                    for (int j = -(width / 2) + 1; j < width / 2 - 1; j++)
                                    {
                                        if (position[0] + j >= 0 && position[0] + j < Generator.level.GetLength(0))
                                            Generator.level[position[0] + j, position[1]] = 4;
                                    }
                                    break;
                                case 270:
                                    for (int j = -(width / 2) + 1; j < width / 2 - 1; j++)
                                    {
                                        if (position[0] + j >= 0 && position[0] + j < Generator.level.GetLength(0))
                                            Generator.level[position[0] + j, position[1]] = 4;
                                    }
                                    break;
                            }
                            doNext = false;

                            // Put a cap on the corridor (preventing hitting corners)
                            for (int f = 0; f < 3 && position[0] >= 0 && position[0] < Generator.level.GetLength(0) && position[1] >= 0 && position[1] < Generator.level.GetLength(1); f++)
                            {
                                for (int i = -(width / 2); i < width / 2; i++)
                                {
                                    switch (direction)
                                    {
                                        // Horrizontal
                                        case 0:
                                        case 180:
                                            if (position[1] + i < Generator.level.GetLength(1) && position[1] + i > 0)
                                            {
                                                if (i == -(width / 2) || i + 1 >= width / 2 || f == 2)
                                                {
                                                    if (Generator.level[position[0], position[1] + i] == 0)
                                                    {
                                                        Generator.level[position[0], position[1] + i] = 2;
                                                    }
                                                }
                                                else { Generator.level[position[0], position[1] + i] = 1; }
                                            }
                                            break;

                                        // Vertical
                                        case 90:
                                        case 270:
                                            if (position[0] + i < Generator.level.GetLength(0) && position[0] + i > 0)
                                            {
                                                if (i == -(width / 2) || i + 1 >= width / 2 || f == 2)
                                                {
                                                    if (Generator.level[position[0] + i, position[1]] == 0)
                                                    {
                                                        Generator.level[position[0] + i, position[1]] = 2;
                                                    }
                                                }
                                                else { Generator.level[position[0] + i, position[1]] = 1; }
                                            }

                                            break;
                                    }
                                }
                                switch (direction)
                                {
                                    case 0:
                                        position[0]++;
                                        break;
                                    case 180:
                                        position[0]--;
                                        break;
                                    case 90:
                                        position[1]--;
                                        break;
                                    case 270:
                                        position[1]++;
                                        break;
                                }
                            }


                        }
                        // If we colide with floor, no need to generate a cap
                        else if (Generator.level[position[0], position[1]] == 1) { doNext = false; }
                    }
                    // if not in boundaries after movement, stop
                    else { doNext = false; }
                }
                // If not in boundaries, stop
                else { doNext = false; }
            }
        }

        // Self explanatory
        public Corridor(int[] position, int direction, int width)
        {
            this.position = position;
            this.direction = direction;
            this.width = width;
        }
    }


}
