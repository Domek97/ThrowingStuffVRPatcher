using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Noggog;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using System.Text.RegularExpressions;

namespace ThrowingStuffVRPatcher
{
    public class Program
    {
        IEnumerable<DestructionStage>? destructionStages;
        Dictionary<string, string> alcoholDict = new Dictionary<string, string>();
        Dictionary<string, string> glassDict = new Dictionary<string, string>();
        Dictionary<string, string> soupDict = new Dictionary<string, string>();
        Dictionary<string, string> whiteBombBigDict = new Dictionary<string, string>();
        Dictionary<string, string> whiteBombSmallDict = new Dictionary<string, string>();
        Dictionary<string, string> blackBombBigDict = new Dictionary<string, string>();
        Dictionary<string, string> blackBombSmallDict = new Dictionary<string, string>();
        Dictionary<string, string> purseDict = new Dictionary<string, string>();
        Dictionary<string, string> nullwhiteDict = new Dictionary<string, string>();
        Dictionary<string, string> firebombDict = new Dictionary<string, string>();
        Dictionary<string, string> oilbombDict = new Dictionary<string, string>();
        Dictionary<string, string> frostbombDict = new Dictionary<string, string>();
        Dictionary<string, string> shockbombDict = new Dictionary<string, string>();
        Dictionary<string, string> poisonbombDict = new Dictionary<string, string>();
        Dictionary<string, string> potionDict = new Dictionary<string, string>();
        Dictionary<string, string> poisonDict = new Dictionary<string, string>();
        Dictionary<string, string> exclusionsDict = new Dictionary<string, string>();

        public Program()
        {

            /////////////////////////////////////////////////////////
            // Destruction Stages

            // 1
            var destructedStage1 = new DestructionStage();
            var destructedStage1Data = new DestructionStageData();
            destructedStage1.Data = destructedStage1Data;

            destructedStage1Data.HealthPercent = 88;
            destructedStage1Data.Index = 0;
            destructedStage1Data.ModelDamageStage = 1;
            destructedStage1Data.Flags = destructedStage1Data.Flags.SetFlag(DestructionStageData.Flag.CapDamage, true);

            // 2
            var destructedStage2 = new DestructionStage();
            var destructedStage2Data = new DestructionStageData();
            destructedStage2.Data = destructedStage2Data;

            destructedStage2Data.HealthPercent = 75;
            destructedStage2Data.Index = 1;
            destructedStage2Data.ModelDamageStage = 2;
            destructedStage2Data.Flags = destructedStage2Data.Flags.SetFlag(DestructionStageData.Flag.CapDamage, true);

            // 3
            var destructedStage3 = new DestructionStage();
            var destructedStage3Data = new DestructionStageData();
            destructedStage3.Data = destructedStage3Data;

            destructedStage3Data.HealthPercent = 63;
            destructedStage3Data.Index = 2;
            destructedStage3Data.ModelDamageStage = 3;
            destructedStage3Data.Flags = destructedStage3Data.Flags.SetFlag(DestructionStageData.Flag.CapDamage, true);

            // 4
            var destructedStage4 = new DestructionStage();
            var destructedStage4Data = new DestructionStageData();
            destructedStage4.Data = destructedStage4Data;

            destructedStage4Data.HealthPercent = 50;
            destructedStage4Data.Index = 3;
            destructedStage4Data.ModelDamageStage = 4;
            destructedStage4Data.Flags = destructedStage4Data.Flags.SetFlag(DestructionStageData.Flag.CapDamage, true);

            // 5
            var destructedStage5 = new DestructionStage();
            var destructedStage5Data = new DestructionStageData();
            destructedStage5.Data = destructedStage5Data;

            destructedStage5Data.HealthPercent = 38;
            destructedStage5Data.Index = 4;
            destructedStage5Data.ModelDamageStage = 5;
            destructedStage5Data.Flags = destructedStage5Data.Flags.SetFlag(DestructionStageData.Flag.CapDamage, true);

            // 6
            var destructedStage6 = new DestructionStage();
            var destructedStage6Data = new DestructionStageData();
            destructedStage6.Data = destructedStage6Data;

            destructedStage6Data.HealthPercent = 26;
            destructedStage6Data.Index = 5;
            destructedStage6Data.ModelDamageStage = 6;
            destructedStage6Data.Flags = destructedStage6Data.Flags.SetFlag(DestructionStageData.Flag.CapDamage, true);

            // 7
            var destructedStage7 = new DestructionStage();
            var destructedStage7Data = new DestructionStageData();
            destructedStage7.Data = destructedStage7Data;

            destructedStage7Data.HealthPercent = 13;
            destructedStage7Data.Index = 6;
            destructedStage7Data.ModelDamageStage = 7;
            destructedStage7Data.Flags = destructedStage7Data.Flags.SetFlag(DestructionStageData.Flag.CapDamage, true);

            // 8
            var destructedStage8 = new DestructionStage();
            var destructedStage8Data = new DestructionStageData();
            destructedStage8.Data = destructedStage8Data;

            destructedStage8Data.HealthPercent = 0;
            destructedStage8Data.Index = 7;
            destructedStage8Data.ModelDamageStage = 8;
            destructedStage8Data.Flags = destructedStage8Data.Flags.SetFlag(DestructionStageData.Flag.CapDamage, true);

            destructionStages = new[] { destructedStage1, destructedStage2, destructedStage3, destructedStage4, destructedStage5, destructedStage6, destructedStage7, destructedStage8 };

        }

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimVR, "ThrowingStuffVRPatcher.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (!state.LoadOrder.ContainsKey(ThrowStuff.ModKey))
                throw new Exception("ERROR: ThrowStuff.esp missing from load order. Check that the esp is enabled.");

            #if DEBUG
            foreach (var mod in state.LoadOrder)
            {
                Console.WriteLine(mod.Value.ToString());
            }
            #endif
            ////////////////////////////////////////////////////////
            // Read from ThrowStuff_KID.ini

            Program program = new Program();

            try
            {
                Console.WriteLine("Trying to read ThrowStuff_KID.ini from " + state.DataFolderPath);
                using StreamReader reader = new(state.DataFolderPath + "\\ThrowStuff_KID.ini");
                Console.WriteLine("Found ThrowStuff_KID.ini");
                string text = reader.ReadToEnd();

                var sections = text.Split(";");
                foreach (var section in sections)
                {
                    var itemsWithHeader = section.Replace("\r\n", "").Split("Keyword = ");
                    var items = itemsWithHeader.Skip(1);

                    if (section.StartsWith("[D]Alcohol"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "Alcohol");
                            } else
                            {
                                program.alcoholDict.Add(itemName, itemType);
                            }
                        }
                        continue;
                    }
                    else if (section.StartsWith("[D]MaterialGlass"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "MaterialGlass");
                            }
                            else
                            {
                                program.glassDict.Add(itemName, itemType);
                            }
                        }
                        continue;
                    }
                    else if (section.StartsWith("[D]isSoup"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "isSoup");
                            }
                            else
                            {
                                program.soupDict.Add(itemName, itemType);
                            }
                        }
                        continue;
                    }
                    else if (section.StartsWith("[D]WhiteDustBombBig"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "WhiteDustBombBig");
                            }
                            else
                            {
                                program.whiteBombBigDict.Add(itemName, itemType);
                            }
                        }
                        continue;

                    }
                    else if (section.StartsWith("[D]WhiteDustBombSmall"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "WhiteDustBombSmall");
                            }
                            else
                            {
                                program.whiteBombSmallDict.Add(itemName, itemType);
                            }
                        }
                        continue;

                    }
                    else if (section.StartsWith("[D]BlackDustBombBig"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "BlackDustBombBig");
                            }
                            else
                            {
                                program.blackBombBigDict.Add(itemName, itemType);
                            }
                        }
                        continue;

                    }
                    else if (section.StartsWith("[D]BlackDustBombSmall"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "BlackDustBombSmall");
                            }
                            else
                            {
                                program.blackBombSmallDict.Add(itemName, itemType);
                            }
                        }
                        continue;

                    }
                    else if (section.StartsWith("[D]PurseLarge"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "PurseLarge");
                            }
                            else
                            {
                                program.purseDict.Add(itemName, itemType);
                            }
                        }
                        continue;

                    }
                    else if (section.StartsWith("[D]nullWhite"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "nullWhite");
                            }
                            else
                            {
                                program.nullwhiteDict.Add(itemName, itemType);
                            }
                        }
                        continue;

                    }
                    else if (section.StartsWith("[D]FireBomb"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "FireBomb");
                            }
                            else
                            {
                                program.firebombDict.Add(itemName, itemType);
                            }
                        }
                        continue;

                    }
                    else if (section.StartsWith("[D]OilBomb"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "OilBomb");
                            }
                            else
                            {
                                program.oilbombDict.Add(itemName, itemType);
                            }
                        }
                        continue;

                    }
                    else if (section.StartsWith("[D]FrostBomb"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "FrostBomb");
                            }
                            else
                            {
                                program.frostbombDict.Add(itemName, itemType);
                            }
                        }
                        continue;

                    }
                    else if (section.StartsWith("[D]ShockBomb"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "ShockBomb");
                            }
                            else
                            {
                                program.shockbombDict.Add(itemName, itemType);
                            }
                        }
                        continue;

                    }
                    else if (section.StartsWith("[D]PoisonBomb"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "PoisonBomb");
                            }
                            else
                            {
                                program.poisonbombDict.Add(itemName, itemType);
                            }
                        }
                        continue;

                    }
                    else if (section.StartsWith("[D]isPotion"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "isPotion");
                            }
                            else
                            {
                                program.potionDict.Add(itemName, itemType);
                            }
                        }
                        continue;

                    }
                    else if (section.StartsWith("[D]IsPoison"))
                    {
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            string itemType = item.Substring(item.IndexOf("|") + 1, item.LastIndexOf("|") - item.IndexOf("|") - 1);
                            if (itemName.StartsWith("-"))
                            {
                                program.exclusionsDict.Add(Regex.Replace(itemName, "-", "").ToLower(), "IsPoison");
                            }
                            else
                            {
                                program.poisonDict.Add(itemName, itemType);
                            }
                        }
                        continue;

                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Did not find ThrowStuff_KID.ini in data folder.");
                Console.WriteLine(e.Message);
                throw;
            }

            //////////////////////////////////////////////////////////
            // Begin patch

            // patch onmo coin purse
            foreach (var floraGetter in state.LoadOrder.PriorityOrder.Flora().WinningContextOverrides())
            {
                if (floraGetter.ModKey == "DropOnDeath.esp")
                {
                    var flora = state.PatchMod.Florae.GetOrAddAsOverride(floraGetter.Record);

                    addDestruction(flora);
                    
                    if (flora.Keywords == null)
                    {
                        flora.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
                    }

                    flora.Keywords?.Add(ThrowStuff.Keyword.onmoPurse);

                    if (flora.Keywords.EmptyIfNull().Contains(ThrowStuff.Keyword.onmoPurse))
                    {
                        Console.WriteLine("Added keyword \"" + ThrowStuff.Keyword.onmoPurse.ResolveIdentifier(state.LinkCache) + "\" to " + flora?.Name);
                    } else
                    {
                        Console.WriteLine("Failed to add keyword \"" + ThrowStuff.Keyword.onmoPurse.ToString() + "\" to " + flora?.Name);
                    }
                    if (flora?.Destructible?.Stages.Count == 8)
                    {
                        Console.WriteLine("Added destruction to \"" + flora?.Name + "\" from " + floraGetter.ModKey);
                    } else
                    {
                        Console.WriteLine("Failed to add destruction to Potion/Poison \"" + flora?.Name + "\" from " + floraGetter.ModKey);
                    }
                }
            }

            // patch potions/food/drink
            foreach (var ingestibleGetter in state.LoadOrder.PriorityOrder.Ingestible().WinningContextOverrides())
            {
                bool addedDestruction = false;
                    var ingestible = state.PatchMod.Ingestibles.GetOrAddAsOverride(ingestibleGetter.Record);
                if (ingestible.ToLinkGetter().TryResolveContext<ISkyrimMod, ISkyrimModGetter, IIngestible, IIngestibleGetter>(state.LinkCache, out var context))
                {
                    foreach (var keyword in ingestible.Keywords.EmptyIfNull())
                    {
                        if (!state.LinkCache.TryResolveIdentifier(Skyrim.Keyword.VendorItemPotion, out var potionKeyword) || !state.LinkCache.TryResolveIdentifier(Skyrim.Keyword.VendorItemPoison, out var poisonKeyword) || !state.LinkCache.TryResolveIdentifier(keyword, out var ourKeyword))
                        {
                            Console.WriteLine("Failed to find keywords while checking Ingestible " + ingestible.Name + " from " + ingestibleGetter.ModKey);
                            // handle if it's not found.
                            continue;
                        }

                        // Potions/Poisons
                        if (ingestible.Name?.String != null && ingestible.Name.String.ToLower().Contains("potion") || ingestible.Name?.String != null && ingestible.Name.String.ToLower().Contains("poison")
                            || ourKeyword == potionKeyword || ourKeyword == poisonKeyword || ingestible.Flags.HasFlag(Ingestible.Flag.Poison) || (!ingestible.Flags.HasFlag(Ingestible.Flag.FoodItem) && !ingestible.Flags.HasFlag(Ingestible.Flag.Medicine)))
                        {
                            addDestruction(ingestible);

                            if (ingestible.Destructible?.Stages.Count == 8)
                            {
                                Console.WriteLine("Added destruction to Potion/Poison \"" + ingestible.Name + "\" from " + ingestibleGetter.ModKey);
                            }
                            else
                            {
                                Console.WriteLine("Failed to add destruction to Potion/Poison \"" + ingestible.Name + "\" from " + ingestibleGetter.ModKey);
                            }
                            addedDestruction = true;
                            break;
                        }

                    }
                    // Everything else
                    if (!addedDestruction)
                    {
                        patchINIitems(ingestible, ingestibleGetter.ModKey, program);
                    }
                }
            }

            // patch ingredients
            foreach (var ingredientGetter in state.LoadOrder.PriorityOrder.Ingredient().WinningContextOverrides())
            {
                var ingredient = state.PatchMod.Ingredients.GetOrAddAsOverride(ingredientGetter.Record);

                patchINIitems(ingredient, ingredientGetter.ModKey, program);
            }

            // patch misc items
            foreach (var miscItemGetter in state.LoadOrder.PriorityOrder.MiscItem().WinningContextOverrides())
            {
                var miscItem = state.PatchMod.MiscItems.GetOrAddAsOverride(miscItemGetter.Record);

                patchINIitems(miscItem, miscItemGetter.ModKey, program);
            }
        }

        static void addDestruction(dynamic myObject)
        {
            var program = new Program();
            myObject.Destructible = new Destructible();
            myObject.Destructible.Data = new DestructableData();
            myObject.Destructible.Data.Health = 8;
            myObject.Destructible.Data.DESTCount = 8;

            myObject.Destructible.Stages.AddRange(program.destructionStages.EmptyIfNull());

        }

        static void patchINIitems(dynamic myObject, ModKey modKey, Program program)
        {

            #if DEBUG
            if (myObject.Name?.String != null)
                if (myObject.Name.String.ToLower().Contains("flour"))
                    System.Diagnostics.Debugger.Break();
            #endif
            string type = myObject.GetType().ToString().Substring(myObject.GetType().ToString().LastIndexOf(".") + 1);
            switch (type)
            {
                case "Ingestible":
                    type = "Potion";
                    break;
                case "MiscItem":
                    type = "Misc Item";
                    break;
            }

            // Patch Alcohol
            if (program.alcoholDict.Any(alcohol => 
                isValidObject(program, alcohol, myObject, type, "Alcohol")))
            {
                addDestruction(myObject);
                if (myObject.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " Alcohol \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " Alcohol \"" + myObject.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch Glass
            if (program.glassDict.Any(glass =>
            isValidObject(program, glass, myObject, type, "MaterialGlass")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " Glass Object \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " Glass Object \"" + myObject?.Name + "\" from " + modKey);
                }
                return;
            }

            // Patch soups
            if (program.soupDict.Any(soup =>
                isValidObject(program, soup, myObject, type, "isSoup")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " Soup \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " Soup \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch WhiteDustBombBig
            if (program.whiteBombBigDict.Any(bomb =>
                isValidObject(program, bomb, myObject, type, "WhiteDustBombBig")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " WhiteDustBombBig \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " WhiteDustBombBig \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch WhiteDustBombSmall
            if (program.whiteBombSmallDict.Any(bomb =>
                isValidObject(program, bomb, myObject, type, "WhiteDustBombSmall")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " WhiteDustBombSmall \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " WhiteDustBombSmall \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch BlackDustBombBig
            if (program.blackBombBigDict.Any(bomb =>
                isValidObject(program, bomb, myObject, type, "BlackDustBombBig")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " BlackDustBombBig \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " BlackDustBombBig \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch BlackDustBombSmall
            if (program.blackBombSmallDict.Any(bomb =>
                isValidObject(program, bomb, myObject, type, "BlackDustBombSmall")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " BlackDustBombSmall \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " BlackDustBombSmall \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch Coin purses
            if (program.purseDict.Any(purse =>
                isValidObject(program, purse, myObject, type, "PurseLarge")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " Coin purse \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " Coin purse \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch nullWhite
            if (program.nullwhiteDict.Any(bomb =>
                isValidObject(program, bomb, myObject, type, "nullWhite")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " nullWhite \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " nullWhite \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch Firebomb
            if (program.firebombDict.Any(bomb =>
                isValidObject(program, bomb, myObject, type, "FireBomb")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " Firebomb \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " Firebomb \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch Oilbomb
            if (program.oilbombDict.Any(bomb =>
                isValidObject(program, bomb, myObject, type, "OilBomb")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " Oilbomb \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " Oilbomb \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch Frostbomb
            if (program.frostbombDict.Any(bomb =>
                isValidObject(program, bomb, myObject, type, "FrostBomb")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " Frostbomb \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " Frostbomb \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch Shockbomb
            if (program.shockbombDict.Any(bomb =>
                isValidObject(program, bomb, myObject, type, "ShockBomb")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " Shockbomb \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " Shockbomb \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch Poisonbomb
            if (program.poisonbombDict.Any(bomb =>
                isValidObject(program, bomb, myObject, type, "PoisonBomb")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " Poisonbomb \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " Poisonbomb \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch Potion
            if (program.potionDict.Any(potion =>
                isValidObject(program, potion, myObject, type, "isPotion")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " Potion \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " Potion \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }

            // Patch Poison
            if (program.poisonDict.Any(poison =>
                isValidObject(program, poison, myObject, type, "IsPoison")))
            {
                addDestruction(myObject);
                if (myObject?.Destructible?.Stages.Count == 8)
                {
                    Console.WriteLine("Added destruction to " + type + " Poison \"" + myObject.Name + "\" from " + modKey);
                }
                else
                {
                    Console.WriteLine("Failed to add destruction to " + type + " Poison \"" + myObject?.Name + "\" from " + modKey);
                }
                return;

            }
        }

        static bool isValidObject(Program program, KeyValuePair<string, string> entry, dynamic myObject, string type, string section)
        {
            if (myObject?.Name?.String != null)
            {
                if (program.exclusionsDict.TryGetValue(myObject?.Name.String?.ToLower(), out string exclusionType))
                {
                    return myObject?.Name?.String != null && (myObject?.Name.String.ToLower().Contains(Regex.Replace(entry.Key.ToLower(), "-|\\+|\\*", "")) && entry.Key.Contains("*") || (entry.Key.ToLower().Equals(myObject?.Name.String.ToLower()))) && entry.Value.Equals(type)
                    && !(program.exclusionsDict.ContainsKey(myObject?.Name.String.ToLower()) && exclusionType == section);
                }
            }
            return myObject?.Name?.String != null && (myObject?.Name.String.ToLower().Contains(Regex.Replace(entry.Key.ToLower(), "-|\\+|\\*", "")) && entry.Key.Contains("*") || (entry.Key.ToLower().Equals(myObject?.Name.String.ToLower()))) && entry.Value.Equals(type);
        }
    }
}
