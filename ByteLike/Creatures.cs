using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ByteLike
{

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

        public bool AddTo(Item item)
        {
            if (item == null)
            {
                return true;
            }

            if (item.GearType > 9 || item.GearType == 0)
            {
                for (int i = 1; i < Inventory.GetLength(1); i++)
                {
                    for (int j = 0; j < Inventory.GetLength(0); j++)
                    {
                        if (Inventory[j, i] != null)
                        {
                            if (Inventory[j, i].GearType == item.GearType && Inventory[j, i].Name == item.Name)
                            {
                                Inventory[j, i].Quantity += item.Quantity;
                                return true;
                            }
                        }
                    }
                }
            }

            for (int i = 1; i < Inventory.GetLength(1); i++)
            {
                for (int j = 0; j < Inventory.GetLength(0); j++)
                {
                    if (Inventory[j, i] == null)
                    {
                        Inventory[j, i] = item;
                        return true;
                    }
                }
            }


            return false;
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
                Stats["HP"] += 2;

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
                if (Potentials[i] > GetStat("MagicDefense") / 2)
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

        public int GetChest(ref List<Chest> chests)
        {
            for (int i = 0; i < chests.Count; i++)
            {
                if (chests[i].position[0] == position[0] && chests[i].position[1] == position[1])
                {
                    return i;
                }
            }
            return -1;
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
        public bool OpenSpell = false;

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
            Stats.Add("Torch", 1);

            Buffs.Add("Torch", 0);

            BuffLevels.Add("Torch", 0);


            Inventory[0, 1] = new Item(90);
            Inventory[0, 2] = new Item(90);
            Inventory[0, 3] = new Item(90);
            Inventory[0, 4] = new Item(90);
            Inventory[0, 5] = new Item(90);
        }

        // Main Logics
        public string Logics(ref int[,] level, ref List<Chest> chests)
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
                if (!OpenSpell)
                {
                    int currentChest = GetChest(ref chests);
                    if (CurrentSlot[0] >= Inventory.GetLength(0)) { CurrentSlot[0] = 0; }
                    if (CurrentSlot[0] < 0) { CurrentSlot[0] = Inventory.GetLength(0) - 1; }

                    if (currentChest == -1)
                    {
                        if (CurrentSlot[1] >= Inventory.GetLength(1)) { CurrentSlot[1] = 0; }
                        if (CurrentSlot[1] < 0) { CurrentSlot[1] = Inventory.GetLength(1) - 1; }
                    }
                    else
                    {
                        if (CurrentSlot[1] >= Inventory.GetLength(1) + chests[currentChest].Inventory.GetLength(1)) { CurrentSlot[1] = 0; }
                        if (CurrentSlot[1] < 0) { CurrentSlot[1] = Inventory.GetLength(1) + chests[currentChest].Inventory.GetLength(1) - 1; }
                    }

                    if (CurrentSlot[1] < Inventory.GetLength(1))
                    {
                        if (Inventory[CurrentSlot[0], CurrentSlot[1]] != null)
                        {
                            response = Inventory[CurrentSlot[0], CurrentSlot[1]].Name + "\n";
                        }
                    }
                    else
                    {
                        if (chests[currentChest].Inventory[CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1)] != null)
                        {
                            response = chests[currentChest].Inventory[CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1)].Name + "\n";
                        }
                    }


                    if (Keyboard.IsKeyDown(Key.E))
                    {
                        if (SelectedSlot[0] >= 0)
                        {
                            if (CurrentSlot[1] != 0 && SelectedSlot[1] != 0)
                            {

                                if (CurrentSlot[1] < Inventory.GetLength(1) && SelectedSlot[1] < Inventory.GetLength(1))
                                {
                                    // Swap
                                    if (CurrentSlot[0] != SelectedSlot[0] || CurrentSlot[1] != SelectedSlot[1])
                                    {
                                        if (Inventory[SelectedSlot[0], SelectedSlot[1]] != null)
                                        {
                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]].GearType > 9 || Inventory[SelectedSlot[0], SelectedSlot[1]].GearType == 0)
                                            {
                                                if (Inventory[CurrentSlot[0], CurrentSlot[1]] != null)
                                                {
                                                    if (Inventory[CurrentSlot[0], CurrentSlot[1]].Name == Inventory[SelectedSlot[0], SelectedSlot[1]].Name)
                                                    {
                                                        Inventory[CurrentSlot[0], CurrentSlot[1]].Quantity += Inventory[SelectedSlot[0], SelectedSlot[1]].Quantity;
                                                        Inventory[SelectedSlot[0], SelectedSlot[1]] = null;
                                                    }
                                                    else
                                                    {
                                                        Item temp2 = Inventory[CurrentSlot[0], CurrentSlot[1]];
                                                        Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];
                                                        Inventory[SelectedSlot[0], SelectedSlot[1]] = temp2;
                                                    }
                                                }
                                                else
                                                {
                                                    Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];
                                                    Inventory[SelectedSlot[0], SelectedSlot[1]] = null;
                                                }
                                            }
                                            else
                                            {
                                                Item temp = Inventory[CurrentSlot[0], CurrentSlot[1]];
                                                Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];

                                                Inventory[SelectedSlot[0], SelectedSlot[1]] = temp;
                                            }
                                        }
                                        else
                                        {
                                            Item temp = Inventory[CurrentSlot[0], CurrentSlot[1]];
                                            Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];

                                            Inventory[SelectedSlot[0], SelectedSlot[1]] = temp;
                                        }
                                    }
                                    // End Swap

                                }
                                else if (CurrentSlot[1] < Inventory.GetLength(1))
                                {
                                    Inventory[CurrentSlot[0], CurrentSlot[1]] = chests[currentChest].TakeOut(Inventory[CurrentSlot[0], CurrentSlot[1]], new int[] { SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1) });
                                }
                                else if (SelectedSlot[1] < Inventory.GetLength(1))
                                {
                                    Inventory[SelectedSlot[0], SelectedSlot[1]] = chests[currentChest].PutIn(Inventory[SelectedSlot[0], SelectedSlot[1]], new int[] { CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1) });
                                }
                                else
                                {
                                    chests[currentChest].Swap(new int[] { CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1) }, new int[] { SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1) });
                                }
                                SelectedSlot[0] = -100;


                            }
                            else if (SelectedSlot[1] == 0)
                            {
                                if (CurrentSlot[1] < Inventory.GetLength(1))
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
                                                if (Inventory[SelectedSlot[0], SelectedSlot[1]] != null)
                                                {
                                                    if (Inventory[SelectedSlot[0], SelectedSlot[1]].GearType != 10)
                                                    {
                                                        response += $"{Name}: I can't directly use that.\n";
                                                    }
                                                    else
                                                    {
                                                        CurrentSlot[0] = 0;
                                                        CurrentSlot[1] = 0;
                                                        OpenSpell = true;
                                                    }
                                                }
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
                                else
                                {
                                    response += $"{Name}: I need to take it off first.\n";
                                }
                            }
                            else if (CurrentSlot[1] == 0)
                            {
                                if (SelectedSlot[1] < Inventory.GetLength(1))
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
                                    else if (CurrentSlot[0] == 9)
                                    {
                                        if (Inventory[SelectedSlot[0], SelectedSlot[1]] != null)
                                        {
                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]].GearType != 10)
                                            {
                                                response += $"{Name}: I can't directly use that.\n";
                                            }
                                            else
                                            {
                                                CurrentSlot[0] = 0;
                                                CurrentSlot[1] = 0;
                                                OpenSpell = true;
                                            }
                                        }
                                    }
                                    else if (CurrentSlot[0] == 10)
                                    {
                                        if (GetChest(ref chests) >= 0)
                                        {
                                            if (chests[GetChest(ref chests)].AddTo(Inventory[SelectedSlot[0], SelectedSlot[1]]))
                                            {
                                                Inventory[SelectedSlot[0], SelectedSlot[1]] = null;
                                            }
                                            else
                                            {
                                                response += $"{Name}: There's not enough space here.\n";
                                            }
                                        }
                                        else
                                        {
                                            Chest temp = new Chest(new int[] { position[0], position[1] }, -1);
                                            temp.AddTo(Inventory[SelectedSlot[0], SelectedSlot[1]]);
                                            chests.Add(temp);
                                            Inventory[SelectedSlot[0], SelectedSlot[1]] = null;
                                        }
                                        SelectedSlot[0] = -100;
                                    }
                                }
                                else
                                {
                                    response += $"{Name}: I need to take it out first.\n";
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
                            else if (CurrentSlot[0] == 10 && GetChest(ref chests) >= 0)
                            {
                                for (int i = 0; i < chests[GetChest(ref chests)].Inventory.GetLength(1); i++)
                                {
                                    for (int j = 0; j < chests[GetChest(ref chests)].Inventory.GetLength(0); j++)
                                    {
                                        if (AddTo(chests[GetChest(ref chests)].Inventory[j, i]))
                                        {
                                            chests[GetChest(ref chests)].Inventory[j, i] = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Keyboard.IsKeyDown(Key.E))
                    {
                        if (CurrentSlot[0] == 0 && CurrentSlot[1] == 0)
                        {
                            OpenSpell = false;
                            CurrentSlot[0] = SelectedSlot[0];
                            CurrentSlot[1] = SelectedSlot[1];
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
                OpenSpell = false;
                CurrentSlot[0] = SelectedSlot[0];
                CurrentSlot[1] = SelectedSlot[1];
                SelectedSlot[0] = -100;
            }

            return response;

        }
        //

        public new int GetStat(string index)
        {
            int current = Stats[index];
            for (int i = 0; i < 9; i++)
            {
                if (!OpenInventory || CurrentSlot[1] == 0 || OpenSpell || CurrentSlot[1] >= Inventory.GetLength(1))
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



    }


}
