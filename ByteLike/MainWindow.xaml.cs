using System;
using System.IO;
using System.Collections.Generic;
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
using Newtonsoft.Json;

namespace ByteLike
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        static MediaPlayer music = new MediaPlayer();
        static int[,] level = new int[40, 40];
        static int[,] darkness = new int[40, 40];

        static Player player = new Player("Player");
        static int[] camera = new int[2];
        static int[] cameraSize = new int[] { 40, 30 };
        static int floor = 0;
        static List<Chest> chests = new List<Chest>();
        static List<Creature> enemies = new List<Creature>();
        static List<Effect> effects = new List<Effect>();

        static int[] sizes = new int[2];
        static string response = "";
        static string currentSound = "";
        static string tempSound = "";

        static Random rand = new Random();

        // Torch and darkness
        static void ClearLight()
        {
            int[] position = new int[2];
            bool doNext = true;

            // clear light around the player, replacing it with faded light
            for (int i = -player.GetStat("Torch") - 5; i <= player.GetStat("Torch") + 5; i++)
            {
                for (int j = -player.GetStat("Torch") - 5; j <= player.GetStat("Torch") + 5; j++)
                {
                    if (player.position[0] + j > 0 && player.position[0] + j < level.GetLength(0) && player.position[1] + i > 0 && player.position[1] + i < level.GetLength(1))
                    {
                        if (darkness[player.position[0] + j, player.position[1] + i] != 0)
                        {
                            darkness[player.position[0] + j, player.position[1] + i] = 2;
                        }
                    }
                }
            }
            // set a light source at the player position
            // extra light sources are in case the player is inside a wall
            darkness[player.position[0], player.position[1]] = 3;

            while (doNext)
            {
                doNext = false;
                // once again, cheking only inside a small area around the player
                for (int i = -player.GetStat("Torch") - 5; i <= player.GetStat("Torch") + 5; i++)
                {
                    for (int j = -player.GetStat("Torch") - 5; j <= player.GetStat("Torch") + 5; j++)
                    {
                        // check inbounds
                        if (player.position[0] + j >= 0 && player.position[0] + j < level.GetLength(0) && player.position[1] + i >= 0 && player.position[1] + i < level.GetLength(1))
                        {
                            // If we're trying to light this spot up
                            if (darkness[player.position[0] + j, player.position[1] + i] >= 3)
                            {
                                // set currently checked position
                                position[0] = player.position[0];
                                position[1] = player.position[1];
                                position[0] += j;
                                position[1] += i;
                                // if we have a light source (a tile of 3 or above inside the darkness array),
                                // and we're not on a wall -> spread the light and repeat the cycle so we spread it again later
                                if (level[position[0], position[1]] != 2 && level[position[0], position[1]] != 0 && level[position[0], position[1]] != 5 && level[position[0], position[1]] != 4 || j == 0 && i == 0)
                                {
                                    // Right
                                    if (position[0] + 1 >= 0 && position[0] + 1 < level.GetLength(0) && position[1] >= 0 && position[1] < level.GetLength(1) && darkness[position[0], position[1]] - 3 < player.GetStat("Torch"))
                                    {
                                        // since we set future lit tiles to 1, check if we should relight it again
                                        if (darkness[position[0] + 1, position[1]] != 1)
                                            darkness[position[0] + 1, position[1]] = darkness[position[0], position[1]] + 1;
                                        doNext = true;
                                    }

                                    // Left
                                    if (position[0] - 1 >= 0 && position[0] - 1 < level.GetLength(0) && position[1] >= 0 && position[1] < level.GetLength(1) && darkness[position[0], position[1]] - 3 < player.GetStat("Torch"))
                                    {
                                        if (darkness[position[0] - 1, position[1]] != 1)
                                            darkness[position[0] - 1, position[1]] = darkness[position[0], position[1]] + 1;
                                        doNext = true;
                                    }

                                    // Down
                                    if (position[0] >= 0 && position[0] < level.GetLength(0) && position[1] + 1 >= 0 && position[1] + 1 < level.GetLength(1) && darkness[position[0], position[1]] - 3 < player.GetStat("Torch"))
                                    {
                                        if (darkness[position[0], position[1] + 1] != 1)
                                            darkness[position[0], position[1] + 1] = darkness[position[0], position[1]] + 1;
                                        doNext = true;
                                    }

                                    // Up
                                    if (position[0] >= 0 && position[0] < level.GetLength(0) && position[1] - 1 >= 0 && position[1] - 1 < level.GetLength(1) && darkness[position[0], position[1]] - 3 < player.GetStat("Torch"))
                                    {
                                        if (darkness[position[0], position[1] - 1] != 1)
                                            darkness[position[0], position[1] - 1] = darkness[position[0], position[1]] + 1;
                                        doNext = true;
                                    }
                                }
                                // Here we're inside a wall, so we'll spread light only to nearby wall tiles
                                // This makes sure we can't see behind thin walls, but can see two tiles inside the wall
                                // The player's light level is also decreased here
                                else
                                {
                                    // Right
                                    if (position[0] + 1 >= 0 && position[0] + 1 < level.GetLength(0) && position[1] >= 0 && position[1] < level.GetLength(1) && darkness[position[0], position[1]] - 3 < player.GetStat("Torch")/1.5)
                                    {
                                        if (level[position[0] + 1, position[1]] == 0 || level[position[0] + 1, position[1]] == 2 || level[position[0] + 1, position[1]] == 4 || level[position[0] + 1, position[1]] == 5)
                                        {
                                            if (darkness[position[0] + 1, position[1]] != 1)
                                                darkness[position[0] + 1, position[1]] = darkness[position[0], position[1]] + 2;
                                        }
                                    }

                                    // Left
                                    if (position[0] - 1 >= 0 && position[0] - 1 < level.GetLength(0) && position[1] >= 0 && position[1] < level.GetLength(1) && darkness[position[0], position[1]] - 3 < player.GetStat("Torch")/1.5)
                                    {
                                        if (level[position[0] - 1, position[1]] == 0 || level[position[0] - 1, position[1]] == 2 || level[position[0] - 1, position[1]] == 4 || level[position[0] - 1, position[1]] == 5)
                                        {
                                            if (darkness[position[0] - 1, position[1]] != 1)
                                                darkness[position[0] - 1, position[1]] = darkness[position[0], position[1]] + 2;
                                        }
                                    }

                                    // Down
                                    if (position[0] >= 0 && position[0] < level.GetLength(0) && position[1] + 1 >= 0 && position[1] + 1 < level.GetLength(1) && darkness[position[0], position[1]] - 3 < player.GetStat("Torch")/1.5)
                                    {
                                        if (level[position[0], position[1] + 1] == 0 || level[position[0], position[1] + 1] == 2 || level[position[0], position[1] + 1] == 4 || level[position[0], position[1] + 1] == 5)
                                        {
                                            if (darkness[position[0], position[1] + 1] != 1)
                                                darkness[position[0], position[1] + 1] = darkness[position[0], position[1]] + 2;
                                        }
                                    }

                                    // Up
                                    if (position[0] >= 0 && position[0] < level.GetLength(0) && position[1] - 1 >= 0 && position[1] - 1 < level.GetLength(1) && darkness[position[0], position[1]] - 3 < player.GetStat("Torch")/1.5)
                                    {
                                        if (level[position[0], position[1] - 1] == 0 || level[position[0], position[1] - 1] == 2 || level[position[0], position[1] - 1] == 4 || level[position[0], position[1] - 1] == 5)
                                        {
                                            if (darkness[position[0], position[1] - 1] != 1)
                                                darkness[position[0], position[1] - 1] = darkness[position[0], position[1]] + 2;
                                        }
                                    }
                                }

                                // set the current tile to lit
                                darkness[position[0], position[1]] = 1;
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


        // DRAW MAP

        static Image DrawMap(string response)
        {
            // Clear darkness
            if (!player.OpenSpell)
            {
                ClearLight();
            }

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

            int currentChest = player.GetChest(ref chests);

            using (DrawingContext dc = drawingGroup.Open())
            {
                // Create dialogues with the response text
                FormattedText dialogue = new FormattedText(response, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 14, System.Windows.Media.Brushes.White);
                dialogue.MaxTextWidth = (cameraSize[0] * 16) - 48;

                // Draw the tiles
                for (int i = 0; i < level.GetLength(1) && i < cameraSize[1]; i++)
                {
                    for (int j = 0; j < level.GetLength(0) && j < cameraSize[0]; j++)
                    {
                        string floorImage = "";
                        // if visible
                        if (darkness[camera[0] + j, camera[1] + i] > 0)
                        {
                            floorImage = "Graphics/ByteLikeGraphics/Tiles/darkness.png";
                            // TILE SWITCH
                            switch (level[camera[0] + j, camera[1] + i])
                            {
                                // Void wall
                                case 0:
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/darkwall";
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
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/floor.png";
                                    break;
                                // Wall
                                case 2:
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/wall";
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
                                // Cracked floors
                                case 3:
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/floor1.png";
                                    break;
                                // Door
                                case 4:
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/door.png";
                                    break;
                                // Outter wall
                                case 5:
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/darkwall";

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
                                        if (camera[0] + i == 0 || camera[0] + i == level.GetLength(0) - 1 || level[camera[0] + j, level.GetLength(1) - 2] == 2 || level[camera[0] + j, level.GetLength(1) - 2] == 0)
                                        {
                                            floorImage += "1111";
                                        }
                                        else { floorImage += "0000"; }
                                    }

                                    floorImage += ".png";
                                    break;
                                // Water
                                case 6:
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/water";
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
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/lava";
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
                                case 16:
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/grass";
                                    if (level[camera[0] + j + 1, camera[1] + i] == 8 || level[camera[0] + j + 1, camera[1] + i] == 9 || level[camera[0] + j + 1, camera[1] + i] == 10 || level[camera[0] + j + 1, camera[1] + i] == 16) { floorImage += "1"; }
                                    else { floorImage += "0"; }
                                    if (level[camera[0] + j, camera[1] + i - 1] == 8 || level[camera[0] + j, camera[1] + i - 1] == 9 || level[camera[0] + j, camera[1] + i - 1] == 10 || level[camera[0] + j, camera[1] + i - 1] == 16) { floorImage += "1"; }
                                    else { floorImage += "0"; }
                                    if (level[camera[0] + j - 1, camera[1] + i] == 8 || level[camera[0] + j - 1, camera[1] + i] == 9 || level[camera[0] + j - 1, camera[1] + i] == 10 || level[camera[0] + j - 1, camera[1] + i] == 16) { floorImage += "1"; }
                                    else { floorImage += "0"; }
                                    if (level[camera[0] + j, camera[1] + i + 1] == 8 || level[camera[0] + j, camera[1] + i + 1] == 9 || level[camera[0] + j, camera[1] + i + 1] == 10 || level[camera[0] + j, camera[1] + i + 1] == 16) { floorImage += "1"; }
                                    else { floorImage += "0"; }
                                    floorImage += ".png";
                                    if (floorImage == "Graphics/ByteLikeGraphics/Tiles/grass0000.png" && level[camera[0] + j, camera[1] + i] != 8) { floorImage = "Graphics/ByteLikeGraphics/Tiles/hiddentrap.png"; }
                                    break;
                                // Trap
                                case 11:
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/spiketrap.png";
                                    break;
                                // Posion Trap
                                case 12:
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/poisontrap.png";
                                    break;
                                // Poisonous vines
                                case 13:
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/vines";
                                    if (level[camera[0] + j + 1, camera[1] + i] == 13) { floorImage += "1"; }
                                    else { floorImage += "0"; }
                                    if (level[camera[0] + j, camera[1] + i - 1] == 13) { floorImage += "1"; }
                                    else { floorImage += "0"; }
                                    if (level[camera[0] + j - 1, camera[1] + i] == 13) { floorImage += "1"; }
                                    else { floorImage += "0"; }
                                    if (level[camera[0] + j, camera[1] + i + 1] == 13) { floorImage += "1"; }
                                    else { floorImage += "0"; }
                                    floorImage += ".png";
                                    break;
                                // Electro terrain
                                case 14:
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/electro";
                                    if (level[camera[0] + j + 1, camera[1] + i] == 14) { floorImage += "1"; }
                                    else { floorImage += "0"; }
                                    if (level[camera[0] + j, camera[1] + i - 1] == 14) { floorImage += "1"; }
                                    else { floorImage += "0"; }
                                    if (level[camera[0] + j - 1, camera[1] + i] == 14) { floorImage += "1"; }
                                    else { floorImage += "0"; }
                                    if (level[camera[0] + j, camera[1] + i + 1] == 14) { floorImage += "1"; }
                                    else { floorImage += "0"; }
                                    floorImage += ".png";
                                    break;
                                // Exit
                                case 15:
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/nextfloor.png";
                                    break;

                            }

                        }
                        // if covered in darkness
                        else
                        {
                            floorImage = "Graphics/ByteLikeGraphics/Tiles/darkness.png";
                        }

                        // draw tile
                        dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));

                        // if partly in darkness, draw partial darkness
                        if (darkness[camera[0] + j, camera[1] + i] == 2)
                        {
                            // EFFECTS IN PARTIAL DARKNESS
                            foreach (Effect item in effects)
                            {
                                if (camera[0] + j == item.position[0] && camera[1] + i == item.position[1])
                                    dc.DrawImage(new BitmapImage(new Uri(item.File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                            }

                            dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Tiles/partialdarkness.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                        }
                        // if lit (again) draw chest sprites
                        else if (darkness[camera[0] + j, camera[1] + i] > 0)
                        {
                            // CHESTS
                            foreach (Chest item in chests)
                            {
                                if (item.position[0] == camera[0] + j && item.position[1] == camera[1] + i && level[item.position[0],item.position[1]] != 2 && level[item.position[0], item.position[1]] != 0 && level[item.position[0], item.position[1]] != 5 && level[item.position[0], item.position[1]] != 4)
                                {
                                    dc.DrawImage(new BitmapImage(new Uri(item.File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }
                            }

                            // PLAYER
                            if (player.position[0] == camera[0] + j && player.position[1] == camera[1] + i)
                            {
                                // Draw quiver if slot is full and is a quiver
                                if (player.Inventory[4, 0] != null)
                                {
                                    if (player.Inventory[4, 0].Name.Contains("Quiver"))
                                    {
                                        dc.DrawImage(new BitmapImage(new Uri(player.Inventory[4, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                    }
                                }

                                // If not a ghost, draw regular sprite
                                if (!player.IsGhost)
                                {
                                    dc.DrawImage(new BitmapImage(new Uri(player.File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }
                                // Otherwise, draw the ghosty appearance
                                else
                                {
                                    dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Creatures/player4.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }


                                // Draw legs
                                if (player.Inventory[2, 0] != null)
                                    dc.DrawImage(new BitmapImage(new Uri(player.Inventory[2, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                // Draw chestplate
                                if (player.Inventory[1, 0] != null)
                                    dc.DrawImage(new BitmapImage(new Uri(player.Inventory[1, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                // Draw hat
                                if (player.Inventory[0, 0] != null)
                                    dc.DrawImage(new BitmapImage(new Uri(player.Inventory[0, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                // Draw weapon
                                if (player.Inventory[3, 0] != null)
                                    dc.DrawImage(new BitmapImage(new Uri(player.Inventory[3, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));

                                // Draw offhand if not a quiver (drew quiver earlier)
                                if (player.Inventory[4, 0] != null)
                                {
                                    if (!player.Inventory[4, 0].Name.Contains("Quiver"))
                                    {
                                        dc.DrawImage(new BitmapImage(new Uri(player.Inventory[4, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                    }
                                }

                            }
                            // End player draw code


                            // ENEMIES
                            foreach (Creature item in enemies)
                            {
                                if (camera[0] + j == item.position[0] && camera[1] + i == item.position[1])
                                {
                                    if (item.Inventory[4, 0] != null && item.DrawEquipment)
                                    {
                                        if (item.Inventory[4, 0].Name.Contains("Quiver"))
                                        {
                                            dc.DrawImage(new BitmapImage(new Uri(item.Inventory[4, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                        }
                                    }

                                    dc.DrawImage(new BitmapImage(new Uri(item.File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));

                                    if (item.DrawEquipment)
                                    {
                                        // Draw legs
                                        if (item.Inventory[2, 0] != null)
                                            dc.DrawImage(new BitmapImage(new Uri(item.Inventory[2, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                        // Draw chestplate
                                        if (item.Inventory[1, 0] != null)
                                            dc.DrawImage(new BitmapImage(new Uri(item.Inventory[1, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                        // Draw hat
                                        if (item.Inventory[0, 0] != null)
                                            dc.DrawImage(new BitmapImage(new Uri(item.Inventory[0, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                        // Draw weapon
                                        if (item.Inventory[3, 0] != null)
                                            dc.DrawImage(new BitmapImage(new Uri(item.Inventory[3, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));

                                        // Draw offhand if not a quiver (drew quiver earlier)
                                        if (item.Inventory[4, 0] != null)
                                        {
                                            if (!item.Inventory[4, 0].Name.Contains("Quiver"))
                                            {
                                                dc.DrawImage(new BitmapImage(new Uri(item.Inventory[4, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                            }
                                        }
                                    }

                                    if (i > 0 && (!player.OpenInventory || player.OpenSpell))
                                    {
                                        //FormattedText dialogueen = new FormattedText($"{item.Stats["HP"]}/{item.GetStat("MaxHP")}", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 4, Brushes.Red);
                                        //dc.DrawText(dialogueen, new Point(j*16 + (16 - dialogueen.Width)/2, i*16 - 2));

                                        for (int f = 0; f < 14; f++)
                                        {
                                            string hpbar = "Graphics/ByteLikeGraphics/Hud/healthbar";
                                            if (f == 0)
                                            {
                                                if (item.Stats["HP"] < (item.GetStat("MaxHP") / 16.0))
                                                    hpbar += "0.png";
                                                else
                                                    hpbar += "3.png";

                                                dc.DrawImage(new BitmapImage(new Uri(hpbar, UriKind.Relative)), new Rect(j * 16 + f, i * 16 - 4, 2.25, 4));
                                            }
                                            else if (f == 13)
                                            {
                                                if (item.Stats["HP"] < (item.GetStat("MaxHP") / 16.0) * 15.0)
                                                    hpbar += "2.png";
                                                else
                                                    hpbar += "5.png";

                                                dc.DrawImage(new BitmapImage(new Uri(hpbar, UriKind.Relative)), new Rect(j * 16 + f, i * 16 - 4, 2, 4));
                                            }
                                            else
                                            {
                                                if (item.Stats["HP"] < (item.GetStat("MaxHP") / 16.0) * (f+1.0))
                                                    hpbar += "1.png";
                                                else
                                                    hpbar += "4.png";

                                                dc.DrawImage(new BitmapImage(new Uri(hpbar, UriKind.Relative)), new Rect(j * 16 + f, i * 16 - 4, 3, 4));
                                            }
                                        }
                                    }
                                }
                            }

                            // EFFECTS
                            foreach (Effect item in effects)
                            {
                                if (camera[0] + j == item.position[0] && camera[1] + i == item.position[1])
                                    dc.DrawImage(new BitmapImage(new Uri(item.File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                            }
                        }


                        // Inventory draw code, still in j-i switch
                        if (player.OpenInventory)
                        {
                            // Don't draw inventory if casting
                            if (!player.OpenSpell)
                            {
                                // If j-i is in the regular inventory. Mind we have an offset here
                                if (j >= 4 && j - 4 < player.Inventory.GetLength(0) && i - 1 < player.Inventory.GetLength(1) && i > 0)
                                {
                                    // Draw the slot
                                    floorImage = "Graphics/ByteLikeGraphics/Hud/invslot";
                                    if (i == 1)
                                    {
                                        floorImage += (j - 3);
                                    }
                                    else { floorImage += 0; }
                                    floorImage += ".png";
                                    dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));

                                    // Draw an item
                                    if (player.Inventory[j - 4, i - 1] != null)
                                    {
                                        dc.DrawImage(new BitmapImage(new Uri(player.Inventory[j - 4, i - 1].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                        // Draw quantity
                                        if (player.Inventory[j - 4, i - 1].Quantity > 1)
                                        {
                                            FormattedText dialogue3 = new FormattedText(player.Inventory[j - 4, i - 1].Quantity.ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 8, System.Windows.Media.Brushes.White);
                                            dialogue3.MaxTextWidth = 56;
                                            dc.DrawText(dialogue3, new Point(j * 16, 8 + i * 16));
                                        }
                                    }
                                }
                                // If j-i is in spell slots
                                else if (j - 4 >= player.Inventory.GetLength(0) && j - 4 < player.Inventory.GetLength(0) + 5 && i - 1 < player.Stats["SpellSlots"] && i > 0)
                                {
                                    floorImage = "Graphics/ByteLikeGraphics/Hud/spellslot";
                                    if (j - 4 == player.Inventory.GetLength(0))
                                    {
                                        floorImage += "0.png";
                                    }
                                    else if (j - 4 == player.Inventory.GetLength(0) + 4)
                                    {
                                        floorImage += "2.png";
                                    }
                                    else
                                    {
                                        floorImage += "1.png";
                                    }
                                    dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }
                                // Delete spell slot
                                else if (j - 4 == player.Inventory.GetLength(0) + 5 && i - 1 < player.Stats["SpellSlots"] && i > 0)
                                {
                                    dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/spellslot6.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }
                                // If j-i is outside of the inventory but we have a chest
                                else if (currentChest != -1)
                                {
                                    if (j >= 4 && j - 4 < player.Inventory.GetLength(0) && i - 1 - player.Inventory.GetLength(1) - 1 < chests[currentChest].Inventory.GetLength(1) && i - 1 > player.Inventory.GetLength(1))
                                    {
                                        // Draw slot
                                        floorImage = "Graphics/ByteLikeGraphics/Hud/invslot0.png";
                                        dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));

                                        // Draw item
                                        if (chests[currentChest].Inventory[j - 4, i - 2 - player.Inventory.GetLength(1)] != null)
                                        {
                                            dc.DrawImage(new BitmapImage(new Uri(chests[currentChest].Inventory[j - 4, i - 2 - player.Inventory.GetLength(1)].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));

                                            // Draw quantity
                                            if (chests[currentChest].Inventory[j - 4, i - 2 - player.Inventory.GetLength(1)].Quantity > 1)
                                            {
                                                FormattedText dialogue3 = new FormattedText(chests[currentChest].Inventory[j - 4, i - 2 - player.Inventory.GetLength(1)].Quantity.ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 8, System.Windows.Media.Brushes.White);
                                                dialogue3.MaxTextWidth = 56;
                                                dc.DrawText(dialogue3, new Point(j * 16, 8 + i * 16));
                                            }
                                        }
                                    }
                                }


                                // Selected slot is in inventory
                                if (j - 4 == player.SelectedSlot[0] && i - 1 == player.SelectedSlot[1] && player.SelectedSlot[1] < player.Inventory.GetLength(1))
                                {
                                    dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/invslot13.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }
                                // Selected slot is in chest
                                else if (j - 4 == player.SelectedSlot[0] && i - 1 == player.SelectedSlot[1] + 1 && player.SelectedSlot[1] >= player.Inventory.GetLength(1))
                                {
                                    dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/invslot13.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }


                                // Current slot is in inventory
                                if ((j - 4 == player.CurrentSlot[0] && player.CurrentSlot[0] < player.Inventory.GetLength(0) || j-4 == player.CurrentSlot[0]+4 && player.CurrentSlot[0] == player.Inventory.GetLength(0)+1) && i - 1 == player.CurrentSlot[1] && (player.CurrentSlot[1] < player.Inventory.GetLength(1) || player.CurrentSlot[0] > player.Inventory.GetLength(0)) && player.CurrentSlot[0] != player.Inventory.GetLength(0))
                                {
                                    dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/invslot12.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }
                                // Current slot in spells
                                else if (j - 4 >= player.Inventory.GetLength(0) && j - 4 < player.Inventory.GetLength(0) + 5 && i - 1 == player.CurrentSlot[1] && player.CurrentSlot[0] == player.Inventory.GetLength(0))
                                {
                                    floorImage = "Graphics/ByteLikeGraphics/Hud/spellslot";
                                    if (j - 4 == player.Inventory.GetLength(0))
                                    {
                                        floorImage += "3.png";
                                    }
                                    else if (j - 4 == player.Inventory.GetLength(0) + 4)
                                    {
                                        floorImage += "5.png";
                                    }
                                    else
                                    {
                                        floorImage += "4.png";
                                    }
                                    dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }
                                // Current slot is in chest
                                else if (j - 4 == player.CurrentSlot[0] && i - 1 == player.CurrentSlot[1] + 1 && player.CurrentSlot[1] >= player.Inventory.GetLength(1) && player.CurrentSlot[0] < player.Inventory.GetLength(0))
                                {
                                    dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/invslot12.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }



                            }

                        }
                        // End inventory draw code


                        // Draw the response dialogue box
                        if (response != "" && j>1)
                        {
                            if (i * 16 >= (cameraSize[1] * 16) - dialogue.Height - 16)
                            {
                                if ((i - 1) * 16 < (cameraSize[1] * 16) - dialogue.Height - 16)
                                {
                                    if (j == 2) { dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/dialoguebox1.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                    else if (j + 1 == cameraSize[0]) { dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/dialoguebox4.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                    else { dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/dialoguebox2.png", UriKind.Relative)), new Rect(j * 16, i * 16, 16, 16)); }
                                }
                                else
                                {
                                    if (j == 2) { dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/dialoguebox3.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                    else if (j + 1 == cameraSize[0]) { dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/dialoguebox5.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                    else { dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/dialoguebox6.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                }
                            }
                        }
                        // end response dialogue box


                    } // end j
                } // end i
                //


                // Spell casting
                if (player.OpenSpell)
                {
                    int[] spellpos = new int[2] { player.position[0], player.position[1] };

                    int counter = 0;
                    int direction = 0;

                    if (player.CurrentSlot[0] + player.position[0] > camera[0] && player.CurrentSlot[0] + player.position[0] + 1 < camera[0] + cameraSize[0] && player.CurrentSlot[1] + player.position[1] > camera[1] && player.CurrentSlot[1] + player.position[1] + 1 < camera[1] + cameraSize[1])
                    {
                        dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/invslot12.png", UriKind.Relative)), new Rect((player.position[0] + player.CurrentSlot[0] - camera[0]) * 16, (player.position[1] + player.CurrentSlot[1] - camera[1]) * 16, 17, 17));
                    }


                    if (DistanceBetween(new int[2] { player.position[0], player.position[1] }, new int[2] { player.position[0] + player.CurrentSlot[0], player.position[1] }) >= DistanceBetween(new int[2] { player.position[0], player.position[1] }, new int[2] { player.position[0], player.position[1] + player.CurrentSlot[1] }))
                    {
                        if (player.CurrentSlot[0] < 0)
                        {
                            direction = 180;
                        }
                    }
                    else
                    {
                        direction = 270;
                        if (player.CurrentSlot[1] < 0)
                        {
                            direction = 90;
                        }
                    }


                    while (spellpos[0] > camera[0] && spellpos[0] + 1 < camera[0] + cameraSize[0] && spellpos[1] > camera[1] && spellpos[1] + 1 < camera[1] + cameraSize[1] && (player.CurrentSlot[0] != 0 || player.CurrentSlot[1] != 0) && (darkness[spellpos[0], spellpos[1]] == 1 || darkness[spellpos[0], spellpos[1]] == 2) && level[spellpos[0], spellpos[1]] != 0 && level[spellpos[0], spellpos[1]] != 2 && level[spellpos[0], spellpos[1]] != 5 && level[spellpos[0], spellpos[1]] != 4 && player.DrawSpellLine)
                    {
                        switch (direction)
                        {
                            case 0:
                                spellpos[0]++;
                                if (player.CurrentSlot[1] != 0)
                                {
                                    if (player.CurrentSlot[1] < 0)
                                    {
                                        counter++;
                                        if (counter >= player.CurrentSlot[0] / (-player.CurrentSlot[1]))
                                        {
                                            spellpos[1]--;
                                            counter = 0;
                                        }
                                    }
                                    else
                                    {
                                        counter++;
                                        if (counter >= player.CurrentSlot[0] / player.CurrentSlot[1])
                                        {
                                            spellpos[1]++;
                                            counter = 0;
                                        }
                                    }
                                }
                                break;
                            case 180:
                                spellpos[0]--;
                                if (player.CurrentSlot[1] != 0)
                                {
                                    if (player.CurrentSlot[1] < 0)
                                    {
                                        counter--;
                                        if (counter <= player.CurrentSlot[0] / (-player.CurrentSlot[1]))
                                        {
                                            spellpos[1]--;
                                            counter = 0;
                                        }
                                    }
                                    else
                                    {
                                        counter--;
                                        if (counter <= player.CurrentSlot[0] / player.CurrentSlot[1])
                                        {
                                            spellpos[1]++;
                                            counter = 0;
                                        }
                                    }
                                }
                                break;
                            case 90:
                                spellpos[1]--;
                                if (player.CurrentSlot[0] != 0)
                                {
                                    if (player.CurrentSlot[0] < 0)
                                    {
                                        counter--;
                                        if (counter <= player.CurrentSlot[1] / (-player.CurrentSlot[0]))
                                        {
                                            spellpos[0]--;
                                            counter = 0;
                                        }
                                    }
                                    else
                                    {
                                        counter--;
                                        if (counter <= player.CurrentSlot[1] / player.CurrentSlot[0])
                                        {
                                            spellpos[0]++;
                                            counter = 0;
                                        }
                                    }
                                }
                                break;
                            case 270:
                                spellpos[1]++;
                                if (player.CurrentSlot[0] != 0)
                                {
                                    if (player.CurrentSlot[0] < 0)
                                    {
                                        counter++;
                                        if (counter >= player.CurrentSlot[1] / (-player.CurrentSlot[0]))
                                        {
                                            spellpos[0]--;
                                            counter = 0;
                                        }
                                    }
                                    else
                                    {
                                        counter++;
                                        if (counter >= player.CurrentSlot[1] / player.CurrentSlot[0])
                                        {
                                            spellpos[0]++;
                                            counter = 0;
                                        }
                                    }
                                }
                                break;
                        }
                        if (spellpos[0] - camera[0] != (player.position[0] + player.CurrentSlot[0] - camera[0]) || spellpos[1] - camera[1] != (player.position[1] + player.CurrentSlot[1] - camera[1]))
                            dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/invslot13.png", UriKind.Relative)), new Rect((spellpos[0] - camera[0]) * 16, (spellpos[1] - camera[1]) * 16, 17, 17));
                    }

                }
                // End spell casting


                // Draw stats hud
                for (int i = 0; i < 10; i++)
                {
                    string floorImage2 = "";
                    string floorImage = "Graphics/ByteLikeGraphics/Hud/hud";
                    floorImage += i;
                    floorImage += ".png";
                    if (i == 0)
                    {
                        dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(0, i * 32, 17, 17));
                        if (effects.Count > 0)
                            dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/effectplayout.png", UriKind.Relative)), new Rect(cameraSize[0] * 16 - 16, i * 32, 17, 17));
                    }
                    else if (i != 9)
                        dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(0, i * 32 - 16, 17, 17));
                    else
                    {
                        if (player.Inventory[3, 0] != null)
                        {
                            if (player.Inventory[3, 0].Name.ToLower().Contains("bow"))
                                dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(0, i * 32 - 16, 17, 17));
                        }
                    }

                    switch (i)
                    {
                        case 0:
                            floorImage = String.Format("{0}/{1}", player.Stats["XP"], (int)(90 + Math.Pow(player.Stats["Level"], 2) * 10));
                            break;
                        case 1:
                            floorImage = player.Stats["Level"].ToString();
                            break;
                        case 2:
                            floorImage = String.Format("{0}/{1}", player.Stats["HP"].ToString(), player.GetStat("MaxHP"));
                            if (player.GetStat("HPRegen") > 0)
                                floorImage2 = $"| {player.Stats["MaxHPRegen"]}/{player.GetStat("HPRegen")}";
                            else
                                floorImage2 = $"| {-player.GetStat("HPRegen") + 2}";
                            break;
                        case 3:
                            floorImage = String.Format("{0}/{1}", player.Stats["Mana"].ToString(), player.GetStat("MaxMana"));
                            if (player.GetStat("ManaRegen") > 0)
                                floorImage2 = $"| {player.Stats["MaxManaRegen"]}/{player.GetStat("ManaRegen")}";
                            else
                                floorImage2 = $"| {-player.GetStat("ManaRegen") + 2}";
                            break;
                        case 4:
                            floorImage = player.GetStat("Defense").ToString();
                            if (player.Buffs["Defense"] > 0)
                                floorImage2 = $"| {player.BuffLevels["Defense"]}:{player.Buffs["Defense"]}";
                            break;
                        case 5:
                            floorImage = player.GetStat("MagicDefense").ToString();
                            if (player.Buffs["MagicDefense"] > 0)
                                floorImage2 = $"| {player.BuffLevels["MagicDefense"]}:{player.Buffs["MagicDefense"]}";
                            break;
                        case 6:
                            floorImage = player.GetStat("Strength").ToString();
                            if (player.Buffs["Strength"] > 0)
                                floorImage2 = $"| {player.BuffLevels["Strength"]}:{player.Buffs["Strength"]}";
                            break;
                        case 7:
                            floorImage = player.GetStat("Magic").ToString();
                            if (player.Buffs["Magic"] > 0)
                                floorImage2 = $"| {player.BuffLevels["Magic"]}:{player.Buffs["Magic"]}";
                            break;
                        case 8:
                            floorImage = player.GetStat("Agility").ToString();
                            if (player.Buffs["Agility"] > 0)
                                floorImage2 = $"| {player.BuffLevels["Agility"]}:{player.Buffs["Agility"]}";
                            break;
                        case 9:
                            floorImage = player.GetArrows().ToString();
                            break;
                    }
                    FormattedText dialogue2 = new FormattedText(floorImage, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 16, Brushes.White);
                    FormattedText dialogue3 = new FormattedText(floorImage2, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 8, Brushes.White);
                    dialogue2.MaxTextWidth = 320;
                    dialogue3.MaxTextWidth = 320;
                    if (i != 0 && i != 9)
                    {
                        dc.DrawText(dialogue2, new Point(4, 14 + i * 32 - 16));
                        if (floorImage2 != "")
                            dc.DrawText(dialogue3, new Point(20, 4 + i * 32 - 16));
                    }
                    else if (i != 9)
                    {
                        dc.DrawText(dialogue2, new Point(20, 0));
                        // Draw statuses
                        int statsOffset = 0;
                        while (statsOffset < dialogue2.Width + 20)
                        {
                            statsOffset += 16;
                        }
                        for (int j = 0; j < 4; j++)
                        {
                            if (player.Statuses[j] > 0)
                            {
                                dialogue3 = new FormattedText(player.Statuses[j].ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 8, Brushes.White);
                                dc.DrawImage(new BitmapImage(new Uri($"Graphics/ByteLikeGraphics/Hud/statuses{j}.png", UriKind.Relative)), new Rect(statsOffset, 0, 17, 17));
                                dc.DrawText(dialogue3, new Point(statsOffset+4, 8));
                                statsOffset += 16;
                            }
                        }
                        // end draw statuses
                    }
                    else
                    {
                        if (player.Inventory[3, 0] != null)
                        {
                            if (player.Inventory[3, 0].Name.ToLower().Contains("bow"))
                                dc.DrawText(dialogue2, new Point(4, 14 + i * 32 - 16));
                        }
                    }
                }
                // End stats hud

                

                // Spell names
                if (player.OpenInventory && !player.OpenSpell)
                {
                    int i = 0;
                    foreach (string item in player.Spells)
                    {
                        FormattedText dialogue3 = new FormattedText(item, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 10, System.Windows.Media.Brushes.White);
                        dialogue3.MaxTextWidth = 80;
                        dc.DrawText(dialogue3, new Point((player.Inventory.GetLength(0)+4)*16 + 4, 18 + i * 16));
                        i++;
                    }
                }

                // Response text
                if (response != "")
                {
                    dc.DrawText(dialogue, new Point(40, (cameraSize[1]*16)-dialogue.Height-8));
                }
                //


            } // end of using dc.open


            DrawingImage drawingImageSource = new DrawingImage(drawingGroup);

            System.Windows.Controls.Image imageControl = new System.Windows.Controls.Image();
            imageControl.Stretch = Stretch.Fill;
            imageControl.Source = drawingImageSource;



            return imageControl;
        }


        static void NewLevel()
        {
            level = new int[40 + floor*2, 40 + floor*2];
            darkness = new int[40 + floor*2, 40 + floor*2];

            for (int i = 0; i < level.GetLength(1); i++)
            {
                for (int j = 0; j < level.GetLength(0); j++)
                {
                    level[j, i] = 0;
                    darkness[j, i] = 0;
                }
            }
            chests = new List<Chest>();
            enemies = new List<Creature>();
            Generator.Reset();

            // Generate level, set player's initial position
            floor++;
            level = Generator.Generate(level, out player.position, floor, ref chests);

            camera[0] = player.position[0];
            camera[1] = player.position[1];
        }

        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            NewLevel();
            response = $"{player.Name} enters the dungeon.\n";
            Border imageBorder = new Border();
            imageBorder.BorderBrush = System.Windows.Media.Brushes.Gray;
            imageBorder.BorderThickness = new Thickness(0);
            imageBorder.HorizontalAlignment = HorizontalAlignment.Left;
            imageBorder.VerticalAlignment = VerticalAlignment.Top;
            imageBorder.Margin = new Thickness(0);
            imageBorder.Child = DrawMap(response);

            this.Background = System.Windows.Media.Brushes.White;
            this.Margin = new Thickness(0);
            this.Content = imageBorder;

            music.Open(new Uri(@"Graphics/Sounds/firstpart.wav", UriKind.Relative));
            music.Play();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            

        }

        // MAIN LOGIC

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            bool cameraManagment = false;
            currentSound = "";
            tempSound = "";

            if ( Keyboard.IsKeyDown(Key.Up))
            {
                if (cameraSize[1] > 20)
                cameraSize[1]--;
                cameraManagment = true;
            }
            if ( Keyboard.IsKeyDown(Key.Down))
            {
                if (cameraSize[1] < level.GetLength(1) - 1)
                cameraSize[1]++;
                cameraManagment = true;
            }
            if ( Keyboard.IsKeyDown(Key.Left))
            {
                if (cameraSize[0] > 21)
                cameraSize[0]--;
                cameraManagment = true;
            }
            if ( Keyboard.IsKeyDown(Key.Right))
            {
                if (cameraSize[0] < level.GetLength(0) - 1)
                cameraSize[0]++;
                cameraManagment = true;
            }

            if ( Keyboard.IsKeyDown(Key.F12))
            {
                switch (ByteLikeWindow.WindowState)
                {
                    case WindowState.Normal:
                        ByteLikeWindow.WindowState = WindowState.Maximized;
                        ByteLikeWindow.WindowStyle = WindowStyle.None;
                            break;
                    case WindowState.Maximized:
                        ByteLikeWindow.WindowState = WindowState.Normal;
                        ByteLikeWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                        break;
                }
                cameraManagment = true;
            }

            if (!cameraManagment)
            {
                bool doEnemies = false;

                if (effects.Count > 0)
                {
                    doEnemies = true;
                    List<int> deleteus = new List<int>();
                    for (int i = 0; i < effects.Count; i++)
                    {
                        if (!effects[i].Logics(ref level, ref enemies, ref effects, ref player, out response, response, out tempSound))
                            deleteus.Add(i);

                        if (tempSound != "")
                            currentSound = tempSound;
                    }
                    if (deleteus.Count > 0)
                    {
                        for (int i = deleteus.Count - 1; i >= 0; i--)
                        {
                            effects.RemoveAt(deleteus[i]);
                        }
                    }

                }
                else
                {
                    response = player.Logics(ref level, ref chests, ref effects, ref enemies, ref player, ref darkness, out tempSound);
                    currentSound = tempSound;

                    if (!player.OpenInventory && !Keyboard.IsKeyDown(Key.Q))
                    {
                        doEnemies = true;
                    }

                    if (enemies.Count < 10 + floor / 2)
                    {
                        int[] newPos = new int[] { rand.Next(level.GetLength(0)), rand.Next(level.GetLength(1)) };

                        if (level[newPos[0], newPos[1]] == 1 && (darkness[newPos[0], newPos[1]] <= 0 || darkness[newPos[0], newPos[1]] == 2) && DistanceBetween(new int[] { newPos[0], newPos[1] }, new int[] { player.position[0], player.position[1] }) > player.GetStat("Torch")+2)
                        {
                            enemies.Add(new Critter(floor + player.DangerLevel, new int[] { newPos[0], newPos[1] }));
                        }
                    }
                }

                if (effects.Count <= 0)
                {
                    if (doEnemies)
                    {
                        List<int> deletusXL = new List<int>();
                        int pos = 0;

                        foreach (Creature enemy in enemies)
                        {
                            response += enemy.Logics(ref level, ref chests, ref effects, ref enemies, ref player, ref darkness, out string tempSound);

                            if (tempSound != "" && currentSound == "")
                                tempSound = currentSound;

                            if (enemy.Stats["HP"] <= 0)
                                deletusXL.Add(pos);

                            pos++;

                        }


                        if (deletusXL.Count > 0)
                        {
                            for (int i = deletusXL.Count - 1; i >= 0; i--)
                            {
                                if (enemies[deletusXL[i]].DropEquipment)
                                {
                                    Chest temp = new Chest(new int[] { enemies[deletusXL[i]].position[0], enemies[deletusXL[i]].position[1] }, -(enemies[deletusXL[i]].Inventory.GetLength(1) + 1));
                                    for (int f = 0; f < enemies[deletusXL[i]].Inventory.GetLength(1); f++)
                                    {
                                        for (int k = 0; k < enemies[deletusXL[i]].Inventory.GetLength(0); k++)
                                        {
                                            temp.Inventory[k, f] = enemies[deletusXL[i]].Inventory[k, f];
                                        }
                                    }
                                    chests.Add(temp);
                                }

                                if (enemies[deletusXL[i]].GetType() == typeof(DoppleGanger))
                                {
                                    response += "The darkness around you glooms...\n";
                                    player.DangerLevel += 3;
                                }
                                enemies.RemoveAt(deletusXL[i]);
                            }
                        }
                    }

                    if (effects.Count > 0)
                    {
                        List<int> deleteus = new List<int>();
                        for (int i = 0; i < effects.Count; i++)
                        {
                            if (!effects[i].Logics(ref level, ref enemies, ref effects, ref player, out response, response, out tempSound))
                                deleteus.Add(i);

                            if (currentSound == "" && tempSound != "")
                                currentSound = tempSound;
                        }

                        if (deleteus.Count > 0)
                        {
                            for (int i = deleteus.Count - 1; i >= 0; i--)
                            {
                                effects.RemoveAt(deleteus[i]);
                            }
                        }
                    }
                }
            }
            else
            {
                response = $"Camera size: {cameraSize[0]} by {cameraSize[1]}\n";
            }
            // Set camera to player's position
            camera[0] = player.position[0];
            camera[1] = player.position[1];


            if (level[player.position[0], player.position[1]] == 15 && Keyboard.IsKeyDown(Key.E) && !player.OpenInventory && !Keyboard.IsKeyDown(Key.W) && !Keyboard.IsKeyDown(Key.S) && !Keyboard.IsKeyDown(Key.A) && !Keyboard.IsKeyDown(Key.D) && effects.Count <= 0)
            {
                currentSound = "Graphics/Sounds/newfloor.wav";
                NewLevel();
                response = $"{player.Name} finds their way to floor #{floor}!\n";
                if (floor <= 16)
                {
                    music.Stop();
                    music.Open(new Uri(@"Graphics/Sounds/firstpart.wav", UriKind.Relative));
                    music.Play();
                }
                else if (floor > 16 && floor < 32)
                {
                    music.Stop();
                    music.Open(new Uri(@"Graphics/Sounds/secondpart.wav", UriKind.Relative));
                    music.Play();
                }
                else if (floor >= 32)
                {
                    music.Stop();
                    music.Open(new Uri(@"Graphics/Sounds/thirdpart.wav", UriKind.Relative));
                    music.Play();
                }

                if (Directory.Exists("Memories"))
                {
                    if (File.Exists($"Memories/floor{floor}.json"))
                    {
                        using (StreamReader sr = File.OpenText($"Memories/floor{floor}.json"))
                        {
                            Player temp = new JsonSerializer().Deserialize<Player>(new JsonTextReader(sr));
                            if (temp != null)
                            {
                                int[] spawnpoint = new int[2] { 0, 0 };
                                int counter = 0;
                                while (level[spawnpoint[0], spawnpoint[1]] != 1 && level[spawnpoint[0], spawnpoint[1]] != 3 && level[spawnpoint[0], spawnpoint[1]] != 8 && counter < 50)
                                {
                                    spawnpoint[0] = rand.Next(level.GetLength(0));
                                    spawnpoint[1] = rand.Next(level.GetLength(1));

                                    counter++;
                                    if (level[spawnpoint[0], spawnpoint[1]] == 1 || level[spawnpoint[0], spawnpoint[1]] == 3 || level[spawnpoint[0], spawnpoint[1]] == 8)
                                    {
                                        enemies.Add(new DoppleGanger(temp, new int[] { spawnpoint[0], spawnpoint[1] }));
                                        counter = 50;
                                        response += "An eerie chill goes down your spine...\n";
                                        music.Stop();
                                        music.Open(new Uri(@"Graphics/Sounds/doppleganger.wav", UriKind.Relative));
                                        music.Play();
                                    }
                                }
                            }
                            sr.Close();
                        }
                        File.Delete($"Memories/floor{floor}.json");
                    }
                }
                if (cameraSize[0] >= level.GetLength(0))
                    cameraSize[0] = level.GetLength(0) - 1;
                if (cameraSize[1] >= level.GetLength(1))
                    cameraSize[1] = level.GetLength(1) - 1;
            }

            if (player.Stats["HP"] <= 0)
            {
                music.Stop();
                music.Open(new Uri(@"Graphics/Sounds/firstpart.wav", UriKind.Relative));
                music.Play();
                if (!Directory.Exists("Memories"))
                    Directory.CreateDirectory("Memories");
                if (!File.Exists($"Memories/floor{floor}.json"))
                {
                    using (StreamWriter sw = File.CreateText($"Memories/floor{floor}.json"))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(player));
                        sw.Close();
                    }
                }
                currentSound = "Graphics/Sounds/scream.wav";
                floor = 0;
                player = new Player("Player");
                NewLevel();
                response = $"{player.Name} enters the dungeon.\nScreams of an unfortunate adventurer echo somewhere in the depths...\n";

                if (cameraSize[0] >= level.GetLength(0))
                    cameraSize[0] = level.GetLength(0) - 1;
                if (cameraSize[1] >= level.GetLength(1))
                    cameraSize[1] = level.GetLength(1) - 1;
            }

            if (File.Exists(currentSound))
            {
                System.Media.SoundPlayer sound = new System.Media.SoundPlayer(currentSound);
                sound.Play();
            }

            Border imageBorder = new Border();
            imageBorder.BorderBrush = System.Windows.Media.Brushes.Gray;
            imageBorder.BorderThickness = new Thickness(0);
            imageBorder.HorizontalAlignment = HorizontalAlignment.Left;
            imageBorder.VerticalAlignment = VerticalAlignment.Top;
            imageBorder.Margin = new Thickness(0);
            imageBorder.Child = DrawMap(response);

            this.Background = System.Windows.Media.Brushes.White;
            this.Margin = new Thickness(0);
            this.Content = imageBorder;
            

        }
    }



    public class Effect
    {
        static Random rand = new Random();

        public int[] position = new int[2];
        int[] target = new int[2];
        int direction = 0;
        int counter = 0;
        public string File = "Graphics/ByteLikeGraphics/placeholder.png";
        int strength = 0;
        int element = 0;
        int spawntile = 0;
        int radius = -100;

        bool isBullet = false;
        bool isExplosion = false;
        bool isWide = false;
        bool isDamaging = true;

        string stat = "";

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

        public Effect(int[] pos, int[] targt, int power, string spell)
        {
            position[0] = pos[0];
            position[1] = pos[1];

            target[0] = targt[0];
            target[1] = targt[1];

            strength = power;

            // Directions
            direction = 0;
            // Horrizontal
            if (DistanceBetween(new int[2] { position[0], position[1] }, new int[2] { position[0] + target[0], position[1] }) >= DistanceBetween(new int[2] { position[0], position[1] }, new int[2] { position[0], position[1] + target[1] }))
            {
                if (target[0] < 0)
                {
                    direction = 180;
                }
            }
            // Vertical
            else
            {
                direction = 270;
                if (target[1] < 0)
                {
                    direction = 90;
                }
            }
            // End Direction


            // SPELL SWITCH
            switch (spell)
            {
                // Search
                case "Search":
                    spawntile = -1;
                    isDamaging = false;
                    strength = 0;
                    File = "Graphics/ByteLikeGraphics/Effects/search.png";
                    break;
                // Explosions
                case "Explosion":
                case "Fire Explosion":
                case "Poison Explosion":
                case "Ice Explosion":
                case "Lightning Explosion":
                    if (spell.Contains("Fire"))
                        element = 1;
                    else if (spell.Contains("Poison"))
                        element = 2;
                    else if (spell.Contains("Ice"))
                        element = 3;
                    else if (spell.Contains("Lightning"))
                        element = 4;
                    else
                    {
                        spawntile = 3;
                        radius = 0;
                    }

                    File = "Graphics/ByteLikeGraphics/Effects/explosion";
                    File += element.ToString();
                    File += ".png";
                    if (element > 0)
                        element += 4;
                    break;
                // Arrows
                case "Shoot Arrow":
                case "Shoot Fire Arrow":
                case "Shoot Poison Arrow":
                case "Shoot Ice Arrow":
                case "Shoot Lightning Arrow":
                    isBullet = true;
                    if (spell.Contains("Fire"))
                        element = 1;
                    else if (spell.Contains("Poison"))
                        element = 2;
                    else if (spell.Contains("Ice"))
                        element = 3;
                    else if (spell.Contains("Lightning"))
                        element = 4;

                    File = "Graphics/ByteLikeGraphics/Effects/arrow";
                    File += element;
                    File += direction;
                    File += ".png";
                    if (element > 0)
                        element += 4;
                    break;
                // Projectiles
                case "Focus":
                case "Ember":
                case "Ice Shard":
                case "Zap":
                case "Posion Sting":
                case "Fireball":
                case "Ice Storm":
                case "Electro Bolt":
                case "Sludge Bomb":
                case "Meteor":
                case "Blizzard":
                case "Thunder":
                case "Plague Bomb":
                    isBullet = true;

                    // Setting up elemental spells
                    switch (spell)
                    {
                        case "Ember":
                            element = 9;
                            break;
                        case "Ice Shard":
                            element = 11;
                            break;
                        case "Zap":
                            element = 12;
                            break;
                        case "Poison Sting":
                            element = 10;
                            break;
                        case "Fireball":
                            element = 5;
                            isExplosion = true;
                            radius = 1;
                            break;
                        case "Ice Storm":
                            element = 7;
                            isWide = true;
                            radius = 1;
                            break;
                        case "Electro Bolt":
                            element = 8;
                            isWide = true;
                            radius = 1;
                            break;
                        case "Sludge Bomb":
                            element = 6;
                            isExplosion = true;
                            radius = 1;
                            break;
                        case "Meteor":
                            element = 1;
                            isExplosion = true;
                            radius = 2;
                            break;
                        case "Blizzard":
                            element = 3;
                            isWide = true;
                            radius = 2;
                            break;
                        case "Thunder":
                            element = 4;
                            isWide = true;
                            radius = 2;
                            break;
                        case "Plague Bomb":
                            element = 2;
                            isExplosion = true;
                            isWide = true;
                            radius = 2;
                            break;
                    }
                    // end set up


                    File = "Graphics/ByteLikeGraphics/Effects/blast";
                    if (element < 5)
                        File += element;
                    else if (element < 9)
                        File += (element - 4);
                    else
                        File += (element - 8);
                    File += direction;
                    File += ".png";

                    if (spell == "Focus")
                        element = rand.Next(5)+8;
                    break;
                // Tile Spawners
                case "Liquify":
                    spawntile = 6;
                    isDamaging = false;
                    element = 3;
                    radius = 2;
                    File = "Graphics/ByteLikeGraphics/Effects/explosion3.png";
                    break;
                case "Lavafy":
                    spawntile = 7;
                    isDamaging = false;
                    element = 1;
                    radius = 2;
                    File = "Graphics/ByteLikeGraphics/Effects/explosion1.png";
                    break;
                case "Ivy Growth":
                    spawntile = 13;
                    isDamaging = false;
                    element = 2;
                    radius = 2;
                    File = "Graphics/ByteLikeGraphics/Effects/explosion2.png";
                    break;
                case "Charge":
                    spawntile = 14;
                    isDamaging = false;
                    element = 4;
                    radius = 2;
                    File = "Graphics/ByteLikeGraphics/Effects/explosion4.png";
                    break;
                case "Tsunami":
                    spawntile = 6;
                    isDamaging = false;
                    element = 3;
                    radius = 3;
                    File = "Graphics/ByteLikeGraphics/Effects/explosion3.png";
                    break;
                case "Erruption":
                    spawntile = 7;
                    isDamaging = false;
                    element = 1;
                    radius = 3;
                    File = "Graphics/ByteLikeGraphics/Effects/explosion1.png";
                    break;
                case "Forest Growth":
                    spawntile = 13;
                    isDamaging = false;
                    element = 2;
                    radius = 3;
                    File = "Graphics/ByteLikeGraphics/Effects/explosion2.png";
                    break;
                case "Electrify":
                    spawntile = 14;
                    isDamaging = false;
                    element = 4;
                    radius = 3;
                    File = "Graphics/ByteLikeGraphics/Effects/explosion4.png";
                    break;
                // End tile spawners

                // Statuses
                case "Recover":
                    stat = "HP";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 10;
                    break;
                case "Ironize":
                    stat = "Defense";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 5;
                    strength *= 5;
                    break;
                case "Enchant":
                    stat = "MagicDefense";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 5;
                    strength *= 5;
                    break;
                case "Corrode armor":
                    stat = "MagicDefense|Defense";
                    File = "Graphics/ByteLikeGraphics/Effects/debuff.png";
                    isDamaging = false;
                    element = -3;
                    strength *= 5;
                    break;
                case "Sharpen":
                    stat = "Strength";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 3;
                    strength *= 5;
                    break;
                case "Enlighten":
                    stat = "Agility";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 3;
                    strength *= 5;
                    break;
                case "Prepare":
                    stat = "Magic";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 3;
                    strength *= 5;
                    break;
                case "Energy Drain":
                    stat = "Strength";
                    File = "Graphics/ByteLikeGraphics/Effects/debuff.png";
                    isDamaging = false;
                    element = -2;
                    strength *= 5;
                    break;
                case "Confuse":
                    stat = "Agility";
                    File = "Graphics/ByteLikeGraphics/Effects/debuff.png";
                    isDamaging = false;
                    element = -2;
                    strength *= 5;
                    break;
                case "Scare":
                    stat = "Magic";
                    File = "Graphics/ByteLikeGraphics/Effects/debuff.png";
                    isDamaging = false;
                    element = -2;
                    strength *= 5;
                    break;
                case "Heal Wounds":
                    stat = "HP";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 20;
                    break;
                case "Protective Field":
                    stat = "Defense|MagicDefense";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 7;
                    strength *= 5;
                    break;
                case "Regenerate":
                    stat = "HPRegen|ManaRegen";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = -1;
                    strength *= 5;
                    break;
                case "Melt Armor":
                    stat = "Defense|MagicDefense";
                    File = "Graphics/ByteLikeGraphics/Effects/debuff.png";
                    isDamaging = false;
                    element = -6;
                    strength *= 5;
                    break;
                case "Enchanse Vission":
                    stat = "Torch";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 2;
                    strength *= 5;
                    break;
                case "Rage":
                    stat = "Strength";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 7;
                    strength *= 5;
                    break;
                case "Speed Up":
                    stat = "Agility";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 7;
                    strength *= 5;
                    break;
                case "Concentrate":
                    stat = "Magic";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 7;
                    strength *= 5;
                    break;
                case "Weaken":
                    stat = "Strength";
                    File = "Graphics/ByteLikeGraphics/Effects/debuff.png";
                    isDamaging = false;
                    element = -5;
                    strength *= 5;
                    break;
                case "Slow Down":
                    stat = "Agility";
                    File = "Graphics/ByteLikeGraphics/Effects/debuff.png";
                    isDamaging = false;
                    element = -5;
                    strength *= 5;
                    break;
                case "Terrify":
                    stat = "Magic";
                    File = "Graphics/ByteLikeGraphics/Effects/debuff.png";
                    isDamaging = false;
                    element = -5;
                    strength *= 5;
                    break;
                case "Restore":
                    stat = "HP";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 35;
                    break;
                case "Full Protection":
                    stat = "Defense|MagicDefense";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 10;
                    strength *= 5;
                    break;
                case "Manafy":
                    stat = "Mana";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 30;
                    break;
                case "Destroy Armor":
                    stat = "Defense|MagicDefense";
                    File = "Graphics/ByteLikeGraphics/Effects/debuff.png";
                    isDamaging = false;
                    element = -10;
                    strength *= 5;
                    break;
                case "Demonify":
                    stat = "Strength";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 10;
                    strength *= 5;
                    break;
                case "Featherify":
                    stat = "Agility";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 10;
                    strength *= 5;
                    break;
                case "Transcend":
                    stat = "Magic";
                    File = "Graphics/ByteLikeGraphics/Effects/buff.png";
                    isDamaging = false;
                    element = 10;
                    strength *= 5;
                    break;
                case "Wither":
                    stat = "Strength";
                    File = "Graphics/ByteLikeGraphics/Effects/debuff.png";
                    isDamaging = false;
                    element = -8;
                    strength *= 5;
                    break;
                case "Chain Up":
                    stat = "Agility";
                    File = "Graphics/ByteLikeGraphics/Effects/debuff.png";
                    isDamaging = false;
                    element = -8;
                    strength *= 5;
                    break;
                case "Hypnotize":
                    stat = "Torch";
                    File = "Graphics/ByteLikeGraphics/Effects/debuff.png";
                    isDamaging = false;
                    element = -8;
                    strength *= 5;
                    break;
                // End statuses
            }
            // end Spell switch


            // Moving towards right position
            if (!isBullet)
            {
                position[0] += target[0];
                position[1] += target[1];
            }
            else { Move(); }
        }

        public bool Logics(ref int[,] level, ref List<Creature> enemies, ref List<Effect> effects, ref Player player, out string response, string currentresponse, out string currentSound)
        {
            bool result = true;

            response = currentresponse;
            string sound = "";

            if (strength == 0)
            {
                sound = "Graphics/Sounds/attack.wav";
                result = false;
            }

            int xp = 0;
            bool check = false;

            // Damage/buff
            if (strength != 0)
            {
                foreach (Creature enemy in enemies)
                {
                    if (position[0] == enemy.position[0] && position[1] == enemy.position[1])
                    {
                        sound = "Graphics/Sounds/attack.wav";
                        if (isDamaging)
                            xp += enemy.TakeDamage(strength, element);
                        else if (stat != "Mana" && stat != "HP" && stat != "")
                        {
                            string trueStat = stat;
                            if (stat.Contains("|"))
                            {
                                trueStat = stat.Split("|")[0];
                                if (enemy.Buffs[trueStat] <= 0)
                                {
                                    enemy.Buffs[trueStat] = strength;
                                    enemy.BuffLevels[trueStat] = element;
                                }
                                else
                                {
                                    enemy.Buffs[trueStat] = strength;
                                    enemy.BuffLevels[trueStat] = (int)((enemy.BuffLevels[trueStat] + element) / 1.5);
                                }
                                trueStat = stat.Split("|")[1];
                            }

                            if (enemy.Buffs[trueStat] <= 0)
                            {
                                enemy.Buffs[trueStat] = strength;
                                enemy.BuffLevels[trueStat] = element;
                            }
                            else
                            {
                                enemy.Buffs[trueStat] = strength;
                                enemy.BuffLevels[trueStat] = (int)((enemy.BuffLevels[trueStat] + element) / 1.5);
                            }
                        }
                        // Manafy
                        else if (stat == "Mana")
                        {
                            enemy.Stats["Mana"] += 30;
                            enemy.Stats["HP"] -= 10;
                            if (enemy.Stats["Mana"] > enemy.GetStat("MaxMana"))
                                enemy.Stats["Mana"] = enemy.GetStat("MaxMana");
                        }
                        // Healing
                        else if (stat != "")
                        {
                            enemy.Stats["HP"] += element;
                            if (enemy.Stats["HP"] > enemy.GetStat("MaxHP"))
                                enemy.Stats["HP"] = enemy.GetStat("MaxHP");
                        }
                        check = true;
                    }
                }

                if (position[0] == player.position[0] && position[1] == player.position[1])
                {
                    if (isDamaging)
                        player.TakeDamage(strength, element);
                    else if (stat != "Mana" && stat != "HP" && stat != "")
                    {
                        string trueStat = stat;
                        if (stat.Contains("|"))
                        {
                            trueStat = stat.Split("|")[0];
                            if (player.Buffs[trueStat] <= 0)
                            {
                                player.Buffs[trueStat] = strength;
                                player.BuffLevels[trueStat] = element;
                            }
                            else
                            {
                                player.Buffs[trueStat] = strength;
                                player.BuffLevels[trueStat] = (int)((player.BuffLevels[trueStat] + element) / 1.5);
                            }
                            trueStat = stat.Split("|")[1];
                        }

                        if (player.Buffs[trueStat] <= 0)
                        {
                            player.Buffs[trueStat] = strength;
                            player.BuffLevels[trueStat] = element;
                        }
                        else
                        {
                            player.Buffs[trueStat] = strength;
                            player.BuffLevels[trueStat] = (int)((player.BuffLevels[trueStat] + element) / 1.5);
                        }
                    }
                    // Manafy
                    else if (stat == "Mana")
                    {
                        player.Stats["Mana"] += 30;
                        player.Stats["HP"] -= 10;
                        if (player.Stats["Mana"] > player.GetStat("MaxMana"))
                            player.Stats["Mana"] = player.GetStat("MaxMana");
                    }
                    else if (stat != "")
                    {
                        player.Stats["HP"] += element;
                        if (player.Stats["HP"] > player.GetStat("MaxHP"))
                            player.Stats["HP"] = player.GetStat("MaxHP");
                    }
                    check = true;
                }

                if (check || !isBullet)
                {
                    // Spawn explosions for wide spells
                    if (isWide)
                    {
                        string spell = "";
                        switch (element)
                        {
                            case 1:
                            case 5:
                            case 9:
                                spell = "Fire ";
                                break;
                            case 2:
                            case 6:
                            case 10:
                                spell = "Poison ";
                                break;
                            case 3:
                            case 7:
                            case 11:
                                spell = "Ice ";
                                break;
                            case 4:
                            case 8:
                            case 12:
                                spell = "Lightning ";
                                break;
                        }
                        spell += "Explosion";
                        switch (direction)
                        {
                            case 0:
                            case 180:
                                for (int i = -radius; i <= radius; i++)
                                {
                                    effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { 0, i }, strength, spell));
                                }
                                break;
                            case 270:
                            case 90:
                                for (int i = -radius; i <= radius; i++)
                                {
                                    effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { i, 0 }, strength, spell));
                                }
                                break;
                        }
                    }

                    // Explosions
                    if (isExplosion)
                    {
                        for (int i = -radius - 1; i <= radius + 1; i++)
                        {
                            for (int j = -radius - 1; j <= radius + 1; j++)
                            {
                                if (DistanceBetween(new int[] { position[0], position[1] }, new int[] { position[0] + j, position[1] + i }) <= radius)
                                {
                                    string spell = "";
                                    switch (element)
                                    {
                                        case 1:
                                        case 5:
                                        case 9:
                                            spell = "Fire ";
                                            break;
                                        case 2:
                                        case 6:
                                        case 10:
                                            spell = "Poison ";
                                            break;
                                        case 3:
                                        case 7:
                                        case 11:
                                            spell = "Ice ";
                                            break;
                                        case 4:
                                        case 8:
                                        case 12:
                                            spell = "Lightning ";
                                            break;
                                    }
                                    spell += "Explosion";

                                    effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { j, i }, strength, spell));
                                }
                            }
                        }
                    }
                    // end Explosions
                    strength = 0;
                }


            }
            // end Damage/buff

            // XP yield
            if (xp > 0)
            {
                int extraxp = 0;
                for (int f = 0; f < 9; f++)
                {
                    if (player.Inventory[f, 0] != null)
                    {
                        if (player.Inventory[f, 0].Name.Contains("XP"))
                            extraxp += (int)(xp * 0.25);
                    }
                }
                xp += extraxp;
                response += $"{player.Name} gained {xp} XP!\n";
                player.Stats["XP"] += xp;
            }
            //


            // Movement for bullets
            if (isBullet)
            {
                // Spawn explosions for wide spells
                if (isWide)
                {
                    string spell = "";
                    switch (element)
                    {
                        case 1:
                            spell = "Fire ";
                            break;
                        case 2:
                            spell = "Poison ";
                            break;
                        case 3:
                            spell = "Ice ";
                            break;
                        case 4:
                            spell = "Lightning ";
                            break;
                    }
                    spell += "Explosion";
                    switch (direction)
                    {
                        case 0:
                        case 180:
                            for (int i = -radius; i <= radius; i++)
                            {
                                effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { 0, i }, strength, spell));
                            }
                            break;
                        case 270:
                        case 90:
                            for (int i = -radius; i <= radius; i++)
                            {
                                effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { i, 0 }, strength, spell));
                            }
                            break;
                    }
                }

                // Move, check for collison
                Move();
                sound = "Graphics/Sounds/projectilesound.wav";
                if (level[position[0], position[1]] == 0 || level[position[0], position[1]] == 5 || level[position[0], position[1]] == 4 || level[position[0], position[1]] == 2)
                {
                    result = false;
                }


            }

            // On collison (for tile spawning, explosions, etc)
            if (!result)
            {
                // Tile spawning
                if (spawntile > 0)
                {
                    for (int i = -radius - 5; i <= radius + 5; i++)
                    {
                        for (int j = -radius - 5; j <= radius + 5; j++)
                        {
                            if (position[0] + j > 0 && position[0] + j < level.GetLength(0) && position[1] + i > 0 && position[1] + i < level.GetLength(1) && DistanceBetween(new int[] { position[0], position[1] }, new int[] { position[0] + j, position[1] + i }) <= radius)
                            {
                                switch (level[position[0] + j, position[1] + i])
                                {
                                    // Void wall
                                    case 0:
                                        level[position[0] + j, position[1] + i] = 2;
                                        break;
                                    // Floor
                                    case 1:
                                    // Wall
                                    case 2:
                                    // Center of the room, really only needed for debug
                                    case 3:
                                    // Door
                                    case 4:
                                    // Water
                                    case 6:
                                    // Lava
                                    case 7:
                                    // Grass
                                    case 8:
                                    case 9:
                                    case 10:
                                    case 16:
                                    // Trap
                                    case 11:
                                    // Posion Trap
                                    case 12:
                                    // Poisonous vines
                                    case 13:
                                    // Electro terrain
                                    case 14:
                                        level[position[0] + j, position[1] + i] = spawntile;
                                        break;

                                    // Outter wall
                                    case 5:
                                    // Exit
                                    case 15:
                                        break;
                                } // end switch
                            }
                        } // end j
                    } // end i
                }
                // end tile spawning

                // Search
                else if (spawntile < 0)
                {
                    if (position[0] >= 0 && position[0] < level.GetLength(0) && position[1] >= 0 && position[1] < level.GetLength(1))
                    {
                        if (level[position[0], position[1]] == 8) { level[position[0], position[1]] = 1; }
                        else if (level[position[0], position[1]] == 9) { level[position[0], position[1]] = 11; }
                        else if (level[position[0], position[1]] == 10) { level[position[0], position[1]] = 12; }

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
                }
                // end Search

                if (strength != 0)
                {
                    // Spawn explosions for wide spells
                    if (isWide)
                    {
                        string spell = "";
                        switch (element)
                        {
                            case 1:
                                spell = "Fire ";
                                break;
                            case 2:
                                spell = "Poison ";
                                break;
                            case 3:
                                spell = "Ice ";
                                break;
                            case 4:
                                spell = "Lightning ";
                                break;
                        }
                        spell += "Explosion";
                        switch (direction)
                        {
                            case 0:
                            case 180:
                                for (int i = -radius; i <= radius; i++)
                                {
                                    effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { 0, i }, strength, spell));
                                }
                                break;
                            case 270:
                            case 90:
                                for (int i = -radius; i <= radius; i++)
                                {
                                    effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { i, 0 }, strength, spell));
                                }
                                break;
                        }
                    }


                    // Explosions
                    if (isExplosion)
                    {
                        sound = "Graphics/Sounds/explosion.wav";
                        for (int i = -radius - 1; i <= radius + 1; i++)
                        {
                            for (int j = -radius - 1; j <= radius + 1; j++)
                            {
                                if (DistanceBetween(new int[] { position[0], position[1] }, new int[] { position[0] + j, position[1] + i }) <= radius)
                                {
                                    string spell = "";
                                    switch (element)
                                    {
                                        case 1:
                                            spell = "Fire ";
                                            break;
                                        case 2:
                                            spell = "Poison ";
                                            break;
                                        case 3:
                                            spell = "Ice ";
                                            break;
                                        case 4:
                                            spell = "Lightning ";
                                            break;
                                    }
                                    spell += "Explosion";

                                    effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { j, i }, strength, spell));
                                }
                            }
                        }
                    }
                    // end Explosions
                    strength = 0;
                }


            }
            // end On Collison

            currentSound = sound;

            return result;
        }


        // Self explanatory
        void Move()
        {
            switch (direction)
            {
                case 0:
                    position[0]++;
                    if (target[1] != 0)
                    {
                        if (target[1] < 0)
                        {
                            counter++;
                            if (counter >= target[0] / (-target[1]))
                            {
                                position[1]--;
                                counter = 0;
                            }
                        }
                        else
                        {
                            counter++;
                            if (counter >= target[0] / target[1])
                            {
                                position[1]++;
                                counter = 0;
                            }
                        }
                    }
                    break;
                case 180:
                    position[0]--;
                    if (target[1] != 0)
                    {
                        if (target[1] < 0)
                        {
                            counter--;
                            if (counter <= target[0] / (-target[1]))
                            {
                                position[1]--;
                                counter = 0;
                            }
                        }
                        else
                        {
                            counter--;
                            if (counter <= target[0] / target[1])
                            {
                                position[1]++;
                                counter = 0;
                            }
                        }
                    }
                    break;
                case 90:
                    position[1]--;
                    if (target[0] != 0)
                    {
                        if (target[0] < 0)
                        {
                            counter--;
                            if (counter <= target[1] / (-target[0]))
                            {
                                position[0]--;
                                counter = 0;
                            }
                        }
                        else
                        {
                            counter--;
                            if (counter <= target[1] / target[0])
                            {
                                position[0]++;
                                counter = 0;
                            }
                        }
                    }
                    break;
                case 270:
                    position[1]++;
                    if (target[0] != 0)
                    {
                        if (target[0] < 0)
                        {
                            counter++;
                            if (counter >= target[1] / (-target[0]))
                            {
                                position[0]--;
                                counter = 0;
                            }
                        }
                        else
                        {
                            counter++;
                            if (counter >= target[1] / target[0])
                            {
                                position[0]++;
                                counter = 0;
                            }
                        }
                    }
                    break;
            }
        }
    }

   
}
