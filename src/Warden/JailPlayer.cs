using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Entities.Constants;

public class JailPlayer
{
    public void purge_round()
    {
        is_rebel = false;
    }

    public void reset()
    {
        purge_round();

        // TODO: reset client specific settings
    }

    public void set_rebel(CCSPlayerController? player,bool event_active)
    {
        // dont care if player is invalid
        if(!player.is_valid() || player == null)
        {
            return;
        }

        // on T with no warday or sd active
        if(player.TeamNum == Lib.TEAM_T && !event_active)
        {
            is_rebel = true;
        }
    }

    public void rebel_death(CCSPlayerController? player,CCSPlayerController? killer, bool event_active)
    {
        // event active dont care
        if(event_active)
        {
            return;
        }

        // players aernt valid dont care
        if(killer == null || player == null || !player.is_valid() || !killer.is_valid())
        {
            return;
        }

        // print death if player is rebel and killer on CT
        if(is_rebel && killer.TeamNum == Lib.TEAM_CT)
        {
            Server.PrintToChatAll($"[REBEL]: {killer.PlayerName} killed the rebel {player.PlayerName}");
        }
    }

    public void rebel_weapon_fire(CCSPlayerController? player, String weapon, bool event_active)
    {
        // ignore weapons players are meant to have
        // TODO: we need to use the damage hook when we get our hands on it
        if(weapon != "knife" && weapon != "c4")
        {
            set_rebel(player,event_active);
        }
    }

    // TODO: Laser stuff needs to go here!
    // but we dont have access to the necessary primtives yet


    public bool is_rebel = false;
};
