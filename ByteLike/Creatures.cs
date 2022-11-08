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
        public bool DrawEquipment = false;
        public bool DrawHealthbar = true;
        public bool DropEquipment = false;

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

        public abstract string Logics(ref int[,] level, ref List<Chest> chests, ref List<Effect> effects, ref List<Creature> enemies, ref Player player, ref int[,] darkness, out string currentSound);

        public int TakeDamage(int damage, int type)
        {
            int result = 0;

            int defense = GetStat("Defense");
            if (type > 0)
                defense = GetStat("MagicDefense");

            damage -= (int)(defense / Math.Sqrt(Math.Sqrt(defense)));

            if (damage <= 0)
                damage = 1;

            Stats["HP"] -= damage;

            if (type > 0 && type < 5)
                Potentials[type - 1] += 6;
            else if (type > 0 && type < 9)
                Potentials[type - 5] += 4;
            else if (type > 0)
                Potentials[type - 9] += 2;

            if (Stats["HP"] <= 0)
                result = Stats["Level"] * rand.Next(2, 10) + 1;

            return result;
        }

        public static string GetRandomSpell(int? modifier)
        {
            string response = "Nothing";
            int spellthing = rand.Next(54);

            if (modifier != null)
            {
                int min = (int)modifier - 25;
                if (min < 1)
                    min = 1;
                int max = (int)modifier + 25;
                if (max > 54)
                    max = 54;
                spellthing = rand.Next(min,max);
            }

            switch (spellthing + 5)
            {
                case 5:
                    response = "Nothing";
                    break;
                case 6:
                    response = "Search";
                    break;
                case 7:
                    response = "Focus";
                    break;
                case 8:
                    response = "Ember";
                    break;
                case 9:
                    response = "Ice Shard";
                    break;
                case 10:
                    response = "Zap";
                    break;
                case 11:
                    response = "Posion Sting";
                    break;
                case 12:
                    response = "Recover";
                    break;
                case 13:
                    response = "Ironize";
                    break;
                case 14:
                    response = "Enchant";
                    break;
                case 15:
                    response = "Corrode Armor";
                    break;
                case 16:
                    response = "Sharpen";
                    break;
                case 17:
                    response = "Enlighten";
                    break;
                case 18:
                    response = "Prepare";
                    break;
                case 19:
                    response = "Energy Drain";
                    break;
                case 20:
                    response = "Confuse";
                    break;
                case 21:
                    response = "Scare";
                    break;
                case 22:
                    response = "Fireball";
                    break;
                case 23:
                    response = "Ice Storm";
                    break;
                case 24:
                    response = "Electro Bolt";
                    break;
                case 25:
                    response = "Sludge Bomb";
                    break;
                case 26:
                    response = "Heal Wounds";
                    break;
                case 27:
                    response = "Protective Field";
                    break;
                case 28:
                    response = "Regenerate";
                    break;
                case 29:
                    response = "Melt Armor";
                    break;
                case 30:
                    response = "Enchanse Vission";
                    break;
                case 31:
                    response = "Liquify";
                    break;
                case 32:
                    response = "Lavafy";
                    break;
                case 33:
                    response = "Ivy Growth";
                    break;
                case 34:
                    response = "Charge";
                    break;
                case 35:
                    response = "Rage";
                    break;
                case 36:
                    response = "Speed Up";
                    break;
                case 37:
                    response = "Concentrate";
                    break;
                case 38:
                    response = "Weaken";
                    break;
                case 39:
                    response = "Slow Down";
                    break;
                case 40:
                    response = "Terrify";
                    break;
                case 41:
                    response = "Meteor";
                    break;
                case 42:
                    response = "Blizzard";
                    break;
                case 43:
                    response = "Thunder";
                    break;
                case 44:
                    response = "Plague Bomb";
                    break;
                case 45:
                    response = "Restore";
                    break;
                case 46:
                    response = "Full Protection";
                    break;
                case 47:
                    response = "Tsunami";
                    break;
                case 48:
                    response = "Erruption";
                    break;
                case 49:
                    response = "Forest Growth";
                    break;
                case 50:
                    response = "Electrify";
                    break;
                case 51:
                    response = "Manafy";
                    break;
                case 52:
                    response = "Destroy Armor";
                    break;
                case 53:
                    response = "Demonify";
                    break;
                case 54:
                    response = "Featherify";
                    break;
                case 55:
                    response = "Transcend";
                    break;
                case 56:
                    response = "Wither";
                    break;
                case 57:
                    response = "Chain Up";
                    break;
                case 58:
                    response = "Hypnotize";
                    break;
            }

            return response;
        }
        public static string GetSpellDescription(string spell)
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
                case "Focus":
                    response = "Shoots a random magical projectile in specified direction. 5 Mana\n";
                    break;
                case "Ember":
                    response = "Shoots an ember in a specified direction, burn potential. 3 Mana\n";
                    break;
                case "Ice Shard":
                    response = "Shoots a shard of ice in a specified direction, freeze potential. 3 Mana\n";
                    break;
                case "Zap":
                    response = "Shoots a lightning bold in a specified direction, paralysis potential. 3 Mana\n";
                    break;
                case "Posion Sting":
                    response = "Shoots a poison blob in a specified direction, poison potential. 3 Mana\n";
                    break;
                case "Recover":
                    response = "Regenerates 10 HP. 10 Mana\n";
                    break;
                case "Ironize":
                    response = "Increases Defense by 5. 10 Mana\n";
                    break;
                case "Enchant":
                    response = "Increases Magic Defense by 5. 10 Mana\n";
                    break;
                case "Corrode Armor":
                    response = "Decreases Defense and Magic Defense by 3. 10 Mana\n";
                    break;
                case "Sharpen":
                    response = "Increases Strength by 3. 10 Mana\n";
                    break;
                case "Enlighten":
                    response = "Increases Agility by 3. 10 Mana\n";
                    break;
                case "Prepare":
                    response = "Increases Magic by 3. 10 Mana\n";
                    break;
                case "Energy Drain":
                    response = "Decreases Strength by 2. 10 Mana\n";
                    break;
                case "Confuse":
                    response = "Decreases Agility by 2. 10 Mana\n";
                    break;
                case "Scare":
                    response = "Decreases Magic by 2. 10 Mana\n";
                    break;
                case "Fireball":
                    response = "Shoots an explosive fireball in a specified direction. 5 Mana\n";
                    break;
                case "Ice Storm":
                    response = "Shoots a wide ice flow in a specified direction. 5 Mana\n";
                    break;
                case "Electro Bolt":
                    response = "Shoots a wide bolt of electricity in a specified direction. 5 Mana\n";
                    break;
                case "Sludge Bomb":
                    response = "Shoots an explosive ball of poison in a specified direction. 5 Mana\n";
                    break;
                case "Heal Wounds":
                    response = "Heals 20 hp. 15 Mana\n";
                    break;
                case "Protective Field":
                    response = "Increases Defense and Magic Defense by 7. 15 Mana\n";
                    break;
                case "Regenerate":
                    response = "Increases HP and Mana regeneration by 1. 10 Mana\n";
                    break;
                case "Melt Armor":
                    response = "Decreases Defense and Magic Defense by 6. 15 Mana\n";
                    break;
                case "Enchanse Vission":
                    response = "Increases Light by 2. 10 Mana\n";
                    break;
                case "Liquify":
                    response = "Creates a spot of water tiles. 10 Mana\n";
                    break;
                case "Lavafy":
                    response = "Creates a spot of lava tiles. 10 Mana\n";
                    break;
                case "Ivy Growth":
                    response = "Creates a spot of poison ivy. 10 Mana\n";
                    break;
                case "Charge":
                    response = "Creates a spot of electrified terrain. 10 Mana\n";
                    break;
                case "Rage":
                    response = "Increases Strength by 7. 15 Mana\n";
                    break;
                case "Speed Up":
                    response = "Increases Agility by 7. 15 Mana\n";
                    break;
                case "Concentrate":
                    response = "Increases Magic by 7. 15 Mana\n";
                    break;
                case "Weaken":
                    response = "Decreases Strength by 5. 15 Mana\n";
                    break;
                case "Slow Down":
                    response = "Decreases Agility by 5. 15 Mana\n";
                    break;
                case "Terrify":
                    response = "Decreases Magic by 5. 15 Mana\n";
                    break;
                case "Meteor":
                    response = "Shoots a large explosive fireball in a specified direction. 7 Mana\n";
                    break;
                case "Blizzard":
                    response = "Shoots a really wide ice storm in a specified direction. 7 Mana\n";
                    break;
                case "Thunder":
                    response = "Shoots a really wide thunder storm in a specified direction. 7 Mana\n";
                    break;
                case "Plague Bomb":
                    response = "Shoots a large explosive poison bomb in a specified direction. 7 Mana\n";
                    break;
                case "Restore":
                    response = "Heals 35 HP. 20 Mana\n";
                    break;
                case "Full Protection":
                    response = "Increase Defense and Magic Defense by 10. 25 Mana\n";
                    break;
                case "Tsunami":
                    response = "Creates a large spot of water tiles. 20 Mana\n";
                    break;
                case "Erruption":
                    response = "Creates a large spot of lava tiles. 20 Mana\n";
                    break;
                case "Forest Growth":
                    response = "Creates a large spot of poison ivy. 20 Mana\n";
                    break;
                case "Electrify":
                    response = "Creates a large spot of electrified terrain. 20 Mana\n";
                    break;
                case "Manafy":
                    response = "Converts 10 HP into 30 Mana\n";
                    break;
                case "Destroy Armor":
                    response = "Decreases Defense and Magic Defense by 10. 25 Mana\n";
                    break;
                case "Demonify":
                    response = "Increases Strength by 10. 25 Mana\n";
                    break;
                case "Featherify":
                    response = "Increases Agility by 10. 25 Mana\n";
                    break;
                case "Transcend":
                    response = "Increases Magic by 10. 25 Mana\n";
                    break;
                case "Wither":
                    response = "Decreases Strength by 8. 25 Mana\n";
                    break;
                case "Chain Up":
                    response = "Decreases Agility by 8. 25 Mana\n";
                    break;
                case "Hypnotize":
                    response = "Decreases Magic by 8. 25 Mana\n";
                    break;
                case "Gamble":
                    response = "Casts a random spell. 20 Mana\n";
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
                case "Fireball":
                case "Ice Storm":
                case "Electro Bolt":
                case "Sludge Bomb":
                    result = 5;
                    break;
                case "Ember":
                case "Ice Shard":
                case "Zap":
                case "Poison Sting":
                    result = 3;
                    break;
                case "Recover":
                case "Ironize":
                case "Enchant":
                case "Corrode Armor":
                case "Sharpen":
                case "Enlighten":
                case "Prepare":
                case "Energy Drain":
                case "Confuse":
                case "Scare":
                case "Regenerate":
                case "Enchanse Vission":
                case "Liquify":
                case "Lavafy":
                case "Ivy Growth":
                case "Charge":
                    result = 10;
                    break;
                case "Heal Wounds":
                case "Protective Field":
                case "Melt Armor":
                case "Rage":
                case "Speed Up":
                case "Concentrate":
                case "Weaken":
                case "Slow Down":
                case "Terrify":
                    result = 15;
                    break;
                case "Meteor":
                case "Blizzard":
                case "Thunder":
                case "Plague Bomb":
                    result = 7;
                    break;
                case "Restore":
                case "Tsunami":
                case "Erruption":
                case "Forest Growth":
                case "Electrify":
                case "Gamble":
                    result = 20;
                    break;
                case "Full Protection":
                case "Destroy Armor":
                case "Demonify":
                case "Featherify":
                case "Transcend":
                case "Wither":
                case "Chain Up":
                case "Hypnotize":
                    result = 25;
                    break;
            }

            return result;
        }

        protected string WalkTo(int[] movement, ref int[,] level, string response, ref List<Creature> enemies, ref Player player, ref int[,] darkness, out string sound)
        {
        WalkReset:
            bool InTheWay = false;
            string currentSound = "";

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

                        currentSound = "Graphics/Sounds/attack.wav";

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
                    currentSound = "Graphics/Sounds/attack.wav";

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
                            currentSound = "Graphics/Sounds/footstep.wav";
                        }
                        break;

                    // Door
                    case 4:
                        if (IsGhost)
                        {
                            position[0] += movement[0];
                            position[1] += movement[1];
                            currentSound = "Graphics/Sounds/footstep.wav";
                        }
                        else { level[position[0] + movement[0], position[1] + movement[1]] = 1; currentSound = "Graphics/Sounds/dooropen.wav"; }
                        break;

                    // Regular tile
                    case 1:
                    case 3:
                        position[0] += movement[0];
                        position[1] += movement[1];
                        currentSound = "Graphics/Sounds/footstep.wav";
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
                        currentSound = "Graphics/Sounds/waterfootstep.wav";
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
                        currentSound = "Graphics/Sounds/firefootstep.wav";
                        break;

                    // Grass
                    case 8:
                        position[0] += movement[0];
                        position[1] += movement[1];
                        if (FloorCheck(level[position[0], position[1]]))
                        {
                            if (darkness[position[0], position[1]] == 1 && darkness[position[0], position[1]] != 2)
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
                        }
                        currentSound = "Graphics/Sounds/grassfootstep.wav";
                        break;

                    // Spike traps
                    case 9:
                    case 11:
                        response += $"{Name} steps onto a spike trap!\n";
                        position[0] += movement[0];
                        position[1] += movement[1];
                        if (FloorCheck(level[position[0], position[1]]))
                        {
                            Stats["HP"] -= (int)(Stats["MaxHP"] * 0.2);

                            if (darkness[position[0], position[1]] == 1 && darkness[position[0], position[1]] != 2)
                            {
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
                        }
                        currentSound = "Graphics/Sounds/footstep.wav";
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
                            if (darkness[position[0], position[1]] == 1 && darkness[position[0], position[1]] != 2)
                            {
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
                        }
                        currentSound = "Graphics/Sounds/firefootstep.wav";
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
                        currentSound = "Graphics/Sounds/grassfootstep.wav";
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
                        currentSound = "Graphics/Sounds/footstep.wav";
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
                        currentSound = "Graphics/Sounds/footstep.wav";
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
                        currentSound = "Graphics/Sounds/footstep.wav";
                        break;
                }
            }
            sound = currentSound;
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


        protected int FindDirection(int[] target, ref int[,] level, ref List<Creature> enemies)
        {
            int[] movement = new int[2] { 999, 999 };
            int direction = -1;

            List<int[]> movements = new List<int[]>();

            if (position[0] + 1 < level.GetLength(0))
            {
                if (!IsGhost || (level[position[0] + 1, position[1]] != 2 && level[position[0] + 1, position[1]] != 0 && level[position[0] + 1, position[1]] != 5))
                {
                    bool enemycheck = false;
                    foreach (Creature item in enemies)
                    {
                        if (position[0] + 1 == item.position[0] && position[1] == item.position[1])
                            enemycheck = true;
                    }
                    if (!enemycheck)
                        movements.Add(new int[] { DifferenceBetween(position[0] + 1, target[0]), DifferenceBetween(position[1], target[1]), 0 });
                }
            }
            if (position[0] - 1 >= 0)
            {
                if (!IsGhost || (level[position[0] - 1, position[1]] != 2 && level[position[0] - 1, position[1]] != 0 && level[position[0] - 1, position[1]] != 5))
                {
                    bool enemycheck = false;
                    foreach (Creature item in enemies)
                    {
                        if (position[0] - 1 == item.position[0] && position[1] == item.position[1])
                            enemycheck = true;
                    }
                    if (!enemycheck)
                        movements.Add(new int[] { DifferenceBetween(position[0] - 1, target[0]), DifferenceBetween(position[1], target[1]), 180 });
                }
            }

            if (position[1] + 1 < level.GetLength(1))
            {
                if (!IsGhost || (level[position[0], position[1] + 1] != 2 && level[position[0], position[1] + 1] != 0 && level[position[0], position[1] + 1] != 5))
                {
                    bool enemycheck = false;
                    foreach (Creature item in enemies)
                    {
                        if (position[0] == item.position[0] && position[1] + 1 == item.position[1])
                            enemycheck = true;
                    }
                    if (!enemycheck)
                        movements.Add(new int[] { DifferenceBetween(position[0], target[0]), DifferenceBetween(position[1] + 1, target[1]), 270 });
                }
            }
            if (position[1] - 1 >= 0)
            {
                if (!IsGhost || (level[position[0], position[1] - 1] != 2 && level[position[0], position[1] - 1] != 0 && level[position[0], position[1] - 1] != 5))
                {
                    bool enemycheck = false;
                    foreach (Creature item in enemies)
                    {
                        if (position[0] == item.position[0] && position[1] - 1 == item.position[1])
                            enemycheck = true;
                    }
                    if (!enemycheck)
                        movements.Add(new int[] { DifferenceBetween(position[0], target[0]), DifferenceBetween(position[1] - 1, target[1]), 90 });
                }
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

        protected int DifferenceBetween(int point1, int point2)
        {
            if (point1 > point2)
                return (point1 - point2);
            else return (point2 - point1);
        }

        protected bool IsAggresiveSpell(string spell)
        {

            switch (spell)
            {
                // Explosions
                case "Explosion":
                case "Fire Explosion":
                case "Poison Explosion":
                case "Ice Explosion":
                case "Lightning Explosion":
                // Arrows
                case "Shoot Arrow":
                case "Shoot Fire Arrow":
                case "Shoot Poison Arrow":
                case "Shoot Ice Arrow":
                case "Shoot Lightning Arrow":
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
                // Tile Spawners
                case "Liquify":
                case "Lavafy":
                case "Ivy Growth":
                case "Charge":
                case "Tsunami":
                case "Erruption":
                case "Forest Growth":
                case "Electrify":
                // End tile spawners

                // Statuses
                case "Corrode armor":
                case "Energy Drain":
                case "Confuse":
                case "Scare":
                case "Melt Armor":
                case "Weaken":
                case "Slow Down":
                case "Terrify":
                case "Destroy Armor":
                case "Wither":
                case "Chain Up":
                case "Hypnotize":
                    return true;
                // End statuses
                default:
                    return false;
            }
        }
    }


    public class Player : Creature
    {
        public int DangerLevel = 0;
        public new Item[,] Inventory = new Item[11, 3];

        public bool OpenInventory = false;
        public int[] SelectedSlot = new int[2] { -100, -1 };
        public int[] CurrentSlot = new int[2] { 0, 0 };
        public bool OpenSpell = false;
        public bool DrawSpellLine = false;
        int[] ArrowSlot = new int[2] { 0, 0 };
        bool UseMana = true;

        string RemSpell = "";


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
            DrawEquipment = true;
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


            if (rand.Next(2) == 0)
            {
                Inventory[0, 1] = new Item(-1, 6);
                Inventory[1, 1] = new Item(-1, 4);
            }
            else
            {
                Inventory[0, 1] = new Item(-1, 4);
                Inventory[1, 1] = new Item(-1, 6);
            }
            Inventory[2, 1] = new Item(-1, 10);
            Inventory[3, 1] = new Item(0, 10);
            Inventory[4, 1] = new Item(0, 0);

        }

        // Main Logics
        public override string Logics(ref int[,] level, ref List<Chest> chests, ref List<Effect> effects, ref List<Creature> enemies, ref Player player, ref int[,] darkness, out string currentSound)
        {
            string sound = "";
            bool wasAlive = true;
            if (Stats["HP"] <= 0) { wasAlive = false; }
            string response = "";
            int[] movement = new int[] { 0, 0 };
            bool invCheck = false;

            // Movement code


            // Opening inventory
            if (Keyboard.IsKeyDown(Key.Q) && !OpenInventory)
            {
                OpenInventory = true;
                DrawSpellLine = false;
                invCheck = true;
                sound = "Graphics/Sounds/openinventory.wav";
            }
            // Move only if not frozen/paralysed (Movement direction)
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


                // Shooting with a bow
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

                response = WalkTo(new int[] { movement[0], movement[1] }, ref level, response, ref enemies, ref player, ref darkness, out sound);
                // Empty amulets
                if (Inventory[6, 0] != null)
                {
                    switch (Inventory[6, 0].Name)
                    {
                        case "Empty Restoration Amulet":
                            Inventory[6, 0].Quantity--;
                            Inventory[6,0].Description = $"A used Restoration Amulet. It requires {Inventory[6, 0].Quantity} more steps to work again\n";
                            if (Inventory[6, 0].Quantity <= 0)
                            {
                                response += $"{Name}'s Restoration Amulet glows brightly!\n";
                                Inventory[6, 0].File = "Graphics/ByteLikeGraphics/Items/amulet8.png";
                                Inventory[6, 0].Quantity = 1;
                                Inventory[6, 0].ClassType += 10;
                                Inventory[6, 0].Name = "Restoration Amulet";
                                Inventory[6, 0].Description = "It will fully heal and grant you some XP once you enter a new floor once. It will need to recharge afterwards to work again\n";
                            }
                            break;
                        case "Empty Survival Amulet":
                            Inventory[6, 0].Quantity--;
                            Inventory[6, 0].Description = $"A used Survival Amulet. It requires {Inventory[6, 0].Quantity} more steps to work again\n";
                            if (Inventory[6, 0].Quantity <= 0)
                            {
                                response += $"{Name}'s Survival Amulet glows brightly!\n";
                                Inventory[6, 0].File = "Graphics/ByteLikeGraphics/Items/amulet10.png";
                                Inventory[6, 0].Quantity = 1;
                                Inventory[6, 0].ClassType += 10;
                                Inventory[6, 0].Name = "Survival Amulet";
                                Inventory[6, 0].Description = "It will leave you at 1 HP instead of dying once. It will need to recharge afterwards to work again\n";
                            }
                            break;
                        case "Empty Moon Amulet":
                            Inventory[6, 0].Quantity--;
                            Inventory[6, 0].Description = $"A used Moon Amulet. It requires {Inventory[6, 0].Quantity} more steps to work again\n";
                            if (Inventory[6, 0].Quantity <= 0)
                            {
                                response += $"{Name}'s Moon Amulet glows brightly!\n";
                                Inventory[6, 0].File = "Graphics/ByteLikeGraphics/Items/amulet12.png";
                                Inventory[6, 0].Quantity = 1;
                                Inventory[6, 0].ClassType += 10;
                                Inventory[6, 0].Name = "Moon Amulet";
                                Inventory[6, 0].Description = "It will fully restore your mana and let you cast a spell if you run out of mana once. It will need to recharge afterwards to work again\n";
                            }
                            break;
                    }
                }
            }
            // if in inventory
            else
            {
                if (!OpenSpell || DistanceBetween(new int[] { position[0], position[1] }, new int[] { position[0] + CurrentSlot[0] + movement[0], position[1] + CurrentSlot[1] + movement[1] }) <= GetStat("Torch"))
                {
                    CurrentSlot[0] += movement[0];
                    CurrentSlot[1] += movement[1];
                    if (movement[0] != 0 || movement[1] != 0)
                        sound = "Graphics/Sounds/menuclick.wav";
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
                        sound = "Graphics/Sounds/paw_squeak.wav";
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
                                            sound = "Graphics/Sounds/equipment.wav";
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
                                    sound = "Graphics/Sounds/equipment.wav";
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
                                    sound = "Graphics/Sounds/equipment.wav";
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
                                                            {
                                                                bowcheck = true;
                                                                sound = "Graphics/Sounds/bow.wav";
                                                            }
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
                                                    // Bombs
                                                    else if (Inventory[SelectedSlot[0], SelectedSlot[1]].Name.Contains("Bomb"))
                                                    {
                                                        response += $"{Name} has planted a {Inventory[SelectedSlot[0], SelectedSlot[1]].Name.ToLower()}!\n";
                                                        if (Inventory[SelectedSlot[0], SelectedSlot[1]].Name.Contains("Large"))
                                                            enemies.Add(new Bomb(new int[] { position[0], position[1] }, 3));
                                                        else
                                                            enemies.Add(new Bomb(new int[] { position[0], position[1] }, 2));
                                                        Inventory[SelectedSlot[0], SelectedSlot[1]].Quantity--;
                                                        if (Inventory[SelectedSlot[0], SelectedSlot[1]].Quantity <= 0)
                                                            Inventory[SelectedSlot[0], SelectedSlot[1]] = null;
                                                        SelectedSlot[0] = -100;
                                                    }
                                                    // Learning new spells
                                                    else if (Inventory[SelectedSlot[0], SelectedSlot[1]].Name.Contains("Book"))
                                                    {
                                                        if (Stats["SpellSlots"] > Spells.Count)
                                                        {
                                                            sound = "Graphics/Sounds/holychoir.wav";
                                                            Spells.Add(Inventory[SelectedSlot[0], SelectedSlot[1]].Spell);
                                                            response += $"{Name} has learnt how to use {Inventory[SelectedSlot[0], SelectedSlot[1]].Spell}!\n";
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
                                                        sound = "Graphics/Sounds/item.wav";
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
                                                                    BuffLevels["Strength"] = (int)((BuffLevels["Strength"] + Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Strength"]) / 1.5);
                                                                else
                                                                    BuffLevels["Strength"] = Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Strength"];
                                                                if (BuffLevels["Strength"] >= 0)
                                                                    Buffs["Strength"] = BuffLevels["Strength"] * 10;
                                                                else Buffs["Strength"] = -(BuffLevels["Strength"]) * 10;
                                                            }

                                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Agility"] > 0)
                                                            {
                                                                if (Buffs["Agility"] > 0)
                                                                    BuffLevels["Agility"] = (int)((BuffLevels["Agility"] + Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Agility"]) / 1.5);
                                                                else
                                                                    BuffLevels["Agility"] = Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Agility"];
                                                                if (BuffLevels["Agility"] >= 0)
                                                                    Buffs["Agility"] = BuffLevels["Agility"] * 10;
                                                                else Buffs["Agility"] = -(BuffLevels["Agility"]) * 10;
                                                            }

                                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Magic"] > 0)
                                                            {
                                                                if (Buffs["Magic"] > 0)
                                                                    BuffLevels["Magic"] = (int)((BuffLevels["Magic"] + Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Magic"]) / 1.5);
                                                                else
                                                                    BuffLevels["Magic"] = Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Magic"];
                                                                if (BuffLevels["Magic"] >= 0)
                                                                    Buffs["Magic"] = BuffLevels["Magic"] * 10;
                                                                else Buffs["Magic"] = -(BuffLevels["Magic"]) * 10;
                                                            }

                                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Defense"] > 0)
                                                            {
                                                                if (Buffs["Defense"] > 0)
                                                                    BuffLevels["Defense"] = (int)((BuffLevels["Defense"] + Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Defense"]) / 1.5);
                                                                else
                                                                    BuffLevels["Defense"] = Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["Defense"];
                                                                if (BuffLevels["Defense"] >= 0)
                                                                    Buffs["Defense"] = BuffLevels["Defense"] * 10;
                                                                else Buffs["Defense"] = -(BuffLevels["Defense"]) * 10;
                                                            }

                                                            if (Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["MagicDefense"] > 0)
                                                            {
                                                                if (Buffs["MagicDefense"] > 0)
                                                                    BuffLevels["MagicDefense"] = (int)((BuffLevels["MagicDefense"] + Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["MagicDefense"]) / 1.5);
                                                                else
                                                                    BuffLevels["MagicDefense"] = Inventory[SelectedSlot[0], SelectedSlot[1]].Stats["MagicDefense"];
                                                                if (BuffLevels["MagicDefense"] >= 0)
                                                                    Buffs["MagicDefense"] = BuffLevels["MagicDefense"] * 10;
                                                                else Buffs["MagicDefense"] = -(BuffLevels["MagicDefense"]) * 10;
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
                                            sound = "Graphics/Sounds/equipment.wav";
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
                                    else if (CurrentSlot[0] < 9)
                                    {
                                        if (chests[GetChest(ref chests)].Inventory[SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1)] == null || (chests[GetChest(ref chests)].Inventory[SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1)].GearType == CurrentSlot[0] + 1 || chests[GetChest(ref chests)].Inventory[SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1)].GearType == CurrentSlot[0] && chests[GetChest(ref chests)].Inventory[SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1)].GearType == 8))
                                        {
                                            sound = "Graphics/Sounds/equipment.wav";
                                            Inventory[CurrentSlot[0], CurrentSlot[1]] = chests[GetChest(ref chests)].TakeOut(Inventory[CurrentSlot[0], CurrentSlot[1]], new int[] { SelectedSlot[0], SelectedSlot[1] - Inventory.GetLength(1) });
                                            SelectedSlot[0] = -100;
                                        }
                                        else
                                        response += $"{Name}: I can't put that there\n";
                                    }
                                    // If trying to use/throw away something from a chest
                                    else response += $"{Name}: I need to take it out first.\n";
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
                                // Using nothing
                                else if (CurrentSlot[0] == 9) { response += $"{Name}: I need to pick something to use first\n"; }
                                // Throwing out nothing
                                else { response += $"{Name}: There's nothing here.\n"; }
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

                // While using a spell
                else
                {
                    if (Keyboard.IsKeyDown(Key.E))
                    {
                        if (CurrentSlot[0] == 0 && CurrentSlot[1] == 0 && DrawSpellLine)
                        {
                            if (RemSpell != "Gamble")
                            {
                                CurrentSlot[0] = rand.Next(-1, 2);
                                CurrentSlot[1] = rand.Next(-1, 2);
                            }
                        }

                        if (UseMana && Stats["Mana"] < GetSpellCost(RemSpell) && Inventory[6,0] != null)
                        {
                            if (Inventory[6, 0].Name == "Moon Amulet")
                            {
                                Stats["Mana"] = GetStat("MaxMana") + GetSpellCost(RemSpell);
                                Inventory[6,0].Name = "Empty Moon Amulet";
                                Inventory[6, 0].Quantity = Inventory[6, 0].ClassType;
                                Inventory[6, 0].ClassType += 10;
                                Inventory[6, 0].Description = $"A used Moon Amulet. It requires {Inventory[6,0].Quantity} more steps to work again\n";
                                Inventory[6, 0].File = "Graphics/ByteLikeGraphics/Items/amulet13.png";
                                response += $"{Name}'s Moon Amulet glows brightly!\n";
                            }
                        }

                        if (Stats["Mana"] >= GetSpellCost(RemSpell) || !UseMana)
                        {
                            if (ArrowSlot[0] != 0 || ArrowSlot[1] != 0)
                            {
                                Inventory[ArrowSlot[0], ArrowSlot[1]].Quantity--;
                                if (Inventory[ArrowSlot[0], ArrowSlot[1]].Quantity <= 0)
                                    Inventory[ArrowSlot[0], ArrowSlot[1]] = null;
                                SelectedSlot[0] = -100;
                            }

                            if (UseMana)
                                Stats["Mana"] -= GetSpellCost(RemSpell);
                            if (!RemSpell.Contains("Arrow"))
                            {
                                if (RemSpell == "Gamble")
                                    RemSpell = GetRandomSpell(null);
                                response += $"{Name} casts {RemSpell}!\n";
                                if (RemSpell != "Nothing")
                                    effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { CurrentSlot[0], CurrentSlot[1] }, GetStat("Magic"), RemSpell));
                                else
                                    response += "... Nothing happened.\n";

                                sound = "Graphics/Sounds/holychoir.wav";
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
            // end inventory

            //

            // Conditions
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
                sound = "Graphics/Sounds/closeinventory.wav";
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

                response = "";
            }

            // leveling up
            if (Stats["XP"] >= (int)(90 + Math.Pow(Stats["Level"], 2) * 10))
                response += LevelUp();


            // Skipping turns
            if (Keyboard.IsKeyDown(Key.R) && !OpenInventory)
            {
                response += $"{Name} is watching carefully...\n";
                sound = "";
            }

            currentSound = sound;

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
                case "Ember":
                case "Ice Shard":
                case "Zap":
                case "Poison Sting":
                case "Fireball":
                case "Ice Storm":
                case "Electro Bolt":
                case "Sludge Bomb":
                case "Meteor":
                case "Blizzard":
                case "Thunder":
                case "Plague Bomb":
                case "Gamble":
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

            if ((floor / 12) >= 4)
            {
                File += "3.png";
                Stats["HPRegen"] += 1;
                Stats["ManaRegen"] += 1;
                statModifier = 1.25;
                Name = "Giant Rat";
            }
            else if ((floor / 12) >= 2)
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

        public override string Logics(ref int[,] level, ref List<Chest> chests, ref List<Effect> effects, ref List<Creature> enemies, ref Player player, ref int[,] darkness, out string currentSound)
        {
            string response = "";
            string sound = "";

            if (DistanceBetween(new int[] { position[0], position[1] }, new int[] { player.position[0], player.position[1] }) <= GetStat("Torch") && !Aggressive)
            {
                Aggressive = true;
                response = $"{Name} notices {player.Name}!\n";
            }

            int movementdirection = rand.Next(4) * 90;

            if (Aggressive)
            {
                movementdirection = FindDirection(new int[] { player.position[0], player.position[1] }, ref level, ref enemies);
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
            response = WalkTo(new int[] { movement[0], movement[1] }, ref level, response, ref enemies, ref player, ref darkness, out sound);


            // Default logics ending
            response = Conditions(response);

            if (Stats["HP"] > GetStat("MaxHP")) { Stats["HP"] = GetStat("MaxHP"); }
            if (Stats["Mana"] > GetStat("MaxMana")) { Stats["Mana"] = GetStat("MaxMana"); }



            currentSound = sound;
            if (DistanceBetween(new int[] { position[0], position[1] }, new int[] { player.position[0], player.position[1] }) <= player.GetStat("Torch") || Aggressive)
                return response;
            else return "";
        }


    }


    public class DoppleGanger : Creature
    {
        bool Aggressive = false;

        public DoppleGanger(Player player, int[] spawnpoint)
            :base()
        {
            DrawEquipment = true;
            DropEquipment = true;
            position[0] = spawnpoint[0];
            position[1] = spawnpoint[1];
            Stats["Level"] = player.Stats["Level"];
            Stats["MaxHP"] = player.Stats["MaxHP"]+5;
            Stats["HP"] = player.Stats["MaxHP"];
            Stats["Mana"] = player.Stats["MaxMana"];
            Stats["MaxMana"] = player.Stats["MaxMana"]+5;
            Stats["Strength"] = player.Stats["Strength"]+3;
            Stats["Magic"] = player.Stats["Magic"]+3;
            Stats["Agility"] = player.Stats["Agility"]+3;
            Stats["Defense"] = player.Stats["Defense"]+3;
            Stats["MagicDefense"] = player.Stats["MagicDefense"]+3;
            Stats["HPRegen"] = player.Stats["HPRegen"];
            Stats["MaxHPRegen"] = player.Stats["MaxHPRegen"];
            Stats["ManaRegen"] = player.Stats["ManaRegen"];
            Stats["MaxManaRegen"] = player.Stats["MaxManaRegen"];
            Stats["Torch"] = player.Stats["Torch"]+1;
            Name = "Ghost of ";
            Name += player.Name;

            for (int i = 0; i < player.Spells.Count; i++)
            {
                Spells.Add(player.Spells[i]);
            }

            switch (player.File)
            {
                case "Graphics/ByteLikeGraphics/Creatures/player1.png":
                    File = "Graphics/ByteLikeGraphics/Creatures/undeadplayer1.png";
                    break;
                case "Graphics/ByteLikeGraphics/Creatures/player2.png":
                    File = "Graphics/ByteLikeGraphics/Creatures/undeadplayer2.png";
                    break;
                case "Graphics/ByteLikeGraphics/Creatures/player3.png":
                    File = "Graphics/ByteLikeGraphics/Creatures/undeadplayer3.png";
                    break;
                default:
                    File = "Graphics/ByteLikeGraphics/Creatures/undeadplayer";
                    File += (rand.Next(3) + 1).ToString();
                    File += ".png";
                    break;
            }

            Inventory = new Item[player.Inventory.GetLength(0), player.Inventory.GetLength(1)];

            for (int i = 0; i < Inventory.GetLength(1); i++)
            {
                for (int j = 0; j < Inventory.GetLength(0); j++)
                {
                    Inventory[j, i] = player.Inventory[j, i];
                }
            }
        }
        public override string Logics(ref int[,] level, ref List<Chest> chests, ref List<Effect> effects, ref List<Creature> enemies, ref Player player, ref int[,] darkness, out string currentSound)
        {
            string response = "";
            string sound = "";

            // Aggressive check
            if (DistanceBetween(new int[] { position[0], position[1] }, new int[] { player.position[0], player.position[1] }) <= GetStat("Torch") && !Aggressive)
            {
                Aggressive = true;
                response = $"{Name} notices {player.Name}!\n";
            }

            // ACTION
            if (Aggressive)
            {
                List<int> actions = new List<int>();

                int chosenSpell = 0;

                int[] arrowSlot = new int[] { 0, 0 };
                int[] healSlot = new int[] { 0, 0 };
                int[] manaSlot = new int[] { 0, 0 };
                int[] itemSlot = new int[] { 0, 0 };


                // 0 - Nothing, 1 - Walk, 2 - Spell, 3 - Bow shot, 4 - Use Healing Item, 5 - Use Mana Item, 6 - Use random item

                actions.Add(0);

                // deciding to - Walk
                if (Statuses[2] == 0 && Statuses[3] % 2 == 0 && Stats["HP"] > 0)
                {
                    for (byte i = 0; i < 5; i++)
                        actions.Add(1);
                }

                // deciding to - Cast a spell
                if (Spells.Count > 0)
                {
                    chosenSpell = rand.Next(Spells.Count);
                    if (Stats["Mana"] >= GetSpellCost(Spells[chosenSpell]))
                    {
                        for (byte i = 0; i < 10; i++)
                            actions.Add(2);
                    }
                    else if (Stats["Mana"] < GetSpellCost(Spells[chosenSpell]) && Inventory[6, 0] != null)
                    {
                        if (Inventory[6, 0].Name == "Moon Amulet")
                        {
                            for (byte i = 0; i < 12; i++)
                                actions.Add(2);
                        }
                    }
                }

                // deciding to - Shooting an arrow
                if (Inventory[3, 0] != null)
                {
                    if (Inventory[3, 0].Name.ToLower().Contains("bow"))
                    {
                        bool stop = false;
                        for (int i = 0; i < Inventory.GetLength(1) && !stop; i++)
                        {
                            for (int j = 0; j < Inventory.GetLength(0) && !stop; j++)
                            {
                                if (Inventory[j, i] != null)
                                {
                                    if (Inventory[j, i].Name.Contains("Arrow") && Inventory[j, i].GearType == 10)
                                    {
                                        stop = true;
                                        arrowSlot[0] = j;
                                        arrowSlot[1] = i;

                                        for (byte f = 0; f < 10; f++)
                                            actions.Add(3);
                                    }
                                }
                            }
                        }
                    }
                }

                // deciding to - Use a healing item
                if (Stats["HP"] < GetStat("MaxHP") * 0.6)
                {
                    bool stop = false;
                    for (int i = 0; i < Inventory.GetLength(1) && !stop; i++)
                    {
                        for (int j = 0; j < Inventory.GetLength(0) && !stop; j++)
                        {
                            if (Inventory[j, i] != null)
                            {
                                if (Inventory[j, i].Stats["MaxHP"] > 0 && Inventory[j, i].GearType == 10)
                                {
                                    stop = true;
                                    healSlot[0] = j;
                                    healSlot[1] = i;

                                    for (byte f = 0; f < 15; f++)
                                        actions.Add(4);
                                }
                            }
                        }
                    }
                }

                // deciding to - Use a mana item
                if (Stats["Mana"] < GetStat("MaxMana") * 0.8)
                {
                    bool stop = false;
                    for (int i = 0; i < Inventory.GetLength(1) && !stop; i++)
                    {
                        for (int j = 0; j < Inventory.GetLength(0) && !stop; j++)
                        {
                            if (Inventory[j, i] != null)
                            {
                                if (Inventory[j, i].Stats["MaxMana"] > 0 && Inventory[j, i].GearType == 10)
                                {
                                    stop = true;
                                    manaSlot[0] = j;
                                    manaSlot[1] = i;

                                    for (byte f = 0; f < 12; f++)
                                        actions.Add(5);
                                }
                            }
                        }
                    }
                }

                // deciding to - Use a random item
                itemSlot[0] = rand.Next(Inventory.GetLength(0));
                itemSlot[1] = rand.Next(Inventory.GetLength(1));
                if (Inventory[itemSlot[0], itemSlot[1]] != null)
                {
                    if (Inventory[itemSlot[0], itemSlot[1]].GearType == 10 && !Inventory[itemSlot[0], itemSlot[1]].Name.Contains("Arrow") && !Inventory[itemSlot[0], itemSlot[1]].Name.Contains("Book") && !Inventory[itemSlot[0], itemSlot[1]].Name.Contains("Bomb"))
                    {
                        for (byte i = 0; i < 7; i++)
                            actions.Add(6);
                    }
                }

                // ACTUAL ACTION
                switch (actions[rand.Next(actions.Count)])
                {
                    case 0:
                        response += $"{Name} is watching carefully...\n";
                        break;
                    case 1:
                        int movementdirection = FindDirection(new int[] { player.position[0], player.position[1] }, ref level, ref enemies);
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
                        response = WalkTo(new int[] { movement[0], movement[1] }, ref level, response, ref enemies, ref player, ref darkness, out sound);
                        break;
                    case 2:
                        if (Stats["Mana"] < GetSpellCost(Spells[chosenSpell]))
                        {
                            Stats["Mana"] = GetStat("MaxMana") + GetSpellCost(Spells[chosenSpell]);
                            Inventory[6, 0].Name = "Empty Moon Amulet";
                            Inventory[6, 0].Quantity = Inventory[6, 0].ClassType;
                            Inventory[6, 0].ClassType += 10;
                            Inventory[6, 0].Description = $"A used Moon Amulet. It requires {Inventory[6, 0].Quantity} more steps to work again\n";
                            Inventory[6, 0].File = "Graphics/ByteLikeGraphics/Items/amulet13.png";
                            response += $"{Name}'s Moon Amulet glows brightly!\n";
                        }
                        Stats["Mana"] -= GetSpellCost(Spells[chosenSpell]);
                        sound = "Graphics/Sounds/holychoir.wav";
                        arrowSlot[0] = 0;
                        arrowSlot[1] = 0;
                        if (IsAggresiveSpell(Spells[chosenSpell]))
                        {
                            arrowSlot[0] = player.position[0] - position[0];
                            arrowSlot[1] = player.position[1] - position[1];
                        }
                        effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { arrowSlot[0], arrowSlot[1] }, GetStat("Magic"), Spells[chosenSpell]));
                        response += $"{Name} casts {Spells[chosenSpell]}!\n";
                        break;
                    case 3:
                        healSlot[0] = player.position[0] - position[0];
                        healSlot[1] = player.position[1] - position[1];
                        sound = "Graphics/Sounds/bow.wav";

                        effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { healSlot[0], healSlot[1] }, GetStat("Agility"), $"Shoot {Inventory[arrowSlot[0], arrowSlot[1]].Name}"));

                        Inventory[arrowSlot[0], arrowSlot[1]].Quantity--;
                        if (Inventory[arrowSlot[0], arrowSlot[1]].Quantity <= 0)
                            Inventory[arrowSlot[0], arrowSlot[1]] = null;
                        break;
                    case 4:
                        Stats["HP"] += Inventory[healSlot[0], healSlot[1]].Stats["MaxHP"];

                        sound = "Graphics/Sounds/closeinventory.wav";

                        response += $"{Name} used a {Inventory[healSlot[0], healSlot[1]].Name}!\n";
                        Inventory[healSlot[0], healSlot[1]].Quantity--;
                        if (Inventory[healSlot[0], healSlot[1]].Quantity <= 0)
                            Inventory[healSlot[0], healSlot[1]] = null;
                        break;
                    case 5:
                        Stats["Mana"] += Inventory[manaSlot[0], manaSlot[1]].Stats["MaxMana"];

                        sound = "Graphics/Sounds/closeinventory.wav";

                        response += $"{Name} used a {Inventory[manaSlot[0], manaSlot[1]].Name}!\n";
                        Inventory[manaSlot[0], manaSlot[1]].Quantity--;
                        if (Inventory[manaSlot[0], manaSlot[1]].Quantity <= 0)
                            Inventory[manaSlot[0], manaSlot[1]] = null;
                        break;
                    case 6:
                        sound = "Graphics/Sounds/closeinventory.wav";
                        // Arrows/Scrolls, aka using spells inside items
                        if (Inventory[itemSlot[0], itemSlot[1]].Name.Contains("Scroll"))
                        {
                            arrowSlot[0] = 0;
                            arrowSlot[1] = 0;
                            if (IsAggresiveSpell(Inventory[itemSlot[0], itemSlot[1]].Spell))
                            {
                                arrowSlot[0] = player.position[0] - position[0];
                                arrowSlot[1] = player.position[1] - position[1];
                            }
                            effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { arrowSlot[0], arrowSlot[1] }, GetStat("Magic"), Inventory[itemSlot[0], itemSlot[1]].Spell));
                        }
                        // Healing items
                        else
                        {
                            if (Inventory[itemSlot[0], itemSlot[1]].Name == "Improvement Potion")
                            {
                                Statuses[0] = 0;
                                Statuses[1] = 0;
                                Statuses[2] = 0;
                                Statuses[3] = 0;
                            }
                            else
                            {
                                Stats["HP"] += Inventory[itemSlot[0], itemSlot[1]].Stats["MaxHP"];
                                Stats["Mana"] += Inventory[itemSlot[0], itemSlot[1]].Stats["MaxMana"];

                                if (Stats["HP"] > GetStat("MaxHP"))
                                    Stats["HP"] = GetStat("MaxHP");
                                if (Stats["Mana"] > GetStat("MaxMana"))
                                    Stats["Mana"] = GetStat("MaxMana");

                                if (Inventory[itemSlot[0], itemSlot[1]].Stats["Strength"] > 0)
                                {
                                    if (Buffs["Strength"] > 0)
                                        BuffLevels["Strength"] = (int)((BuffLevels["Strength"] + Inventory[itemSlot[0], itemSlot[1]].Stats["Strength"]) / 1.5);
                                    else
                                        BuffLevels["Strength"] = Inventory[itemSlot[0], itemSlot[1]].Stats["Strength"];
                                    if (BuffLevels["Strength"] >= 0)
                                        Buffs["Strength"] = BuffLevels["Strength"] * 10;
                                    else Buffs["Strength"] = -(BuffLevels["Strength"]) * 10;
                                }

                                if (Inventory[itemSlot[0], itemSlot[1]].Stats["Agility"] > 0)
                                {
                                    if (Buffs["Agility"] > 0)
                                        BuffLevels["Agility"] = (int)((BuffLevels["Agility"] + Inventory[itemSlot[0], itemSlot[1]].Stats["Agility"]) / 1.5);
                                    else
                                        BuffLevels["Agility"] = Inventory[itemSlot[0], itemSlot[1]].Stats["Agility"];
                                    if (BuffLevels["Agility"] >= 0)
                                        Buffs["Agility"] = BuffLevels["Agility"] * 10;
                                    else Buffs["Agility"] = -(BuffLevels["Agility"]) * 10;
                                }

                                if (Inventory[itemSlot[0], itemSlot[1]].Stats["Magic"] > 0)
                                {
                                    if (Buffs["Magic"] > 0)
                                        BuffLevels["Magic"] = (int)((BuffLevels["Magic"] + Inventory[itemSlot[0], itemSlot[1]].Stats["Magic"]) / 1.5);
                                    else
                                        BuffLevels["Magic"] = Inventory[itemSlot[0], itemSlot[1]].Stats["Magic"];
                                    if (BuffLevels["Magic"] >= 0)
                                        Buffs["Magic"] = BuffLevels["Magic"] * 10;
                                    else Buffs["Magic"] = -(BuffLevels["Magic"]) * 10;
                                }

                                if (Inventory[itemSlot[0], itemSlot[1]].Stats["Defense"] > 0)
                                {
                                    if (Buffs["Defense"] > 0)
                                        BuffLevels["Defense"] = (int)((BuffLevels["Defense"] + Inventory[itemSlot[0], itemSlot[1]].Stats["Defense"]) / 1.5);
                                    else
                                        BuffLevels["Defense"] = Inventory[itemSlot[0], itemSlot[1]].Stats["Defense"];
                                    if (BuffLevels["Defense"] >= 0)
                                        Buffs["Defense"] = BuffLevels["Defense"] * 10;
                                    else Buffs["Defense"] = -(BuffLevels["Defense"]) * 10;
                                }

                                if (Inventory[itemSlot[0], itemSlot[1]].Stats["MagicDefense"] > 0)
                                {
                                    if (Buffs["MagicDefense"] > 0)
                                        BuffLevels["MagicDefense"] = (int)((BuffLevels["MagicDefense"] + Inventory[itemSlot[0], itemSlot[1]].Stats["MagicDefense"]) / 1.5);
                                    else
                                        BuffLevels["MagicDefense"] = Inventory[itemSlot[0], itemSlot[1]].Stats["MagicDefense"];
                                    if (BuffLevels["MagicDefense"] >= 0)
                                        Buffs["MagicDefense"] = BuffLevels["MagicDefense"] * 10;
                                    else Buffs["MagicDefense"] = -(BuffLevels["MagicDefense"]) * 10;
                                }
                            }
                        }

                        response += $"{Name} used a {Inventory[itemSlot[0], itemSlot[1]].Name}!\n";
                        Inventory[itemSlot[0], itemSlot[1]].Quantity--;
                        if (Inventory[itemSlot[0], itemSlot[1]].Quantity <= 0)
                            Inventory[itemSlot[0], itemSlot[1]] = null;
                        break;
                }

            }
            else
            {
                int movementdirection = rand.Next(4) * 90;
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
                response = WalkTo(new int[] { movement[0], movement[1] }, ref level, response, ref enemies, ref player, ref darkness, out sound);

            }


            // Default logics ending really
            response = Conditions(response);

            if (Stats["HP"] > GetStat("MaxHP")) { Stats["HP"] = GetStat("MaxHP"); }
            if (Stats["Mana"] > GetStat("MaxMana")) { Stats["Mana"] = GetStat("MaxMana"); }

            currentSound = sound;

            if (DistanceBetween(new int[] { position[0], position[1] }, new int[] { player.position[0], player.position[1] }) <= player.GetStat("Torch") || Aggressive)
                return response;
            else return "";
        }
    }

    public class Bomb : Creature
    {
        public Bomb(int[] pos, int strength)
            :base()
        {
            DrawHealthbar = false;
            Name = "Bomb";
            position[0] = pos[0];
            position[1] = pos[1];
            Stats["HP"] = 4;
            Stats["HPRegen"] = 999;
            File = "Graphics/ByteLikeGraphics/Items/bomb4.png";
            Stats["Strength"] = strength;
        }

        public override string Logics(ref int[,] level, ref List<Chest> chests, ref List<Effect> effects, ref List<Creature> enemies, ref Player player, ref int[,] darkness, out string currentSound)
        {
            string sound = "";
            Stats["HP"]--;
            if (Stats["HP"] > 4)
                Stats["HP"] = 4;
            if (Stats["HP"] <= 0)
            {
                Stats["HP"] = 0;
                for (int i = -Stats["Strength"] - 1; i < Stats["Strength"] + 1; i++)
                {
                    for (int j = -Stats["Strength"] - 1; j < Stats["Strength"] + 1; j++)
                    {
                        if (DistanceBetween(new int[] { position[0], position[1] }, new int[] { position[0]+j, position[1]+i }) <= Stats["Strength"])
                            effects.Add(new Effect(new int[] { position[0], position[1] }, new int[] { j, i }, 20, "Explosion"));
                    }
                }
                sound = "Graphics/Sounds/explosion.wav";
            }
            File = "Graphics/ByteLikeGraphics/Items/bomb";
            File += Stats["HP"].ToString();
            File += ".png";
            currentSound = sound;

            return "";
        }
    }
}
