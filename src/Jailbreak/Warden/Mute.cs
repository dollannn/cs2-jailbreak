// TODO: we want to just copy hooks from other plugin and name them in here
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CSTimer = CounterStrikeSharp.API.Modules.Timers;
using CS2_SimpleAdminApi;
using System.Linq;

public class Mute
{
    void MuteT()
    {
        if (Config.Prisoner.MuteAlways || !Config.Prisoner.ThirtySecMute)
            return;

        Chat.LocalizeAnnounce(MUTE_PREFIX, "mute.thirty");

        JB.Lib.MuteT();

        if (JB.JailPlugin.globalCtx != null)
            muteTimer = JB.JailPlugin.globalCtx.AddTimer(30.0f, UnMuteAll, CSTimer.TimerFlags.STOP_ON_MAPCHANGE);

        muteActive = true;
    }

    public void UnMuteAll()
    {
        Chat.LocalizeAnnounce(MUTE_PREFIX, "mute.speak_quietly");

        // Go through and unmute all alive players!
        foreach (CCSPlayerController player in JB.Lib.GetAlivePlayers())
        {
            if (JB.JailPlugin.globalCtx?.SimpleAdminEnabled == true && JB.JailPlugin.globalCtx._SimpleAdminsharedApi != null)
            {
                var muteStatus = JB.JailPlugin.globalCtx._SimpleAdminsharedApi.GetPlayerMuteStatus(player);
                if (muteStatus?.Count == 0)
                {
                    player.UnMute();
                }
                else
                {
                    var muted_str = Chat.Localize("mute.muted");
                    var muted_prefix = Chat.Localize("mute.mute_prefix");
                    player.PrintToChat(muted_prefix + muted_str);
                }
            }
        }

        muteTimer = null;

        muteActive = false;
    }

    public void RoundStart()
    {
        JB.Lib.KillTimer(ref muteTimer);

        MuteT();
    }

    public void RoundEnd()
    {
        JB.Lib.KillTimer(ref muteTimer);

        JB.Lib.UnMuteAll(true);
    }

    public void Connect(CCSPlayerController? player)
    {
        // just connected mute them
        player.Mute();
    }

    public void ApplyListenFlags(CCSPlayerController player)
    {
        // default to listen all
        player.ListenAll();

        // if ct cannot hear team, change listen flags to team only
        if (player.IsCt() && Config.Guard.VoiceOnly)
            player.ListenTeam();
    }

    public void Spawn(CCSPlayerController? player)
    {
        if (!player.IsLegal())
            return;

        ApplyListenFlags(player);

        if (Config.Prisoner.MuteAlways && player.IsT())
        {
            player.Mute();
            return;
        }

        // no mute active or on ct unmute
        if (!muteActive || player.IsCt())
            player.UnMute();
    }

    public void Death(CCSPlayerController? player)
    {
        // mute on death
        if (!player.IsLegal())
            return;

        // warden with no forced removal let them keep speaking
        if (JB.JailPlugin.IsWarden(player) && !Config.Guard.Warden.ForceRemoval)
            return;

        if (Config.Settings.MuteDead)
        {
            player.LocalizePrefix(MUTE_PREFIX, "mute.end_round");
            player.Mute();
        }
    }

    public void SwitchTeam(CCSPlayerController? player, int new_team)
    {
        if (!player.IsLegal())
            return;

        ApplyListenFlags(player);

        // player not alive mute
        if (!player.IsLegalAlive())
            player.Mute();

        // player is alive
        else
        {
            // on ct fine to unmute
            if (new_team == Player.TEAM_CT)
                player.UnMute();

            else
            {
                // mute timer active, mute the client
                if (muteActive || Config.Prisoner.MuteAlways)
                    player.Mute();
            }
        }
    }

    public JailConfig Config = new JailConfig();

    CSTimer.Timer? muteTimer = null;

    public static String MUTE_PREFIX = $" {ChatColors.Green}[MUTE]: {ChatColors.White}";

    // has the mute timer finished?
    bool muteActive = false;
};