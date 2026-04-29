using Verse;
using HarmonyLib;

namespace MassGrave
{
    public class MassGraveMod : Mod
    {
        public MassGraveMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("MassGrave.Mod");
            harmony.PatchAll();
        }
    }
}
