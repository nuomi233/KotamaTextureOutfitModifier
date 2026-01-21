using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using Il2CppInterop.Runtime.Injection;
using BepInEx.Logging;

namespace TamaMod
{
    [BepInPlugin("com.mod.tama.boneswapper", "Tama Bone Swapper", "1.0.0")]
    public class TamaBoneSwapper : BasePlugin
    {
        public static ManualLogSource? StaticLog;

        public override void Load()
        {
            StaticLog = Log;
            StaticLog.LogInfo("Tama Bone Swapper (IL2CPP) 正在载入...");

            // 注册组件类
            ClassInjector.RegisterTypeInIl2Cpp<BoneControlComponent>();

            // 显式指定 UnityEngine 以消除 Object 引用歧义
            UnityEngine.GameObject helper = new UnityEngine.GameObject("TamaBoneHelper");
            helper.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
            helper.AddComponent<BoneControlComponent>();
            UnityEngine.Object.DontDestroyOnLoad(helper);

            StaticLog.LogInfo("骨骼缩放已成功注入游戏逻辑。");
        }

        public class BoneControlComponent : MonoBehaviour
        {
            public BoneControlComponent(System.IntPtr ptr) : base(ptr) { }

            private bool hideTail = false;
            private bool hideSkirt = false;
            private bool hideFront = false;

            void Update()
            {
                // 监听键盘按键
                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Alpha6))
                {
                    hideTail = !hideTail;
                    TamaBoneSwapper.StaticLog?.LogInfo($"尾巴显示状态: {!hideTail}");
                }
                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Alpha7))
                {
                    hideSkirt = !hideSkirt;
                    TamaBoneSwapper.StaticLog?.LogInfo($"裙摆显示状态: {!hideSkirt}");
                }
                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Alpha8))
                {
                    hideFront = !hideFront;
                    TamaBoneSwapper.StaticLog?.LogInfo($"裙帘与蝴蝶结显示状态: {!hideFront}");
                }
            }

            void LateUpdate()
            {
                // 6键控制：尾巴
                SetBoneScale("Skinbone014", hideTail ? 0f : 1f);

                // 7键控制：裙摆
                string[] skirtBones = { "Bone022", "Bone029", "Bone034", "Bone022(mirrored)", "Bone029(mirrored)", "Bone034(mirrored)" };
                foreach (string name in skirtBones) SetBoneScale(name, hideSkirt ? 0f : 1f);

                // 8键控制：前裙帘与蝴蝶结
                string[] frontBones = { "Bone001", "Bone006", "Bone010" };
                foreach (string name in frontBones) SetBoneScale(name, hideFront ? 0f : 1f);
            }

            private void SetBoneScale(string boneName, float scaleValue)
            {
                // 使用模糊匹配查找场景中的骨骼节点
                UnityEngine.GameObject boneObj = UnityEngine.GameObject.Find(boneName);
                if (boneObj != null)
                {
                    boneObj.transform.localScale = new UnityEngine.Vector3(scaleValue, scaleValue, scaleValue);
                }
            }
        }
    }
}