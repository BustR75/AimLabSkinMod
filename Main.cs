using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using System.Drawing;

[assembly:MelonGame("Statespace", "aimlab_tb")]
[assembly:MelonInfo(typeof(ReSkins.Main),"Re:Skins","1.0","BustR75", "https://github.com/BustR75/AimLabSkinMod")]
[assembly:MelonColor(ConsoleColor.Red)]
namespace ReSkins
{
    public class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            Patches.Start();
            MelonCoroutines.Start(DumpAsync());
        }
        public override void OnUpdate()
        {
        }
        static Dictionary<WeaponSkin,WeaponProfile> Todump = new Dictionary<WeaponSkin, WeaponProfile>();
        static IEnumerator DumpAsync()
        {
            for(; ; )
            {
                if (Todump.Count > 0)
                    lock (Todump)
                    {
                        foreach (var v in Todump)
                        {
                            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Skins\\Dump"));
                            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Skins\\Dump", v.Value.displayName));
                            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Skins\\Dump", v.Value.displayName, v.Key.title));
                            string dir = Path.Combine(Environment.CurrentDirectory, "Skins\\Dump", v.Value.displayName, v.Key.title);
                            try
                            {
                                if (!File.Exists(Path.Combine(dir, "MainTexture.png")))
                                    File.WriteAllBytes(Path.Combine(dir, "MainTexture.png"), Il2CppImageConversionManager.EncodeToPNG(MakeReadable(v.Key.material.mainTexture.Cast<Texture2D>())));
                            }
                            catch { }
                            try
                            {
                                if (!File.Exists(Path.Combine(dir, "Metallic.png")))
                                    File.WriteAllBytes(Path.Combine(dir, "Metallic.png"), Il2CppImageConversionManager.EncodeToPNG(MakeReadable(v.Key.material.GetTexture("_MetallicGlossMap").Cast<Texture2D>())));
                            }
                            catch { }
                            try
                            {
                                if (!File.Exists(Path.Combine(dir, "Normal.png")))
                                    File.WriteAllBytes(Path.Combine(dir, "Normal.png"), Il2CppImageConversionManager.EncodeToPNG(MakeReadable(v.Key.material.GetTexture("_BumpMap").Cast<Texture2D>())));
                            }
                            catch { }
                            try
                            {
                                if (!File.Exists(Path.Combine(dir, "Height.png")))
                                    File.WriteAllBytes(Path.Combine(dir, "Height.png"), Il2CppImageConversionManager.EncodeToPNG(MakeReadable(v.Key.material.GetTexture("_ParallaxMap").Cast<Texture2D>())));
                            }
                            catch { }
                            try
                            {
                                if (!File.Exists(Path.Combine(dir, "Occlusion.png")))
                                    File.WriteAllBytes(Path.Combine(dir, "Occlusion.png"), Il2CppImageConversionManager.EncodeToPNG(MakeReadable(v.Key.material.GetTexture("_OcclusionMap").Cast<Texture2D>())));
                            }
                            catch { }
                            try
                            {
                                if (!File.Exists(Path.Combine(dir, "Emission.png")))
                                    File.WriteAllBytes(Path.Combine(dir, "Emission.png"), Il2CppImageConversionManager.EncodeToPNG(MakeReadable(v.Key.material.GetTexture("_EmissionMap").Cast<Texture2D>())));
                            }
                            catch { }
                            try
                            {
                                if (!File.Exists(Path.Combine(dir, "Detail.png")))
                                    File.WriteAllBytes(Path.Combine(dir, "Detail.png"), Il2CppImageConversionManager.EncodeToPNG(MakeReadable(v.Key.material.GetTexture("_DetailMask").Cast<Texture2D>())));
                            }
                            catch { }
                        }
                        Todump.Clear();
                    }
                yield return new WaitForSeconds(.1f);
            }
            //yield break;
        }
        public static List<WeaponSkin> Allskins = new List<WeaponSkin>();
        public static Dictionary<WeaponProfile, Il2CppSystem.Collections.Generic.IEnumerable<WeaponSkin>> ModSkins = new Dictionary<WeaponProfile, Il2CppSystem.Collections.Generic.IEnumerable<WeaponSkin>>();
        public static Il2CppSystem.Collections.Generic.IEnumerable<WeaponSkin> GetSkins(WeaponProfile weapon)
        {
            if (ModSkins.ContainsKey(weapon))
                return ModSkins[weapon];
            Il2CppSystem.Collections.Generic.List<WeaponSkin> skins = new Il2CppSystem.Collections.Generic.List<WeaponSkin>();
            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Skins", weapon.displayName)))
            {
                IEnumerable<string> dirs = Directory.EnumerateDirectories(Path.Combine(Environment.CurrentDirectory, "Skins", weapon.displayName));
                foreach (string d in dirs)
                {
                    if (File.Exists(Path.Combine(d, "MainTexture.png")))
                        skins.Add(FromImageDir(d,weapon));
                }
                Allskins.AddRange(skins.ToArray());
                ModSkins.Add(weapon, skins.Cast<Il2CppSystem.Collections.Generic.IEnumerable<WeaponSkin>>());
            }
            return skins.Cast<Il2CppSystem.Collections.Generic.IEnumerable<WeaponSkin>>();
        }
        static Texture2D Load(string dir)
        {
            Texture2D diffuse = new Texture2D(1, 1, TextureFormat.DXT1, false, true);
            Il2CppImageConversionManager.LoadImage(diffuse, File.ReadAllBytes(dir));
            return diffuse;
        }
        public static WeaponSkin FromImageDir(string file, WeaponProfile weapon)
        {

            Material mat = new Material(Shader.Find("Standard"));
            mat.mainTexture = Load(Path.Combine(file, "MainTexture.png"));
            try
            {
                mat.SetTexture("_MetallicGlossMap", Load(Path.Combine(file, "Metallic.png")));
                mat.EnableKeyword("_METALLICGLOSSMAP");
            }
            catch { }
            try
            {
                mat.SetTexture("_BumpMap", Load(Path.Combine(file, "Normal.png")));
                mat.EnableKeyword("_NORMALMAP");
            }
            catch { }
            try
            {
                mat.SetTexture("_ParallaxMap", Load(Path.Combine(file, "Height.png")));
                mat.EnableKeyword("_PARALLAXMAP");
            }
            catch { }
            try
            {
                mat.SetTexture("_OcclusionMap", Load(Path.Combine(file, "Occlusion.png")));
                mat.EnableKeyword("_SPECGLOSSMAP");
            }
            catch { }
            try
            {
                mat.SetTexture("_EmissionMap", Load(Path.Combine(file, "Emission.png")));
                mat.EnableKeyword("_EMISSION");
            }
            catch { }
            try
            {
                mat.SetTexture("_DetailMask", Load(Path.Combine(file, "Detail.png")));
                mat.EnableKeyword("_DETAIL_MULX2");
            }
            catch { }
            Il2CppSystem.Collections.Generic.List<string> list = new Il2CppSystem.Collections.Generic.List<string>();
            list.Add(weapon.name);
            WeaponSkin s = new WeaponSkin()
            {
                //supporter id
                dlcId = "1122400",
                material = mat,
                suppressorMaterial = mat,
                name = file.Split('\\').Last(),
                title = file.Split('\\').Last(),
                steamPageUrl = "https://github.com/BustR75",
                weaponNames = list
            };
            return s;
        }
        public static void Dump(WeaponSkin skin,WeaponProfile weapon)
        {
            Todump.Add(skin, weapon);
        }
        static Texture2D MakeReadable(Texture2D img)
        {
            img.filterMode = FilterMode.Point;
            RenderTexture rt = RenderTexture.GetTemporary(img.width, img.height);
            rt.filterMode = FilterMode.Point;
            RenderTexture.active = rt;
            UnityEngine.Graphics.Blit(img, rt);
            Texture2D img2 = new Texture2D(img.width, img.height);
            img2.ReadPixels(new Rect(0, 0, img.width, img.height), 0, 0);
            img2.Apply();
            RenderTexture.active = null;
            return img2;
        }
    }
}
