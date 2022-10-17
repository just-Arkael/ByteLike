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
            int[] position = new int[2];
            bool doNext = true;


            for (int i = -player.Torch() - 5; i <= player.Torch() + 5; i++)
            {
                for (int j = -player.Torch() - 5; j <= player.Torch() + 5; j++)
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
                for (int i = -player.Torch() - 5; i <= player.Torch() + 5; i++)
                {
                    for (int j = -player.Torch() - 5; j <= player.Torch() + 5; j++)
                    {
                        if (player.position[0] + j > 0 && player.position[0] + j < level.GetLength(0) && player.position[1] + i > 0 && player.position[1] + i < level.GetLength(1))
                        {
                            if (darkness[player.position[0] + j, player.position[1] + i] == 3)
                            {
                                position[0] = player.position[0];
                                position[1] = player.position[1];
                                position[0] += j;
                                position[1] += i;
                                if (level[position[0], position[1]] != 2 && level[position[0], position[1]] != 0 && level[position[0], position[1]] != 5 && level[position[0], position[1]] != 4)
                                {
                                    if (position[0] + 1 > 0 && position[0] + 1 < level.GetLength(0) && position[1] > 0 && position[1] < level.GetLength(1) && DistanceBetween(player.position, new int[] { position[0] + 1, position[1] }) < (player.Torch() + 0.5) * 1.5)
                                    {
                                        if (darkness[position[0] + 1, position[1]] != 1)
                                            darkness[position[0] + 1, position[1]] = 3;
                                        doNext = true;
                                    }

                                    if (position[0] - 1 > 0 && position[0] - 1 < level.GetLength(0) && position[1] > 0 && position[1] < level.GetLength(1) && DistanceBetween(player.position, new int[] { position[0] - 1, position[1] }) < (player.Torch() + 0.5) * 1.5)
                                    {
                                        if (darkness[position[0] - 1, position[1]] != 1)
                                            darkness[position[0] - 1, position[1]] = 3;
                                        doNext = true;
                                    }

                                    if (position[0] > 0 && position[0] < level.GetLength(0) && position[1] + 1 > 0 && position[1] + 1 < level.GetLength(1) && DistanceBetween(player.position, new int[] { position[0], position[1] + 1 }) < (player.Torch() + 0.5) * 1.5)
                                    {
                                        if (darkness[position[0], position[1] + 1] != 1)
                                            darkness[position[0], position[1] + 1] = 3;
                                        doNext = true;
                                    }

                                    if (position[0] > 0 && position[0] < level.GetLength(0) && position[1] - 1 > 0 && position[1] - 1 < level.GetLength(1) && DistanceBetween(player.position, new int[] { position[0], position[1] - 1 }) < (player.Torch() + 0.5) * 1.5)
                                    {
                                        if (darkness[position[0], position[1] - 1] != 1)
                                            darkness[position[0], position[1] - 1] = 3;
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

            using (DrawingContext dc = drawingGroup.Open())
            {
                FormattedText dialogue = new FormattedText(response, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 16, System.Windows.Media.Brushes.White);
                dialogue.MaxTextWidth = (cameraSize[0] * 16) - 96;

                // Draw the tiles
                for (int i = 0; i < level.GetLength(1) && i < cameraSize[1]; i++)
                {
                    for (int j = 0; j < level.GetLength(0) && j < cameraSize[0]; j++)
                    {
                        string floorImage = "";
                        if (darkness[camera[0] + j, camera[1] + i] > 0)
                        {
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

                        }
                        else
                        {
                            floorImage = "Graphycs/ByteLikeGraphycs/darkness.png";
                        }

                        dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));

                        if (darkness[camera[0] + j, camera[1] + i] == 2)
                        {
                            dc.DrawImage(new BitmapImage(new Uri("Graphycs/ByteLikeGraphycs/partialdarkness.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                        }


                        if (player.position[0] == camera[0] + j && player.position[1] == camera[1] + i)
                        {
                            if (player.Inventory[4, 0] != null)
                            {
                                if (player.Inventory[4, 0].Name.Contains("Quiver"))
                                {
                                    dc.DrawImage(new BitmapImage(new Uri(player.Inventory[4,0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }
                            }
                            if (!player.IsGhost)
                            {
                                dc.DrawImage(new BitmapImage(new Uri(player.File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                            }
                            else
                            {
                                dc.DrawImage(new BitmapImage(new Uri("Graphycs/ByteLikeGraphycs/player4.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                            }
                            if (player.Inventory[2, 0] != null)
                                dc.DrawImage(new BitmapImage(new Uri(player.Inventory[2, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                            if (player.Inventory[1, 0] != null)
                                dc.DrawImage(new BitmapImage(new Uri(player.Inventory[1, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                            if (player.Inventory[0, 0] != null)
                                dc.DrawImage(new BitmapImage(new Uri(player.Inventory[0, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                            if (player.Inventory[3, 0] != null)
                                dc.DrawImage(new BitmapImage(new Uri(player.Inventory[3, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));

                            if (player.Inventory[4, 0] != null)
                            {
                                
                                if (!player.Inventory[4, 0].Name.Contains("Quiver"))
                                {
                                    dc.DrawImage(new BitmapImage(new Uri(player.Inventory[4, 0].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                }
                            }

                        }

                        

                        if (player.OpenInventory)
                        {
                            if (j >= 5 && j-5 < player.Inventory.GetLength(0) && i < player.Inventory.GetLength(1))
                            {
                                floorImage = "Graphycs/ByteLikeGraphycs/invslot";
                                if (i == 0)
                                {
                                    floorImage += (j - 4);
                                }
                                else { floorImage += 0; }
                                floorImage += ".png";
                                dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                                if (player.Inventory[j-5,i] != null)
                                    dc.DrawImage(new BitmapImage(new Uri(player.Inventory[j-5,i].File, UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                            }

                            if (j-5 == player.SelectedSlot[0] && i == player.SelectedSlot[1])
                            {
                                dc.DrawImage(new BitmapImage(new Uri("Graphycs/ByteLikeGraphycs/invslot13.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                            }

                            if (j-5 == player.CurrentSlot[0] && i == player.CurrentSlot[1])
                            {
                                dc.DrawImage(new BitmapImage(new Uri("Graphycs/ByteLikeGraphycs/invslot12.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17));
                            }

                        }



                        if (response != "" && j>4)
                        {
                            if (i * 16 >= (cameraSize[1] * 16) - dialogue.Height - 16)
                            {
                                if ((i - 1) * 16 < (cameraSize[1] * 16) - dialogue.Height - 16)
                                {
                                    if (j == 5) { dc.DrawImage(new BitmapImage(new Uri("Graphycs/ByteLikeGraphycs/dialoguebox1.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                    else if (j + 1 == cameraSize[0]) { dc.DrawImage(new BitmapImage(new Uri("Graphycs/ByteLikeGraphycs/dialoguebox4.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                    else { dc.DrawImage(new BitmapImage(new Uri("Graphycs/ByteLikeGraphycs/dialoguebox2.png", UriKind.Relative)), new Rect(j * 16, i * 16, 16, 16)); }
                                }
                                else
                                {
                                    if (j == 5) { dc.DrawImage(new BitmapImage(new Uri("Graphycs/ByteLikeGraphycs/dialoguebox3.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                    else if (j + 1 == cameraSize[0]) { dc.DrawImage(new BitmapImage(new Uri("Graphycs/ByteLikeGraphycs/dialoguebox5.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                    else { dc.DrawImage(new BitmapImage(new Uri("Graphycs/ByteLikeGraphycs/dialoguebox6.png", UriKind.Relative)), new Rect(j * 16, i * 16, 17, 17)); }
                                }
                            }
                        }

                    }


                }
                //

                for (int i = 0; i < 9; i++)
                {
                    string floorImage = "Graphycs/ByteLikeGraphycs/hud";
                    floorImage += i;
                    floorImage += ".png";
                    dc.DrawImage(new BitmapImage(new Uri(floorImage, UriKind.Relative)), new Rect(0, i * 32, 17, 17));
                    switch (i)
                    {
                        case 0:
                            floorImage = String.Format("{0}/{1}", player.Stats["HP"].ToString(), player.GetStat("MaxHP"));
                            break;
                        case 1:
                            floorImage = String.Format("{0}/{1}", player.Stats["Mana"].ToString(), player.GetStat("MaxMana"));
                            break;
                        case 2:
                            floorImage = player.GetStat("Defense").ToString();
                            break;
                        case 3:
                            floorImage = player.GetStat("MagicDefense").ToString();
                            break;
                        case 4:
                            floorImage = player.GetStat("Strength").ToString();
                            break;
                        case 5:
                            floorImage = player.GetStat("Magic").ToString();
                            break;
                        case 6:
                            floorImage = player.GetStat("Agility").ToString();
                            break;
                        case 7:
                            floorImage = player.Stats["Level"].ToString();
                            break;
                        case 8:
                            floorImage = String.Format("{0}/{1}", player.Stats["XP"], 90+ Math.Pow(player.Stats["Level"],2)*10);
                            break;
                    }
                    FormattedText dialogue2 = new FormattedText(floorImage, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 16, System.Windows.Media.Brushes.White);
                    dialogue2.MaxTextWidth = 56;
                    dc.DrawText(dialogue2, new System.Windows.Point(4, 14 + i * 32));
                }

                if (response != "")
                {
                    dc.DrawText(dialogue, new System.Windows.Point(88, (cameraSize[1]*16)-dialogue.Height-8));
                }
            }

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
            imageBorder.Child = DrawMap(response);

            this.Background = System.Windows.Media.Brushes.White;
            this.Margin = new Thickness(0);
            this.Content = imageBorder;

            Console.Write(response);



            
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
                                    if (beampos[0] + j >= 0 && beampos[0] + j < level.GetLength(0) && beampos[1] -1 >= 0 && beampos[1] -1 < level.GetLength(1))
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


    public class Item
    {
        // 0 - Useless, 1 - Head, 2 - Torso, 3 - Legs, 4 - Weapon, 5 - OffHand, 6 - Torch, 7 - Necklace, 8 - Ring, 9 - (For safety, none), 10 - Usable, 11 - Ammo
        public int GearType = 0;
        public int ClassType = 0;
        public string Name = "???";
        public Dictionary<string, int> Stats = new Dictionary<string, int>();
        public string Spell = "Nothing";
        public bool IsHeavy = false;
        static Random rand = new Random();
        public string File = "placeholder.png";
        // None - 0, Fire - 1, Posion - 2, Freeze - 3, Paralysis - 4
        public int Element = 0;

        public Item(int floor)
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


            GearType = rand.Next(4) + 1;

            RandomizeGearType:
            switch (GearType)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    bool sideB = false;
                    int typeSwitch = 0;
                    int strength = (floor / 30) + rand.Next(-0, 1);
                    if (strength < 0) { strength = 0; }
                    else if (strength > 4) { strength = 4; }
                    if (rand.Next(1) == 0 && strength != 0 && strength != 4)
                    {
                        sideB = true;
                        if (strength > 3) { strength = 3; }
                        typeSwitch += 5000;
                    }
                    typeSwitch += strength * 10000;


                    Stats["MaxHP"] += rand.Next(0, strength+1) * 5;
                    Stats["MaxMana"] += rand.Next(0, strength+1) * 5;
                    Stats["Strength"] += rand.Next(0, strength+1);
                    Stats["Magic"] += rand.Next(0, strength+1);
                    Stats["Agility"] += rand.Next(0, strength+1);
                    Stats["Defense"] += rand.Next(0, strength+1);
                    Stats["MagicDefense"] += rand.Next(0, strength+1);

                    if (strength == 0)
                    {
                        GearType = rand.Next(3) + 1;
                        sideB = false;
                    }

                    File = "Graphycs/ByteLikeGraphycs/armor" + strength.ToString() + "-" + GearType.ToString();
                    typeSwitch += GearType * 10;
                    if (GearType == 4 && rand.Next(1) == 0)
                    {
                        File += "b";
                        typeSwitch += 5;
                    }
                    if (strength > 0)
                    {
                        switch (rand.Next(2))
                        {
                            case 0:
                                File += "w";
                                typeSwitch += 100;
                                ClassType = 1;
                                break;
                            case 1:
                                File += "r";
                                typeSwitch += 200;
                                ClassType = 2;
                                break;
                            case 2:
                                File += "m";
                                typeSwitch += 300;
                                ClassType = 3;
                                break;
                        }
                    }

                    if (sideB) { File += "b"; }
                    File += ".png";


                    //Name = "";
                    //Stats["Torch"] += 0;
                    //Stats["MaxHP"] += 0;
                    //Stats["MaxMana"] += 0;
                    //Stats["Strength"] += 0;
                    //Stats["Magic"] += 0;
                    //Stats["Agility"] += 0;
                    //Stats["Defense"] += 0;
                    //Stats["MagicDefense"] += 0;
                    //Stats["HPRegen"] += 0;
                    //Stats["ManaRegen"] += 0;


                    switch (typeSwitch)
                    {
                        case 10:
                            Name = "Leather Cap";
                            Stats["Defense"] += 1;
                            break;
                        case 20:
                            Name = "Leather Vest";
                            Stats["Defense"] += 1;
                            break;
                        case 30:
                            Name = "Leather Boots";
                            Stats["MagicDefense"] += 1;
                            break;
                        case 40:
                            Name = "Makeshift Spear";
                            Stats["Strength"] += 2;
                            Stats["Magic"] += 1;
                            break;
                        case 45:
                            Name = "Makeshift Hatchet";
                            Stats["Strength"] += 3;
                            break;
                        case 10110:
                            Name = "Iron Cap";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 2;
                            break;
                        case 10120:
                            Name = "Chainmail";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 3;
                            break;
                        case 10130:
                            Name = "Leather Leggings";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 1;
                            break;
                        case 10140:
                            Name = "Longsword";
                            Stats["Strength"] += 5;
                            break;
                        case 10145:
                            Name = "Warhammer";
                            Stats["Strength"] += 6;
                            Stats["Magic"] += -2;
                            break;
                        case 10150:
                            Name = "Buckler";
                            Stats["Defense"] += 2;
                            break;
                        case 15110:
                            Name = "Enchanted Cap";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 1;
                            Stats["MagicDefense"] += 1;
                            break;
                        case 15120:
                            Name = "Enchanted Chainmail";
                            Stats["Strength"] += 1;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 15130:
                            Name = "Enchanted Leggings";
                            Stats["Strength"] += 1;
                            Stats["MagicDefense"] += 1;
                            break;
                        case 15140:
                            Name = "Posioned Sword";
                            Stats["Strength"] += 4;
                            Stats["MagicDefense"] += -1;
                            Element = 2;
                            break;
                        case 15145:
                            Name = "Thunder Hammer";
                            Stats["Strength"] += 5;
                            Stats["Magic"] += -3;
                            Element = 4;
                            break;
                        case 15150:
                            Name = "Spiky Buckler";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 1;
                            break;
                        case 10210:
                            Name = "Cloth Hat";
                            Stats["Agility"] += 1;
                            Stats["Defense"] += 1;
                            break;
                        case 10220:
                            Name = "Cloth Shirt";
                            Stats["Agility"] += 1;
                            Stats["Defense"] += 2;
                            break;
                        case 10230:
                            Name = "Cloth Leggings";
                            Stats["Agility"] += 1;
                            Stats["MagicDefense"] += 1;
                            break;
                        case 10240:
                            Name = "Longbow";
                            Stats["Strength"] += 3;
                            Stats["Agility"] += 5;
                            break;
                        case 10245:
                            Name = "Shortbow";
                            Stats["Strength"] += 1;
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 4;
                            break;
                        case 10250:
                            Name = "Basic Quiver";
                            Stats["Agility"] += 2;
                            break;
                        case 15210:
                            Name = "Bandit Bandana";
                            Stats["Agility"] += 1;
                            Stats["MagicDefense"] += 1;
                            break;
                        case 15220:
                            Name = "Bandit Shawl";
                            Stats["Agility"] += 2;
                            Stats["Defense"] += 1;
                            Stats["MagicDefense"] += 1;
                            break;
                        case 15230:
                            Name = "Bandit Pants";
                            Stats["Agility"] += 1;
                            Stats["Defense"] += 1;
                            break;
                        case 15240:
                            Name = "Crystal Bow";
                            Stats["Strength"] += 4;
                            Stats["Magic"] += 3;
                            Stats["Agility"] += 4;
                            break;
                        case 15245:
                            Name = "Poison Shortbow";
                            Stats["Strength"] += 2;
                            Stats["Agility"] += 3;
                            Element = 2;
                            break;
                        case 15250:
                            Name = "Scorpion Quiver";
                            Stats["Agility"] += 1;
                            Stats["MagicDefense"] += 1;
                            break;
                        case 10310:
                            Name = "Apprentice Hood";
                            Stats["Magic"] += 1;
                            Stats["MagicDefense"] += 1;
                            break;
                        case 10320:
                            Name = "Apprentice Cloak";
                            Stats["Magic"] += 2;
                            Stats["Defense"] += 1;
                            break;
                        case 10330:
                            Name = "Apprentice Kilt";
                            Stats["Magic"] += 1;
                            Stats["MagicDefense"] += 1;
                            break;
                        case 10340:
                            Name = "Saphire Staff";
                            Stats["Strength"] += 3;
                            Stats["Magic"] += 6;
                            break;
                        case 10345:
                            Name = "Ember Wand";
                            Stats["Strength"] += 2;
                            Stats["Magic"] += 4;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 10350:
                            Name = "Commoner's Tome";
                            Stats["Magic"] +=        2;
                            break;
                        case 15310:
                            Name = "Bug Master Hood";
                            Stats["MaxMana"] += 10;
                            Stats["Defense"] += 1;
                            break;
                        case 15320:
                            Name = "Bug Master CLoak";
                            Stats["Magic"] +=        1;
                            Stats["Defense"] +=      2;
                            break;
                        case 15330:
                            Name = "Bug Master Kilt";
                            Stats["MaxMana"] +=      5;
                            Stats["Defense"] +=      1;
                            break;
                        case 15340:
                            Name = "Diamond Staff";
                            Stats["MaxMana"] +=      20;
                            Stats["Strength"] +=     3;
                            Stats["Magic"] +=        5;
                            break;
                        case 15345:
                            Name = "Root Wand";
                            Stats["Strength"] +=     2;
                            Stats["Magic"] +=        4;
                            Stats["ManaRegen"] +=    -1;
                            break;
                        case 15350:
                            Name = "Hidden Tome";
                            Stats["MaxMana"] +=      10;
                            break;
                        case 20110:
                            Name = "Iron Helmet";
                            Stats["Strength"] +=     2;
                            Stats["Defense"] +=      4;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 20120:
                            Name = "Iron Vest";
                            Stats["Strength"] +=     2;
                            Stats["Defense"] +=      6;
                            break;
                        case 20130:
                            Name = "Iron Leggings";
                            Stats["Strength"] +=     2;
                            Stats["Defense"] +=      4;
                            Stats["MagicDefense"] += 1;
                            break;
                        case 20140:
                            Name = "Greatsword";
                            Stats["Strength"] +=     8;
                            break;
                        case 20145:
                            Name = "Mythril Hammer";
                            Stats["Strength"] +=     10;
                            Stats["Magic"] +=        -3;
                            break;
                        case 20150:
                            Name = "Warrior Shield";
                            Stats["Strength"] +=     1;
                            Stats["Defense"] +=      4;
                            Stats["MagicDefense"] += 1;
                            break;
                        case 25110:
                            Name = "Mythril Helmet";
                            Stats["Strength"] +=     2;
                            Stats["Defense"] +=      2;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 25120:
                            Name = "Mythril Vest";
                            Stats["Strength"] +=     2;
                            Stats["Defense"] +=      3;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 25130:
                            Name = "Mythril Leggings";
                            Stats["Strength"] +=     2;
                            Stats["Magic"] +=        1;
                            Stats["Defense"] +=      1;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 25140:
                            Name = "Fire Sword";
                            Stats["Strength"] +=     6;
                            Stats["Magic"] +=        3;
                            Element = 1;
                            break;
                        case 25145:
                            Name = "Holy Hammer";
                            Stats["MaxMana"] +=      5;
                            Stats["Strength"] +=     8;
                            Stats["Magic"] +=        2;
                            break;
                        case 25150:
                            Name = "Rubber Shield";
                            Stats["Strength"] +=     1;
                            Stats["Defense"] +=      3;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 20210:
                            Name = "Royal Cap";
                            Stats["Agility"] +=      2;
                            Stats["Defense"] +=      2;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 20220:
                            Name = "Royal Vest";
                            Stats["Agility"] +=      3;
                            Stats["Defense"] +=      3;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 20230:
                            Name = "Royal Leggings";
                            Stats["Agility"] +=      2;
                            Stats["Defense"] +=      1;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 20240:
                            Name = "Royal Longbow";
                            Stats["Strength"] +=     5;
                            Stats["Agility"] +=      8;
                            break;
                        case 20245:
                            Name = "Huntsman's Shortbow";
                            Stats["Strength"] +=     3;
                            Stats["Magic"] +=        4;
                            Stats["Agility"] +=      6;
                            break;
                        case 20250:
                            Name = "Royal Quiver";
                            Stats["Magic"] +=        2;
                            Stats["Agility"] +=      4;
                            break;
                        case 25210:
                            Name = "Wanderer's Hat";
                            Stats["MaxMana"] +=      5;
                            Stats["Agility"] +=      3;
                            Stats["Defense"] +=      1;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 25220:
                            Name = "Wanderer's Clothes";
                            Stats["Magic"] +=        2;
                            Stats["Agility"] +=      2;
                            Stats["Defense"] +=      2;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 25230:
                            Name = "Wanderer's Shorts";
                            Stats["Magic"] +=        1;
                            Stats["Agility"] +=      1;
                            Stats["Defense"] +=      1;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 25240:
                            Name = "Spiky Longbow";
                            Stats["MaxMana"] +=      5;
                            Stats["Strength"] +=     6;
                            Stats["Magic"] +=        2;
                            Stats["Agility"] +=      5;
                            break;
                        case 25245:
                            Name = "Frozen Shortbow";
                            Stats["MaxHP"] +=        10;
                            Stats["Strength"] +=     5;
                            Stats["Magic"] +=        2;
                            Stats["Agility"] +=      4;
                            Element = 3;
                            break;
                        case 25250:
                            Name = "Lava Quiver";
                            Stats["Magic"] +=        3;
                            Stats["Agility"] +=      3;
                            Element = 1;
                            break;
                        case 20310:
                            Name = "Cut-up Hood";
                            Stats["Magic"] +=        2;
                            Stats["MagicDefense"] += 5;
                            break;
                        case 20320:
                            Name = "Cut-up Cloak";
                            Stats["MaxMana"] +=      10;
                            Stats["Magic"] +=        3;
                            Stats["Defense"] +=      2;
                            Stats["MagicDefense"] += 6;
                            break;
                        case 20330:
                            Name = "Cut-up Kilt";
                            Stats["MaxMana"] +=      5;
                            Stats["Magic"] +=        2;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 20340:
                            Name = "Emerald Staff";
                            Stats["MaxMana"] +=      5;
                            Stats["Strength"] +=     5;
                            Stats["Magic"] +=        10;
                            break;
                        case 20345:
                            Name = "Water Wand";
                            Stats["MaxMana"] +=      15;
                            Stats["Strength"] +=     6;
                            Stats["Magic"] +=        7;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 20350:
                            Name = "Scholar's Tome";
                            Stats["MaxMana"] +=      10;
                            Stats["Magic"] +=        3;
                            break;
                        case 25310:
                            Name = "Crystal Hood";
                            Stats["Magic"] +=        1;
                            Stats["Defense"] +=      2;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 25320:
                            Name = "Crystal Cloak";
                            Stats["MaxMana"] +=      15;
                            Stats["Magic"] +=        5;
                            Stats["Defense"] +=      -2;
                            Stats["MagicDefense"] += 8;
                            break;
                        case 25330:
                            Name = "Crystal Kilt";
                            Stats["MaxMana"] +=      15;
                            Stats["Strength"] +=     -2;
                            Stats["Magic"] +=        3;
                            Stats["Defense"] +=      2;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 25340:
                            Name = "Crooked Staff";
                            Stats["Torch"] +=        1;
                            Stats["MaxMana"] +=      15;
                            Stats["Strength"] +=     6;
                            Stats["Magic"] +=        8;
                            break;
                        case 25345:
                            Name = "Flame Wand";
                            Stats["Torch"] +=        1;
                            Stats["MaxMana"] +=      10;
                            Stats["Strength"] +=     3;
                            Stats["Magic"] +=        7;
                            Stats["Defense"] +=      2;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 25350:
                            Name = "Enchanted Tome";
                            Stats["MaxMana"] +=      15;
                            Stats["Strength"] +=     -2;
                            Stats["Magic"] +=        3;
                            Stats["Defense"] +=      2;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 30110:
                            Name = "Hardened Helmet";
                            Stats["MaxHP"] +=        5;
                            Stats["Strength"] +=     3;
                            Stats["Magic"] +=        -1;
                            Stats["Defense"] +=      8;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 30120:
                            Name = "Hardened Armor";
                            Stats["MaxHP"] +=        10;
                            Stats["MaxMana"] +=      -5;
                            Stats["Strength"] +=     3;
                            Stats["Defense"] +=      10;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 30130:
                            Name = "Hardened Leggings";
                            Stats["Strength"] +=     3;
                            Stats["Defense"] +=      6;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 30140:
                            Name = "Hardened Sword";
                            Stats["Strength"] +=     13;
                            Stats["Defense"] +=      2;
                            break;
                        case 30145:
                            Name = "Obsidian Hammer";
                            Stats["MaxMana"] +=      -5;
                            Stats["Strength"] +=     16;
                            Stats["Magic"] +=        -5;
                            Stats["Defense"] +=      4;
                            break;
                        case 30150:
                            Name = "Tower Shield";
                            Stats["MaxHP"] +=        5;
                            Stats["Strength"] +=     2;
                            Stats["Defense"] +=      7;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 35110:
                            Name = "Dragon Helmet";
                            Stats["Strength"] +=     3;
                            Stats["Magic"] +=        1;
                            Stats["Defense"] +=      5;
                            Stats["MagicDefense"] += 6;
                            Stats["HPRegen"] +=      -1;
                            break;
                        case 35120:
                            Name = "Dragon Armor";
                            Stats["MaxHP"] +=        5;
                            Stats["MaxMana"] +=      5;
                            Stats["Strength"] +=     3;
                            Stats["Defense"] +=      7;
                            Stats["MagicDefense"] += 6;
                            Stats["HPRegen"] +=      -2;
                            break;
                        case 35130:
                            Name = "Dragon Leggings";
                            Stats["Torch"] +=        1;
                            Stats["MaxHP"] +=        5;
                            Stats["Strength"] +=     3;
                            Stats["Defense"] +=      4;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 35140:
                            Name = "Obsidian Sword";
                            Stats["Strength"] +=     10;
                            Stats["Defense"] +=      3;
                            Stats["MagicDefense"] += 2;
                            Stats["ManaRegen"] +=    -2;
                            break;
                        case 35145:
                            Name = "Milion Pounds Hammer";
                            Stats["Torch"] +=        -1;
                            Stats["MaxMana"] +=      -10;
                            Stats["Strength"] +=     20;
                            Stats["Magic"] +=        -5;
                            Stats["Defense"] +=      6;
                            break;
                        case 35150:
                            Name = "Reflective Shield";
                            Stats["MaxHP"] +=        5;
                            Stats["Defense"] +=      7;
                            Stats["MagicDefense"] += 4;
                            Stats["HPRegen"] +=      -1;
                            Stats["ManaRegen"] +=    -2;
                            break;
                        case 30210:
                            Name = "Huntsman's Hat";
                            Stats["Magic"] +=        2;
                            Stats["Agility"] +=      3;
                            Stats["Defense"] +=      4;
                            Stats["MagicDefense"] += 5;
                            break;
                        case 30220:
                            Name = "Huntsman's Vest";
                            Stats["MaxMana"] +=      10;
                            Stats["Agility"] +=      3;
                            Stats["Defense"] +=      6;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 30230:
                            Name = "Huntsman's Leggings";
                            Stats["Magic"] +=        2;
                            Stats["Agility"] +=      3;
                            Stats["Defense"] +=      4;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 30240:
                            Name = "Huntsman's Longbow";
                            Stats["MaxMana"] +=      5;
                            Stats["Strength"] +=     8;
                            Stats["Agility"] +=      12;
                            break;
                        case 30245:
                            Name = "Perfected Shortbow";
                            Stats["Torch"] +=        1;
                            Stats["MaxMana"] +=      5;
                            Stats["Strength"] +=     6;
                            Stats["Magic"] +=        3;
                            Stats["Agility"] +=      10;
                            break;
                        case 30250:
                            Name = "Crystal Quiver";
                            Stats["Strength"] +=     2;
                            Stats["Magic"] +=        3;
                            Stats["Agility"] +=      6;
                            break;
                        case 35210:
                            Name = "Camo Hat";
                            Stats["Torch"] +=        1;
                            Stats["Magic"] +=        2;
                            Stats["Agility"] +=      4;
                            Stats["Defense"] +=      2;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 35220:
                            Name = "Camo Vest";
                            Stats["Magic"] +=        1;
                            Stats["Agility"] +=      3;
                            Stats["Defense"] +=      4;
                            Stats["MagicDefense"] += 6;
                            Stats["ManaRegen"] +=    -1;
                            break;
                        case 35230:
                            Name = "Camo Leggings";
                            Stats["Magic"] +=        2;
                            Stats["Agility"] +=      3;
                            Stats["Defense"] +=      3;
                            Stats["HPRegen"] +=      -1;
                            break;
                        case 35240:
                            Name = "Golden Longbow";
                            Stats["Torch"] +=        1;
                            Stats["Strength"] +=     5;
                            Stats["Agility"] +=      10;
                            Element = 4;
                            break;
                        case 35245:
                            Name = "Crystal Shortbow";
                            Stats["Torch"] +=        3;
                            Stats["Strength"] +=     3;
                            Stats["Magic"] +=        5;
                            Stats["Agility"] +=      7;
                            Stats["HPRegen"] +=      -1;
                            Stats["ManaRegen"] +=    -2;
                            break;
                        case 35250:
                            Name = "Enchanted Quiver";
                            Stats["Torch"] +=        2;
                            Stats["Strength"] +=     3;
                            Stats["Agility"] +=      4;
                            Stats["HPRegen"] +=      -1;
                            Stats["ManaRegen"] +=    -1;
                            break;
                        case 30310:
                            Name = "Wizard's Hood";
                            Stats["MaxMana"] +=      10;
                            Stats["Magic"] +=        3;
                            Stats["Defense"] +=      2;
                            Stats["MagicDefense"] += 7;
                            break;
                        case 30320:
                            Name = "Wizard's Cloak";
                            Stats["MaxMana"] +=      15;
                            Stats["Magic"] +=        3;
                            Stats["Defense"] +=      3;
                            Stats["MagicDefense"] += 9;
                            break;
                        case 30330:
                            Name = "Wizard's Kilt";
                            Stats["MaxMana"] +=      10;
                            Stats["Magic"] +=        3;
                            Stats["Defense"] +=      1;
                            Stats["MagicDefense"] += 6;
                            break;
                        case 30340:
                            Name = "Ruby Staff";
                            Stats["Torch"] +=        1;
                            Stats["MaxMana"] +=      10;
                            Stats["Strength"] +=     8;
                            Stats["Magic"] +=        15;
                            Stats["Defense"] +=      2;
                            Stats["ManaRegen"] +=    -1;
                            break;
                        case 30345:
                            Name = "Arkhana Wand";
                            Stats["Torch"] +=        1;
                            Stats["MaxMana"] +=      20;
                            Stats["Strength"] +=     10;
                            Stats["Magic"] +=        10;
                            Stats["Defense"] +=      3;
                            Stats["MagicDefense"] += 4;
                            Stats["HPRegen"] +=      -1;
                            break;
                        case 30350:
                            Name = "Wizard's Tome";
                            Stats["MaxMana"] +=      15;
                            Stats["Magic"] +=        5;
                            Stats["MagicDefense"] += 1;
                            Stats["ManaRegen"] +=    -1;
                            break;
                        case 35310:
                            Name = "Dark Mage's Hood";
                            Stats["Torch"] +=        2;
                            Stats["MaxMana"] +=      5;
                            Stats["Magic"] +=        3;
                            Stats["Defense"] +=      4;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 35320:
                            Name = "Dark Mage's Cloak";
                            Stats["MaxMana"] +=      10;
                            Stats["Magic"] +=        3;
                            Stats["Defense"] +=      4;
                            Stats["MagicDefense"] += 7;
                            Stats["ManaRegen"] +=    -2;
                            break;
                        case 35330:
                            Name = "Dark Mage's Kilt";
                            Stats["Torch"] +=        2;
                            Stats["Magic"] +=        3;
                            Stats["Defense"] +=      3;
                            Stats["MagicDefense"] += 3;
                            Stats["HPRegen"] +=      -2;
                            Stats["ManaRegen"] +=    -1;
                            break;
                        case 35340:
                            Name = "Shattered Staff";
                            Stats["MaxMana"] +=      30;
                            Stats["Magic"] +=        20;
                            Stats["Defense"] +=      -6;
                            Stats["MagicDefense"] += -8;
                            Stats["ManaRegen"] +=    -2;
                            Stats["Strength"] +=     10;
                            break;
                        case 35345:
                            Name = "Two-sided Wand";
                            Stats["Torch"] +=        2;
                            Stats["MaxMana"] +=      10;
                            Stats["Strength"] +=     15;
                            Stats["Magic"] +=        15;
                            Stats["Defense"] +=      -4;
                            Stats["MagicDefense"] += -4;
                            Stats["HPRegen"] +=      1;
                            Stats["ManaRegen"] +=    -2;
                            break;
                        case 35350:
                            Name = "Forbidden Tome";
                            Stats["Torch"] +=        1;
                            Stats["MaxMana"] +=      20;
                            Stats["Magic"] +=        3;
                            Stats["ManaRegen"] +=    -2;
                            break;
                        case 40110:
                            Name = "Paladin's Helmet";
                            Stats["MaxHP"] +=        10;
                            Stats["Strength"] +=     4;
                            Stats["Magic"] +=        -2;
                            Stats["Agility"] +=      -2;
                            Stats["Defense"] +=      10;
                            Stats["MagicDefense"] += 5;
                            Stats["HPRegen"] +=      -1;
                            break;
                        case 40120:
                            Name = "Paladin's Armor";
                            Stats["MaxHP"] +=        15;
                            Stats["MaxMana"] +=      -5;
                            Stats["Strength"] +=     4;
                            Stats["Agility"] +=      -1;
                            Stats["Defense"] +=      15;
                            Stats["MagicDefense"] += 6;
                            Stats["HPRegen"] +=      -2;
                            break;
                        case 40130:
                            Name = "Paladin's Leggings";
                            Stats["MaxHP"] +=        10;
                            Stats["MaxMana"] +=      -5;
                            Stats["Strength"] +=     4;
                            Stats["Agility"] +=      -1;
                            Stats["Defense"] +=      8;
                            Stats["MagicDefense"] += 4;
                            Stats["HPRegen"] +=      -1;
                            break;
                        case 40140:
                            Name = "Sword Of Legends";
                            Stats["Strength"] +=     15;
                            Stats["Defense"] +=      2;
                            Stats["MagicDefense"] += 2;
                            Stats["HPRegen"] +=      -1;
                            break;
                        case 40145:
                            Name = "Mace Of Legends";
                            Stats["MaxHP"] +=        5;
                            Stats["Strength"] +=     20;
                            Stats["Magic"] +=        -3;
                            Stats["Agility"] +=      -3;
                            Stats["Defense"] +=      4;
                            Stats["HPRegen"] +=      1;
                            Stats["ManaRegen"] +=    2;
                            break;
                        case 40150:
                            Name = "Unbreakable Shield";
                            Stats["Strength"] +=     3;
                            Stats["Defense"] +=      10;
                            Stats["MagicDefense"] += 2;
                            Stats["HPRegen"] +=      -2;
                            break;
                        case 40210:
                            Name = "Hawk Hat";
                            Stats["Torch"] +=        2;
                            Stats["Magic"] +=        3;
                            Stats["Agility"] +=      4;
                            Stats["Defense"] +=      7;
                            Stats["MagicDefense"] += 6;
                            break;
                        case 40220:
                            Name = "Hawk Scarf";
                            Stats["Torch"] +=        1;
                            Stats["MaxMana"] +=      10;
                            Stats["Magic"] +=        2;
                            Stats["Agility"] +=      4;
                            Stats["Defense"] +=      9;
                            Stats["MagicDefense"] += 6;
                            break;
                        case 40230:
                            Name = "Hawk Belt";
                            Stats["Magic"] +=        2;
                            Stats["Agility"] +=      4;
                            Stats["Defense"] +=      7;
                            Stats["MagicDefense"] += 4;
                            Stats["ManaRegen"] +=    -1;
                            break;
                        case 40240:
                            Name = "Living Longbow";
                            Stats["MaxMana"] +=      5;
                            Stats["Strength"] +=     10;
                            Stats["Magic"] +=        2;
                            Stats["Agility"] +=      15;
                            Stats["Defense"] +=      2;
                            break;
                        case 40245:
                            Name = "Full Metal Shortbow";
                            Stats["Torch"] +=        3;
                            Stats["MaxMana"] +=      15;
                            Stats["Strength"] +=     7;
                            Stats["Magic"] +=        4;
                            Stats["Agility"] +=      12;
                            Stats["Defense"] +=      1;
                            Stats["HPRegen"] +=      -1;
                            break;
                        case 40250:
                            Name = "Black Feather Quiver";
                            Stats["MaxMana"] +=      5;
                            Stats["Strength"] +=     2;
                            Stats["Agility"] +=      8;
                            Stats["Defense"] +=      2;
                            Stats["MagicDefense"] += 2;
                            Stats["ManaRegen"] +=    -1;
                            break;
                        case 40310:
                            Name = "Forgotten Hood";
                            Stats["Torch"] +=        2;
                            Stats["MaxMana"] +=      20;
                            Stats["Magic"] +=        4;
                            Stats["Defense"] +=      4;
                            Stats["MagicDefense"] += 12;
                            Stats["ManaRegen"] +=    -1;
                            break;
                        case 40320:
                            Name = "Forgotten Cape";
                            Stats["Torch"] +=        2;
                            Stats["MaxMana"] +=      20;
                            Stats["Magic"] +=        4;
                            Stats["Defense"] +=      5;
                            Stats["MagicDefense"] += 15;
                            Stats["ManaRegen"] +=    -1;
                            break;
                        case 40330:
                            Name = "Mana Belt";
                            Stats["Torch"] +=        1;
                            Stats["MaxMana"] +=      30;
                            Stats["Magic"] +=        4;
                            Stats["Defense"] +=      3;
                            Stats["MagicDefense"] += 7;
                            Stats["ManaRegen"] +=    -2;
                            break;
                        case 40340:
                            Name = "Rainbow Staff";
                            Stats["Torch"] +=        1;
                            Stats["MaxMana"] +=      15;
                            Stats["Strength"] +=     10;
                            Stats["Magic"] +=        20;
                            Stats["Defense"] +=      3;
                            Stats["ManaRegen"] +=    -2;
                            break;
                        case 40345:
                            Name = "Refraction Wand";
                            Stats["Torch"] +=        3;
                            Stats["MaxMana"] +=      30;
                            Stats["Strength"] +=     15;
                            Stats["Magic"] +=        15;
                            Stats["Defense"] +=      5;
                            Stats["MagicDefense"] += 3;
                            Stats["ManaRegen"] +=    -3;
                            break;
                        case 40350:
                            Name = "Balance Tome";
                            Stats["MaxMana"] +=      20;
                            Stats["Magic"] +=        6;
                            Stats["Defense"] +=      3;
                            Stats["MagicDefense"] += 3;
                            Stats["HPRegen"] +=      -2;
                            Stats["ManaRegen"] +=    -2;
                            break;
                    }


                    break;


            }

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
        public string File = "Graphycs/ByteLikeGraphycs/placeholder.png";
        static protected Random rand = new Random();

        public Creature()
        {
            Stats.Add("Level", 1);
            Stats.Add("HP", 5);
            Stats.Add("Mana", 10);
            Stats.Add("Strength", 3);
            Stats.Add("Magic", 3);
            Stats.Add("Agility", 3);
            Stats.Add("Defense", 1);
            Stats.Add("MagicDefense", 1);
            Stats.Add("HPRegen", 5);
            Stats.Add("ManaRegen", 7);
            Stats.Add("MaxHP", 5);
            Stats.Add("MaxMana", 10);
            Stats.Add("MaxHPRegen", 0);
            Stats.Add("MaxManaRegen", 0);

            Buffs.Add("MaxHP", 0);
            Buffs.Add("MaxMana", 0);
            Buffs.Add("Defense", 0);
            Buffs.Add("MagicDefense", 0);
            Buffs.Add("Strength", 0);
            Buffs.Add("Magic", 0);
            Buffs.Add("Agility", 0);
            Buffs.Add("HPRegen", 0);
            Buffs.Add("ManaRegen", 0);


            BuffLevels.Add("MaxHP", 0);
            BuffLevels.Add("MaxMana", 0);
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


            Stats["MaxHPRegen"]++;
            Stats["MaxManaRegen"]++;


            foreach (KeyValuePair<string, int> item in Buffs)
            {
                if (item.Value > 0) { Buffs[item.Key]--; }
            }


            if (Stats["MaxHPRegen"] >= GetStat("HPRegen"))
            {
                Stats["HP"]+= 2;

                if (GetStat("HPRegen") < 0)
                {
                    Stats["HP"] -= GetStat("HPRegen");
                }

                Stats["MaxHPRegen"] = 0;
            }


            if (Stats["MaxManaRegen"] >= GetStat("ManaRegen"))
            {
                Stats["Mana"] += 2;

                if (GetStat("ManaRegen") < 0)
                {
                    Stats["Mana"] -= GetStat("ManaRegen");
                }

                Stats["MaxManaRegen"] = 0;
            }



            // Conditions
            // Burn - 0, Poison - 1, Freeze - 2, Parallysis - 3

            for (int i = 0; i < 4; i++)
            {
                if (Potentials[i] > GetStat("MagicDefense")/2)
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

        public int GetStat(string index)
        {
            int current = Stats[index];
            for (int i = 0; i < 9; i++)
            {
                if (Inventory[i, 0] != null)
                    current += Inventory[i, 0].Stats[index];
            }

            if (Buffs[index] > 0)
            {
                current += BuffLevels[index];
            }

            return current;
        }

    }






    public class Player : Creature
    {

        public new Item[,] Inventory = new Item[11, 7];

        public bool OpenInventory = false;
        public int[] SelectedSlot = new int[2] { -100, -1 };
        public int[] CurrentSlot = new int[2] { 0, 0 };


        public Player(string name)
            : base()
        {
            switch (rand.Next(2))
            {
                case 0:
                    File = "Graphycs/ByteLikeGraphycs/player1.png";
                    break;
                case 1:
                    File = "Graphycs/ByteLikeGraphycs/player2.png";
                    break;
                case 2:
                    File = "Graphycs/ByteLikeGraphycs/player3.png";
                    break;
            }
            Name = name;
            Stats["HP"] = 20;
            Stats["Mana"] = 10;
            Stats["MaxHP"] = 20;
            Stats["MaxMana"] = 10;

            Stats.Add("XP", 0);

            Buffs.Add("Torch", 0);

            BuffLevels.Add("Torch", 0);


            Inventory[0, 1] = new Item(90);
            Inventory[0, 2] = new Item(90);
            Inventory[0, 3] = new Item(90);
            Inventory[0, 4] = new Item(90);
            Inventory[0, 5] = new Item(90);
        }

        // Main Logics
        public string Logics(ref int[,] level)
        {
            bool wasAlive = true;
            if (Stats["HP"] <= 0) { wasAlive = false; }
            string response = "";
            int[] movement = new int[] { 0, 0 };
            bool invCheck = false;

            // Movement code

            // Keys
            if (Keyboard.IsKeyDown(Key.Q) && !OpenInventory)
            {
                OpenInventory = true;
                invCheck = true;
            }
            // Move only if not frozen/paralysed
            if (Statuses[2] == 0 && Statuses[3] % 2 == 0 || OpenInventory)
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
            if (position[0] + movement[0] >= 0 && position[0] + movement[0] < level.GetLength(0) && position[1] + movement[1] >= 0 && position[1] + movement[1] < level.GetLength(1) && !OpenInventory)
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
            else if (OpenInventory)
            {
                CurrentSlot[0] += movement[0];
                CurrentSlot[1] += movement[1];
                if (CurrentSlot[0] >= Inventory.GetLength(0)) { CurrentSlot[0] = 0; }
                if (CurrentSlot[0] < 0) { CurrentSlot[0] = Inventory.GetLength(0) - 1; }
                if (CurrentSlot[1] >= Inventory.GetLength(1)) { CurrentSlot[1] = 0; }
                if (CurrentSlot[1] < 0) { CurrentSlot[1] = Inventory.GetLength(1) - 1; }

                if (Inventory[CurrentSlot[0], CurrentSlot[1]] != null)
                {
                    response = Inventory[CurrentSlot[0], CurrentSlot[1]].Name;
                }

                if (Keyboard.IsKeyDown(Key.E))
                {
                    if (SelectedSlot[0] >= 0)
                    {
                        if (CurrentSlot[1] != 0 && SelectedSlot[1] != 0)
                        {
                            Item temp = Inventory[CurrentSlot[0], CurrentSlot[1]];
                            Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];
                            Inventory[SelectedSlot[0], SelectedSlot[1]] = temp;
                            SelectedSlot[0] = -100;
                        }
                        else if (SelectedSlot[1] == 0)
                        {
                            if (SelectedSlot[0] < 9)
                            {
                                if (Inventory[CurrentSlot[0], CurrentSlot[1]] == null)
                                {
                                    if (CurrentSlot[1] != 0 || (CurrentSlot[0] == 7 && SelectedSlot[0] == 8) || (CurrentSlot[0] == 8 && SelectedSlot[0] == 7))
                                    {
                                        Item temp = Inventory[CurrentSlot[0], CurrentSlot[1]];
                                        Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];
                                        Inventory[SelectedSlot[0], SelectedSlot[1]] = temp;
                                        SelectedSlot[0] = -100;
                                    }
                                    else if (CurrentSlot[0] < 9)
                                    {
                                        response += $"{Name}: I can't put that there.\n";
                                    }
                                    else if (CurrentSlot[0] == 9)
                                    {
                                        response += $"{Name}: I can't directly use that.\n";
                                    }
                                }
                                else if (Inventory[CurrentSlot[0], CurrentSlot[1]].GearType == SelectedSlot[0] + 1 || (SelectedSlot[0] == 8 && Inventory[CurrentSlot[0], CurrentSlot[1]].GearType == SelectedSlot[0]))
                                {
                                    Item temp = Inventory[CurrentSlot[0], CurrentSlot[1]];
                                    Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];
                                    Inventory[SelectedSlot[0], SelectedSlot[1]] = temp;
                                    SelectedSlot[0] = -100;
                                }
                                else
                                {
                                    response += $"{Name}: I can't put that there.\n";
                                }
                            }
                        }
                        else if (CurrentSlot[1] == 0)
                        {
                            if (CurrentSlot[0] < 9)
                            {
                                if (Inventory[SelectedSlot[0], SelectedSlot[1]] == null)
                                {
                                    if (SelectedSlot[1] != 0 || (CurrentSlot[0] == 7 && SelectedSlot[0] == 8) || (CurrentSlot[0] == 8 && SelectedSlot[0] == 7))
                                    {
                                        Item temp = Inventory[CurrentSlot[0], CurrentSlot[1]];
                                        Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];
                                        Inventory[SelectedSlot[0], SelectedSlot[1]] = temp;
                                        SelectedSlot[0] = -100;
                                    }
                                    else
                                    {
                                        response += $"{Name}: I can't put that there.\n";
                                    }
                                }
                                else if (Inventory[SelectedSlot[0], SelectedSlot[1]].GearType == CurrentSlot[0] + 1 || (CurrentSlot[0] == 8 && Inventory[SelectedSlot[0], SelectedSlot[1]].GearType == CurrentSlot[0]))
                                {
                                    Item temp = Inventory[CurrentSlot[0], CurrentSlot[1]];
                                    Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];
                                    Inventory[SelectedSlot[0], SelectedSlot[1]] = temp;
                                    SelectedSlot[0] = -100;
                                }
                                else
                                {
                                    response += $"{Name}: I can't put that there.\n";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (CurrentSlot[1] != 0 || CurrentSlot[0] < 9)
                        {
                            SelectedSlot[0] = CurrentSlot[0];
                            SelectedSlot[1] = CurrentSlot[1];
                        }
                    }
                }

            }
            //

            if (!OpenInventory)
            {
                response = Conditions(response);
                if (Stats["HP"] > GetStat("MaxHP")) { Stats["HP"] = GetStat("MaxHP"); }
                if (Stats["Mana"] > GetStat("MaxMana")) { Stats["Mana"] = GetStat("MaxMana"); }
            }



            // Death
            if (wasAlive && Stats["HP"] <= 0) { response += $"{Name} dies!\n"; }

            if (Keyboard.IsKeyDown(Key.Q) && OpenInventory && !invCheck)
            {
                OpenInventory = false;
            }

            return response;

        }
        //

        public new int GetStat(string index)
        {
            int current = Stats[index];
            for (int i = 0; i < 9; i++)
            {
                if (!OpenInventory || CurrentSlot[1] == 0)
                {
                    if (Inventory[i, 0] != null)
                        current += Inventory[i, 0].Stats[index];
                }
                else
                {
                    if (Inventory[CurrentSlot[0], CurrentSlot[1]] != null)
                    {
                        if (Inventory[CurrentSlot[0], CurrentSlot[1]].GearType == i + 1 || (i == 8 && Inventory[CurrentSlot[0], CurrentSlot[1]].GearType == i))
                        {
                            current += Inventory[CurrentSlot[0], CurrentSlot[1]].Stats[index];
                        }
                        else if (Inventory[i, 0] != null)
                        {
                            current += Inventory[i, 0].Stats[index];
                        }
                    }
                    else if (Inventory[i, 0] != null)
                    {
                        current += Inventory[i, 0].Stats[index];
                    }
                }
            }

            if (Buffs[index] > 0)
            {
                current += BuffLevels[index];
            }

            return current;
        }



        public int Torch()
        {
            int result = 1;

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
