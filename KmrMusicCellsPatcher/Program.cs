using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;

namespace KmrMusicCellsPatcher
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "YourPatcher.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var kmrMusicCellsMod = state.LoadOrder.TryGetValue("kmr_music_cells.esp")?.Mod;
            if (kmrMusicCellsMod == null)
            {
                return;
            }

            foreach (var block in kmrMusicCellsMod.Cells)
            {
                foreach (var subBlock  in block.SubBlocks)
                {
                    foreach (var cell in subBlock.Cells)
                    {
                        var allContext = state.LinkCache.ResolveAllContexts<ICell, ICellGetter>(cell.FormKey)
                                .Take(2)
                                .ToArray();
                        var cellContext = allContext[0];
                        if (cellContext.ModKey == kmrMusicCellsMod.ModKey)
                        {
                            if (allContext.Length > 1)
                            {
                                cellContext = allContext[1];
                            }
                        }
                        var overrideCell = cellContext.GetOrAddAsOverride(state.PatchMod);
                        overrideCell.Music.FormKey = cell.Music.FormKey;
                    }
                }
            }
        }
    }
}
