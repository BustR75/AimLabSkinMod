using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;

namespace ReSkins
{
    class patch
    {
        public patch(MethodInfo Original,string Before = "",string After = "")
        {
            Base = Original;
            if (!string.IsNullOrEmpty(Before))
                pre = new HarmonyMethod(typeof(Patches).GetMethod(Before,BindingFlags.NonPublic|BindingFlags.Static));
            if (!string.IsNullOrEmpty(After))
                post = new HarmonyMethod(typeof(Patches).GetMethod(After, BindingFlags.NonPublic | BindingFlags.Static));
        }
        public patch(MethodInfo Original, MethodInfo Before = null, MethodInfo After = null)
        {
            Base = Original;
            if(Before != null)
                pre = new HarmonyMethod(Before);
            if (After != null)
                post = new HarmonyMethod(After);
        }
        public MethodInfo Base;
        public HarmonyMethod pre;
        public HarmonyMethod post;
    }
    public static class Patches
    {
        static void Patch()
        {
            Random r = new Random();
            foreach(patch p in patches)
            {
                new PatchProcessor(HarmonyInstance.Create(p.Base.Name+r.Next(0,1000)), new List<MethodBase>
                {
                    p.Base
                }, p.pre, p.post, null).Patch();
            }
        }
        static List<patch> patches = new List<patch>();
        public static void Start()
        {
            patches.Add(new patch(typeof(WeaponService).GetMethod("GetSkinForWeapon"), "", nameof(GetSkins)));
            patches.Add(new patch(typeof(WeaponService).GetMethod("ValidateSkin"), nameof(ReturnTrue)));

            Patch();
        }
        private static bool ReturnTrue(ref bool __result)
        {
            __result = true;
            return false;
        }
        private static void GetSkins(Il2CppSystem.Collections.Generic.List<WeaponSkin> __result, WeaponProfile weapon)
        {
            foreach (WeaponSkin skin in __result)
            {
                Main.Dump(skin, weapon);
            }
            __result.AddRange(Main.GetSkins(weapon));
        }
    }
}
