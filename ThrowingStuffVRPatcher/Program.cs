using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using System.Text.RegularExpressions;

namespace ThrowingStuffVRPatcher
{
    public class Program
    {
        IEnumerable<DestructionStage>? destructionStages;

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
                .SetTypicalOpen(GameRelease.SkyrimSE, "ThrowingStuffVRPatcher.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            //if (!state.LoadOrder.ContainsKey(ThrowStuff.ModKey))
            //    throw new Exception("ERROR: ThrowStuff.esp missing from load order. Check that the esp is enabled.");

            var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter> (GameRelease.SkyrimVR).Build();
            // get the explosions from throwing things
            string dataFolder = env.DataFolderPath.ToString();
            string throwingStuffPath = dataFolder + "\\ThrowStuff.esp";
            string skyrimPath = dataFolder + "\\Skyrim.esm";
            using var throwingStuffMod = SkyrimMod.CreateFromBinaryOverlay(throwingStuffPath, SkyrimRelease.SkyrimVR);
            using var skyrimMod = SkyrimMod.CreateFromBinaryOverlay(skyrimPath, SkyrimRelease.SkyrimVR);
            Dictionary<string, string> alcoholDict = new Dictionary<string, string>();
            Dictionary<string, string> glassDict = new Dictionary<string, string>();
            Dictionary<string, string> soupDict = new Dictionary<string, string>();

            var explosions = throwingStuffMod.Explosions;

            ILinkCache throwingStuffLinkCache = throwingStuffMod.ToImmutableLinkCache();
            ILinkCache skyrimLinkCache = skyrimMod.ToImmutableLinkCache();

            ////////////////////////////////////////////////////////
            // Read from ThrowStuff_KID.ini
            
            try
            {
                using StreamReader reader = new(dataFolder + "\\ThrowStuff_KID.ini");

                string text = reader.ReadToEnd();

                var sections = text.Split(";");
                foreach ( var section in sections )
                {
                    var itemsWithHeader = section.Replace("\r\n", "").Split("Keyword = ");
                    var items = itemsWithHeader.Skip(1);

                    if (section.StartsWith("[D]Alcohol")) {
                        //var itemsWithHeader = section.Replace("\r\n", "").Split("Keyword = ");
                        //var items = itemsWithHeader.Skip(1);
                        foreach (string item in items) {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            alcoholDict.Add(Regex.Replace(itemName, "-|\\+|\\*", ""), itemName);
                        }
                        continue;
                    }
                    else if (section.StartsWith("[D]MaterialGlass"))
                    {
                        //var itemsWithHeader = section.Replace("\r\n", "").Split("Keyword = ");
                        //var items = itemsWithHeader.Skip(1);
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            glassDict.Add(Regex.Replace(itemName, "-|\\+|\\*", ""), itemName);
                        }
                        continue;
                    }
                    else if (section.StartsWith("[D]isSoup"))
                    {
                        //var itemsWithHeader = section.Replace("\r\n", "").Split("Keyword = ");
                        //var items = itemsWithHeader.Skip(1);
                        foreach (string item in items)
                        {
                            string itemName = item.Substring(item.LastIndexOf("|") + 1);
                            soupDict.Add(Regex.Replace(itemName, "-|\\+|\\*", ""), itemName);
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
                        Console.WriteLine("Added keyword \"" + ThrowStuff.Keyword.onmoPurse.ResolveIdentifier(throwingStuffLinkCache) + "\" to " + flora?.Name);
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
                var ingestible = state.PatchMod.Ingestibles.GetOrAddAsOverride(ingestibleGetter.Record);
                if (ingestible.ToLinkGetter().TryResolveContext<ISkyrimMod, ISkyrimModGetter, IIngestible, IIngestibleGetter>(env.LinkCache, out var context))
                {                     //ingestible.Keywords = new Keywords();
                    foreach (var keyword in ingestible.Keywords.EmptyIfNull())
                    {
                        if (!skyrimLinkCache.TryResolveIdentifier(Skyrim.Keyword.VendorItemPotion, out var potionKeyword) || !skyrimLinkCache.TryResolveIdentifier(Skyrim.Keyword.VendorItemPoison, out var poisonKeyword) || !skyrimLinkCache.TryResolveIdentifier(keyword, out var ourKeyword))
                        {
                            Console.WriteLine("Failed to find keywords while checking Ingestible " + ingestible.Name + " from " + ingestibleGetter.ModKey);
                            // handle if it's not found.
                            continue;
                        }
                        
                        // Potions/Poisons
                        if (ourKeyword == potionKeyword || ourKeyword == poisonKeyword || ingestible.Flags.HasFlag(Ingestible.Flag.Poison) || (ingestible.Flags.HasFlag(Ingestible.Flag.FoodItem) && ingestible.Flags.HasFlag(Ingestible.Flag.Medicine)))
                        {
                            addDestruction(ingestible);

                            if (ingestible.Destructible?.Stages.Count == 8)
                            {
                                Console.WriteLine("Added destruction to Potion/Poison \"" + ingestible.Name + "\" from " + ingestibleGetter.ModKey);
                            } else
                            {
                                Console.WriteLine("Failed to add destruction to Potion/Poison \"" + ingestible.Name + "\" from " + ingestibleGetter.ModKey);
                            }
                            break;
                        }

                        // Patch Alcohol
                        if (alcoholDict.Any(alcohol =>
                        ingestible?.Name?.String != null && ingestible.Name.String.ToLower().Contains(alcohol.Key.ToLower()) && alcohol.Value.Contains("*")))
                        {
                            addDestruction(ingestible);
                            if (ingestible.Destructible?.Stages.Count == 8)
                            {
                                Console.WriteLine("Added destruction to Alcohol \"" + ingestible.Name + "\" from " + ingestibleGetter.ModKey);
                            }
                            else
                            {
                                Console.WriteLine("Failed to add destruction to Alcohol \"" + ingestible.Name + "\" from " + ingestibleGetter.ModKey);
                            }
                            break;

                        }

                        // Patch soups
                        if (soupDict.Any(soup =>
                        ingestible?.Name?.String != null && ingestible.Name.String.ToLower().Contains(soup.Key.ToLower()) && soup.Value.Contains("*")))
                        {
                            addDestruction(ingestible);
                            if (ingestible.Destructible?.Stages.Count == 8)
                            {
                                Console.WriteLine("Added destruction to Soup \"" + ingestible.Name + "\" from " + ingestibleGetter.ModKey);
                            }
                            else
                            {
                                Console.WriteLine("Failed to add destruction to Alcohol \"" + ingestible.Name + "\" from " + ingestibleGetter.ModKey);
                            }
                            break;

                        }

                    }



                }
            }
        }
        static void addDestruction(Ingestible ingestible) {
            var program = new Program();
            ingestible.Destructible = new Destructible();
            ingestible.Destructible.Data = new DestructableData();
            ingestible.Destructible.Data.Health = 8;
            ingestible.Destructible.Data.DESTCount = 8;

            ingestible.Destructible.Stages.AddRange(program.destructionStages.EmptyIfNull());

        }

        static void addDestruction(Flora flora)
        {
            var program = new Program();
            flora.Destructible = new Destructible();
            flora.Destructible.Data = new DestructableData();
            flora.Destructible.Data.Health = 8;
            flora.Destructible.Data.DESTCount = 8;

            flora.Destructible.Stages.AddRange(program.destructionStages.EmptyIfNull());

        }

    }
}
