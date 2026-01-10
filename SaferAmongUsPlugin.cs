using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace SaferAmongUs;

[BepInAutoPlugin("com.matchducking.saferamongus")]
[BepInProcess("Among Us.exe")]
public partial class SaferAmongUsPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        Harmony.PatchAll();

        SceneManager.add_sceneLoaded((System.Action<Scene, LoadSceneMode>)((scene, _) =>
        {
            if (scene.name == "MainMenu")
            {
                ModManager.Instance.ShowModStamp();
            }
        }));
    }
}