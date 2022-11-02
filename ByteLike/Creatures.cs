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
        public int[] position = new int[2];

        public Dictionary<string, int> Stats = new Dictionary<string, int>();

        // Burn (Fire) - 0, Poison (Earth) - 1, Freeze (Water) - 2, Paralysis (Lightning) - 3
        public int[] Statuses = new int[] { 0, 0, 0, 0 };
        public int[] Potentials = new int[] { 0, 0, 0, 0 };

        public Dictionary<string, int> Buffs = new Dictionary<string, int>();
        public Dictionary<string, int> BuffLevels = new Dictionary<string, int>();

        public bool IsGhost = false;
        public List<string> Spells = new List<string>();
        public Item[,] Inventory = new Item[11, 1];
        public string File = "Graphics/ByteLikeGraphics/placeholder.png";
        static protected Random rand = new Random();

        public abstract string Logics(ref int[,] level, ref List<Chest> chests, ref List<Effect> effects, ref List<Creature> enemies, ref Player player);

        public int TakeDamage(int damage, int type)
        {
            int result = 0;

            int defense = GetStat("Defense");
            if (type > 0)
                defense = GetStat("MagicDefense");

            damage -= (int)(defense / Math.Sqrt(Math.Sqrt(defense)));

            Stats["HP"] -= damage;
            Stats["HP"]--;

            if (type > 0)
                Potentials[type - 1] += 5;

            if (Stats["HP"] <= 0)
                result = Stats["Level"] * rand.Next(2, 10) + 1;

            return result;
        }

        public string GetSpellDescription(string spell)
        {
            string response = "You're unsure what this spell does\n";

            switch (spell)
            {
                case "Nothing":
                    response = "Does nothing.\n";
                    break;
                case "Shoot Arrow":
                    response = "Shoots a regular arrow.\n";
                    break;
                case "Shoot Fire Arrow":
                    response = "Shoots a fire arrow.\n";
                    break;
                case "Shoot Ice Arrow":
                    response = "Shoots an ice arrow.\n";
                    break;
                case "Shoot Lightning Arrow":
                    response = "Shoots a lightning arrow.\n";
                    break;
                case "Shoot Poison Arrow":
                    response = "Shoots a poison arrow.\n";
                    break;
                case "Search":
                    response = "Checks a small patch of grass for traps. 5 Mana\n";
                    break;
                case "Leap":
                    response = "Makes the user jump several tiles in specified direction. 15 Mana\n";
                    break;
                case "Teleport":
                    response = "Teleports the user to a random location. 30 Mana\n";
                    break;
                case "Focus":
                    response = "Shoots a random magical projectile in specified direction. 5 Mana\n";
                    break;
            }

            return response;
        }

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
            Stats.Add("Torch", 3);

            Buffs.Add("MaxHP", 0);
            Buffs.Add("MaxMana", 0);
            Buffs.Add("Defense", 0);
            Buffs.Add("MagicDefense", 0);
            Buffs.Add("Strength", 0);
            Buffs.Add("Magic", 0);
            Buffs.Add("Agility", 0);
            Buffs.Add("HPRegen", 0);
            Buffs.Add("ManaRegen", 0);
            Buffs.Add("Torch", 0);


            BuffLevels.Add("MaxHP", 0);
            BuffLevels.Add("MaxMana", 0);
            BuffLevels.Add("Defense", 0);
            BuffLevels.Add("MagicDefense", 0);
            BuffLevels.Add("Strength", 0);
            BuffLevels.Add("Magic", 0);
            BuffLevels.Add("Agility", 0);
            BuffLevels.Add("HPRegen", 0);
            BuffLevels.Add("ManaRegen", 0);
            BuffLevels.Add("Torch", 0);
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

            if (Stats["HP"] > 0)
            {

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
                        if (Statuses[0] > 0) { Stats["HP"] -= (int)(1 + Stats["MaxHP"]*0.015); }
                        break;
                    case 1:
                        if (Statuses[1] % 4 == 0 && Statuses[1] > 0) { Stats["HP"] -= (int)(3 + Stats["MaxHP"]*0.05); }
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

        public int GetSpellCost(string spell)
        {
            int result = 0;

            switch (spell)
            {
                case "Search":
                case "Focus":
                    result = 5;
                    break;
            }

            return result;
        }

        protected string WalkTo(int[] movement, ref int[,] level, string response, ref List<Creature> enemies, ref Player player)
        {
        WalkReset:
            bool InTheWay = false;

            if (movement[0] != 0 || movement[1] != 0)
            {

                foreach (Creature item in enemies)
                {
                    if (item.position[0] == position[0] + movement[0] && item.position[1] == position[1] + movement[1])
                    {
                        InTheWay = true;
                        int element = 0;
                        for (int f = 0; f < 9; f++)
                        {
                            if (Inventory[f, 0] != null)
                            {
                                if (Inventory[f, 0].Element != 0)
                                    element = Inventory[f, 0].Element;
                            }
                        }
                        int xp = item.TakeDamage(GetStat("Strength"), element);

                        switch (rand.Next(6))
                        {
                            case 1:
                                response += $"{Name} slashes {item.Name}!\n";
                                break;
                            case 2:
                                response += $"{Name} pounces at {item.Name}!\n";
                                break;
                            case 3:
                                response += $"{Name} attacks {item.Name}!\n";
                                break;
                            case 4:
                                response += $"{Name} bashes {item.Name}!\n";
                                break;
                            case 5:
                                response += $"{Name} hits {item.Name}!\n";
                                break;
                        }

                        if (Stats.ContainsKey("XP") && xp > 0)
                        {
                            int extraxp = 0;
                            for (int f = 0; f < 9; f++)
                            {
                                if (Inventory[f, 0] != null)
                                {
                                    if (Inventory[f, 0].Name.Contains("XP"))
                                        extraxp += (int)(xp * 0.25);
                                }
                            }
                            xp += extraxp;
                            Stats["XP"] += xp;
                            response += $"{Name} has gained {xp} XP!\n";
                        }
                    }
                }

                if (player.position[0] == position[0] + movement[0] && player.position[1] == position[1] + movement[1] && this.GetType() != typeof(Player))
                {
                    InTheWay = true;
                    int xp2 = player.TakeDamage(GetStat("Strength"), 0);

                    switch (rand.Next(6))
                    {
                        case 1:
                            response += $"{Name} slashes {player.Name}!\n";
                            break;
                        case 2:
                            response += $"{Name} pounces at {player.Name}!\n";
                            break;
                        case 3:
                            response += $"{Name} attacks {player.Name}!\n";
                            break;
                        case 4:
                            response += $"{Name} bashes {player.Name}!\n";
                            break;
                        case 5:
                            response += $"{Name} hits {player.Name}!\n";
                            break;
                    }

                    if (Stats.ContainsKey("XP") && xp2 > 0)
                    {
                        Stats["XP"] += xp2;
                        response += $"{Name} has gained {xp2} XP!\n";
                    }
                }

            }
            if (!InTheWay && position[0] + movement[0] >= 0 && position[0] + movement[0] < level.GetLength(0) && position[1] + movement[1] >= 0 && position[1] + movement[1] < level.GetLength(1))
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
                        if (Potentials[0] == 0)
                            response += $"{Name} steps into lava!\n";
                        position[0] += movement[0];
                        position[1] += movement[1];
                        if (FloorCheck(level[position[0], position[1]]))
                        {
                            Potentials[0] += 2;
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
                            Stats["HP"] -= (int)(Stats["MaxHP"]*0.2);
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
                    // Poisonous vines
                    case 13:
                        if (Potentials[1] == 0)
                            response += $"{Name} steps into poisonous vines!\n";
                        position[0] += movement[0];
                        position[1] += movement[1];
                        if (FloorCheck(level[position[0], position[1]]))
                        {
                            Potentials[1] += 2;
                        }
                        break;
                    // Electro terrain
                    case 14:
                        position[0] += movement[0];
                        position[1] += movement[1];
                        if (Statuses[3] == 0)
                        {
                            if (FloorCheck(level[position[0], position[1]]))
                            {
                                Statuses[3] = 2 + Weight();
                                response += $"{Name} is paralyzed by electric terrain!\n";
                            }
                        }
                        break;
                    // Level exit
                    case 15:
                        position[0] += movement[0];
                        position[1] += movement[1];
                        if (this.GetType() == typeof(Player))
                        {
                            response += $"{Name} has found the exit to the next floor!\n";
                            response += $"Press [E] to move to the next floor\n";
                        }
                        break;
                    // Secret exit
                    case 16:
                        position[0] += movement[0];
                        position[1] += movement[1];
                        level[position[0], position[1]] = 15;
                        if (this.GetType() == typeof(Player))
                        {
                            response += $"{Name} has found a secret exit to the next floor!\n";
                            response += $"Press [E] to move to the next floor\n";
                        }
                        break;

                }
            }
            return response;
        }


        protected static double DistanceBetween(int[] x, int[] y)
        {
            int disx = 0;
            int disy = 0;
            if (x[0] >= y[0]) { disx = x[0] - y[0]; }
            else { disx = y[0] - x[0]; }

            if (x[1] >= y[1]) { disy = x[1] - y[1]; }
            else { disy = y[1] - x[1]; }

            return Math.Sqrt(Math.Pow(disx, 2) + Math.Pow(disy, 2));
        }

    }




    public class Player : Creature
    {

        public new Item[,] Inventory = new Item[11, 3];

        public bool OpenInventory = false;
        public int[] SelectedSlot = new int[2] { -100, -1 };
        public int[] CurrentSlot = new int[2] { 0, 0 };
        public bool OpenSpell = false;
        public bool DrawSpellLine = false;
        int[] ArrowSlot = new int[2] { 0, 0 };
        bool UseMana = true;

        string RemSpell = "";

        public void ReceiveXP(int xp)
        {
            Stats["XP"] += xp;
        }

        protected string LevelUp()
        {
            string response = $"{Name} Leveled up!\n";
            int warrior = 0;
            int ranger = 0;
            int mage = 0;


            string[] statnames = new string[] { "HP", "Mana", "Str", "Mag", "Agi", "Def", "Mg Def", "Spell Slots" };
            string[] statids = new string[] { "MaxHP", "MaxMana", "Strength", "Magic", "Agility", "Defense", "MagicDefense", "SpellSlots" };
            Dictionary<string, int> oldstats = new Dictionary<string, int>();

            for (int i = 0; i < statnames.Length; i++)
            {
                oldstats.Add(statids[i], Stats[statids[i]]);
            }

            for (int i = 0; i < 9; i++)
            {
                if (Inventory[i, 0] != null)
                {
                    switch (Inventory[i, 0].ClassType)
                    {
                        case 1:
                            warrior++;
                            break;
                        case 2:
                            ranger++;
                            break;
                        case 3:
                            mage++;
                            break;
                    }
                }
            }

            Stats["XP"] -= (int)(90 + Math.Pow(Stats["Level"], 2) * 10);
            Stats["Level"]++;

            // Warrior
            if (warrior > ranger && warrior > mage)
            {
                Stats["MaxHP"] += 3;
                Stats["Strength"] += 1;
                if (Stats["Level"] % 2 == 0)
                {
                    Stats["Agility"] += 1;
                    Stats["Defense"] += 1;
                }
                if (Stats["Level"] % 5 == 0)
                {
                    Stats["MagicDefense"] += 1;
                    Stats["Magic"] += 1;
                    Stats["MaxMana"] += 2;
                    Stats["Defense"] += 1;
                }
            }
            // Ranger
            else if (ranger > warrior && ranger > mage)
            {
                Stats["MaxHP"] += 2;
                Stats["Agility"] += 2;
                if (Stats["Level"] % 2 == 0)
                {
                    Stats["MagicDefense"] += 1;
                    Stats["MaxMana"] += 1;
                }
                if (Stats["Level"] % 5 == 0)
                {
                    Stats["Strength"] += 2;
                    Stats["Defense"] += 1;
                    Stats["Magic"] += 1;
                }
                if (Stats["Level"] % 10 == 0)
                {
                    Stats["SpellSlots"] += 1;
                    Stats["Defense"] += 1;
                }

            }
            // Mage
            else if (mage > warrior && mage > ranger)
            {
                Stats["MaxHP"] += 1;
                Stats["Magic"] += 1;
                Stats["MaxMana"] += 1;
                if (Stats["Level"] % 2 == 0)
                {
                    Stats["MagicDefense"] += 1;
                    Stats["Magic"] += 1;
                    Stats["MaxMana"] += 2;
                }
                if (Stats["Level"] % 5 == 0)
                {
                    Stats["Agility"] += 2;
                    Stats["Strength"] += 1;
                    Stats["Defense"] += 1;
                    Stats["SpellSlots"] += 1;
                }
            }
            // Guardian
            else if (warrior == ranger && warrior > mage)
            {
                Stats["MaxHP"] += 2;
                Stats["Strength"] += 1;
                Stats["Agility"] += 1;
                if (Stats["Level"] % 2 == 0)
                {
                    Stats["Defense"] += 1;
                    Stats["MaxMana"] += 1;
                }
                if (Stats["Level"] % 5 == 0)
                {
                    Stats["MagicDefense"] += 1;
                    Stats["Magic"] += 1;
                    Stats["Strength"] += 1;
                    Stats["Agility"] += 1;
                }
                if (Stats["Level"] % 20 == 0)
                {
                    Stats["SpellSlots"] += 1;
                    Stats["Defense"] += 1;
                    Stats["MaxHP"] += 1;
                    Stats["MagicDefense"] += 1;
                }
            }
            // Warlock
            else if (warrior == mage && warrior > ranger)
            {
                Stats["MaxHP"] += 2;
                Stats["Magic"] += 1;
                Stats["MaxMana"] += 1;
                if (Stats["Level"] % 2 == 0)
                {
                    Stats["Strength"] += 1;
                    Stats["MagicDefense"] += 1;
                    
                }
                if (Stats["Level"] % 5 == 0)
                {
                    Stats["Defense"] += 2;
                    Stats["Agility"] += 1;
                    Stats["Strength"] += 1;
                }
                if (Stats["Level"] % 10 == 0)
                {
                    Stats["SpellSlots"] += 1;
                    Stats["Magic"] += 1;
                }
            }
            // Wizard
            else if (ranger == mage && ranger > warrior)
            {
                Stats["Magic"] += 1;
                Stats["Agility"] += 1;
                if (Stats["Level"] % 2 == 0)
                {
                    Stats["MaxMana"] += 2;
                    Stats["MaxHP"] += 1;
                    Stats["MagicDefense"] += 2;
                    Stats["Defense"] += 1;
                }
                if (Stats["Level"] % 5 == 0)
                {
                    Stats["Strength"] += 2;
                    Stats["Magic"] += 1;
                    Stats["Agility"] += 1;
                }
                if (Stats["Level"] % 10 == 0)
                {
                    Stats["SpellSlots"] += 1;
                    Stats["Defense"] += 1;
                }
            }
            // Adventurer
            else
            {
                Stats["MaxHP"] += 1;
                Stats["MaxMana"] += 1;
                if (Stats["Level"] % 2 == 0)
                {
                    Stats["Agility"] += 1;
                    Stats["Strength"] += 1;
                    Stats["Magic"] += 1;
                    Stats["Defense"] += 1;
                }
                if (Stats["Level"] % 5 == 0)
                {
                    Stats["MaxHP"] += 2;
                    Stats["MaxMana"] += 1;
                    Stats["Agility"] += 1;
                    Stats["Strength"] += 1;
                    Stats["Magic"] += 1;
                    Stats["MagicDefense"] += 2;
                    Stats["Defense"] += 1;
                }
                if (Stats["Level"] % 10 == 0)
                {
                    Stats["SpellSlots"] += 1;
                    Stats["MagicDefense"] += 1;
                }
            }

            Stats["HP"] = GetStat("MaxHP");
            Stats["Mana"] = GetStat("MaxMana");

            if (Stats["SpellSlots"] > 19) { Stats["SpellSlots"] = 19; }

            bool first = true;


            for (int pos = 0; pos < statnames.Length; pos++)
            {
                if (first)
                {
                    if (Stats[statids[pos]] - oldstats[statids[pos]] > 0)
                    {
                        response += string.Format("+{0} {1}", Stats[statids[pos]] - oldstats[statids[pos]], statnames[pos]);
                        first = false;
                    }
                }
                else
                {
                    if (Stats[statids[pos]] - oldstats[statids[pos]] > 0)
                        response += string.Format(", +{0} {1}", Stats[statids[pos]] - oldstats[statids[pos]], statnames[pos]);
                }
            }

            response += "\n";
            return response;

        }


        protected bool AddTo(Item item)
        {
            if (item == null)
            {
                return true;
            }
            if (item != null)
            {
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
            }
            return false;
        }

        public int GetArrows()
        {
            int result = 0;

            for (int i = 0; i < Inventory.GetLength(1); i++)
            {
                for (int j = 0; j < Inventory.GetLength(0); j++)
                {
                    if (Inventory[j, i] != null)
                    {
                        if (Inventory[j, i].Name.Contains("Arrow"))
                            result += Inventory[j, i].Quantity;
                    }
                }
            }

            return result;
        }

        public Player(string name)
            : base()
        {

            Spells.Add("Search");
            Spells.Add("Focus");

            File = "Graphics/ByteLikeGraphics/Creatures/player";
            File += (rand.Next(3)+1).ToString();
            File += ".png";

            Name = name;
            Stats["HP"] = 20;
            Stats["Mana"] = 10;
            Stats["MaxHP"] = 20;
            Stats["MaxMana"] = 10;

            Stats.Add("XP", 0);
            Stats["Torch"] = 1;
            Stats.Add("SpellSlots", 3);

            bool TorchCheck = false;
            bool WeaponCheck = false;

            for (int i = 0; i < 5; i++)
            {
                if (!TorchCheck && rand.Next(2) == 0)
                {
                    TorchCheck = true;
                    Inventory[i, 1] = new Item(-1, 6);
                }
                else if (!WeaponCheck && rand.Next(2) == 0)
                {
                    WeaponCheck = true;
                    Inventory[i, 1] = new Item(-1, 4);
                }
                else
                    Inventory[i, 1] = new Item(-1, 0);
            }
        }

        // Main Logics
        public override string Logics(ref int[,] level, ref List<Chest> chests, ref List<Effect> effects, ref List<Creature> enemies, ref Player player)
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
                DrawSpellLine = false;
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

            // Actual Movement if not in inventory
            if (!OpenInventory)
            {
                bool movecheck = true;


                if (Keyboard.IsKeyDown(Key.E) && Inventory[3, 0] != null && level[position[0],position[1]] != 15)
                {
                    if (Inventory[3, 0].Name.ToLower().Contains("bow"))
                    {
                        ArrowSlot[0] = 0;
                        ArrowSlot[1] = 0;
                        for (int i = 1; i < Inventory.GetLength(1) && ArrowSlot[0] == 0 && ArrowSlot[1] == 0; i++)
                        {
                            for (int j = 0; j < Inventory.GetLength(0) && ArrowSlot[0] == 0 && ArrowSlot[1] == 0; j++)
                            {
                                if (Inventory[j, i] != null)
                                {
                                    if (Inventory[j, i].Name.Contains("Arrow"))
                                    {
                                        RemSpell = $"Shoot {Inventory[j, i].Name}";
                                        ArrowSlot[0] = j;
                                        ArrowSlot[1] = i;
                                        for (int f = 0; f < 9; f++)
                                        {
                                            if (Inventory[f, 0] != null)
                                            {
                                                switch (Inventory[f, 0].Element)
                                                {
                                                    case 1:
                                                        RemSpell = $"Shoot Fire Arrow";
                                                        break;
                                                    case 2:
                                                        RemSpell = $"Shoot Poison Arrow";
                                                        break;
                                                    case 3:
                                                        RemSpell = $"Shoot Ice Arrow";
                                                        break;
                                                    case 4:
                                                        RemSpell = $"Shoot Lightning Arrow";
                                                        break;
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        if (ArrowSlot[0] != 0 || ArrowSlot[1] != 0)
                        {
                            OpenInventory = true;
                            OpenSpell = true;
                            DrawSpellLine = true;
                            UseMana = false;
                            CurrentSlot[0] = 0;
                            CurrentSlot[1] = 0;
                        }
                    }
                        
                }

                response = WalkTo(new int[] { movement[0], movement[1] }, ref level, response, ref enemies, ref player);
            }
            // if in inventory
            else
            {
                if (!OpenSpell || DistanceBetween(new int[] { position[0], position[1] }, new int[] { position[0] + CurrentSlot[0] + movement[0], position[1] + CurrentSlot[1] + movement[1] }) <= GetStat("Torch"))
                {
                    CurrentSlot[0] += movement[0];
                    CurrentSlot[1] += movement[1];
                }
                // Actual inventory, not a spell usage
                if (!OpenSpell)
                {
                    if (movement[0] != 0 || movement[1] != 0)
                        DrawSpellLine = false;
                    int currentChest = GetChest(ref chests);

                    // Horizontal slot movement
                    if (SelectedSlot[0] < 0)
                    {
                        if (CurrentSlot[0] > Inventory.GetLength(0)+1) { CurrentSlot[0] = 0; }
                        if (CurrentSlot[0] < 0) { CurrentSlot[0] = Inventory.GetLength(0)+1; }
                    }
                    else
                    {
                        if (CurrentSlot[0] >= Inventory.GetLength(0)) { CurrentSlot[0] = 0; }
                        if (CurrentSlot[0] < 0) { CurrentSlot[0] = Inventory.GetLength(0)-1; }
                    }

                    // Vertical slot movement for no chests
                    if (currentChest == -1 && CurrentSlot[0] < Inventory.GetLength(0))
                    {
                        if (CurrentSlot[1] >= Inventory.GetLength(1)) { CurrentSlot[1] = 0; }
                        if (CurrentSlot[1] < 0) { CurrentSlot[1] = Inventory.GetLength(1) - 1; }
                    }
                    // Vertical slot movement for a spot with a chest on it
                    else if (CurrentSlot[0] < Inventory.GetLength(0))
                    {
                        if (CurrentSlot[1] >= Inventory.GetLength(1) + chests[currentChest].Inventory.GetLength(1)) { CurrentSlot[1] = 0; }
                        if (CurrentSlot[1] < 0) { CurrentSlot[1] = Inventory.GetLength(1) + chests[currentChest].Inventory.GetLength(1) - 1; }
                    }
                    // Vertical slot movement for spell slots
                    else
                    {
                        if (CurrentSlot[1] >= Stats["SpellSlots"]) { CurrentSlot[1] = 0; }
                        if (CurrentSlot[1] < 0) { CurrentSlot[1] = Stats["SpellSlots"]-1; }
                    }


                    // Inventory names
                    if (CurrentSlot[1] < Inventory.GetLength(1) && CurrentSlot[0] < Inventory.GetLength(0))
                    {
                        if (Inventory[CurrentSlot[0], CurrentSlot[1]] != null)
                        {
                            response = Inventory[CurrentSlot[0], CurrentSlot[1]].Name + "\n";
                            response += Inventory[CurrentSlot[0], CurrentSlot[1]].Description;
                        }
                    }
                    // Chest names
                    else if (CurrentSlot[0] < Inventory.GetLength(0))
                    {
                        if (chests[currentChest].Inventory[CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1)] != null)
                        {
                            response = chests[currentChest].Inventory[CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1)].Name + "\n";
                            response += chests[currentChest].Inventory[CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1)].Description;
                        }
                    }
                    // Spell names
                    else
                    {
                        if (CurrentSlot[1] < Spells.Count)
                            response += GetSpellDescription(Spells.ElementAt(CurrentSlot[1]));
                    }




                    if (Keyboard.IsKeyDown(Key.E))
                    {
                        // if we're in normal inventory
                        if (CurrentSlot[0] < Inventory.GetLength(0))
                        {
                            // if we have something selected
                            if (SelectedSlot[0] >= 0)
                            {
                                // Regular inventory, aka not equipment
                                if (CurrentSlot[1] != 0 && SelectedSlot[1] != 0)
                                {


                                    // Our inventory, aka not interacting with a chest
                                    if (CurrentSlot[1] < Inventory.GetLength(1) && SelectedSlot[1] < Inventory.GetLength(1))
                                    {
                                        // Swap

                                        // if we're not on the same item as we selected
                                        if (CurrentSlot[0] != SelectedSlot[0] || CurrentSlot[1] != SelectedSlot[1])
                                        {
                                            // If selected item is not null, aka trying to put something in our non equipment slots
                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]] != null)
                                            {
                                                // Stackables
                                                if (Inventory[SelectedSlot[0], SelectedSlot[1]].GearType > 9 || Inventory[SelectedSlot[0], SelectedSlot[1]].GearType == 0)
                                                {
                                                    // Check if we're trying to put an item in an empty slot or no
                                                    if (Inventory[CurrentSlot[0], CurrentSlot[1]] != null)
                                                    {
                                                        // If they're the same stackable item, combine
                                                        if (Inventory[CurrentSlot[0], CurrentSlot[1]].Name == Inventory[SelectedSlot[0], SelectedSlot[1]].Name)
                                                        {
                                                            Inventory[CurrentSlot[0], CurrentSlot[1]].Quantity += Inventory[SelectedSlot[0], SelectedSlot[1]].Quantity;
                                                            Inventory[SelectedSlot[0], SelectedSlot[1]] = null;
                                                        }
                                                        // if not the same stackable item, swap
                                                        else
                                                        {
                                                            Item temp2 = Inventory[CurrentSlot[0], CurrentSlot[1]];
                                                            Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];
                                                            Inventory[SelectedSlot[0], SelectedSlot[1]] = temp2;
                                                        }
                                                    }
                                                    // if it's an empty slot just put, whichever selected item we have, in it
                                                    else
                                                    {
                                                        Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];
                                                        Inventory[SelectedSlot[0], SelectedSlot[1]] = null;
                                                    }
                                                }
                                                // Non-stackables, just plain swap
                                                else
                                                {
                                                    Item temp = Inventory[CurrentSlot[0], CurrentSlot[1]];
                                                    Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];

                                                    Inventory[SelectedSlot[0], SelectedSlot[1]] = temp;
                                                }
                                            }
                                            // If selected item is null (we're in regular non-gear inventory), swap with the null (who cares)
                                            else
                                            {
                                                Item temp = Inventory[CurrentSlot[0], CurrentSlot[1]];
                                                Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];

                                                Inventory[SelectedSlot[0], SelectedSlot[1]] = temp;
                                            }
                                        }
                                        // End Swap

                                        // Gathering all items with double clicks
                                        else if (Inventory[CurrentSlot[0], CurrentSlot[1]] != null)
                                        {
                                            if (Inventory[CurrentSlot[0], CurrentSlot[1]].GearType > 9 || Inventory[CurrentSlot[0], CurrentSlot[1]].GearType == 0)
                                            {
                                                for (int i = 0; i < Inventory.GetLength(1); i++)
                                                {
                                                    for (int j = 0; j < Inventory.GetLength(0); j++)
                                                    {
                                                        if (Inventory[j, i] != null)
                                                        {
                                                            if (Inventory[j, i].Name == Inventory[CurrentSlot[0], CurrentSlot[1]].Name && (CurrentSlot[0] != j || CurrentSlot[1] != i))
                                                            {
                                                                Inventory[CurrentSlot[0], CurrentSlot[1]].Quantity += Inventory[j, i].Quantity;
                                                                Inventory[j, i] = null;
                                                            }
                                                        }
                                                    }
                                                }

                                                if (GetChest(ref chests) >= 0)
                                                {
                                                    for (int i = 0; i < chests[GetChest(ref chests)].Inventory.GetLength(1); i++)
                                                    {
                                                        for (int j = 0; j < chests[GetChest(ref chests)].Inventory.GetLength(0); j++)
                                                        {
                                                            if (chests[GetChest(ref chests)].Inventory[j, i] != null)
                                                            {
                                                                if (chests[GetChest(ref chests)].Inventory[j, i].Name == Inventory[CurrentSlot[0], CurrentSlot[1]].Name)
                                                                {
                                                                    Inventory[CurrentSlot[0], CurrentSlot[1]].Quantity += chests[GetChest(ref chests)].Inventory[j, i].Quantity;
                                                                    chests[GetChest(ref chests)].Inventory[j, i] = null;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        // End gathering

                                    }
                                    // if we selected something from a chest and trying to put it in our inventory
                                    else if (CurrentSlot[1] < Inventory.GetLength(1))
                                    {
                                        Inventory[CurrentSlot[0], CurrentSlot[1]] = chests[currentChest].TakeOut(Inventory[CurrentSlot[0], CurrentSlot[1]], new int[] { SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1) });
                                    }
                                    // If we selected something from our inventory and trying to put it in a chest
                                    else if (SelectedSlot[1] < Inventory.GetLength(1))
                                    {
                                        Inventory[SelectedSlot[0], SelectedSlot[1]] = chests[currentChest].PutIn(Inventory[SelectedSlot[0], SelectedSlot[1]], new int[] { CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1) });
                                    }
                                    // Otherwise, swap items inside the chest (not selected something in inventory nor currently on something in inventory)
                                    else
                                    {
                                        chests[currentChest].Swap(new int[] { CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1) }, new int[] { SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1) });
                                    }


                                    // Reset selection to none
                                    SelectedSlot[0] = -100;


                                }
                                // non Equipment


                                // if selected something from equipment
                                else if (SelectedSlot[1] == 0)
                                {
                                    // if inside our inventory, otherwise do nothing
                                    if (CurrentSlot[1] < Inventory.GetLength(1))
                                    {
                                        // If we are trying to swap an equipment item with something else
                                        if (Inventory[CurrentSlot[0], CurrentSlot[1]] == null)
                                        {
                                            // Check for compatibility
                                            if (CurrentSlot[1] != 0 || (CurrentSlot[0] == 7 && SelectedSlot[0] == 8) || (CurrentSlot[0] == 8 && SelectedSlot[0] == 7))
                                            {
                                                Item temp = Inventory[CurrentSlot[0], CurrentSlot[1]];
                                                Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];
                                                Inventory[SelectedSlot[0], SelectedSlot[1]] = temp;
                                                SelectedSlot[0] = -100;
                                            }
                                            // if we're trying to switch equipment between eachother, prevent
                                            else if (CurrentSlot[0] < 9)
                                            {
                                                response += $"{Name}: I can't put that there.\n";
                                            }
                                            // Trying to use equipment
                                            else if (CurrentSlot[0] == 9 && Inventory[SelectedSlot[0], SelectedSlot[1]] != null)
                                            {
                                                response += $"{Name}: I can't directly use that.\n";
                                            }
                                            // Trying to drop equipment
                                            else
                                            {
                                                // if there's a chest, put in the chest
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
                                                // no chest - create one
                                                else
                                                {
                                                    Chest temp = new Chest(new int[] { position[0], position[1] }, -1);
                                                    temp.AddTo(Inventory[SelectedSlot[0], SelectedSlot[1]]);
                                                    chests.Add(temp);
                                                    Inventory[SelectedSlot[0], SelectedSlot[1]] = null;
                                                }
                                                // reset selection
                                                SelectedSlot[0] = -100;
                                            }
                                        }
                                        // if trying to swap equipment between eachother
                                        // if somehow fits
                                        else if (Inventory[CurrentSlot[0], CurrentSlot[1]].GearType == SelectedSlot[0] + 1 || (SelectedSlot[0] == 8 && Inventory[CurrentSlot[0], CurrentSlot[1]].GearType == SelectedSlot[0]))
                                        {
                                            Item temp = Inventory[CurrentSlot[0], CurrentSlot[1]];
                                            Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];
                                            Inventory[SelectedSlot[0], SelectedSlot[1]] = temp;
                                            SelectedSlot[0] = -100;
                                        }
                                        // if doesn't fit
                                        else
                                        {
                                            response += $"{Name}: I can't put that there.\n";
                                        }
                                    }
                                    // if we're trying to put an item from equipment into a chest, prevent
                                    // just too lazy to make a bilion checks for gear types when switching equipment from a chest
                                    else
                                    {
                                        if (chests[GetChest(ref chests)].Inventory[CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1)] == null || (chests[GetChest(ref chests)].Inventory[CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1)].GearType == SelectedSlot[0] + 1 || chests[GetChest(ref chests)].Inventory[CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1)].GearType == SelectedSlot[0] && chests[GetChest(ref chests)].Inventory[CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1)].GearType == 8))
                                        {
                                            Inventory[SelectedSlot[0], SelectedSlot[1]] = chests[GetChest(ref chests)].PutIn(Inventory[SelectedSlot[0], SelectedSlot[1]], new int[] { CurrentSlot[0], CurrentSlot[1] - Inventory.GetLength(1) });
                                            SelectedSlot[0] = -100;
                                        }
                                        else
                                        response += $"{Name}: I need to take it off first.\n";
                                    }
                                }

                                // if we're currently on something from equipment, not selecting something from equipment
                                else if (CurrentSlot[1] == 0)
                                {
                                    // if selected something from our inventory, not equipment
                                    if (SelectedSlot[1] < Inventory.GetLength(1))
                                    {
                                        // if trying to put some new equipment on
                                        if (CurrentSlot[0] < 9)
                                        {
                                            // if trying to put "nothing" on ourselves, just take it off
                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]] == null)
                                            {
                                                Item temp = Inventory[CurrentSlot[0], CurrentSlot[1]];
                                                Inventory[CurrentSlot[0], CurrentSlot[1]] = Inventory[SelectedSlot[0], SelectedSlot[1]];
                                                Inventory[SelectedSlot[0], SelectedSlot[1]] = temp;
                                                SelectedSlot[0] = -100;
                                            }
                                            // if it's not nothing we're putting on, check for compatability
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
                                        // Trying to use some item, not equipment
                                        else if (CurrentSlot[0] == 9)
                                        {
                                            // Use only if not null
                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]] != null)
                                            {
                                                // If not a usable (GearType 10), don't use
                                                if (Inventory[SelectedSlot[0], SelectedSlot[1]].GearType != 10)
                                                {
                                                    response += $"{Name}: I can't directly use that.\n";
                                                }
                                                // if it is type 10, use it
                                                else
                                                {
                                                    // Arrows/Scrolls, aka using spells inside items
                                                    if (Inventory[SelectedSlot[0], SelectedSlot[1]].Name.Contains("Arrow") || Inventory[SelectedSlot[0], SelectedSlot[1]].Name.Contains("Scroll"))
                                                    {

                                                        bool bowcheck = false;
                                                        if (Inventory[3, 0] != null)
                                                        {
                                                            if (Inventory[3, 0].Name.ToLower().Contains("bow"))
                                                                bowcheck = true;
                                                        }

                                                        if (!Inventory[SelectedSlot[0], SelectedSlot[1]].Name.Contains("Arrow") || bowcheck)
                                                        {
                                                            CurrentSlot[0] = 0;
                                                            CurrentSlot[1] = 0;
                                                            OpenSpell = true;
                                                            UseMana = false;
                                                            RemSpell = Inventory[SelectedSlot[0], SelectedSlot[1]].Spell;
                                                            DrawSpellLine = isALineSpell(RemSpell);
                                                            ArrowSlot[0] = SelectedSlot[0];
                                                            ArrowSlot[1] = SelectedSlot[1];
                                                        }
                                                        else
                                                        {
                                                            response += $"{Name}: I need a bow to use that.\n";
                                                        }
                                                    }
                                                    // Learning new spells
                                                    else if (Inventory[SelectedSlot[0], SelectedSlot[1]].Name.Contains("Book"))
                                                    {
                                                        if (Stats["SpellSlots"] > Spells.Count)
                                                        {
                                                            Spells.Add(Inventory[SelectedSlot[0], SelectedSlot[1]].Spell);
                                                            Inventory[SelectedSlot[0], SelectedSlot[1]].Quantity--;
                                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]].Quantity <= 0)
                                                                Inventory[SelectedSlot[0], SelectedSlot[1]] = null;
                                                            SelectedSlot[0] = -100;
                                                        }
                                                        else
                                                        {
                                                            response += $"{Name}: I can't remember more spells.\n";
                                                        }
                                                    }
                                                    // Healing items
                                                    else
                                                    {
                                                        if (Inventory[SelectedSlot[0], SelectedSlot[1]].Name == "Improvement Potion")
                                                        {
                                                            Statuses[0] = 0;
                                                            Statuses[1] = 0;
                                                            Statuses[2] = 0;
                                                            Statuses[3] = 0;
                                                        }
                                                        else
                                                        {
                                                            Stats["HP"] += Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["MaxHP"];
                                                            Stats["Mana"] += Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["MaxMana"];

                                                            if (Stats["HP"] > GetStat("MaxHP"))
                                                                Stats["HP"] = GetStat("MaxHP");
                                                            if (Stats["Mana"] > GetStat("MaxMana"))
                                                                Stats["Mana"] = GetStat("MaxMana");

                                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Strength"] > 0)
                                                            {
                                                                if (Buffs["Strength"] > 0)
                                                                    BuffLevels["Strength"] = (int)((BuffLevels["Strength"] + Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Strength"]) / 2);
                                                                else
                                                                    BuffLevels["Strength"] = Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Strength"];
                                                                Buffs["Strength"] = 100;
                                                            }

                                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Agility"] > 0)
                                                            {
                                                                if (Buffs["Agility"] > 0)
                                                                    BuffLevels["Agility"] = (int)((BuffLevels["Agility"] + Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Agility"]) / 2);
                                                                else
                                                                    BuffLevels["Agility"] = Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Agility"];
                                                                Buffs["Agility"] = 100;
                                                            }

                                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Magic"] > 0)
                                                            {
                                                                if (Buffs["Magic"] > 0)
                                                                    BuffLevels["Magic"] = (int)((BuffLevels["Magic"] + Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Magic"]) / 2);
                                                                else
                                                                    BuffLevels["Magic"] = Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Magic"];
                                                                Buffs["Magic"] = 100;
                                                            }

                                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Defense"] > 0)
                                                            {
                                                                if (Buffs["Defense"] > 0)
                                                                    BuffLevels["Defense"] = (int)((BuffLevels["Defense"] + Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Defense"]) / 2);
                                                                else
                                                                    BuffLevels["Defense"] = Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Defense"];
                                                                Buffs["Defense"] = 100;
                                                            }

                                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["MagicDefense"] > 0)
                                                            {
                                                                if (Buffs["MagicDefense"] > 0)
                                                                    BuffLevels["MagicDefense"] = (int)((BuffLevels["MagicDefense"] + Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["MagicDefense"]) / 2);
                                                                else
                                                                    BuffLevels["MagicDefense"] = Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["MagicDefense"];
                                                                Buffs["MagicDefense"] = 100;
                                                            }
                                                        }
                                                        Inventory[SelectedSlot[0], SelectedSlot[1]].Quantity--;
                                                        if (Inventory[SelectedSlot[0], SelectedSlot[1]].Quantity <= 0)
                                                            Inventory[SelectedSlot[0], SelectedSlot[1]] = null;
                                                        SelectedSlot[0] = -100;
                                                    }
                                                }
                                            }
                                        }
                                        // If throwing non-equipment out
                                        else if (CurrentSlot[0] == 10)
                                        {
                                            // if there's a chest, put in the chest
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
                                            // No chest - create one
                                            else
                                            {
                                                Chest temp = new Chest(new int[] { position[0], position[1] }, -1);
                                                temp.AddTo(Inventory[SelectedSlot[0], SelectedSlot[1]]);
                                                chests.Add(temp);
                                                Inventory[SelectedSlot[0], SelectedSlot[1]] = null;
                                            }
                                            // Reset selection
                                            SelectedSlot[0] = -100;
                                        }
                                    }
                                    // if selected something from a chest and trying to put it into equipment
                                    else
                                    {
                                        if (chests[GetChest(ref chests)].Inventory[SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1)] == null || (chests[GetChest(ref chests)].Inventory[SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1)].GearType == CurrentSlot[0] + 1 || chests[GetChest(ref chests)].Inventory[SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1)].GearType == CurrentSlot[0] && chests[GetChest(ref chests)].Inventory[SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1)].GearType == 8))
                                        {
                                            Inventory[CurrentSlot[0], CurrentSlot[1]] = chests[GetChest(ref chests)].TakeOut(Inventory[CurrentSlot[0], CurrentSlot[1]], new int[] { SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1) });
                                            SelectedSlot[0] = -100;
                                        }
                                        else
                                        response += $"{Name}: I need to take it out first.\n";
                                    }
                                }

                            }
                            // if we don't have something selected
                            else
                            {
                                // if we're not selecting slots 9 (use item slot) or slot 10 (place down slot), select current slot
                                if (CurrentSlot[1] != 0 || CurrentSlot[0] < 9)
                                {
                                    SelectedSlot[0] = CurrentSlot[0];
                                    SelectedSlot[1] = CurrentSlot[1];
                                }
                                // if there's a chest and we're pressing the put down slot, take everything from the chest
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
                                // DEBUG, REMOVE LATER
                                else { Stats["XP"] += 10000; }
                            }


                        }
                        // Spell slot activation
                        else if (CurrentSlot[0] == Inventory.GetLength(0))
                        {
                            if (CurrentSlot[1] < Spells.Count)
                            {
                                OpenSpell = true;
                                UseMana = true;
                                RemSpell = Spells[CurrentSlot[1]];
                                DrawSpellLine = isALineSpell(RemSpell);
                                CurrentSlot[0] = 0;
                                CurrentSlot[1] = 0;
                                ArrowSlot[0] = 0;
                                ArrowSlot[1] = 0;
                            }
                        }
                        // Deleting slots
                        else
                        {
                            switch (DrawSpellLine)
                            {
                                case true:
                                    DrawSpellLine = false;
                                    if (Spells.Count > CurrentSlot[1])
                                        Spells.RemoveAt(CurrentSlot[1]);
                                    break;
                                case false:
                                    response += "Press [E] again to delete spell\n";
                                    DrawSpellLine = true;
                                    break;
                            }
                        }

                    }
                }
                // End actual inventory

                // While using a spell, CHANGE CODE LATER
                else
                {
                    if (Keyboard.IsKeyDown(Key.E))
                    {
                        if (CurrentSlot[0] == 0 && CurrentSlot[1] == 0 && DrawSpellLine)
                        {
                            OpenSpell = false;
                            if (SelectedSlot[0] >= 0)
                            {
                                CurrentSlot[0] = SelectedSlot[0];
                                CurrentSlot[1] = SelectedSlot[1];
                            }
                        }
                        else
                        {
                            if (Stats["Mana"] >= GetSpellCost(RemSpell) || !UseMana)
                            {
                                if (ArrowSlot[0] != 0 || ArrowSlot[1] != 0)
                                {
                                    Inventory[ArrowSlot[0], ArrowSlot[1]].Quantity--;
                                    if (Inventory[ArrowSlot[0], ArrowSlot[1]].Quantity <= 0)
                                        Inventory[ArrowSlot[0], ArrowSlot[1]] = null;
                                }

                                if (UseMana)
                                    Stats["Mana"] -= GetSpellCost(RemSpell);
                                if (!RemSpell.Contains("Arrow"))
                                {
                                    response += $"{Name} used {RemSpell}!\n";
                                    effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { CurrentSlot[0], CurrentSlot[1] }, GetStat("Magic"), RemSpell));
                                }
                                else
                                {
                                    effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { CurrentSlot[0], CurrentSlot[1] }, GetStat("Agility"), RemSpell));
                                }
                                OpenInventory = false;
                                OpenSpell = false;
                            }
                            else
                            {
                                response += "You don't have enough mana!\n";
                            }
                        }
                    }
                }

            }
            // end inventory

            //

            if (!OpenInventory)
            {
                response = Conditions(response);
                if (Stats["HP"] > GetStat("MaxHP")) { Stats["HP"] = GetStat("MaxHP"); }
                if (Stats["Mana"] > GetStat("MaxMana")) { Stats["Mana"] = GetStat("MaxMana"); }
            }



            // Death
            if (wasAlive && Stats["HP"] <= 0) { response += $"{Name} dies!\n"; }

            // closing inventory
            if (Keyboard.IsKeyDown(Key.Q) && OpenInventory && !invCheck)
            {
                DrawSpellLine = false;
                OpenInventory = false;
                OpenSpell = false;
                if (SelectedSlot[0] > 0 && SelectedSlot[1] < Inventory.GetLength(1))
                {
                    CurrentSlot[0] = SelectedSlot[0];
                    CurrentSlot[1] = SelectedSlot[1];
                }
                else if (CurrentSlot[1] > Inventory.GetLength(1))
                {
                    CurrentSlot[1] = 0;
                }

                SelectedSlot[0] = -100;
            }

            // leveling up
            if (Stats["XP"] >= (int)(90 + Math.Pow(Stats["Level"], 2) * 10))
                response += LevelUp();




            return response;

        }
        //

        public new int GetStat(string index)
        {
            int current = Stats[index];
            for (int i = 0; i < 9; i++)
            {
                if (!OpenInventory || CurrentSlot[1] == 0 || OpenSpell || CurrentSlot[1] >= Inventory.GetLength(1) || CurrentSlot[0] >= Inventory.GetLength(0))
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


        bool isALineSpell(string spell)
        {
            switch (spell)
            {
                case "Shoot Arrow":
                case "Shoot Fire Arrow":
                case "Shoot Poison Arrow":
                case "Shoot Ice Arrow":
                case "Shoot Lightning Arrow":
                case "Focus":
                    return true;
                default:
                    return false;
            }
        }

    }


    public class Critter : Creature
    {
        bool Aggressive = false;
        public Critter(int floor, int[] pos)
            :base()
        {
            position[0] = pos[0];
            position[1] = pos[1];
            File = "Graphics/ByteLikeGraphics/Creatures/enemycritter";

            double statModifier = 1;

            floor += rand.Next(-5, 10);

            
            Stats["Level"] = floor;
            if (Stats["Level"] <= 0)
                Stats["Level"] = 1;

            if ((floor / 23) >= 4)
            {
                File += "3.png";
                Stats["HPRegen"] += 1;
                Stats["ManaRegen"] += 1;
                statModifier = 1.25;
                Name = "Giant Rat";
            }
            else if ((floor / 23) >= 2)
            {
                File += "2.png";
                Stats["HPRegen"] += 1;
                Name = "Smart Rat";
            }
            else
            {
                File += "1.png";
                statModifier = 0.75;
                Name = "Rat";
            }

            Stats["MaxHP"] = (int)(((floor / 1.25) + 10)*statModifier);
            Stats["HP"] = Stats["MaxHP"];
            Stats["MaxMana"] = (int)(((floor / 8) + 10) * statModifier);
            Stats["Mana"] = Stats["MaxMana"];
            Stats["Strength"] = (int)(((floor / 4) + 3) * statModifier);
            Stats["Agility"] = (int)(((floor / 8) + 3) * statModifier);
            Stats["Magic"] = (int)(((floor / 7) + 4) * statModifier);
            Stats["Defense"] = (int)(((floor / 10) + 4) * statModifier);
            Stats["MagicDefense"] = (int)(((floor / 12) + 4) * statModifier);

        }

        public override string Logics(ref int[,] level, ref List<Chest> chests, ref List<Effect> effects, ref List<Creature> enemies, ref Player player)
        {
            string response = "";

            if (DistanceBetween(new int[] { position[0], position[1] }, new int[] { player.position[0], player.position[1] }) <= GetStat("Torch") && !Aggressive)
            {
                Aggressive = true;
                response = $"{Name} notices {player.Name}!\n";
            }

            int movementdirection = rand.Next(4) * 90;

            if (Aggressive)
            {
                movementdirection = FindDirection(new int[] { player.position[0], player.position[1] }, ref level);
            }

            int[] movement = new int[2];
            switch (movementdirection)
            {
                case 0:
                    movement[0] = 1;
                    movement[1] = 0;
                    break;
                case 180:
                    movement[0] = -1;
                    movement[1] = 0;
                    break;
                case 270:
                    movement[0] = 0;
                    movement[1] = 1;
                    break;
                case 90:
                    movement[0] = 0;
                    movement[1] = -1;
                    break;
            }

            if (Statuses[2] != 0 || Statuses[3] % 2 != 0 || Stats["HP"] <= 0)
            {
                movement[0] = 0;
                movement[1] = 0;
            }
            response += WalkTo(new int[] { movement[0], movement[1] }, ref level, response, ref enemies, ref player);

            response = Conditions(response);

            if (Stats["HP"] > GetStat("MaxHP")) { Stats["HP"] = GetStat("MaxHP"); }
            if (Stats["Mana"] > GetStat("MaxMana")) { Stats["Mana"] = GetStat("MaxMana"); }

            if (Stats["HP"] < GetStat("MaxHP"))
                response += $"{Name} has {Stats["HP"]} HP left!\n";

            if (DistanceBetween(new int[] { position[0], position[1] }, new int[] { player.position[0], player.position[1] }) <= player.GetStat("Torch") + 1)
                return response;
            else return "";
        }


        int FindDirection(int[] target, ref int[,] level)
        {
            int[] movement = new int[2] { 999, 999 };
            int direction = -1;

            List<int[]> movements = new List<int[]>();

            if (position[0] + 1 < level.GetLength(0))
            {
                if (!IsGhost || (level[position[0] + 1, position[1]] != 2 && level[position[0] + 1, position[1]] != 0 && level[position[0] + 1, position[1]] != 5))
                    movements.Add(new int[] { DifferenceBetween(position[0] + 1, target[0]), DifferenceBetween(position[1], target[1]), 0 });
            }
            if (position[0] - 1 >= 0)
            {
                if (!IsGhost || (level[position[0] - 1, position[1]] != 2 && level[position[0] - 1, position[1]] != 0 && level[position[0] - 1, position[1]] != 5))
                    movements.Add(new int[] { DifferenceBetween(position[0] - 1, target[0]), DifferenceBetween(position[1], target[1]), 180 });
            }

            if (position[1] + 1 < level.GetLength(1))
            {
                if (!IsGhost || (level[position[0], position[1] + 1] != 2 && level[position[0], position[1] + 1] != 0 && level[position[0], position[1] + 1] != 5))
                    movements.Add(new int[] { DifferenceBetween(position[0], target[0]), DifferenceBetween(position[1] + 1, target[1]), 270 });
            }
            if (position[1] - 1 >= 0)
            {
                if (!IsGhost || (level[position[0], position[1] - 1] != 2 && level[position[0], position[1] - 1] != 0 && level[position[0], position[1] - 1] != 5))
                    movements.Add(new int[] { DifferenceBetween(position[0], target[0]), DifferenceBetween(position[1] - 1, target[1]), 90 });
            }

            foreach (int[] item in movements)
            {
                if (item[0] + item[1] < movement[0] + movement[1])
                {
                    movement[0] = item[0];
                    movement[1] = item[1];
                    direction = item[2];
                }
                else if (item[0] + item[1] == movement[0] + movement[1] && rand.Next(2) == 0)
                {
                    movement[0] = item[0];
                    movement[1] = item[1];
                    direction = item[2];
                }
            }

            return direction;
        }

        int DifferenceBetween(int point1, int point2)
        {
            if (point1 > point2)
                return (point1 - point2);
            else return (point2 - point1);
        }
    }

    

}
