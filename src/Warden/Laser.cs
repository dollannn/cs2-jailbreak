using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using JB;
using System.Drawing;

public partial class Warden
{
    void RemoveMarker()
    {
        marker.Destroy();
    }

    public void Ping(CCSPlayerController? player, float x, float y, float z)
    {
        JailPlayer? jailPlayer = JailPlayerFromPlayer(player);

        // draw marker
        if (IsWarden(player) && player.IsLegal() && jailPlayer != null)
        {
            // make sure we destroy the old marker
            // because this generates alot of ents
            RemoveMarker();

            marker.Draw(60.0f, 75.0f, x, y, z, jailPlayer.markerColour);
        }
    }

    void RemoveLaser()
    {
        laser.Destroy();
    }

    public void LaserTick()
    {
        if (!Config.Guard.Warden.Laser)
            return;

        if (wardenSlot == INAVLID_SLOT)
            return;

        CCSPlayerController? warden = Utilities.GetPlayerFromSlot(wardenSlot);

        if (!warden.IsLegalAliveCT())
            return;

        bool useKey = (warden.Buttons & PlayerButtons.Use) == PlayerButtons.Use;

        CCSPlayerPawn? pawn = warden.Pawn();
        CPlayer_CameraServices? camera = pawn?.CameraServices;

        JailPlayer? jailPlayer = JailPlayerFromPlayer(warden);

        if (pawn != null && pawn.AbsOrigin != null && camera != null && useKey && jailPlayer != null)
        {
            Vector eye = new Vector(pawn.AbsOrigin.X,pawn.AbsOrigin.Y,pawn.AbsOrigin.Z + camera.OldPlayerViewOffsetZ);

            Vector? eyeVector = warden.EyeVector();

            if (eyeVector == null)
                return;

            // scale out to an arbitary length
            eyeVector = Vec.Scale(eyeVector,3000);

            // add the vectors toegher
            Vector end = Vec.Add(eye,eyeVector);

            /*
                warden.PrintToChat($"end: {end.X} {end.Y} {end.Z}");
                warden.PrintToChat($"angle: {eye_angle.X} {eye_angle.Y}");
            */

            laser.colour = jailPlayer.laserColour;
            laser.Move(eye, end, 2.0f, laser.colour);
        }

        // hide laser
        else RemoveLaser();
    }

    void SetLaser(CCSPlayerController player, ChatMenuOption option)
    {
        if (!player.IsLegal())
            return;

        var text = option.Text!;
        JailPlayer? jailPlayer = JailPlayerFromPlayer(player);

        if (jailPlayer != null)
            jailPlayer.SetLaser(player,text);
    }

    void SetMarker(CCSPlayerController player, ChatMenuOption option)
    {
        if (!player.IsLegal())
            return;

        var text = option.Text!;
        JailPlayer? jailPlayer = JailPlayerFromPlayer(player);

        if (jailPlayer != null)
            jailPlayer.SetMarker(player,text);
    }

    public void LaserColourCmd(CCSPlayerController? player)
    {
        if (!player.IsLegal())
            return;

        JB.Lib.ColourMenu(player,SetLaser,"Laser colour");
    }

    public void MarkerColourCmd(CCSPlayerController? player)
    {
        if (!player.IsLegal())
            return;

        JB.Lib.ColourMenu(player,SetMarker,"Marker colour");
    }

    public static readonly float LASER_TIME = 0.1f;

    Circle marker = new Circle();
    Line laser = new Line();
}