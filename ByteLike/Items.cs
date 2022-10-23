using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteLike
{


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
        public int Quantity = 1;
        public string Description = "";

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
            if (rand.Next(2) == 0) { GearType = 10; }

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


                    Stats["MaxHP"] += rand.Next(0, strength + 1) * 5;
                    Stats["MaxMana"] += rand.Next(0, strength + 1) * 5;
                    Stats["Strength"] += rand.Next(0, strength + 1);
                    Stats["Magic"] += rand.Next(0, strength + 1);
                    Stats["Agility"] += rand.Next(0, strength + 1);
                    Stats["Defense"] += rand.Next(0, strength + 1);
                    Stats["MagicDefense"] += rand.Next(0, strength + 1);

                    if (strength == 0)
                    {
                        GearType = rand.Next(3) + 1;
                        sideB = false;
                    }

                    File = "Graphics/ByteLikeGraphics/Armor/armor" + strength.ToString() + "-" + GearType.ToString();
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
                            Stats["Magic"] += 2;
                            break;
                        case 15310:
                            Name = "Bug Master Hood";
                            Stats["MaxMana"] += 10;
                            Stats["Defense"] += 1;
                            break;
                        case 15320:
                            Name = "Bug Master CLoak";
                            Stats["Magic"] += 1;
                            Stats["Defense"] += 2;
                            break;
                        case 15330:
                            Name = "Bug Master Kilt";
                            Stats["MaxMana"] += 5;
                            Stats["Defense"] += 1;
                            break;
                        case 15340:
                            Name = "Diamond Staff";
                            Stats["MaxMana"] += 20;
                            Stats["Strength"] += 3;
                            Stats["Magic"] += 5;
                            break;
                        case 15345:
                            Name = "Root Wand";
                            Stats["Strength"] += 2;
                            Stats["Magic"] += 4;
                            Stats["ManaRegen"] += -1;
                            break;
                        case 15350:
                            Name = "Hidden Tome";
                            Stats["MaxMana"] += 10;
                            break;
                        case 20110:
                            Name = "Iron Helmet";
                            Stats["Strength"] += 2;
                            Stats["Defense"] += 4;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 20120:
                            Name = "Iron Vest";
                            Stats["Strength"] += 2;
                            Stats["Defense"] += 6;
                            break;
                        case 20130:
                            Name = "Iron Leggings";
                            Stats["Strength"] += 2;
                            Stats["Defense"] += 4;
                            Stats["MagicDefense"] += 1;
                            break;
                        case 20140:
                            Name = "Greatsword";
                            Stats["Strength"] += 8;
                            break;
                        case 20145:
                            Name = "Mythril Hammer";
                            Stats["Strength"] += 10;
                            Stats["Magic"] += -3;
                            break;
                        case 20150:
                            Name = "Warrior Shield";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 4;
                            Stats["MagicDefense"] += 1;
                            break;
                        case 25110:
                            Name = "Mythril Helmet";
                            Stats["Strength"] += 2;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 25120:
                            Name = "Mythril Vest";
                            Stats["Strength"] += 2;
                            Stats["Defense"] += 3;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 25130:
                            Name = "Mythril Leggings";
                            Stats["Strength"] += 2;
                            Stats["Magic"] += 1;
                            Stats["Defense"] += 1;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 25140:
                            Name = "Fire Sword";
                            Stats["Strength"] += 6;
                            Stats["Magic"] += 3;
                            Element = 1;
                            break;
                        case 25145:
                            Name = "Holy Hammer";
                            Stats["MaxMana"] += 5;
                            Stats["Strength"] += 8;
                            Stats["Magic"] += 2;
                            break;
                        case 25150:
                            Name = "Rubber Shield";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 3;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 20210:
                            Name = "Royal Cap";
                            Stats["Agility"] += 2;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 20220:
                            Name = "Royal Vest";
                            Stats["Agility"] += 3;
                            Stats["Defense"] += 3;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 20230:
                            Name = "Royal Leggings";
                            Stats["Agility"] += 2;
                            Stats["Defense"] += 1;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 20240:
                            Name = "Royal Longbow";
                            Stats["Strength"] += 5;
                            Stats["Agility"] += 8;
                            break;
                        case 20245:
                            Name = "Huntsman's Shortbow";
                            Stats["Strength"] += 3;
                            Stats["Magic"] += 4;
                            Stats["Agility"] += 6;
                            break;
                        case 20250:
                            Name = "Royal Quiver";
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 4;
                            break;
                        case 25210:
                            Name = "Wanderer's Hat";
                            Stats["MaxMana"] += 5;
                            Stats["Agility"] += 3;
                            Stats["Defense"] += 1;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 25220:
                            Name = "Wanderer's Clothes";
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 2;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 25230:
                            Name = "Wanderer's Shorts";
                            Stats["Magic"] += 1;
                            Stats["Agility"] += 1;
                            Stats["Defense"] += 1;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 25240:
                            Name = "Spiky Longbow";
                            Stats["MaxMana"] += 5;
                            Stats["Strength"] += 6;
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 5;
                            break;
                        case 25245:
                            Name = "Frozen Shortbow";
                            Stats["MaxHP"] += 10;
                            Stats["Strength"] += 5;
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 4;
                            Element = 3;
                            break;
                        case 25250:
                            Name = "Lava Quiver";
                            Stats["Magic"] += 3;
                            Stats["Agility"] += 3;
                            Element = 1;
                            break;
                        case 20310:
                            Name = "Cut-up Hood";
                            Stats["Magic"] += 2;
                            Stats["MagicDefense"] += 5;
                            break;
                        case 20320:
                            Name = "Cut-up Cloak";
                            Stats["MaxMana"] += 10;
                            Stats["Magic"] += 3;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 6;
                            break;
                        case 20330:
                            Name = "Cut-up Kilt";
                            Stats["MaxMana"] += 5;
                            Stats["Magic"] += 2;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 20340:
                            Name = "Emerald Staff";
                            Stats["MaxMana"] += 5;
                            Stats["Strength"] += 5;
                            Stats["Magic"] += 10;
                            break;
                        case 20345:
                            Name = "Water Wand";
                            Stats["MaxMana"] += 15;
                            Stats["Strength"] += 6;
                            Stats["Magic"] += 7;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 20350:
                            Name = "Scholar's Tome";
                            Stats["MaxMana"] += 10;
                            Stats["Magic"] += 3;
                            break;
                        case 25310:
                            Name = "Crystal Hood";
                            Stats["Magic"] += 1;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 25320:
                            Name = "Crystal Cloak";
                            Stats["MaxMana"] += 15;
                            Stats["Magic"] += 5;
                            Stats["Defense"] += -2;
                            Stats["MagicDefense"] += 8;
                            break;
                        case 25330:
                            Name = "Crystal Kilt";
                            Stats["MaxMana"] += 15;
                            Stats["Strength"] += -2;
                            Stats["Magic"] += 3;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 25340:
                            Name = "Crooked Staff";
                            Stats["Torch"] += 1;
                            Stats["MaxMana"] += 15;
                            Stats["Strength"] += 6;
                            Stats["Magic"] += 8;
                            break;
                        case 25345:
                            Name = "Flame Wand";
                            Stats["Torch"] += 1;
                            Stats["MaxMana"] += 10;
                            Stats["Strength"] += 3;
                            Stats["Magic"] += 7;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 25350:
                            Name = "Enchanted Tome";
                            Stats["MaxMana"] += 15;
                            Stats["Strength"] += -2;
                            Stats["Magic"] += 3;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 30110:
                            Name = "Hardened Helmet";
                            Stats["MaxHP"] += 5;
                            Stats["Strength"] += 3;
                            Stats["Magic"] += -1;
                            Stats["Defense"] += 8;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 30120:
                            Name = "Hardened Armor";
                            Stats["MaxHP"] += 10;
                            Stats["MaxMana"] += -5;
                            Stats["Strength"] += 3;
                            Stats["Defense"] += 10;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 30130:
                            Name = "Hardened Leggings";
                            Stats["Strength"] += 3;
                            Stats["Defense"] += 6;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 30140:
                            Name = "Hardened Sword";
                            Stats["Strength"] += 13;
                            Stats["Defense"] += 2;
                            break;
                        case 30145:
                            Name = "Obsidian Hammer";
                            Stats["MaxMana"] += -5;
                            Stats["Strength"] += 16;
                            Stats["Magic"] += -5;
                            Stats["Defense"] += 4;
                            break;
                        case 30150:
                            Name = "Tower Shield";
                            Stats["MaxHP"] += 5;
                            Stats["Strength"] += 2;
                            Stats["Defense"] += 7;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 35110:
                            Name = "Dragon Helmet";
                            Stats["Strength"] += 3;
                            Stats["Magic"] += 1;
                            Stats["Defense"] += 5;
                            Stats["MagicDefense"] += 6;
                            Stats["HPRegen"] += -1;
                            break;
                        case 35120:
                            Name = "Dragon Armor";
                            Stats["MaxHP"] += 5;
                            Stats["MaxMana"] += 5;
                            Stats["Strength"] += 3;
                            Stats["Defense"] += 7;
                            Stats["MagicDefense"] += 6;
                            Stats["HPRegen"] += -2;
                            break;
                        case 35130:
                            Name = "Dragon Leggings";
                            Stats["Torch"] += 1;
                            Stats["MaxHP"] += 5;
                            Stats["Strength"] += 3;
                            Stats["Defense"] += 4;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 35140:
                            Name = "Obsidian Sword";
                            Stats["Strength"] += 10;
                            Stats["Defense"] += 3;
                            Stats["MagicDefense"] += 2;
                            Stats["ManaRegen"] += -2;
                            break;
                        case 35145:
                            Name = "Milion Pounds Hammer";
                            Stats["Torch"] += -1;
                            Stats["MaxMana"] += -10;
                            Stats["Strength"] += 20;
                            Stats["Magic"] += -5;
                            Stats["Defense"] += 6;
                            break;
                        case 35150:
                            Name = "Reflective Shield";
                            Stats["MaxHP"] += 5;
                            Stats["Defense"] += 7;
                            Stats["MagicDefense"] += 4;
                            Stats["HPRegen"] += -1;
                            Stats["ManaRegen"] += -2;
                            break;
                        case 30210:
                            Name = "Huntsman's Hat";
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 3;
                            Stats["Defense"] += 4;
                            Stats["MagicDefense"] += 5;
                            break;
                        case 30220:
                            Name = "Huntsman's Vest";
                            Stats["MaxMana"] += 10;
                            Stats["Agility"] += 3;
                            Stats["Defense"] += 6;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 30230:
                            Name = "Huntsman's Leggings";
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 3;
                            Stats["Defense"] += 4;
                            Stats["MagicDefense"] += 3;
                            break;
                        case 30240:
                            Name = "Huntsman's Longbow";
                            Stats["MaxMana"] += 5;
                            Stats["Strength"] += 8;
                            Stats["Agility"] += 12;
                            break;
                        case 30245:
                            Name = "Perfected Shortbow";
                            Stats["Torch"] += 1;
                            Stats["MaxMana"] += 5;
                            Stats["Strength"] += 6;
                            Stats["Magic"] += 3;
                            Stats["Agility"] += 10;
                            break;
                        case 30250:
                            Name = "Crystal Quiver";
                            Stats["Strength"] += 2;
                            Stats["Magic"] += 3;
                            Stats["Agility"] += 6;
                            break;
                        case 35210:
                            Name = "Camo Hat";
                            Stats["Torch"] += 1;
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 4;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 2;
                            break;
                        case 35220:
                            Name = "Camo Vest";
                            Stats["Magic"] += 1;
                            Stats["Agility"] += 3;
                            Stats["Defense"] += 4;
                            Stats["MagicDefense"] += 6;
                            Stats["ManaRegen"] += -1;
                            break;
                        case 35230:
                            Name = "Camo Leggings";
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 3;
                            Stats["Defense"] += 3;
                            Stats["HPRegen"] += -1;
                            break;
                        case 35240:
                            Name = "Golden Longbow";
                            Stats["Torch"] += 1;
                            Stats["Strength"] += 5;
                            Stats["Agility"] += 10;
                            Element = 4;
                            break;
                        case 35245:
                            Name = "Crystal Shortbow";
                            Stats["Torch"] += 3;
                            Stats["Strength"] += 3;
                            Stats["Magic"] += 5;
                            Stats["Agility"] += 7;
                            Stats["HPRegen"] += -1;
                            Stats["ManaRegen"] += -2;
                            break;
                        case 35250:
                            Name = "Enchanted Quiver";
                            Stats["Torch"] += 2;
                            Stats["Strength"] += 3;
                            Stats["Agility"] += 4;
                            Stats["HPRegen"] += -1;
                            Stats["ManaRegen"] += -1;
                            break;
                        case 30310:
                            Name = "Wizard's Hood";
                            Stats["MaxMana"] += 10;
                            Stats["Magic"] += 3;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 7;
                            break;
                        case 30320:
                            Name = "Wizard's Cloak";
                            Stats["MaxMana"] += 15;
                            Stats["Magic"] += 3;
                            Stats["Defense"] += 3;
                            Stats["MagicDefense"] += 9;
                            break;
                        case 30330:
                            Name = "Wizard's Kilt";
                            Stats["MaxMana"] += 10;
                            Stats["Magic"] += 3;
                            Stats["Defense"] += 1;
                            Stats["MagicDefense"] += 6;
                            break;
                        case 30340:
                            Name = "Ruby Staff";
                            Stats["Torch"] += 1;
                            Stats["MaxMana"] += 10;
                            Stats["Strength"] += 8;
                            Stats["Magic"] += 15;
                            Stats["Defense"] += 2;
                            Stats["ManaRegen"] += -1;
                            break;
                        case 30345:
                            Name = "Arkhana Wand";
                            Stats["Torch"] += 1;
                            Stats["MaxMana"] += 20;
                            Stats["Strength"] += 10;
                            Stats["Magic"] += 10;
                            Stats["Defense"] += 3;
                            Stats["MagicDefense"] += 4;
                            Stats["HPRegen"] += -1;
                            break;
                        case 30350:
                            Name = "Wizard's Tome";
                            Stats["MaxMana"] += 15;
                            Stats["Magic"] += 5;
                            Stats["MagicDefense"] += 1;
                            Stats["ManaRegen"] += -1;
                            break;
                        case 35310:
                            Name = "Dark Mage's Hood";
                            Stats["Torch"] += 2;
                            Stats["MaxMana"] += 5;
                            Stats["Magic"] += 3;
                            Stats["Defense"] += 4;
                            Stats["MagicDefense"] += 4;
                            break;
                        case 35320:
                            Name = "Dark Mage's Cloak";
                            Stats["MaxMana"] += 10;
                            Stats["Magic"] += 3;
                            Stats["Defense"] += 4;
                            Stats["MagicDefense"] += 7;
                            Stats["ManaRegen"] += -2;
                            break;
                        case 35330:
                            Name = "Dark Mage's Kilt";
                            Stats["Torch"] += 2;
                            Stats["Magic"] += 3;
                            Stats["Defense"] += 3;
                            Stats["MagicDefense"] += 3;
                            Stats["HPRegen"] += -2;
                            Stats["ManaRegen"] += -1;
                            break;
                        case 35340:
                            Name = "Shattered Staff";
                            Stats["MaxMana"] += 30;
                            Stats["Magic"] += 20;
                            Stats["Defense"] += -6;
                            Stats["MagicDefense"] += -8;
                            Stats["ManaRegen"] += -2;
                            Stats["Strength"] += 10;
                            break;
                        case 35345:
                            Name = "Two-sided Wand";
                            Stats["Torch"] += 2;
                            Stats["MaxMana"] += 10;
                            Stats["Strength"] += 15;
                            Stats["Magic"] += 15;
                            Stats["Defense"] += -4;
                            Stats["MagicDefense"] += -4;
                            Stats["HPRegen"] += 1;
                            Stats["ManaRegen"] += -2;
                            break;
                        case 35350:
                            Name = "Forbidden Tome";
                            Stats["Torch"] += 1;
                            Stats["MaxMana"] += 20;
                            Stats["Magic"] += 3;
                            Stats["ManaRegen"] += -2;
                            break;
                        case 40110:
                            Name = "Paladin's Helmet";
                            Stats["MaxHP"] += 10;
                            Stats["Strength"] += 4;
                            Stats["Magic"] += -2;
                            Stats["Agility"] += -2;
                            Stats["Defense"] += 10;
                            Stats["MagicDefense"] += 5;
                            Stats["HPRegen"] += -1;
                            break;
                        case 40120:
                            Name = "Paladin's Armor";
                            Stats["MaxHP"] += 15;
                            Stats["MaxMana"] += -5;
                            Stats["Strength"] += 4;
                            Stats["Agility"] += -1;
                            Stats["Defense"] += 15;
                            Stats["MagicDefense"] += 6;
                            Stats["HPRegen"] += -2;
                            break;
                        case 40130:
                            Name = "Paladin's Leggings";
                            Stats["MaxHP"] += 10;
                            Stats["MaxMana"] += -5;
                            Stats["Strength"] += 4;
                            Stats["Agility"] += -1;
                            Stats["Defense"] += 8;
                            Stats["MagicDefense"] += 4;
                            Stats["HPRegen"] += -1;
                            break;
                        case 40140:
                            Name = "Sword Of Legends";
                            Stats["Strength"] += 15;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 2;
                            Stats["HPRegen"] += -1;
                            break;
                        case 40145:
                            Name = "Mace Of Legends";
                            Stats["MaxHP"] += 5;
                            Stats["Strength"] += 20;
                            Stats["Magic"] += -3;
                            Stats["Agility"] += -3;
                            Stats["Defense"] += 4;
                            Stats["HPRegen"] += 1;
                            Stats["ManaRegen"] += 2;
                            break;
                        case 40150:
                            Name = "Unbreakable Shield";
                            Stats["Strength"] += 3;
                            Stats["Defense"] += 10;
                            Stats["MagicDefense"] += 2;
                            Stats["HPRegen"] += -2;
                            break;
                        case 40210:
                            Name = "Hawk Hat";
                            Stats["Torch"] += 2;
                            Stats["Magic"] += 3;
                            Stats["Agility"] += 4;
                            Stats["Defense"] += 7;
                            Stats["MagicDefense"] += 6;
                            break;
                        case 40220:
                            Name = "Hawk Scarf";
                            Stats["Torch"] += 1;
                            Stats["MaxMana"] += 10;
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 4;
                            Stats["Defense"] += 9;
                            Stats["MagicDefense"] += 6;
                            break;
                        case 40230:
                            Name = "Hawk Belt";
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 4;
                            Stats["Defense"] += 7;
                            Stats["MagicDefense"] += 4;
                            Stats["ManaRegen"] += -1;
                            break;
                        case 40240:
                            Name = "Living Longbow";
                            Stats["MaxMana"] += 5;
                            Stats["Strength"] += 10;
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 15;
                            Stats["Defense"] += 2;
                            break;
                        case 40245:
                            Name = "Full Metal Shortbow";
                            Stats["Torch"] += 3;
                            Stats["MaxMana"] += 15;
                            Stats["Strength"] += 7;
                            Stats["Magic"] += 4;
                            Stats["Agility"] += 12;
                            Stats["Defense"] += 1;
                            Stats["HPRegen"] += -1;
                            break;
                        case 40250:
                            Name = "Black Feather Quiver";
                            Stats["MaxMana"] += 5;
                            Stats["Strength"] += 2;
                            Stats["Agility"] += 8;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 2;
                            Stats["ManaRegen"] += -1;
                            break;
                        case 40310:
                            Name = "Forgotten Hood";
                            Stats["Torch"] += 2;
                            Stats["MaxMana"] += 20;
                            Stats["Magic"] += 4;
                            Stats["Defense"] += 4;
                            Stats["MagicDefense"] += 12;
                            Stats["ManaRegen"] += -1;
                            break;
                        case 40320:
                            Name = "Forgotten Cape";
                            Stats["Torch"] += 2;
                            Stats["MaxMana"] += 20;
                            Stats["Magic"] += 4;
                            Stats["Defense"] += 5;
                            Stats["MagicDefense"] += 15;
                            Stats["ManaRegen"] += -1;
                            break;
                        case 40330:
                            Name = "Mana Belt";
                            Stats["Torch"] += 1;
                            Stats["MaxMana"] += 30;
                            Stats["Magic"] += 4;
                            Stats["Defense"] += 3;
                            Stats["MagicDefense"] += 7;
                            Stats["ManaRegen"] += -2;
                            break;
                        case 40340:
                            Name = "Rainbow Staff";
                            Stats["Torch"] += 1;
                            Stats["MaxMana"] += 15;
                            Stats["Strength"] += 10;
                            Stats["Magic"] += 20;
                            Stats["Defense"] += 3;
                            Stats["ManaRegen"] += -2;
                            break;
                        case 40345:
                            Name = "Refraction Wand";
                            Stats["Torch"] += 3;
                            Stats["MaxMana"] += 30;
                            Stats["Strength"] += 15;
                            Stats["Magic"] += 15;
                            Stats["Defense"] += 5;
                            Stats["MagicDefense"] += 3;
                            Stats["ManaRegen"] += -3;
                            break;
                        case 40350:
                            Name = "Balance Tome";
                            Stats["MaxMana"] += 20;
                            Stats["Magic"] += 6;
                            Stats["Defense"] += 3;
                            Stats["MagicDefense"] += 3;
                            Stats["HPRegen"] += -2;
                            Stats["ManaRegen"] += -2;
                            break;
                    }


                    break;
                case 10:
                    if (rand.Next(2) == 0)
                    {
                        Element = rand.Next(4);
                    }
                    File = "Graphics/ByteLikeGraphics/Items/arrow";
                    File += Element;
                    Name = "";
                    switch (Element)
                    {
                        case 1:
                            Name += "Fire ";
                            break;
                        case 2:
                            Name += "Poison ";
                            break;
                        case 3:
                            Name += "Ice ";
                            break;
                        case 4:
                            Name += "Lightning";
                            break;
                    }
                    Name += "Arrow";
                    File += ".png";
                    Stats["Agility"] = 3;
                    Spell = "Shoot Arrow";
                    Quantity = (rand.Next(3) + 1) * 5;
                    break;
            }

            string[] statnames = new string[] { "Light", "HP", "Mana", "Str", "Mag", "Agi", "Def", "Mg Def", "HP Regen", "Mana Regen" };
            int pos = 0;
            bool first = true;

            foreach (KeyValuePair<string,int> item in Stats)
            {
                int value = item.Value;
                if (item.Key.Contains("Regen"))
                    value = -value;
                if (first)
                {
                    if (value > 0)
                    {
                        Description += string.Format("+{0} {1}", value, statnames[pos]);
                        first = false;
                    }
                    else if (value < 0)
                    {
                        Description += string.Format("{0} {1}", value, statnames[pos]);
                        first = false;
                    }
                }
                else
                {
                    if (value > 0)
                        Description += string.Format(", +{0} {1}", value, statnames[pos]);
                    else if (value < 0)
                        Description += string.Format(", {0} {1}", value, statnames[pos]);
                }
                pos++;
            }

            Description += "\n";
            switch (Element)
            {
                case 1:
                    Description += "Fire Attack\n";
                    break;
                case 2:
                    Description += "Poison Attack\n";
                    break;
                case 3:
                    Description += "Freezing Attack\n";
                    break;
                case 4:
                    Description += "Paralyzing Attack\n";
                    break;
            }

        }
    }



    public class Chest
    {
        public Item[,] Inventory = new Item[11, 7];

        public string File = "Graphics/ByteLikeGraphics/chest0.png";

        public int[] position = new int[2];

        static Random rand = new Random();
        public bool IsEmpty()
        {
            bool check = true;
            for (int i = 0; i < Inventory.GetLength(1); i++)
            {
                for (int j = 0; j < Inventory.GetLength(0); j++)
                {
                    if (Inventory[j, i] != null)
                    {
                        check = false;
                    }
                }
            }
            return check;
        }

        public Chest(int[] coordinates, int floor)
        {
            position[0] = coordinates[0];
            position[1] = coordinates[1];

            if (floor > 0)
            {
                for (int i = 0; i < Inventory.GetLength(1); i++)
                {
                    for (int j = 0; j < Inventory.GetLength(0); j++)
                    {
                        if (rand.Next(20) == 0)
                        {
                            Inventory[j, i] = new Item(floor);
                        }
                    }
                }
            }

        }

        public void CopyInto(Item[,] inventory)
        {
            for (int i = 0; i < inventory.GetLength(1); i++)
            {
                for (int j = 0; j < inventory.GetLength(0); j++)
                {
                    Inventory[j, i] = inventory[j, i];
                }
            }
        }

        public Item PutIn(Item item, int[] slot)
        {
            if (item != null)
            {
                if (item.GearType > 9 || item.GearType == 0)
                {
                    if (Inventory[slot[0], slot[1]] != null)
                    {
                        if (Inventory[slot[0], slot[1]].Name == item.Name)
                        {
                            Inventory[slot[0], slot[1]].Quantity += item.Quantity;
                            return null;
                        }
                        else
                        {
                            Item temp2 = Inventory[slot[0], slot[1]];
                            Inventory[slot[0], slot[1]] = item;
                            return temp2;
                        }
                    }
                    else
                    {
                        Inventory[slot[0], slot[1]] = item;
                        return null;
                    }
                }
            }
            Item temp = Inventory[slot[0], slot[1]];
            Inventory[slot[0], slot[1]] = item;

            return temp;
        }

        public Item TakeOut(Item item, int[] slot)
        {
            if (item != null)
            {
                if (item.GearType > 9 || item.GearType == 0)
                {
                    if (Inventory[slot[0], slot[1]] != null)
                    {
                        if (Inventory[slot[0], slot[1]].Name == item.Name)
                        {
                            item.Quantity += Inventory[slot[0], slot[1]].Quantity;
                            Inventory[slot[0], slot[1]] = null;
                            return item;
                        }
                        else
                        {
                            Item temp = Inventory[slot[0], slot[1]];
                            Inventory[slot[0], slot[1]] = item;
                            return temp;
                        }
                    }
                    else
                    {
                        Inventory[slot[0], slot[1]] = item;
                        return null;
                    }
                }
            }
            else if (Inventory[slot[0], slot[1]] != null)
            {
                Item temp2 = Inventory[slot[0], slot[1]];
                Inventory[slot[0], slot[1]] = null;
                return temp2;
            }
            return item;
        }

        public void Swap(int[] CurrentSlot, int[] SelectedSlot)
        {
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
        }

        public bool AddTo(Item item)
        {
            if (item == null)
            {
                return true;
            }

            if (item.GearType > 9 || item.GearType == 0)
            {
                for (int i = 0; i < Inventory.GetLength(1); i++)
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

            for (int i = 0; i < Inventory.GetLength(1); i++)
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
    }


}
