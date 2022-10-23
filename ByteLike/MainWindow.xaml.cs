using System;
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
        static int floor = 0;
        static List<Chest> chests = new List<Chest>();
        static List<Creature> enemies = new List<Creature>();

        static int[] sizes = new int[2];
        static string response = "";

        // Torch and darkness
        static void ClearLight()
        {
            int[] position = new int[2];
            bool doNext = true;


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
                        else
                        {
                            darkness[player.position[0] + j, player.position[1] + i] = 0;
                        }
                    }
                }
            }
            darkness[player.position[0], player.position[1]] = 3;

            while (doNext)
            {
                doNext = false;
                for (int i = -player.GetStat("Torch") - 5; i <= player.GetStat("Torch") + 5; i++)
                {
                    for (int j = -player.GetStat("Torch") - 5; j <= player.GetStat("Torch") + 5; j++)
                    {
                        if (player.position[0] + j > 0 && player.position[0] + j < level.GetLength(0) && player.position[1] + i > 0 && player.position[1] + i < level.GetLength(1))
                        {
                            if (darkness[player.position[0] + j, player.position[1] + i] >= 3)
                            {
                                position[0] = player.position[0];
                                position[1] = player.position[1];
                                position[0] += j;
                                position[1] += i;
                                if (level[position[0], position[1]] != 2 && level[position[0], position[1]] != 0 && level[position[0], position[1]] != 5 && level[position[0], position[1]] != 4)
                                {
                                    if (position[0] + 1 > 0 && position[0] + 1 < level.GetLength(0) && position[1] > 0 && position[1] < level.GetLength(1) && darkness[position[0],position[1]]-3 < player.GetStat("Torch"))
                                    {
                                        if (darkness[position[0] + 1, position[1]] != 1)
                                            darkness[position[0] + 1, position[1]] = darkness[position[0],position[1]]+1;
                                        doNext = true;
                                    }

                                    if (position[0] - 1 > 0 && position[0] - 1 < level.GetLength(0) && position[1] > 0 && position[1] < level.GetLength(1) && darkness[position[0], position[1]] - 3 < player.GetStat("Torch"))
                                    {
                                        if (darkness[position[0] - 1, position[1]] != 1)
                                            darkness[position[0] - 1, position[1]] = darkness[position[0], position[1]] + 1;
                                        doNext = true;
                                    }

                                    if (position[0] > 0 && position[0] < level.GetLength(0) && position[1] + 1 > 0 && position[1] + 1 < level.GetLength(1) && darkness[position[0], position[1]] - 3 < player.GetStat("Torch"))
                                    {
                                        if (darkness[position[0], position[1] + 1] != 1)
                                            darkness[position[0], position[1] + 1] = darkness[position[0], position[1]] + 1;
                                        doNext = true;
                                    }

                                    if (position[0] > 0 && position[0] < level.GetLength(0) && position[1] - 1 > 0 && position[1] - 1 < level.GetLength(1) && darkness[position[0], position[1]] - 3 < player.GetStat("Torch"))
                                    {
                                        if (darkness[position[0], position[1] - 1] != 1)
                                            darkness[position[0], position[1] - 1] = darkness[position[0], position[1]] + 1;
                                        doNext = true;
                                    }
                                }

                                
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



        static System.Windows.Controls.Image DrawMap(string response)
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
                FormattedText dialogue = new FormattedText(response, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 16, System.Windows.Media.Brushes.White);
                dialogue.MaxTextWidth = (cameraSize[0] * 16) - 96;

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
                                // Center of the room, really only needed for debug
                                case 3:
                                    floorImage = "Graphics/ByteLikeGraphics/Tiles/floor.png";
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
                            dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Tiles/partialdarkness.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                        }
                        // if lit (again) draw chest sprites
                        else if (darkness[camera[0] + j, camera[1] + i] > 0)
                        {
                            foreach (Chest item in chests)
                            {
                                if (item.position[0] == camera[0] + j && item.position[1] == camera[1] + i)
                                {
                                    dc.DrawImage(new BitmapImage(new Uri(item.File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }
                            }
                        }

                        // Player draw code
                        if (player.position[0] == camera[0] + j && player.position[1] == camera[1] + i)
                        {
                            // Draw quiver if slot is full and is a quiver
                            if (player.Inventory[4, 0] != null)
                            {
                                if (player.Inventory[4, 0].Name.Contains("Quiver"))
                                {
                                    dc.DrawImage(new BitmapImage(new Uri(player.Inventory[4,0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
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

                        

                        // Inventory draw code, still in j-i switch
                        if (player.OpenInventory)
                        {
                            // Don't draw inventory if casting
                            if (!player.OpenSpell)
                            {
                                // If j-i is in the regular inventory. Mind we have an offset here
                                if (j >= 5 && j - 5 < player.Inventory.GetLength(0) && i - 1 < player.Inventory.GetLength(1) && i > 0)
                                {
                                    // Draw the slot
                                    floorImage = "Graphics/ByteLikeGraphics/Hud/invslot";
                                    if (i == 1)
                                    {
                                        floorImage += (j - 4);
                                    }
                                    else { floorImage += 0; }
                                    floorImage += ".png";
                                    dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));

                                    // Draw an item
                                    if (player.Inventory[j - 5, i - 1] != null)
                                    {
                                        dc.DrawImage(new BitmapImage(new Uri(player.Inventory[j - 5, i - 1].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                        // Draw quantity
                                        if (player.Inventory[j - 5, i - 1].Quantity > 1)
                                        {
                                            FormattedText dialogue3 = new FormattedText(player.Inventory[j - 5, i - 1].Quantity.ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 8, System.Windows.Media.Brushes.White);
                                            dialogue3.MaxTextWidth = 56;
                                            dc.DrawText(dialogue3, new System.Windows.Point(j * 16, 8 + i * 16));
                                        }
                                    }
                                }
                                // If j-i is outside of the inventory but we have a chest
                                else if (currentChest != -1)
                                {
                                    if (j >= 5 && j - 5 < player.Inventory.GetLength(0) && i - 1 - player.Inventory.GetLength(1) - 1 < chests[currentChest].Inventory.GetLength(1) && i - 1 > player.Inventory.GetLength(1))
                                    {
                                        // Draw slot
                                        floorImage = "Graphics/ByteLikeGraphics/Hud/invslot0.png";
                                        dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));

                                        // Draw item
                                        if (chests[currentChest].Inventory[j - 5, i - 2 - player.Inventory.GetLength(1)] != null)
                                        {
                                            dc.DrawImage(new BitmapImage(new Uri(chests[currentChest].Inventory[j - 5, i - 2 - player.Inventory.GetLength(1)].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                            
                                            // Draw quantity
                                            if (chests[currentChest].Inventory[j - 5, i - 2 - player.Inventory.GetLength(1)].Quantity > 1)
                                            {
                                                FormattedText dialogue3 = new FormattedText(chests[currentChest].Inventory[j - 5, i - 2 - player.Inventory.GetLength(1)].Quantity.ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 8, System.Windows.Media.Brushes.White);
                                                dialogue3.MaxTextWidth = 56;
                                                dc.DrawText(dialogue3, new System.Windows.Point(j * 16, 8 + i * 16));
                                            }
                                        }
                                    }
                                }


                                // Selected slot is in inventory
                                if (j - 5 == player.SelectedSlot[0] && i - 1 == player.SelectedSlot[1] && player.SelectedSlot[1] < player.Inventory.GetLength(1))
                                {
                                    dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/invslot13.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }
                                // Selected slot is in chest
                                else if (j - 5 == player.SelectedSlot[0] && i - 1 == player.SelectedSlot[1] + 1 && player.SelectedSlot[1] >= player.Inventory.GetLength(1))
                                {
                                    dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/invslot13.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }


                                // Current slot is in inventory
                                if (j - 5 == player.CurrentSlot[0] && i - 1 == player.CurrentSlot[1] && player.CurrentSlot[1] < player.Inventory.GetLength(1))
                                {
                                    dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/invslot12.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }
                                // Current slot is in chest
                                else if (j - 5 == player.CurrentSlot[0] && i - 1 == player.CurrentSlot[1] + 1 && player.CurrentSlot[1] >= player.Inventory.GetLength(1))
                                {
                                    dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/invslot12.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }



                            }
                            // Spell casting code
                            else
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


                                while (spellpos[0] > camera[0] && spellpos[0] + 1 < camera[0] + cameraSize[0] && spellpos[1] > camera[1] && spellpos[1] + 1 < camera[1] + cameraSize[1] && (player.CurrentSlot[0] != 0 || player.CurrentSlot[1] != 0) && (darkness[spellpos[0], spellpos[1]] == 1 || darkness[spellpos[0], spellpos[1]] == 2) && level[spellpos[0], spellpos[1]] != 0 && level[spellpos[0], spellpos[1]] != 2 && level[spellpos[0], spellpos[1]] != 5 && level[spellpos[0], spellpos[1]] != 4)
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
                                    dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/invslot13.png", UriKind.Relative)), new Rect((spellpos[0] - camera[0]) * 16, (spellpos[1] - camera[1]) * 16, 17, 17));
                                }

                            }
                            // End spell casting

                        }
                        // End inventory draw code


                        // Draw the response dialogue box
                        if (response != "" && j>4)
                        {
                            if (i * 16 >= (cameraSize[1] * 16) - dialogue.Height - 16)
                            {
                                if ((i - 1) * 16 < (cameraSize[1] * 16) - dialogue.Height - 16)
                                {
                                    if (j == 5) { dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/dialoguebox1.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                    else if (j + 1 == cameraSize[0]) { dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/dialoguebox4.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                    else { dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/dialoguebox2.png", UriKind.Relative)), new Rect(j * 16, i * 16, 16, 16)); }
                                }
                                else
                                {
                                    if (j == 5) { dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/dialoguebox3.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                    else if (j + 1 == cameraSize[0]) { dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/dialoguebox5.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                    else { dc.DrawImage(new BitmapImage(new Uri("Graphics/ByteLikeGraphics/Hud/dialoguebox6.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                }
                            }
                        }
                        // end response dialogue box


                    } // end j
                } // end i
                //



                // Draw stats hud
                for (int i = 0; i < 9; i++)
                {
                    string floorImage = "Graphics/ByteLikeGraphics/Hud/hud";
                    floorImage += i;
                    floorImage += ".png";
                    if (i == 0)
                        dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(0, i * 32, 17, 17));
                    else
                        dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(0, i * 32 - 16, 17, 17));

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
                            break;
                        case 3:
                            floorImage = String.Format("{0}/{1}", player.Stats["Mana"].ToString(), player.GetStat("MaxMana"));
                            break;
                        case 4:
                            floorImage = player.GetStat("Defense").ToString();
                            break;
                        case 5:
                            floorImage = player.GetStat("MagicDefense").ToString();
                            break;
                        case 6:
                            floorImage = player.GetStat("Strength").ToString();
                            break;
                        case 7:
                            floorImage = player.GetStat("Magic").ToString();
                            break;
                        case 8:
                            floorImage = player.GetStat("Agility").ToString();
                            break;
                    }
                    FormattedText dialogue2 = new FormattedText(floorImage, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 16, System.Windows.Media.Brushes.White);
                    dialogue2.MaxTextWidth = 320;
                    if (i != 0)
                        dc.DrawText(dialogue2, new System.Windows.Point(4, 14 + i * 32 - 16));
                    else
                        dc.DrawText(dialogue2, new System.Windows.Point(20, 0));
                }
                // End stats hud


                // Response text
                if (response != "")
                {
                    dc.DrawText(dialogue, new System.Windows.Point(88, (cameraSize[1]*16)-dialogue.Height-8));
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

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyToggled(Key.Up) && Keyboard.IsKeyDown(Key.Up) && cameraSize[1] > 20)
            {
                cameraSize[1]--;
            }
            if (Keyboard.IsKeyToggled(Key.Down) && Keyboard.IsKeyDown(Key.Down) && cameraSize[1] < 100)
            {
                cameraSize[1]++;
            }
            if (Keyboard.IsKeyToggled(Key.Left) && Keyboard.IsKeyDown(Key.Left) && cameraSize[0] > 20)
            {
                cameraSize[0]--;
            }
            if (Keyboard.IsKeyToggled(Key.Right) && Keyboard.IsKeyDown(Key.Right) && cameraSize[0] < 100)
            {
                cameraSize[0]++;
            }

            response = player.Logics(ref level, ref chests);

            // Set camera to player's position
            camera[0] = player.position[0];
            camera[1] = player.position[1];





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

            if (level[player.position[0], player.position[1]] == 15 && Keyboard.IsKeyDown(Key.E) && !player.OpenInventory && !Keyboard.IsKeyDown(Key.W) && !Keyboard.IsKeyDown(Key.S) && !Keyboard.IsKeyDown(Key.A) && !Keyboard.IsKeyDown(Key.D))
            {
                NewLevel();
            }

            if (player.Stats["HP"] <= 0)
            {
                floor = 0;
                player = new Player("Player");
                NewLevel();
            }

        }
    }


   
}
