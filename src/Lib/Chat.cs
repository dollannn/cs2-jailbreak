using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;


public static class Chat
{
    // chat + centre text print
    static public void Announce(String prefix,String str)
    {
        Server.PrintToChatAll(prefix + str);
        PrintCenterAll(str);
    }

    static public void PrintPrefix(this CCSPlayerController? player, String prefix, String str)
    {
        if (player.IsLegal() && player.IsConnected() && !player.IsBot)
            player.PrintToChat(prefix + str);
    }

    static public void Announce(this CCSPlayerController? player,String prefix,String str)
    {
        if (player.IsLegal() && player.IsConnected() && !player.IsBot)
        {
            player.PrintPrefix(prefix,str);
            player.PrintToCenter(str);
        }
    }

    // TODO: i dont think there is a builtin func for this...
    static public void PrintCenterAll(String str)
    {
        foreach(CCSPlayerController player in JB.Lib.GetPlayers())
            player.PrintToCenter(str);
    }

    static public void PrintConsoleAll(String str, bool admin_only = false)
    {
        foreach (CCSPlayerController player in JB.Lib.GetPlayers())
        {
            if (admin_only && !player.IsGenericAdmin() || player.IsBot)
                continue;

            player.PrintToConsole(str);
        }
    }

    static public void LocalizeAnnounce(this CCSPlayerController? player,String prefix, String name, params Object[] args)
    {
        player.Announce(prefix,Localize(name,args));
    }

    static public void LocalizeAnnounce(String prefix, String name, params Object[] args)
    {
        String str = Localize(name,args);

        Server.PrintToChatAll(prefix + str);
        PrintCenterAll(str);
    }

    public static String Localize(String name, params Object[] args)
    {
        return JB.JailPlugin.Localize(name,args);
    }

    static public void Localize(this CCSPlayerController? player,String name, params Object[] args)
    {
        if(player.IsLegal())
            player.PrintToChat(Localize(name,args)); 
    }

    static public void LocalizePrefix(this CCSPlayerController? player,String prefix, String name, params Object[] args)
    {
        if(player.IsLegal())
            player.PrintToChat(prefix + Localize(name,args));
    }
}