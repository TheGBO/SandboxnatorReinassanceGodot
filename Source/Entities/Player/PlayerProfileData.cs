using Godot;
using Godot.Collections;
using System;
using MessagePack;
using MessagePackGodot;
using NullCyan.Util;
using NullCyan.Util.Log;
namespace NullCyan.Sandboxnator.Entity;

[MessagePackObject(true)]
public partial class PlayerProfileData
{
    [Key(0)]
    public string PlayerName { get; set; } = $"DEFAULT_PLAYER";

    [Key(1)]
    public Color PlayerColor { get; set; } = Colors.White;
    //TODO: PlayerTexture : The skin texture of the player, a Map that maps a string to a .tres material, this class will only get the texture, there will be a singleton in the future that will hold the registry for player skins in that dictionary.

    public void PrintProperties(string message = "")
    {
        //using the british spelling on debug logs to avoid rich text conflict lol
        NcLogger.Log($"{message} : [color={PlayerColor.ToHtml()}] name:{PlayerName} colour:{PlayerColor.ToHtml()}[/color]");
        // byte[] binaryData = MPacker.Pack(this);
        // GD.Print("Raw bytes: ", BitConverter.ToString(binaryData).ToLower().Replace("-", ""));
        // GD.Print($"when packed as a byte array, this has {binaryData.Length} bytes");
    }
}

