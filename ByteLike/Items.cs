using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteLike
{


    public class Item
    {
        // 0 - Useless, 1 - Head, 2 - Torso, 3 - Legs, 4 - Weapon, 5 - OffHand, 6 - Torch, 7 - Necklace, 8 - Ring, 9 - (For safety, none), 10 - Usable/Ammo
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

        public static int GetSpellElement(string Spell)
        {
            switch (Spell)
            {
                case "Ember":
                case "Shoot Fire Arrow":
                case "Fireball":
                case "Meteor":
                case "Lavafy":
                case "Erruption":
                    return 1;
                case "Posion Sting":
                case "Shoot Poison Arrow":
                case "Sludge Bomb":
                case "Ivy Growth":
                case "Plague Bomb":
                case "Forest Growth":
                    return 2;
                case "Ice Shard":
                case "Shoot Ice Arrow":
                case "Ice Storm":
                case "Liquify":
                case "Blizzard":
                case "Tsunami":
                    return 3;
                case "Zap":
                case "Shoot Lightning Arrow":
                case "Electro Bolt":
                case "Charge":
                case "Thunder":
                case "Electrify":
                    return 4;
                default:
                    return 0;
            }
        }

        public Item(int floor, int type)
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

            if (type == 0)
            {
                GearType = 10;
                if (rand.Next(5) == 0) { GearType = rand.Next(6) + 1; }
            }
            else { GearType = type; }

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
                    int strength = (floor / 30) + rand.Next(2);
                    if (floor < 0)
                        strength = 0;
                    if (strength < 0) { strength = 0; }
                    else if (strength > 4) { strength = 4; }
                    if (rand.Next(2) == 0 && strength != 0 && strength != 4)
                    {
                        sideB = true;
                        if (strength > 3) { strength = 3; }
                        typeSwitch += 5000;
                    }
                    typeSwitch += strength * 10000;


                    Stats["MaxHP"] += rand.Next(0, strength + 1) * rand.Next(4);
                    Stats["MaxMana"] += rand.Next(0, strength + 1) * rand.Next(4);
                    Stats["Strength"] += rand.Next(0, strength + 1);
                    Stats["Magic"] += rand.Next(0, strength + 1);
                    Stats["Agility"] += rand.Next(0, strength + 1);
                    Stats["Defense"] += rand.Next(0, strength + 1);
                    Stats["MagicDefense"] += rand.Next(0, strength + 1);

                    if (strength == 0)
                    {
                        GearType = rand.Next(4) + 1;
                        sideB = false;
                    }

                    File = "Graphics/ByteLikeGraphics/Armor/armor" + strength.ToString() + "-" + GearType.ToString();
                    typeSwitch += GearType * 10;
                    if (GearType == 4 && rand.Next(2) == 0)
                    {
                        File += "b";
                        typeSwitch += 5;
                    }
                    if (strength > 0)
                    {
                        switch (rand.Next(0,3))
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
                            Description = "A hat made from leather scraps. A bit too big for you\n";
                            break;
                        case 20:
                            Name = "Leather Vest";
                            Stats["Defense"] += 1;
                            Description = "A vest made from leather scraps. Usually worn underneath proper armor\n";
                            break;
                        case 30:
                            Name = "Leather Boots";
                            Stats["MagicDefense"] += 1;
                            Description = "Boots made from leather scraps. The stitching could be better\n";
                            break;
                        case 40:
                            Name = "Makeshift Spear";
                            Stats["Strength"] += 2;
                            Stats["Magic"] += 1;
                            Description = "A day-to-day spear. Usually used for hunting, but requires sharpening\n";
                            break;
                        case 45:
                            Name = "Makeshift Hatchet";
                            Stats["Strength"] += 3;
                            Description = "Somewhat dull hatchet. Could probably still chop a log or two\n";
                            break;
                        case 10110:
                            Name = "Iron Cap";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 2;
                            Description = "A common helmet for travelers. No more gravel falling on your head!\n";
                            break;
                        case 10120:
                            Name = "Chainmail";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 3;
                            Description = "Lightweight, but still protects from a blade\n";
                            break;
                        case 10130:
                            Name = "Leather Leggings";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 1;
                            Description = "Underwear for paladins! Perfect for a battlefield\n";
                            break;
                        case 10140:
                            Name = "Longsword";
                            Stats["Strength"] += 5;
                            Description = "The same blade knights receive at the age of 10. Despite not being sharpened properly, still pretty good or a fight\n";
                            break;
                        case 10145:
                            Name = "Warhammer";
                            Stats["Strength"] += 6;
                            Stats["Magic"] += -2;
                            Description = "A large hammer. Way too heavy for blacksmithing, but smashes bones with extra effectiveness\n";
                            break;
                        case 10150:
                            Name = "Buckler";
                            Stats["Defense"] += 2;
                            Description = "A common shield for travelers. Isn't that durable, but will protect from a stray arrow\n";
                            break;
                        case 15110:
                            Name = "Enchanted Cap";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 1;
                            Stats["MagicDefense"] += 1;
                            Description = "A helmet infused with magic. It has a strange aura around it\n";
                            break;
                        case 15120:
                            Name = "Enchanted Chainmail";
                            Stats["Strength"] += 1;
                            Stats["MagicDefense"] += 2;
                            Description = "A great mix between magical and protective. Was the blacksmith practicing mana arts on the job?\n";
                            break;
                        case 15130:
                            Name = "Enchanted Leggings";
                            Stats["Strength"] += 1;
                            Stats["MagicDefense"] += 1;
                            Description = "Leggings weaved with an enchanted thread. Despite all the smol torns, the tag is still fully in tact\n";
                            break;
                        case 15140:
                            Name = "Posioned Sword";
                            Stats["Strength"] += 4;
                            Stats["MagicDefense"] += -1;
                            Description = "A blade of evil, dunked into some kind of poison. Be very careful not to touch the dried poison yourself\n";
                            Element = 2;
                            break;
                        case 15145:
                            Name = "Thunder Hammer";
                            Stats["Strength"] += 5;
                            Stats["Magic"] += -3;
                            Description = "An enchanted hammer once struck by a lightning. You can feel the handle pulsate\n";
                            Element = 4;
                            break;
                        case 15150:
                            Name = "Spiky Buckler";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 1;
                            Description = "The spikes aren't the best for its structure, but they help deliver an extra punch\n";
                            break;
                        case 10210:
                            Name = "Cloth Hat";
                            Stats["Agility"] += 1;
                            Stats["Defense"] += 1;
                            Description = "The thread is a mix between cotton and wool. It's warm and helps with keeping your hair from your eyes\n";
                            break;
                        case 10220:
                            Name = "Cloth Shirt";
                            Stats["Agility"] += 1;
                            Stats["Defense"] += 2;
                            Description = "An attempt to make a warm doublet. Despite being torn at the sides, still helps keep your hands from shaking\n";
                            break;
                        case 10230:
                            Name = "Cloth Leggings";
                            Stats["Agility"] += 1;
                            Stats["MagicDefense"] += 1;
                            Description = "The soles have fine leather stitched to them. Perfect for that extra grip on the ground while not getting in the way\n";
                            break;
                        case 10240:
                            Name = "Longbow";
                            Stats["Strength"] += 3;
                            Stats["Agility"] += 5;
                            Description = "These bows are home-made. It's said you can hear it tell its tales when you shoot an arrow from it\n";
                            break;
                        case 10245:
                            Name = "Shortbow";
                            Stats["Strength"] += 1;
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 4;
                            Description = "Preffered for their compactness. It's never a bad idea to chug one in your bag if you're going on an adventure\n";
                            break;
                        case 10250:
                            Name = "Basic Quiver";
                            Stats["Agility"] += 2;
                            Description = "A medium sized quiver. No more reaching in your bag for arrows!\n";
                            break;
                        case 15210:
                            Name = "Bandit Bandana";
                            Stats["Agility"] += 1;
                            Stats["MagicDefense"] += 1;
                            Description = "It's said the king wanted to stop the production of such wares, but too many of his own men were in on it\n";
                            break;
                        case 15220:
                            Name = "Bandit Shawl";
                            Stats["Agility"] += 2;
                            Stats["Defense"] += 1;
                            Stats["MagicDefense"] += 1;
                            Description = "If you feel creepy wearing this, just imagine it's dark magic and not the souls of those killed by the previous owner\n";
                            break;
                        case 15230:
                            Name = "Bandit Pants";
                            Stats["Agility"] += 1;
                            Stats["Defense"] += 1;
                            Description = "It's only a myth that bandits don't shower... Right?\n";
                            break;
                        case 15240:
                            Name = "Crystal Bow";
                            Stats["Strength"] += 4;
                            Stats["Magic"] += 3;
                            Stats["Agility"] += 4;
                            Description = "This bow was made in a family of sorcerers. You can still read the runes on the crystals\n";
                            break;
                        case 15245:
                            Name = "Poison Shortbow";
                            Stats["Strength"] += 2;
                            Stats["Agility"] += 3;
                            Description = "The arrow shelf is extra wide to make sure the poison doesn't drip to your hand\n";
                            Element = 2;
                            break;
                        case 15250:
                            Name = "Scorpion Quiver";
                            Stats["Agility"] += 1;
                            Stats["MagicDefense"] += 1;
                            Description = "The belief that scorpion quivers have poison in them started after one man wrongfully assumed so and died from drinking an antidote after\n";
                            break;
                        case 10310:
                            Name = "Apprentice Hood";
                            Stats["Magic"] += 1;
                            Stats["MagicDefense"] += 1;
                            Description = "A hood given to all who join the arkhana school. It has runes that dull out the magic so you wouldn't hurt yourself\n";
                            break;
                        case 10320:
                            Name = "Apprentice Cloak";
                            Stats["Magic"] += 2;
                            Stats["Defense"] += 1;
                            Description = "Many mages start with these cloaks. The enchanted thread helps channel your mana into your staff or wand\n";
                            break;
                        case 10330:
                            Name = "Apprentice Kilt";
                            Stats["Magic"] += 1;
                            Stats["MagicDefense"] += 1;
                            Description = "There's a rumor that women are generally better at magic because they're not as ashamed of wearing mana kilts\n";
                            break;
                        case 10340:
                            Name = "Saphire Staff";
                            Stats["Strength"] += 3;
                            Stats["Magic"] += 6;
                            Description = "A thick stick with a polished saphire on the tip. The saphire glows with every spell you cast\n";
                            break;
                        case 10345:
                            Name = "Ember Wand";
                            Stats["Strength"] += 2;
                            Stats["Magic"] += 4;
                            Stats["MagicDefense"] += 2;
                            Description = "One of the most basic wands. Rumor has it that the ember is actually just a decoration, but nobody's brave enough to check\n";
                            break;
                        case 10350:
                            Name = "Commoner's Tome";
                            Stats["Magic"] += 2;
                            Description = "A tome perfect for writing down spells. Being able to read the spell instead of remembering it helps concentrate\n";
                            break;
                        case 15310:
                            Name = "Bug Master Hood";
                            Stats["MaxMana"] += 10;
                            Stats["Defense"] += 1;
                            Description = "Bug Masters are a strange group of sorceres in the west. They don't use tome's, forks or anything made from oak\n";
                            break;
                        case 15320:
                            Name = "Bug Master CLoak";
                            Stats["Magic"] += 1;
                            Stats["Defense"] += 2;
                            Description = "Bug Masters use thin metal string instead of cotton for their cloaks. Travelers are often unprepared for their extra defenses\n";
                            break;
                        case 15330:
                            Name = "Bug Master Kilt";
                            Stats["MaxMana"] += 5;
                            Stats["Defense"] += 1;
                            Description = "A waterproof kilt from the west. The wings emmit a buzz whenever you channel mana to your staff/wand\n";
                            break;
                        case 15340:
                            Name = "Diamond Staff";
                            Stats["MaxMana"] += 20;
                            Stats["Strength"] += 3;
                            Stats["Magic"] += 5;
                            Description = "A second most popular use for diamonds! To make a diamond wand, you must polish the diamond with an enchanted cloth\n";
                            break;
                        case 15345:
                            Name = "Root Wand";
                            Stats["Strength"] += 2;
                            Stats["Magic"] += 4;
                            Stats["ManaRegen"] += -1;
                            Description = "A wand made from a cut off birch root. A living proof that a good mage can turn anything into a wand\n";
                            break;
                        case 15350:
                            Name = "Hidden Tome";
                            Stats["MaxMana"] += 10;
                            Description = "This tome contains strange symbols you can't read. There's still enough room for new spells to be written in\n";
                            break;
                        case 20110:
                            Name = "Iron Helmet";
                            Stats["Strength"] += 2;
                            Stats["Defense"] += 4;
                            Stats["MagicDefense"] += 2;
                            Description = "A helmet made for newbie guards of the palace. It's an honor to be taken into the royal army\n";
                            break;
                        case 20120:
                            Name = "Iron Vest";
                            Stats["Strength"] += 2;
                            Stats["Defense"] += 6;
                            IsHeavy = true;
                            Description = "A well piece of armor only royal guards may wear. Some guards request chainmail sleeves to be added to their vests\n";
                            break;
                        case 20130:
                            Name = "Iron Leggings";
                            Stats["Strength"] += 2;
                            Stats["Defense"] += 4;
                            Stats["MagicDefense"] += 1;
                            Description = "Royal piece of armor for legs. Despite popular belief, both men and women wear the same type of iron leggings\n";
                            break;
                        case 20140:
                            Name = "Greatsword";
                            Stats["Strength"] += 8;
                            Description = "A rather large sword. Knights must train their entire life to keep the strength to wield one\n";
                            break;
                        case 20145:
                            Name = "Mythril Hammer";
                            Stats["Strength"] += 10;
                            Stats["Magic"] += -3;
                            IsHeavy = true;
                            Description = "A warhammer coated with mystical metal. Mythril is very easy to polish\n";
                            break;
                        case 20150:
                            Name = "Warrior Shield";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 4;
                            Stats["MagicDefense"] += 1;
                            Description = "A fine shield usually used for war. Different kingdoms paint their warrior shield's in different colors\n";
                            break;
                        case 25110:
                            Name = "Mythril Helmet";
                            Stats["Strength"] += 2;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 4;
                            Description = "A helmet made from the mystical metal. Only royalty can afford to equip their army with mythril\n";
                            break;
                        case 25120:
                            Name = "Mythril Vest";
                            Stats["Strength"] += 2;
                            Stats["Defense"] += 3;
                            Stats["MagicDefense"] += 3;
                            IsHeavy = true;
                            Description = "A vest made from the mystical metal. Absence of any scratches on one's armor either means they're a rookie, or wearing mythril\n";
                            break;
                        case 25130:
                            Name = "Mythril Leggings";
                            Stats["Strength"] += 2;
                            Stats["Magic"] += 1;
                            Stats["Defense"] += 1;
                            Stats["MagicDefense"] += 4;
                            Description = "Leggings made from the mystical metal. Mythril leggings are usually oversized so knights could use one set for a long time\n";
                            break;
                        case 25140:
                            Name = "Fire Sword";
                            Stats["Strength"] += 5;
                            Stats["Magic"] += 3;
                            Stats["Torch"] += 1;
                            Element = 1;
                            Description = "A blade that spews out fire! Perfect for scorching your enemies and your food\n";
                            break;
                        case 25145:
                            Name = "Holy Hammer";
                            Stats["MaxMana"] += 5;
                            Stats["Strength"] += 8;
                            Stats["Magic"] += 2;
                            IsHeavy = true;
                            Description = "A hammer blessed by a priest. The undead aren't afraid of them anymore, however they're still made due to giving knights confidence\n";
                            break;
                        case 25150:
                            Name = "Rubber Shield";
                            Stats["Strength"] += 1;
                            Stats["Defense"] += 3;
                            Stats["MagicDefense"] += 4;
                            Description = "A shield with a rubber coating. Despite being less effective than reflective shield, they're still considered more prestige\n";
                            break;
                        case 20210:
                            Name = "Royal Cap";
                            Stats["Agility"] += 2;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 3;
                            Description = "A hat worn by royal archers. The silky interior feels luxurious\n";
                            break;
                        case 20220:
                            Name = "Royal Vest";
                            Stats["Agility"] += 3;
                            Stats["Defense"] += 3;
                            Stats["MagicDefense"] += 2;
                            Description = "Armor worn by royal archers. Pointy shoulders allow your arms to move more freely than in a regular platemail\n";
                            break;
                        case 20230:
                            Name = "Royal Leggings";
                            Stats["Agility"] += 2;
                            Stats["Defense"] += 1;
                            Stats["MagicDefense"] += 2;
                            Description = "Leggings worn by royal archers. Silk helps to combat the cold and magic attacks\n";
                            break;
                        case 20240:
                            Name = "Royal Longbow";
                            Stats["Strength"] += 5;
                            Stats["Agility"] += 8;
                            Description = "A bow crafted for the royal army. It has a tiny mark on the center on the string for better aiming\n";
                            break;
                        case 20245:
                            Name = "Huntsman's Shortbow";
                            Stats["Strength"] += 3;
                            Stats["Magic"] += 4;
                            Stats["Agility"] += 6;
                            Description = "A bow carried by novice hunters. When making a bow for hunting, fletchers ask the spirits of the forest for a blessing\n";
                            break;
                        case 20250:
                            Name = "Royal Quiver";
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 4;
                            Description = "A luxurious quiver sewn for the royal army. Its extra thick to make taking arrows out of it easier\n";
                            break;
                        case 25210:
                            Name = "Wanderer's Hat";
                            Stats["MaxMana"] += 5;
                            Stats["Agility"] += 3;
                            Stats["Defense"] += 1;
                            Stats["MagicDefense"] += 2;
                            Description = "A hat usualy worn by those who choose to live alone. It's good for any season or environment\n";
                            break;
                        case 25220:
                            Name = "Wanderer's Clothes";
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 2;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 3;
                            Description = "Clothes of some unfortunate adventurer. The soul of its previous owner still protects anybody who puts it on with respect\n";
                            break;
                        case 25230:
                            Name = "Wanderer's Shorts";
                            Stats["Magic"] += 1;
                            Stats["Agility"] += 1;
                            Stats["Defense"] += 1;
                            Stats["MagicDefense"] += 3;
                            Description = "Summer wears of some unfortunate adventurer. The boots are waterproof to combat rain\n";
                            break;
                        case 25240:
                            Name = "Spiky Longbow";
                            Stats["MaxMana"] += 5;
                            Stats["Strength"] += 6;
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 5;
                            Description = "A bow made made from spiky vines. Witches of the forest ask a fortune for these, but its worth it\n";
                            break;
                        case 25245:
                            Name = "Frozen Shortbow";
                            Stats["MaxHP"] += 10;
                            Stats["Strength"] += 5;
                            Stats["Magic"] += 2;
                            Stats["Agility"] += 4;
                            Description = "A bow made by a lonely sorcerer from the mountains. The ice crystals don't melt even under extreme heat\n";
                            Element = 3;
                            break;
                        case 25250:
                            Name = "Lava Quiver";
                            Stats["Magic"] += 3;
                            Stats["Agility"] += 3;
                            Description = "A quiver made by a lonely sorcerer from the mountains. The bottom is warm and metal on the inside\n";
                            Element = 1;
                            break;
                        case 20310:
                            Name = "Cut-up Hood";
                            Stats["Magic"] += 2;
                            Stats["MagicDefense"] += 5;
                            Description = "A hood from a foreign magic school. You see many different plants weaved in between all the fabric\n";
                            break;
                        case 20320:
                            Name = "Cut-up Cloak";
                            Stats["MaxMana"] += 10;
                            Stats["Magic"] += 3;
                            Stats["Defense"] += 2;
                            Stats["MagicDefense"] += 6;
                            Description = "A cloak from a foreign magic school. Yes, it's supposed to be worn like that\n";
                            break;
                        case 20330:
                            Name = "Cut-up Kilt";
                            Stats["MaxMana"] += 5;
                            Stats["Magic"] += 2;
                            Stats["MagicDefense"] += 4;
                            Description = "A kilt from a foreign magic school. Easters were the first to use kilts, they were always ahead in magic knowledge\n";
                            break;
                        case 20340:
                            Name = "Emerald Staff";
                            Stats["MaxMana"] += 5;
                            Stats["Strength"] += 5;
                            Stats["Magic"] += 10;
                            Description = "A staff with a rare gem on it. Easters will trade many wares for a single one of these\n";
                            break;
                        case 20345:
                            Name = "Water Wand";
                            Stats["MaxMana"] += 15;
                            Stats["Strength"] += 6;
                            Stats["Magic"] += 7;
                            Stats["MagicDefense"] += 4;
                            Description = "A wand with a liquid stone on it. The ''gem'' is actually just liquid keeping form of a gem. The liquid glows slightly\n";
                            break;
                        case 20350:
                            Name = "Scholar's Tome";
                            Stats["MaxMana"] += 10;
                            Stats["Magic"] += 3;
                            Description = "A tome given to many magic scolars. First few pages are filled with excersises to boost your mana\n";
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
                case 6:

                    File = "Graphics/ByteLikeGraphics/Items/torch";
                    typeSwitch = rand.Next(6);
                    if (floor < 0)
                        typeSwitch = 0;
                    File += typeSwitch;
                    File += ".png";
                    switch (typeSwitch)
                    {
                        case 0:
                            Name = "Makeshift Torch";
                            Stats["Torch"] += 1;
                            Description = "An old cloth doused in oil wrapped around a stick. Will do for now\n";
                            break;
                        case 1:
                            Name = "Torch";
                            Stats["Torch"] += 3;
                            while (rand.Next(10) == 0)
                                Stats["Torch"] += 1;
                            Description = "A regular torch\n";
                            break;
                        case 2:
                            Name = "Mana Torch";
                            Stats["Torch"] += 2;
                            Stats["MaxMana"] += (int)(rand.Next(8)+4 + floor/23);
                            Description = "A torch infused with some mana powder\n";
                            break;
                        case 3:
                            Name = "Heart Torch";
                            Stats["Torch"] += 2;
                            Stats["MaxHP"] += (int)(rand.Next(8) + 4 + floor / 23);
                            Description = "A crystal heart with a burning aura to it\n";
                            break;
                        case 4:
                            Name = "XP Torch";
                            Stats["Torch"] += 1;
                            Description = "A magical torch that seems to make you learn faster\n";
                            break;
                        case 5:
                            Name = "Rage Torch";
                            Stats["Torch"] += 2;
                            Stats["Strength"] += (int)(rand.Next(6) + 2 + floor / 23);
                            Description = "A torch with a small pouch of hellroot in it\n";
                            break;
                    }

                    while (rand.Next(15) <= (floor/23)+1)
                        Stats["Torch"] += 1;

                    break;
                case 10:
                    int itemtype = rand.Next(27);
                    switch (itemtype)
                    {
                        case 0:
                            File = "Graphics/ByteLikeGraphics/Items/food0.png";
                            Name = "Apple";
                            Description = "A regular apple. A good snack in a pinch\n";
                            Stats["MaxHP"] = 3;
                            break;
                        case 1:
                            File = "Graphics/ByteLikeGraphics/Items/food1.png";
                            Name = "Bread Stick";
                            Description = "A stick of fine bread. Food that unites fools and gods\n";
                            Stats["MaxHP"] = 5;
                            break;
                        case 2:
                            File = "Graphics/ByteLikeGraphics/Items/food2.png";
                            Name = "Small Meal";
                            Description = "A small meal composed of peas, oats, a small bun and a glass of vegetable juice\n";
                            Stats["MaxHP"] = 10;
                            break;
                        case 3:
                            File = "Graphics/ByteLikeGraphics/Items/food3.png";
                            Name = "Cooked Meat";
                            Description = "A veal steak. Perfect for a strong warrior!\n";
                            Stats["MaxHP"] = 7;
                            break;
                        case 4:
                            File = "Graphics/ByteLikeGraphics/Items/food4.png";
                            Name = "Large Meal";
                            Description = "A large meal composed of peas, oats, a small steak, a large bun and a big glass of vegetable juice\n";
                            Stats["MaxHP"] = 15;
                            break;
                        case 5:
                            File = "Graphics/ByteLikeGraphics/Items/food5.png";
                            Name = "Health Potion";
                            Description = "A vial of red liquid. It's bubbling\n";
                            Stats["MaxHP"] = 25;
                            break;
                        case 6:
                            File = "Graphics/ByteLikeGraphics/Items/food6.png";
                            Name = "Ironleaf";
                            Description = "A plant that improves one's pain tolerance. It grows in caves\n";
                            Stats["Defense"] = 5;
                            break;
                        case 7:
                            File = "Graphics/ByteLikeGraphics/Items/food7.png";
                            Name = "Hellroot";
                            Description = "A plant that improves one's physical capabilities. It grows near moster infected areas\n";
                            Stats["Strength"] = 5;
                            break;
                        case 8:
                            File = "Graphics/ByteLikeGraphics/Items/food8.png";
                            Name = "Moonglow";
                            Description = "A plant that improves one's concentration. It grows in mountains and blooms at night\n";
                            Stats["Magic"] = 5;
                            break;
                        case 9:
                            File = "Graphics/ByteLikeGraphics/Items/food9.png";
                            Name = "Speedroot";
                            Description = "A plant that improves one's reaction time. It grows in lush forests\n";
                            Stats["Agility"] = 5;
                            break;
                        case 10:
                            File = "Graphics/ByteLikeGraphics/Items/food10.png";
                            Name = "Nightswipe";
                            Description = "A plant that improves one's focus. It grows in twighlight swamps\n";
                            Stats["MagicDefense"] = 5;
                            break;
                        case 11:
                            File = "Graphics/ByteLikeGraphics/Items/food11.png";
                            Name = "Tiny Mana Drop";
                            Description = "A small drop of pure mana. Its smell is enchanting\n";
                            Stats["MaxMana"] = 5;
                            break;
                        case 12:
                            File = "Graphics/ByteLikeGraphics/Items/food12.png";
                            Name = "Medium Mana Drop";
                            Description = "A moderately sized drop of pure mana. It feels both hot and cold on touch\n";
                            Stats["MaxMana"] = 10;
                            break;
                        case 13:
                            File = "Graphics/ByteLikeGraphics/Items/food13.png";
                            Name = "Large Mana Drop";
                            Description = "A large drop of pure mana. Physics seem to bend around it\n";
                            Stats["MaxMana"] = 20;
                            break;
                        case 14:
                            File = "Graphics/ByteLikeGraphics/Items/food14.png";
                            Name = "Mana Potion";
                            Description = "A vial of blue liquid. It sparkles\n";
                            Stats["MaxMana"] = 50;
                            break;
                        case 15:
                            File = "Graphics/ByteLikeGraphics/Items/food15.png";
                            Name = "Improvement Potion";
                            Description = "A vial of yellow liquid. It seems lighter than air\nRemoves any negative effects\n";
                            break;
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                        case 20:
                            File = "Graphics/ByteLikeGraphics/Items/scroll";
                            Spell = Creature.GetRandomSpell(floor);
                            if (rand.Next(10) == 0)
                                Element = 0;
                            else
                                Element = GetSpellElement(Spell) + 1;

                            File += Element.ToString();
                            File += ".png";

                            if (Element == 0)
                            {
                                Name = $"Book of {Spell}";
                                Description = $"Teaches you {Spell}\n{Spell}: {Creature.GetSpellDescription(Spell)}";
                            }
                            else
                            {
                                Name = $"Scroll of {Spell}";
                                Description = $"Casts {Spell} with no Mana cost\n{Spell}: {Creature.GetSpellDescription(Spell)}";
                            }
                            Element = 0;
                            break;
                        // Arrows
                        default:
                            if (rand.Next(3) == 0)
                            {
                                Element = rand.Next(5);
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
                                    Name += "Lightning ";
                                    break;
                            }
                            Name += "Arrow";
                            File += ".png";
                            Spell = $"Shoot {Name}";
                            Quantity = rand.Next(4) + 1;
                            Description += "Can be shot using a bow\n";
                            break;
                    }
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
            if (!first)
            {
                Description += "\n";
            }
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

            int quality = rand.Next(50);

            if (floor <= 0)
                quality = 0;

            if (floor >= 100)
                quality = 50;

            if (quality < 35)
            {
                Inventory = new Item[11, 2];
            }
            else if (quality < 48)
            {
                Inventory = new Item[11, 4];
                floor += 3;
                File = "Graphics/ByteLikeGraphics/chest1.png";
            }
            else if (quality < 50)
            {
                Inventory = new Item[11, 6];
                floor += 7;
                File = "Graphics/ByteLikeGraphics/chest2.png";
            }
            else
            {
                Inventory = new Item[11, 7];
                floor += 15;
                File = "Graphics/ByteLikeGraphics/chest3.png";
            }

            int counter = rand.Next(150);

            if (floor > 0)
            {
                for (int i = 0; i < Inventory.GetLength(1); i++)
                {
                    for (int j = 0; j < Inventory.GetLength(0); j++)
                    {
                        counter += rand.Next(15);
                        if (counter > 150-(quality*2))
                        {
                            counter = 0;
                            Inventory[j, i] = new Item(floor, 0);
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
                else
                {
                    if (Inventory[slot[0], slot[1]] != null)
                    {
                        Item temp = Inventory[slot[0], slot[1]];
                        Inventory[slot[0], slot[1]] = item;
                        return temp;
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
