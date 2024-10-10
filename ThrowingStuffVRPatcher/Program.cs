using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;

namespace ThrowingStuffVRPatcher
{
    public class Program
    {
        public enum destructionFlags
        {
            Disable = 0,
            Destroy = 1,
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
            var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter> (GameRelease.SkyrimVR).Build();
            // get the explosions from throwing things
            string dataFolder = env.DataFolderPath.ToString();
            string path = dataFolder + "\\ThrowStuff.esp";
            using var throwingStuffMod = SkyrimMod.CreateFromBinaryOverlay(path, SkyrimRelease.SkyrimVR);
            var explosions = throwingStuffMod.Explosions;

            // patch ingredients
            foreach(var explosion in explosions)
            {
                Console.WriteLine(explosion.EditorID!.ToString());
            }
            foreach(var floraGetter in state.LoadOrder.PriorityOrder.Flora().WinningOverrides())
            {

                // var alcohols = 
                var flora = state.PatchMod.Florae.GetOrAddAsOverride(floraGetter);
                flora.Destructible = new Destructible();
                flora.Destructible.Data = new DestructableData();
                var destructedStage = new DestructionStage();
                var destructedStageData = new DestructionStageData();

                destructedStageData.DebrisCount = 0;
                flora.Destructible.Data.Health = 1;
                destructedStageData.HealthPercent = 0;
                destructedStageData.Index = 0;
                destructedStageData.ModelDamageStage = 0;
                destructedStageData.

                destructedStageData.Explosion = ThrowStuff.Explosion._SOL_FrostBombEXP;
                
                //destructedStage.Flags.SetFlag(, true);

                destructedStage.Data = destructedStageData;

                flora.Destructible.Stages.Add(destructedStage);
                Console.WriteLine(flora.Name);
                Console.WriteLine(destructedStage.Data.Explosion.ToString());
                //Console.WriteLine(ingredient.Destructible!.Stages.Last().Data!.Flags.ToString());
            }
            foreach(var ingestibleGetter in state.LoadOrder.PriorityOrder.Ingestible().WinningOverrides())
            {
                var ingestible = state.PatchMod.Ingestibles.GetOrAddAsOverride(ingestibleGetter);

            }
        }
    }
}
