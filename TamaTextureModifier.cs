using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using Il2CppInterop.Runtime.Injection;
using BepInEx.Logging;

namespace TamaMod
{
    [BepInPlugin("com.mod.tama.texture", "Tama Texture Modifier", "1.0.0")]
    public class TamaTextureModifier : BasePlugin
    {
        public static ManualLogSource StaticLog;

        public override void Load()
        {
            StaticLog = Log;
            ClassInjector.RegisterTypeInIl2Cpp<SocksAdjusterComponent>();

            UnityEngine.GameObject helper = new UnityEngine.GameObject("TamaSocksHelper");
            helper.hideFlags = HideFlags.HideAndDontSave;
            helper.AddComponent<SocksAdjusterComponent>();
            UnityEngine.Object.DontDestroyOnLoad(helper);

            if (StaticLog != null)
            {
                StaticLog.LogInfo("TamaTextureModifier Loaded: -/= for Holes, [] for Black Edge, P to Reset Width.");
            }
        }

        public class SocksAdjusterComponent : MonoBehaviour
        {
            public SocksAdjusterComponent(System.IntPtr ptr) : base(ptr) { }

            private float threshold = 0f;
            private float dissolveWidth = 0.33f;

            void Update()
            {
                bool changed = false;

                // 1. 调节破损程度 (_MagneticFluidCoveringThreshold)
                if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Equals))
                {
                    threshold += 0.005f;
                    if (threshold > 1f) threshold = 1f;
                    changed = true;
                }
                if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Minus))
                {
                    threshold -= 0.005f;
                    if (threshold < 0f) threshold = 0f;
                    changed = true;
                }

                // 2. 调节黑边宽度 (_DissolveWidth)
                if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightBracket))
                {
                    dissolveWidth += 0.005f;
                    if (dissolveWidth > 1f) dissolveWidth = 1f;
                    changed = true;
                }
                if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftBracket))
                {
                    dissolveWidth -= 0.005f;
                    if (dissolveWidth < 0f) dissolveWidth = 0f;
                    changed = true;
                }

                // 3. P 键恢复初始宽度 (0.33)
                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.P))
                {
                    dissolveWidth = 0.33f;
                    changed = true;
                    if (TamaTextureModifier.StaticLog != null)
                        TamaTextureModifier.StaticLog.LogInfo("DissolveWidth Reset to 0.33");
                }

                if (changed)
                {
                    ApplySettings();
                }
            }

            void ApplySettings()
            {
                UnityEngine.GameObject body = UnityEngine.GameObject.Find("tama5.2_body");
                if (body == null) return;

                UnityEngine.SkinnedMeshRenderer smr = body.GetComponent<UnityEngine.SkinnedMeshRenderer>();
                if (smr == null) return;

                UnityEngine.Material mat = smr.material;
                if (mat == null) return;

                // 应用数值到 Shader 属性
                mat.SetFloat("_MagneticFluidCoveringThreshold", threshold);
                mat.SetFloat("_DissolveWidth", dissolveWidth);

                // 实时反馈到控制台
                if (TamaTextureModifier.StaticLog != null)
                {
                    TamaTextureModifier.StaticLog.LogInfo("Threshold: " + threshold.ToString("F3") + " | Width: " + dissolveWidth.ToString("F3"));
                }
            }
        }
    }
}