﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ByteLike
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static int[,] level = new int[100, 100];
        static int[,] darkness = new int[100, 100];

        static Player player = new Player("Player");
        static int[] camera = new int[2];
        static int[] cameraSize = new int[] { 40, 30 };
        static int floor = 1;

        static int[] sizes = new int[2];
        static string response = "";
        static string prevresponse = "";

        static void InitLevel()
        {
            for (int i = 0; i < level.GetLength(1); i++)
            {
                for (int j = 0; j < level.GetLength(0); j++)
                {
                    level[j, i] = 0;
                    darkness[j, i] = 0;
                }
            }
        }

        // Torch and darkness
        static void ClearLight()
        {
            for (int i = -player.Torch() - 5; i <= player.Torch() + 5; i++)
            {
                for (int j = -player.Torch() - 5; j <= player.Torch() + 5; j++)
                {
                    if (player.position[0] + j > 0 && player.position[0] + j < level.GetLength(0) && player.position[1] + i > 0 && player.position[1] + i < level.GetLength(1))
                    {
                        if ((level[player.position[0] + j, player.position[1] + i] != 0 && level[player.position[0] + j, player.position[1] + i] != 5 && DistanceBetween(player.position, new int[] { player.position[0] + j, player.position[1] + i }) < (player.Torch() + 0.5) * 1.5) || DistanceBetween(player.position, new int[] { player.position[0] + j, player.position[1] + i }) < (player.Torch() + 0.5))
                        {
                            darkness[player.position[0] + j, player.position[1] + i] = 1;
                        }
                        else
                        {
                            if (darkness[player.position[0] + j, player.position[1] + i] != 0)
                            {
                                darkness[player.position[0] + j, player.position[1] + i] = 2;
                            }
                            else
                            {
                                darkness[player.position[0] + j, player.position[1] + i] = 0;
                            }
                        }
                    }
                }
            }
        }
        //


        // Calculator for distances
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



        static System.Windows.Controls.Image DrawMap(string response)
        {
            // Clear darkness
            ClearLight();

            // Set camera to it's top left corner
            camera[0] -= cameraSize[0] / 2;
            camera[1] -= cameraSize[1] / 2;

            // Check wheather camera is outside of level's boundaries, if so change that
            if (camera[0] + cameraSize[0] > level.GetLength(0)) { camera[0] = level.GetLength(0) - cameraSize[0]; }
            if (camera[1] + cameraSize[1] > level.GetLength(1)) { camera[1] = level.GetLength(1) - cameraSize[1]; }

            if (camera[0] < 0) { camera[0] = 0; }
            if (camera[1] < 0) { camera[1] = 0; }
            //

            DrawingGroup drawingGroup = new DrawingGroup();

            drawingGroup.Open();
            // Draw the tiles
            for (int i = 0; i < level.GetLength(1) && i < cameraSize[1]; i++)
            {
                for (int j = 0; j < level.GetLength(0) && j < cameraSize[0]; j++)
                {
                    ImageDrawing imageDrawing = new ImageDrawing();


                    imageDrawing.Rect = new Rect(j * 16, i * 16, 17, 17);
                    string floorImage = "";
                    if (darkness[camera[0] + j, camera[1] + i] > 0)
                    {
                        imageDrawing.ImageSource = new BitmapImage(new Uri("Graphycs/ByteLikeGraphycs/darkness.png", UriKind.Relative));
                        floorImage = "Graphycs/ByteLikeGraphycs/darkness.png";
                        switch (level[camera[0] + j, camera[1] + i])
                        {
                            // Void wall
                            case 0:
                                floorImage = "Graphycs/ByteLikeGraphycs/darkwall";
                                if (level[camera[0] + j + 1, camera[1] + i] == 2 || level[camera[0] + j + 1, camera[1] + i] == 0 || level[camera[0] + j + 1, camera[1] + i] == 5) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j, camera[1] + i - 1] == 2 || level[camera[0] + j, camera[1] + i - 1] == 0 || level[camera[0] + j, camera[1] + i - 1] == 5) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j - 1, camera[1] + i] == 2 || level[camera[0] + j - 1, camera[1] + i] == 0 || level[camera[0] + j - 1, camera[1] + i] == 5) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j, camera[1] + i + 1] == 2 || level[camera[0] + j, camera[1] + i + 1] == 0 || level[camera[0] + j, camera[1] + i + 1] == 5) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                floorImage += ".png";
                                break;
                            // Floor
                            case 1:
                                floorImage = "Graphycs/ByteLikeGraphycs/floor.png";
                                break;
                            // Wall
                            case 2:
                                floorImage = "Graphycs/ByteLikeGraphycs/wall";
                                if (level[camera[0] + j + 1, camera[1] + i] == 2 || level[camera[0] + j + 1, camera[1] + i] == 0 || level[camera[0] + j + 1, camera[1] + i] == 5) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j, camera[1] + i - 1] == 2 || level[camera[0] + j, camera[1] + i - 1] == 0 || level[camera[0] + j, camera[1] + i - 1] == 5) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j - 1, camera[1] + i] == 2 || level[camera[0] + j - 1, camera[1] + i] == 0 || level[camera[0] + j - 1, camera[1] + i] == 5) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j, camera[1] + i + 1] == 2 || level[camera[0] + j, camera[1] + i + 1] == 0 || level[camera[0] + j, camera[1] + i + 1] == 5) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                floorImage += ".png";
                                break;
                            // Center of the room, really only needed for debug
                            case 3:
                                floorImage = "Graphycs/ByteLikeGraphycs/floor.png";
                                break;
                            // Door
                            case 4:
                                floorImage = "Graphycs/ByteLikeGraphycs/door.png";
                                break;
                            // Outter wall
                            case 5:
                                floorImage = "Graphycs/ByteLikeGraphycs/darkwall";

                                if (camera[0] + j == 0)
                                {
                                    if (camera[1] + i == 0 || camera[1] + i == level.GetLength(1) - 1 || level[1, camera[1] + i] == 2 || level[1, camera[1] + i] == 0)
                                    {
                                        floorImage += "1111";
                                    }
                                    else { floorImage += "0000"; }
                                }
                                else if (camera[0] + j == level.GetLength(0) - 1)
                                {
                                    if (camera[1] + i == 0 || camera[1] + i == level.GetLength(1) - 1 || level[level.GetLength(0) - 2, camera[1] + i] == 2 || level[level.GetLength(0) - 2, camera[1] + i] == 0)
                                    {
                                        floorImage += "1111";
                                    }
                                    else { floorImage += "0000"; }
                                }
                                else if (camera[1] + i == 0)
                                {
                                    if (camera[0] + i == 0 || camera[0] + i == level.GetLength(0) - 1 || level[camera[0] + j, 1] == 2 || level[camera[0] + j, 1] == 0)
                                    {
                                        floorImage += "1111";
                                    }
                                    else { floorImage += "0000"; }
                                }
                                else
                                {
                                    if (camera[0] + i == 0 || camera[0] + i == level.GetLength(0) - 1 || level[camera[0] + j, level.GetLength(1)-2] == 2 || level[camera[0] + j, level.GetLength(1) - 2] == 0)
                                    {
                                        floorImage += "1111";
                                    }
                                    else { floorImage += "0000"; }
                                }

                                floorImage += ".png";
                                break;
                            // Water
                            case 6:
                                floorImage = "Graphycs/ByteLikeGraphycs/water";
                                if (level[camera[0] + j + 1, camera[1] + i] == 6) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j, camera[1] + i - 1] == 6) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j - 1, camera[1] + i] == 6) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j, camera[1] + i + 1] == 6) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                floorImage += ".png";
                                break;
                            // Lava
                            case 7:
                                floorImage = "Graphycs/ByteLikeGraphycs/lava";
                                if (level[camera[0] + j + 1, camera[1] + i] == 7) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j, camera[1] + i - 1] == 7) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j - 1, camera[1] + i] == 7) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j, camera[1] + i + 1] == 7) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                floorImage += ".png";
                                break;
                            // Grass
                            case 8:
                            case 9:
                            case 10:
                                floorImage = "Graphycs/ByteLikeGraphycs/grass";
                                if (level[camera[0] + j + 1, camera[1] + i] == 8 || level[camera[0] + j + 1, camera[1] + i] == 9 || level[camera[0] + j + 1, camera[1] + i] == 10) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j, camera[1] + i - 1] == 8 || level[camera[0] + j, camera[1] + i - 1] == 9 || level[camera[0] + j, camera[1] + i - 1] == 10) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j - 1, camera[1] + i] == 8 || level[camera[0] + j - 1, camera[1] + i] == 9 || level[camera[0] + j - 1, camera[1] + i] == 10) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                if (level[camera[0] + j, camera[1] + i + 1] == 8 || level[camera[0] + j, camera[1] + i + 1] == 9 || level[camera[0] + j, camera[1] + i + 1] == 10) { floorImage += "1"; }
                                else { floorImage += "0"; }
                                floorImage += ".png";
                                if (floorImage == "Graphycs/ByteLikeGraphycs/grass0000.png" && level[camera[0] + j, camera[1] + i] != 8) { floorImage = "Graphycs/ByteLikeGraphycs/hiddentrap.png"; }
                                break;
                            // Trap
                            case 11:
                                floorImage = "Graphycs/ByteLikeGraphycs/spiketrap.png";
                                break;
                            // Posion Trap
                            case 12:
                                floorImage = "Graphycs/ByteLikeGraphycs/poisontrap.png";
                                break;
                        }
                        imageDrawing.ImageSource = new BitmapImage(new Uri(floorImage, UriKind.Relative));
                    }
                    else
                    {
                        imageDrawing.ImageSource = new BitmapImage(new Uri("Graphycs/ByteLikeGraphycs/darkness.png", UriKind.Relative));
                        floorImage = "Graphycs/ByteLikeGraphycs/darkness.png";
                    }

                    drawingGroup.Children.Add(imageDrawing);

                    if (player.position[0] == camera[0] + j && player.position[1] == camera[1] + i)
                    {
                        //Bitmap source1 = new Bitmap(System.Drawing.Image.FromFile("Graphycs/ByteLikeGraphycs/player1.png")); // your source images - assuming they're the same size
                        //Bitmap source2 = new Bitmap(System.Drawing.Image.FromFile(floorImage));
                        //var target = new Bitmap(source1.Width, source1.Height, PixelFormat.Format32bppArgb);
                        //var graphics = Graphics.FromImage(target);
                        //graphics.CompositingMode = CompositingMode.SourceOver; // this is the default, but just to be clear

                        //graphics.DrawImage(source1, 0, 0);
                        //graphics.DrawImage(source2, 0, 0);



                        imageDrawing.ImageSource = new BitmapImage(new Uri("Graphycs/ByteLikeGraphycs/player1.png", UriKind.Relative));
                        drawingGroup.Children.Add(imageDrawing);
                    }

                }

                
            }
            //

            DrawingImage drawingImageSource = new DrawingImage(drawingGroup);

            // Freeze the DrawingImage for performance benefits.
            //drawingImageSource.Freeze();

            System.Windows.Controls.Image imageControl = new System.Windows.Controls.Image();
            imageControl.Stretch = Stretch.Fill;
            imageControl.Source = drawingImageSource;

            return imageControl;

        }



        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

            // Clear arrays
            InitLevel();

            // Generate level, set player's initial position
            level = Generator.Generate(level, out player.position, floor);
            floor++;

            camera[0] = player.position[0];
            camera[1] = player.position[1];

            Border imageBorder = new Border();
            imageBorder.BorderBrush = System.Windows.Media.Brushes.Gray;
            imageBorder.BorderThickness = new Thickness(0);
            imageBorder.HorizontalAlignment = HorizontalAlignment.Left;
            imageBorder.VerticalAlignment = VerticalAlignment.Top;
            imageBorder.Margin = new Thickness(0);
            imageBorder.Child = DrawMap(prevresponse);

            this.Background = System.Windows.Media.Brushes.White;
            this.Margin = new Thickness(0);
            this.Content = imageBorder;
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {


            response = player.Logics(ref level);

            // Set camera to player's position
            camera[0] = player.position[0];
            camera[1] = player.position[1];





            Border imageBorder = new Border();
            imageBorder.BorderBrush = System.Windows.Media.Brushes.Gray;
            imageBorder.BorderThickness = new Thickness(0);
            imageBorder.HorizontalAlignment = HorizontalAlignment.Left;
            imageBorder.VerticalAlignment = VerticalAlignment.Top;
            imageBorder.Margin = new Thickness(0);
            imageBorder.Child = DrawMap(prevresponse);

            this.Background = System.Windows.Media.Brushes.White;
            this.Margin = new Thickness(0);
            this.Content = imageBorder;

            Console.Write(response);


            prevresponse = response;
            Console.WriteLine("HP - {0} / {1}", player.Stats["HP"], player.Stats["MaxHP"]);
            prevresponse += String.Format("HP - {0} / {1}", player.Stats["HP"], player.Stats["MaxHP"]);

            Console.WriteLine("Mana - {0} / {1}", player.Stats["Mana"], player.Stats["MaxMana"]);
            prevresponse += String.Format("Mana - {0} / {1}", player.Stats["Mana"], player.Stats["MaxMana"]);


            Console.WriteLine("Defense - {0} / {1}", player.Stats["Defense"], player.Stats["MagicDefense"]);
            prevresponse += String.Format("Defense - {0} / {1}", player.Stats["Defense"], player.Stats["MagicDefense"]);


            Console.WriteLine("Level {0} - {1} / {2}", player.Stats["Level"], player.Stats["XP"], 90 + Math.Pow(player.Stats["Level"], 2) * 10);
            prevresponse += String.Format("Level {0} - {1} / {2}", player.Stats["Level"], player.Stats["XP"], 90 + Math.Pow(player.Stats["Level"], 2) * 10);

            Console.WriteLine("{0} Str, {1} Mag, {2} Agi", player.Stats["Strength"], player.Stats["Magic"], player.Stats["Agility"]);
            prevresponse += String.Format("{0} Str, {1} Mag, {2} Agi", player.Stats["Strength"], player.Stats["Magic"], player.Stats["Agility"]);


            
            //goto miniReset;
        }
    }



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
        }

        // Room creator
        static void CreateRoom(bool remember)
        {
            int water = 0;
            int[] waterCords = new int[] { 0, 0 };
            bool lava = false;
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

            // Generate tiles
            for (int i = -(room[1] / 2); i < room[1] / 2; i++)
            {
                for (int j = -(room[0] / 2); j < room[0] / 2; j++)
                {
                    // If we're in boundaries
                    if (position[0] + j >= 0 && position[0] + j < level.GetLength(0) && position[1] + i >= 0 && position[1] + i < level.GetLength(1))
                    {
                        // If we're on room's edges or we're generating a rock -> set to wall
                        if (i == -(room[1] / 2) || i + 1 >= (room[1] / 2) || j == -(room[0] / 2) || j + 1 >= (room[0] / 2) || remember == false)
                        {
                            level[position[0] + j, position[1] + i] = 2;
                        }
                        // if we're in the center of the room, set tile to Room Center
                        else if (j == 0 && i == 0)
                        {
                            level[position[0] + j, position[1] + i] = 3;
                        }
                        // Else, set to floor
                        else
                        {
                            level[position[0] + j, position[1] + i] = 1;
                            if (rand.Next(5) == 0) { level[position[0] + j, position[1] + i] = 8; }
                            else if (rand.Next(20) == 0) { level[position[0] + j, position[1] + i] = 9; }
                            else if (rand.Next(15) == 0) { level[position[0] + j, position[1] + i] = 10; }
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
        static public int[,] Generate(int[,] game, out int[] player, int currentfloor)
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
                    CreateRoom(true);
                }
            }
            //

            // Create a bunch of rocks
            for (int i = 0; i < size; i++)
            {
                if (Randomize())
                {
                    CreateRoom(false);
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
                                        Generator.level[position[0], position[1] + j] = 4;
                                    }
                                    break;
                                case 180:
                                    for (int j = -(width / 2) + 1; j < width / 2 - 1; j++)
                                    {
                                        Generator.level[position[0], position[1] + j] = 4;
                                    }
                                    break;
                                case 90:
                                    for (int j = -(width / 2) + 1; j < width / 2 - 1; j++)
                                    {
                                        Generator.level[position[0] + j, position[1]] = 4;
                                    }
                                    break;
                                case 270:
                                    for (int j = -(width / 2) + 1; j < width / 2 - 1; j++)
                                    {
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


    public class Item
    {
        // 0 - Useless, 1 - Head, 2 - Torso, 3 - Legs, 4 - Weapon, 5 - OffHand, 6 - Torch, 7 - Ring, 8 - Necklace, 9 - Consumable, 10 - Ammo
        public int GearType = 0;
        public string Name = "???";
        public string Description = "This seems useless";
        public Dictionary<string, int> Stats = new Dictionary<string, int>();
        public string Spell;
        public bool IsHeavy = false;

        public Item()
        {
            Stats.Add("Torch", 0);
            Stats.Add("MaxHP", 0);
            Stats.Add("MaxMana", 0);
            Stats.Add("Strength", 0);
            Stats.Add("Magic", 0);
            Stats.Add("Agility", 0);
            Stats.Add("Defense", 0);
            Stats.Add("MagicDefense", 0);
            Stats.Add("HPRegen", 0);
            Stats.Add("ManaRegen", 0);
        }
    }





    public abstract class Creature
    {
        public string Name;
        // x - 0, y - 1
        public int[] position;

        public Dictionary<string, int> Stats = new Dictionary<string, int>();

        // Burn (Fire) - 0, Poison (Earth) - 1, Freeze (Water) - 2, Paralysis (Lightning) - 3
        public int[] Statuses = new int[] { 0, 0, 0, 0 };
        public int[] Potentials = new int[] { 0, 0, 0, 0 };

        public Dictionary<string, int> Buffs = new Dictionary<string, int>();
        public Dictionary<string, int> BuffLevels = new Dictionary<string, int>();

        public bool IsGhost = false;
        public List<string> Spells = new List<string>();
        public Item[,] Inventory = new Item[11, 1];

        public Creature()
        {
            Stats.Add("Level", 1);
            Stats.Add("HP", 5);
            Stats.Add("Mana", 10);
            Stats.Add("Strength", 3);
            Stats.Add("Magic", 3);
            Stats.Add("Agility", 3);
            Stats.Add("Defense", 2);
            Stats.Add("MagicDefense", 2);
            Stats.Add("HPRegen", 0);
            Stats.Add("ManaRegen", 0);
            Stats.Add("MaxHP", 5);
            Stats.Add("MaxMana", 10);
            Stats.Add("MaxHPRegen", 5);
            Stats.Add("MaxManaRegen", 7);

            Buffs.Add("HP", 0);
            Buffs.Add("Mana", 0);
            Buffs.Add("Defense", 0);
            Buffs.Add("MagicDefense", 0);
            Buffs.Add("Strength", 0);
            Buffs.Add("Magic", 0);
            Buffs.Add("Agility", 0);
            Buffs.Add("HPRegen", 0);
            Buffs.Add("ManaRegen", 0);


            BuffLevels.Add("HP", 0);
            BuffLevels.Add("Mana", 0);
            BuffLevels.Add("Defense", 0);
            BuffLevels.Add("MagicDefense", 0);
            BuffLevels.Add("Strength", 0);
            BuffLevels.Add("Magic", 0);
            BuffLevels.Add("Agility", 0);
            BuffLevels.Add("HPRegen", 0);
            BuffLevels.Add("ManaRegen", 0);
        }

        protected bool FloorCheck(int tile)
        {
            if (IsGhost) { return false; }

            return true;
        }

        protected int Weight()
        {
            int result = 0;

            for (int i = 0; i < 9; i++)
            {
                if (Inventory[i, 0] != null)
                {
                    if (Inventory[i, 0].IsHeavy) { result++; }
                }
            }

            return result;
        }


        protected string Conditions(string response)
        {

            // Regeneration


            Stats["HPRegen"]++;
            Stats["ManaRegen"]++;


            foreach (KeyValuePair<string, int> item in Buffs)
            {
                if (item.Value > 0) { Buffs[item.Key]--; }
            }


            if ((Stats["HPRegen"] >= Stats["MaxHPRegen"] - BuffLevels["HPRegen"] && Stats["HP"] < Stats["MaxHP"] && Buffs["HPRegen"] > 0) || (Stats["HPRegen"] >= Stats["MaxHPRegen"] && Stats["HP"] < Stats["MaxHP"] && Buffs["HPRegen"] <= 0))
            {
                Stats["HP"]++;
                Stats["HPRegen"] = 0;
            }


            if ((Stats["ManaRegen"] >= Stats["MaxManaRegen"] - BuffLevels["ManaRegen"] && Stats["Mana"] < Stats["MaxMana"] && Buffs["ManaRegen"] > 0) || (Stats["ManaRegen"] >= Stats["MaxManaRegen"] && Stats["Mana"] < Stats["MaxMana"] && Buffs["ManaRegen"] <= 0))
            {
                Stats["Mana"]++;
                Stats["ManaRegen"] = 0;
            }



            // Conditions
            // Burn - 0, Poison - 1, Freeze - 2, Parallysis - 3

            for (int i = 0; i < 4; i++)
            {
                if (Potentials[i] > Stats["MagicDefense"] && Buffs["MagicDefense"] == 0 || Potentials[i] > Stats["MagicDefense"] + BuffLevels["MagicDefense"])
                {
                    Statuses[i] = 5;
                    switch (i)
                    {
                        case 0:
                            response += $"{Name} is on fire!\n";
                            break;
                        case 1:
                            response += $"{Name} is poisoned!\n";
                            break;
                        case 2:
                            response += $"{Name} is frozen!\n";
                            break;
                        case 3:
                            response += $"{Name} is paralysed!\n";
                            break;
                    }
                }
                if (Potentials[i] > 0) { Potentials[i]--; }
                if (Statuses[i] > 0) { Statuses[i]--; }

                switch (i)
                {
                    case 0:
                        if (Statuses[0] > 0) { Stats["HP"] -= 1; }
                        break;
                    case 1:
                        if (Statuses[1] % 4 == 0 && Statuses[1] > 0) { Stats["HP"] -= 5; }
                        break;
                }
            }

            return response;

        }

    }






    public class Player : Creature
    {

        public new Item[,] Inventory = new Item[11, 7];


        public Player(string name)
            : base()
        {
            Name = name;
            Stats["HP"] = 20;
            Stats["Mana"] = 10;
            Stats["MaxHP"] = 20;
            Stats["MaxMana"] = 10;

            Stats.Add("XP", 0);
            Stats.Add("Torch", 20);

            Buffs.Add("Torch", 0);

            BuffLevels.Add("Torch", 0);
        }

        // Main Logics
        public string Logics(ref int[,] level)
        {
            bool wasAlive = true;
            if (Stats["HP"] <= 0) { wasAlive = false; }
            string response = "";
            int[] movement = new int[] { 0, 0 };

            // Movement code

            // Keys
            // Move only if not frozen/paralysed
            if (Statuses[2] == 0 && Statuses[3] % 2 == 0)
            {
                if (Keyboard.IsKeyDown(Key.W)) { movement[1] = -1; }
                else if (Keyboard.IsKeyDown(Key.S)) { movement[1] = 1; }
                else if (Keyboard.IsKeyDown(Key.A)) { movement[0] = -1; }
                else if (Keyboard.IsKeyDown(Key.D)) { movement[0] = 1; }

            }
        //

        // Actual Movement
        WalkReset:
            // Check inbounds
            if (position[0] + movement[0] >= 0 && position[0] + movement[0] < level.GetLength(0) && position[1] + movement[1] >= 0 && position[1] + movement[1] < level.GetLength(1))
            {
                // FloorCheck does a check whether we actually step on the floor
                // We don't if we're ghost or have certain conditions or something
                switch (level[position[0] + movement[0], position[1] + movement[1]])
                {
                    // If walls, walk only if ghost, reset to check underfoot
                    case 2:
                    case 0:
                    case 5:
                        if (level[position[0] + movement[0], position[1] + movement[1]] == 5)
                        {
                            movement[0] = 0;
                            movement[1] = 0;
                            goto WalkReset;
                        }
                        else if (level[position[0], position[1]] != 2 && level[position[0], position[1]] != 0 && IsGhost == false)
                        {
                            movement[0] = 0;
                            movement[1] = 0;
                            goto WalkReset;
                        }
                        else if (IsGhost)
                        {
                            if (level[position[0], position[1]] == 0 && (position[0] + position[1]) % 2 == 0)
                            {
                                Stats["HP"]--;
                            }
                            position[0] += movement[0];
                            position[1] += movement[1];
                        }
                        break;

                    // Door
                    case 4:
                        if (IsGhost)
                        {
                            position[0] += movement[0];
                            position[1] += movement[1];
                        }
                        else { level[position[0] + movement[0], position[1] + movement[1]] = 1; }
                        break;

                    // Regular tile
                    case 1:
                    case 3:
                        position[0] += movement[0];
                        position[1] += movement[1];
                        break;

                    // Water tile, Freeze for a turn time from time
                    case 6:
                        if (Statuses[2] == 0)
                        {
                            position[0] += movement[0];
                            position[1] += movement[1];
                            if (FloorCheck(level[position[0], position[1]]))
                            {
                                Statuses[2] = 2 + Weight();
                                response += $"{Name} is splashing in a pool of water\n";
                            }
                        }
                        break;

                    // Lava
                    case 7:
                        position[0] += movement[0];
                        position[1] += movement[1];
                        if (FloorCheck(level[position[0], position[1]]))
                        {
                            Potentials[0]++;
                        }
                        break;

                    // Grass
                    case 8:
                        position[0] += movement[0];
                        position[1] += movement[1];
                        if (FloorCheck(level[position[0], position[1]]))
                        {
                            level[position[0], position[1]] = 1;
                            if (position[0] + 1 < level.GetLength(0))
                            {
                                if (level[position[0] + 1, position[1]] == 8) { level[position[0] + 1, position[1]] = 1; }
                            }
                            if (position[0] - 1 >= 0)
                            {
                                if (level[position[0] - 1, position[1]] == 8) { level[position[0] - 1, position[1]] = 1; }
                            }

                            if (position[1] + 1 < level.GetLength(1))
                            {
                                if (level[position[0], position[1] + 1] == 8) { level[position[0], position[1] + 1] = 1; }
                            }
                            if (position[1] - 1 >= 0)
                            {
                                if (level[position[0], position[1] - 1] == 8) { level[position[0], position[1] - 1] = 1; }
                            }
                        }
                        break;

                    // Spike traps
                    case 9:
                    case 11:
                        response += $"{Name} steps onto a spike trap!\n";
                        position[0] += movement[0];
                        position[1] += movement[1];
                        if (FloorCheck(level[position[0], position[1]]))
                        {
                            Stats["HP"] -= 3;
                            level[position[0], position[1]] = 11;

                            if (position[0] + 1 < level.GetLength(0))
                            {
                                if (level[position[0] + 1, position[1]] == 8) { level[position[0] + 1, position[1]] = 1; }
                                else if (level[position[0] + 1, position[1]] == 9) { level[position[0] + 1, position[1]] = 11; }
                                else if (level[position[0] + 1, position[1]] == 10) { level[position[0] + 1, position[1]] = 12; }
                            }
                            if (position[0] - 1 >= 0)
                            {
                                if (level[position[0] - 1, position[1]] == 8) { level[position[0] - 1, position[1]] = 1; }
                                else if (level[position[0] - 1, position[1]] == 9) { level[position[0] - 1, position[1]] = 11; }
                                else if (level[position[0] - 1, position[1]] == 10) { level[position[0] - 1, position[1]] = 12; }
                            }

                            if (position[1] + 1 < level.GetLength(1))
                            {
                                if (level[position[0], position[1] + 1] == 8) { level[position[0], position[1] + 1] = 1; }
                                else if (level[position[0], position[1] + 1] == 9) { level[position[0], position[1] + 1] = 11; }
                                else if (level[position[0], position[1] + 1] == 10) { level[position[0], position[1] + 1] = 12; }
                            }
                            if (position[1] - 1 >= 0)
                            {
                                if (level[position[0], position[1] - 1] == 8) { level[position[0], position[1] - 1] = 1; }
                                else if (level[position[0], position[1] - 1] == 9) { level[position[0], position[1] - 1] = 11; }
                                else if (level[position[0], position[1] - 1] == 10) { level[position[0], position[1] - 1] = 12; }
                            }
                        }
                        break;

                    // Poison traps
                    case 10:
                    case 12:
                        response += $"{Name} steps onto a poison trap!\n";
                        position[0] += movement[0];
                        position[1] += movement[1];
                        if (FloorCheck(level[position[0], position[1]]))
                        {
                            Statuses[1] = 10;
                            level[position[0], position[1]] = 12;

                            if (position[0] + 1 < level.GetLength(0))
                            {
                                if (level[position[0] + 1, position[1]] == 8) { level[position[0] + 1, position[1]] = 1; }
                                else if (level[position[0] + 1, position[1]] == 9) { level[position[0] + 1, position[1]] = 11; }
                                else if (level[position[0] + 1, position[1]] == 10) { level[position[0] + 1, position[1]] = 12; }
                            }
                            if (position[0] - 1 >= 0)
                            {
                                if (level[position[0] - 1, position[1]] == 8) { level[position[0] - 1, position[1]] = 1; }
                                else if (level[position[0] - 1, position[1]] == 9) { level[position[0] - 1, position[1]] = 11; }
                                else if (level[position[0] - 1, position[1]] == 10) { level[position[0] - 1, position[1]] = 12; }
                            }

                            if (position[1] + 1 < level.GetLength(1))
                            {
                                if (level[position[0], position[1] + 1] == 8) { level[position[0], position[1] + 1] = 1; }
                                else if (level[position[0], position[1] + 1] == 9) { level[position[0], position[1] + 1] = 11; }
                                else if (level[position[0], position[1] + 1] == 10) { level[position[0], position[1] + 1] = 12; }
                            }
                            if (position[1] - 1 >= 0)
                            {
                                if (level[position[0], position[1] - 1] == 8) { level[position[0], position[1] - 1] = 1; }
                                else if (level[position[0], position[1] - 1] == 9) { level[position[0], position[1] - 1] = 11; }
                                else if (level[position[0], position[1] - 1] == 10) { level[position[0], position[1] - 1] = 12; }
                            }
                        }
                        break;

                }
            }
            //


            response = Conditions(response);


            // Death
            if (wasAlive && Stats["HP"] <= 0) { response += $"{Name} dies!\n"; }

            return response;

        }
        //




        public int Torch()
        {
            int result = Stats["Torch"];

            if (Buffs["Torch"] > 0) { result += BuffLevels["Torch"]; }
            for (int i = 0; i < 9; i++)
            {
                if (Inventory[i, 0] != null)
                {
                    result += Inventory[i, 0].Stats["Torch"];
                }
            }

            return result;
        }
    }


}
