using AmongUs.GameOptions;
using HarmonyLib;
using InnerNet;
using TMPro;
using UnityEngine;

namespace SaferAmongUs;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Toggle))]
public static class TogglePatch
{
    public static void Postfix(ChatController __instance)
    {
        __instance.UpdateChatMode();
    }
}
[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
public static class AddChatPatch
{
    public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer, [HarmonyArgument(1)] string chatText)
    {
        if (LobbyBehaviour.Instance && (BlockedWords.ContainsWord(chatText) || DaterCheck.IsDaterChat(chatText)))
        {
            if (sourcePlayer == PlayerControl.LocalPlayer)
            {
                __instance.AddChatWarning("Message not sent");
            }
            return false;
        }

        return true;
    }
}
[HarmonyPatch(typeof(BlockedWords), nameof(BlockedWords.ContainsWord))]
public static class ImprovedBlockedWords
{
    public static void Postfix([HarmonyArgument(0)] string chatText, ref bool __result)
    {
        if (DaterCheck.IsDater(chatText)) __result = true;
    }
}
[HarmonyPatch(typeof(NameTextBehaviour), nameof(NameTextBehaviour.IsValidName))]
public static class ImprovedBlockedWordsSpecific
{
    public static void Postfix([HarmonyArgument(0)] string text, ref bool __result)
    {
        if (DaterCheck.IsDaterChat(text)) __result = false;
    }
}
[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.MakePublic))]
public static class RemoveMakePublic
{
    public static bool Prefix()
    {
        return false;
    }
}
[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.DoHostSetup))]
public static class PublicButtonNotInteractable
{
    public static bool Prefix(GameStartManager __instance)
    {
        string text = GameCode.IntToGameName(AmongUsClient.Instance.GameId);
        if (!AmongUsClient.Instance.AmHost)
        {
            __instance.HostPrivacyButtons.SetActive(false);
            __instance.ClientPrivacyValue.SetActive(true);
            __instance.StartButton.gameObject.SetActive(false);
            __instance.StartButtonClient.gameObject.SetActive(true);
            __instance.GameStartTextParent.SetActive(false);
            __instance.HostInfoPanelButtons.SetActive(false);
            __instance.ClientInfoPanelButtons.SetActive(true);
            return false;
        }
        if (text != null)
        {
            __instance.HostPrivacyButtons.SetActive(false);
            __instance.ClientPrivacyValue.SetActive(true);
        }
        __instance.HostInfoPanelButtons.SetActive(true);
        __instance.ClientInfoPanelButtons.SetActive(false);
        __instance.StartButton.gameObject.SetActive(true);
        __instance.StartButtonClient.gameObject.SetActive(false);
        return false;
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
public static class SetLobbyVisibility
{
    public static void Postfix()
    {
        AmongUsClient.Instance.ChangeGamePublic(AddPublicPrivateToggle.isPublic);
    }
}
[HarmonyPatch(typeof(BanMenu), nameof(BanMenu.SetVisible))]
public static class DisableBanButton
{
    public static bool Prefix(BanMenu __instance, [HarmonyArgument(0)] bool show)
    {
        bool flag;
        if (PlayerControl.LocalPlayer)
        {
            NetworkedPlayerInfo data = PlayerControl.LocalPlayer.Data;
            flag = data != null && !data.IsDead;
        }
        else
        {
            flag = false;
        }
        bool flag2 = flag;
        show &= PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Data != null;
        __instance.BanButton.gameObject.SetActive(false);
        __instance.KickButton.gameObject.SetActive(flag2);
        __instance.MenuButton.gameObject.SetActive(show);
        return false;
    }
}
[HarmonyPatch(typeof(BanMenu), nameof(BanMenu.Kick))]
public static class ForceVoteKick
{
    public static bool Prefix(BanMenu __instance)
    {
        if (__instance.selectedClientId >= 0)
        {
            VoteBanSystem.Instance.CmdAddVote(__instance.selectedClientId);
            __instance.Select(-1);
        }
        return false;
    }
}
[HarmonyPatch(typeof(ConfirmCreatePopUp), nameof(ConfirmCreatePopUp.OnEnable))]
public static class AddPublicPrivateToggle
{
    public static GameObject VisibilityToggle;
    public static bool isPublic = false;

    public static void Postfix(ConfirmCreatePopUp __instance)
    {
        if (VisibilityToggle != null) return;

        var containerConfirm = __instance.transform.Find("ContainerConfirm");
        var createGame = containerConfirm.Find("CreateGame");

        VisibilityToggle = GameObject.Instantiate(createGame).gameObject;
        VisibilityToggle.transform.parent = containerConfirm;
        VisibilityToggle.transform.localScale = Vector3.one;
        VisibilityToggle.transform.localPosition = new Vector3(0f, -2.42f, -1f);
        VisibilityToggle.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        VisibilityToggle.GetComponent<PassiveButton>().OnClick.AddListener(new System.Action(() => ToggleStatus(VisibilityToggle)));
        VisibilityToggle.GetComponent<PassiveButton>().ClickSound = null;
        UpdateStatus(VisibilityToggle);
    }

    public static void ToggleStatus(GameObject button)
    {
        isPublic = !isPublic;
        UpdateStatus(button);
    }

    public static void UpdateStatus(GameObject button)
    {
        var tmpro = button.transform.Find("ModeText").GetComponent<TextMeshPro>();
        if (tmpro.GetComponent<TextTranslatorTMP>())
        {
            GameObject.DestroyImmediate(tmpro.GetComponent<TextTranslatorTMP>());
        }
        if (isPublic)
        {
            tmpro.text = TranslationController.Instance.GetString(StringNames.PublicHeader);
        }
        else
        {
            tmpro.text = TranslationController.Instance.GetString(StringNames.PrivateHeader);
        }
    }
}
[HarmonyPatch(typeof(CreateGameOptions), nameof(CreateGameOptions.Start))]
public static class ChangeMaxPlayers
{
    public static void Postfix(CreateGameOptions __instance)
    {
        var maxPlayerOpt = GameObject.Find("GameOption_Number").GetComponent<NumberOption>();
        maxPlayerOpt.ValidRange = new FloatRange(7, 15);
        if (GameOptionsManager.Instance.CurrentGameOptions.TryGetInt(Int32OptionNames.MaxPlayers, out var originalValue) && originalValue >= 7)
        {
            maxPlayerOpt.Value = originalValue;
        }
        else
        {
            maxPlayerOpt.Value = 15;
        }
    }
}