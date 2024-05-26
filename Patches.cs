
using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Machines;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Pants;
using StardewValley.GameData.Shirts;
using StardewValley.GameData.Tools;
using StardewValley.GameData.Weapons;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace AdvancedFishingTreasure;

public class Patches
{
    public class TreasureMenuPatch
    {
        public static void Prefix(int remainingFish)
        {
            if (!ModEntry.Config.ModEnabled)
            {
                return;
            }

            FishingRod.baseChanceForTreasure = ModEntry.Config.ChestChance / 100f;
        }
        
        public static void Postfix(int remainingFish, FishingRod __instance)
        {
            if (!ModEntry.Config.ModEnabled)
            {
                return;
            }
            
            // Code snippet from https://github.com/atravita-mods/StardewMods/blob/1e9459a60006240a5bb545bf04557a5d4878ba1e/TrashDoesNotConsumeBait/TrashDoesNotConsumeBait/HarmonyPatches/TreasureMenuPatches.cs#L25
            if (Game1.activeClickableMenu is not ItemGrabMenu itemGrab || itemGrab.source != ItemGrabMenu.source_fishingChest )
            {
                return;
            }
            
            List<Item> vanillaLoot = new();

            for (int i = 0; i < ModEntry.Config.VanillaMultiplier; i++)
            {
                vanillaLoot = vanillaLoot.Concat(ModEntry.GetVanillaGrabMenu(__instance, remainingFish)).ToList();
            }
            
            var inventory = new List<Item>();
            
            if (remainingFish == 1)
            {
                inventory.Add(ModEntry.mCreateFish(__instance));
            }
            
            // filter vanilla exclusions
            List<string> vanillaExclude = ModEntry.Config.ExcludeVanilla.Split(",").ToList();
            vanillaLoot = vanillaLoot.Where(v => !vanillaExclude.Contains(v.QualifiedItemId)).ToList();
            
            List<Item> modInventory = new();
            List<Item> categoryInventory = new();
            
            Random rnd = new();
            
            // add modded items
            if (ModEntry.Config.IncludeModded)
            {
                Dictionary<string, string> CategoryIds = new()
                {
                    { "ExpBooks", "-103" },
                    { "SkillBooks", "-102" },
                    { "Rings", "-96" },
                    { "Greens", "-81" },
                    { "Flowers", "-80" },
                    { "Fruit", "-79" },
                    { "Vegetables", "-75" },
                    { "Seeds", "-74" },
                    { "MonsterLoot", "-28" },
                    { "TreeGoods", "-27" },
                    { "ArtisanGoods", "-26" },
                    { "Floors", "-24" },
                    { "FishingItems", "-23" },
                    { "Tackle", "-22" },
                    { "Bait", "-21" },
                    { "Trash", "-20" },
                    { "FarmBoosts", "-19" },
                    { "AnimalGoods", "-18" },
                    { "SpecialFarming", "-17" },
                    { "Components", "-16" },
                    { "OresBars", "-15" },
                    { "Stones", "-12" },
                    { "SmallCraft", "-8" },
                    { "Food", "-7" },
                    { "Milk", "-6" },
                    { "Eggs", "-5" },
                    { "Fish", "-4" },
                    { "Gems", "-2" },
                    { "Special", "0" }
                };
                List<int> Chances = new()
                {
                    ModEntry.Config.ExpBooksChance * (ModEntry.Config.IncludeExpBooks ? 1 : 0),
                    ModEntry.Config.SkillBooksChance * (ModEntry.Config.IncludeSkillBooks ? 1 : 0),
                    ModEntry.Config.RingsChance * (ModEntry.Config.IncludeRings ? 1 : 0),
                    ModEntry.Config.GreensChance * (ModEntry.Config.IncludeGreens ? 1 : 0),
                    ModEntry.Config.FlowersChance * (ModEntry.Config.IncludeFlowers ? 1 : 0),
                    ModEntry.Config.FruitChance * (ModEntry.Config.IncludeFruit ? 1 : 0),
                    ModEntry.Config.VegetablesChance * (ModEntry.Config.IncludeVegetables ? 1 : 0),
                    ModEntry.Config.SeedsChance * (ModEntry.Config.IncludeSeeds ? 1 : 0),
                    ModEntry.Config.MonsterLootChance * (ModEntry.Config.IncludeMonsterLoot ? 1 : 0),
                    ModEntry.Config.TreeGoodsChance * (ModEntry.Config.IncludeTreeGoods ? 1 : 0),
                    ModEntry.Config.ArtisanGoodsChance * (ModEntry.Config.IncludeArtisanGoods ? 1 : 0),
                    ModEntry.Config.FloorsChance * (ModEntry.Config.IncludeFloors ? 1 : 0),
                    ModEntry.Config.FishingItemsChance * (ModEntry.Config.IncludeFishingItems ? 1 : 0),
                    ModEntry.Config.TackleChance * (ModEntry.Config.IncludeTackle ? 1 : 0),
                    ModEntry.Config.BaitChance * (ModEntry.Config.IncludeBait ? 1 : 0),
                    ModEntry.Config.TrashChance * (ModEntry.Config.IncludeTrash ? 1 : 0),
                    ModEntry.Config.FarmBoostsChance * (ModEntry.Config.IncludeFarmBoosts ? 1 : 0),
                    ModEntry.Config.AnimalGoodsChance * (ModEntry.Config.IncludeAnimalGoods ? 1 : 0),
                    ModEntry.Config.SpecialFarmingChance * (ModEntry.Config.IncludeSpecialFarming ? 1 : 0),
                    ModEntry.Config.ComponentsChance * (ModEntry.Config.IncludeComponents ? 1 : 0),
                    ModEntry.Config.OresBarsChance * (ModEntry.Config.IncludeOresBars ? 1 : 0),
                    ModEntry.Config.StonesChance * (ModEntry.Config.IncludeStones ? 1 : 0),
                    ModEntry.Config.SmallCraftChance * (ModEntry.Config.IncludeSmallCraft ? 1 : 0),
                    ModEntry.Config.FoodChance * (ModEntry.Config.IncludeFood ? 1 : 0),
                    ModEntry.Config.MilkChance * (ModEntry.Config.IncludeMilk ? 1 : 0),
                    ModEntry.Config.EggsChance * (ModEntry.Config.IncludeEggs ? 1 : 0),
                    ModEntry.Config.FishChance * (ModEntry.Config.IncludeFish ? 1 : 0),
                    ModEntry.Config.GemsChance * (ModEntry.Config.IncludeGems ? 1 : 0),
                    ModEntry.Config.SpecialChance * (ModEntry.Config.IncludeSpecial ? 1 : 0),
                };

                for (int i = 0; i < ModEntry.Config.ModdedMultiplier; i++)
                {
                    foreach (var ind in Enumerable.Range(0, Chances.Count - 1))
                    {
                        if (rnd.Next(0, 100) < Chances[ind])
                        {
                            string newId = rnd.ChooseFrom(ModEntry.CachedItems[CategoryIds.Values.ToList()[ind]]).id;
                            modInventory.Add(new Object(newId, 1));
                        }
                    }
                }

                for (int i = 0; i < ModEntry.Config.ModdedMultiplier; i++)
                {
                    if (ModEntry.Config.IncludeBigCraft)
                    {
                        if (rnd.Next(0, 100) < ModEntry.Config.BigCraftChance)
                        {
                            IDictionary<string, BigCraftableData> idata = Game1.bigCraftableData;
                            IList<string> ikeys = idata.Keys.ToList();
                            
                            categoryInventory.Add(ItemRegistry.Create("(BC)" + rnd.ChooseFrom(ikeys), 1));
                        }
                    }
                    
                    if (ModEntry.Config.IncludeTools)
                    {
                        if (rnd.Next(0, 100) < ModEntry.Config.ToolsChance)
                        {
                            IDictionary<string, ToolData> idata = Game1.toolData;
                            IList<string> ikeys = idata.Keys.ToList();

                            Item toolitem = ItemRegistry.Create("(T)" + rnd.ChooseFrom(ikeys), 1);
                            
                            categoryInventory.Add(toolitem);
                        }
                    }
                    
                    if (ModEntry.Config.IncludeWeapons)
                    {
                        if (rnd.Next(0, 100) < ModEntry.Config.WeaponsChance)
                        {
                            IDictionary<string, WeaponData> idata = Game1.weaponData;
                            IList<string> ikeys = idata.Keys.ToList();
                            
                            categoryInventory.Add(new MeleeWeapon(rnd.ChooseFrom(ikeys)));
                        }
                    }
                    
                    if (ModEntry.Config.IncludePants)
                    {
                        if (rnd.Next(0, 100) < ModEntry.Config.PantsChance)
                        {
                            IDictionary<string, PantsData> idata = Game1.pantsData;
                            IList<string> ikeys = idata.Keys.ToList();
                            
                            categoryInventory.Add(new Clothing(rnd.ChooseFrom(ikeys)));
                        }
                    }
                    
                    if (ModEntry.Config.IncludeShirts)
                    {
                        if (rnd.Next(0, 100) < ModEntry.Config.ShirtsChance)
                        {
                            IDictionary<string, ShirtData> idata = Game1.shirtData;
                            IList<string> ikeys = idata.Keys.ToList();
                            
                            categoryInventory.Add(new Clothing(rnd.ChooseFrom(ikeys)));
                        }
                    }
                }
            }

            List<Item> moddedInventory = categoryInventory.Concat(modInventory).ToList();

            moddedInventory = moddedInventory
                .Where(v => !ModEntry.Config.ExcludeModded.Split(",").ToList().Contains(v.ItemId)).ToList();

            inventory = inventory.Concat(moddedInventory).ToList();

            inventory = inventory.Concat(vanillaLoot).ToList();
            inventory = ModEntry.ShuffleInventory(ModEntry.CondenseDisorganizedInventory(inventory));

            inventory = inventory.Where(v => !v.DisplayName.StartsWith("Error Item")).ToList();
            if (ModEntry.Config.EnableBlacklist)
            {
                inventory = inventory.Where(v => !ModEntry.Blacklisted.Contains(v.QualifiedItemId)).ToList();
            }
            
            /* Patch for:
             [SmokedFish] DONE y
             [SpecificBait] DONE y
             [DriedFruit] DONE y
             [812] "Roe" DONE y
             [447] "Aged Roe" DONE y
             [350] "Juice" DONE y 
             [348] "Wine" DONE y 
             [344] "Jelly" DONE y 
             [342] "Pickles" DONE y 
            */

            if (inventory.Count > 0)
            {
                foreach (int iInd in Enumerable.Range(0, inventory.Count - 1))
                {
                    Item cItem = inventory[iInd];

                    if (cItem.QualifiedItemId == "(O)SmokedFish")
                    {
                        Object flavor = new Object(rnd.ChooseFrom(ModEntry.CachedItems["-4"]).id, 1);
                        Item newItem = new ObjectDataDefinition().CreateFlavoredSmokedFish(flavor);

                        newItem.Stack = cItem.Stack;

                        inventory[iInd] = newItem;
                    }

                    if (cItem.QualifiedItemId == "(O)SpecificBait")
                    {
                        Object flavor = new Object(rnd.ChooseFrom(ModEntry.CachedItems["-4"].Where(v => !v.obj.ContextTags.Contains("fish_crab_pot")).ToList()).id, 1);
                        Item newItem = new ObjectDataDefinition().CreateFlavoredBait(flavor);

                        newItem.Stack = cItem.Stack;
                        
                        inventory[iInd] = newItem;
                    }
                    
                    if (cItem.QualifiedItemId == "(O)DriedFruit")
                    {
                        Object flavor = new Object(rnd.ChooseFrom(ModEntry.CachedItems["-79"]).id, 1);
                        Item newItem = new ObjectDataDefinition().CreateFlavoredDriedFruit(flavor);

                        newItem.Stack = cItem.Stack;
                        
                        inventory[iInd] = newItem;
                    }
                    
                    if (cItem.QualifiedItemId == "(O)812")
                    {
                        Object flavor = new Object(rnd.ChooseFrom(ModEntry.CachedItems["-4"]).id, 1);
                        Item newItem = new ObjectDataDefinition().CreateFlavoredRoe(flavor);

                        newItem.Stack = cItem.Stack;
                        
                        inventory[iInd] = newItem;
                    }
                    
                    if (cItem.QualifiedItemId == "(O)447")
                    {
                        Object flavor = new Object(rnd.ChooseFrom(ModEntry.CachedItems["-4"]).id, 1);
                        Item newItem = new ObjectDataDefinition().CreateFlavoredAgedRoe(flavor);

                        newItem.Stack = cItem.Stack;
                        
                        inventory[iInd] = newItem;
                    }
                    
                    if (cItem.QualifiedItemId == "(O)350")
                    {
                        List<ModEntry.IdItemPair> bases = ModEntry.CachedItems["-75"].Concat(ModEntry.CachedItems["-81"]).ToList();
                        Object flavor = new Object(rnd.ChooseFrom(bases).id, 1);
                        Item newItem = new ObjectDataDefinition().CreateFlavoredJuice(flavor);

                        newItem.Stack = cItem.Stack;
                        
                        inventory[iInd] = newItem;
                    }
                    
                    if (cItem.QualifiedItemId == "(O)348")
                    {
                        Object flavor = new Object(rnd.ChooseFrom(ModEntry.CachedItems["-79"]).id, 1);
                        Item newItem = new ObjectDataDefinition().CreateFlavoredWine(flavor);

                        newItem.Stack = cItem.Stack;
                        
                        inventory[iInd] = newItem;
                    }
                    
                    if (cItem.QualifiedItemId == "(O)344")
                    {
                        Object flavor = new Object(rnd.ChooseFrom(ModEntry.CachedItems["-79"]).id, 1);
                        Item newItem = new ObjectDataDefinition().CreateFlavoredJelly(flavor);

                        newItem.Stack = cItem.Stack;
                        
                        inventory[iInd] = newItem;
                    }
                    
                    if (cItem.QualifiedItemId == "(O)342")
                    {
                        List<ModEntry.IdItemPair> bases = ModEntry.CachedItems["-75"].Concat(ModEntry.CachedItems["-81"]).ToList();
                        Object flavor = new Object(rnd.ChooseFrom(bases).id, 1);
                        Item newItem = new ObjectDataDefinition().CreateFlavoredPickle(flavor);

                        newItem.Stack = cItem.Stack;
                        
                        inventory[iInd] = newItem;
                    }
                    
                    if (cItem.QualifiedItemId == "(O)DriedMushrooms")
                    {
                        List<string> tagList = new() { "edible_mushroom" };
                        List<string> bases = ModEntry.GetAllItemsWithContextTags(tagList);
                        
                        Object flavor = new Object(rnd.ChooseFrom(bases), 1);
                        Item newItem = new ObjectDataDefinition().CreateFlavoredDriedMushroom(flavor);

                        newItem.Stack = cItem.Stack;
                        
                        inventory[iInd] = newItem;
                    }
                    
                    if (cItem.QualifiedItemId == "(O)340")
                    {
                        List<ModEntry.IdItemPair> bases = ModEntry.CachedItems["-80"];
                        Object flavor = new Object(rnd.ChooseFrom(bases).id, 1);
                        Item newItem = new ObjectDataDefinition().CreateFlavoredHoney(flavor);

                        newItem.Stack = cItem.Stack;
                        
                        inventory[iInd] = newItem;
                    }
                }
            }

            inventory = inventory.Where(v => ModEntry.Config.PriceMin <= v.sellToStorePrice() / v.Stack && (v.sellToStorePrice() / v.Stack <= ModEntry.Config.PriceMax || ModEntry.Config.PriceMax <= -1)).ToList();

            ItemGrabMenu itemMenu = new ItemGrabMenu(inventory, ItemGrabMenu.source_fishingChest).setEssential(true);
            
            Game1.player.Money += ModEntry.Config.MoneyPrize;
            
            Game1.activeClickableMenu = itemMenu;
        }
    }
}