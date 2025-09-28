using Godot;
using Godot.Collections;
using System;
using MessagePack;
using MessagePackGodot;
using NullCyan.Util;
using NullCyan.Util.Log;
using NullCyan.Sandboxnator.Registry;
namespace NullCyan.Sandboxnator.Entity;

[MessagePackObject(true)]
public partial class PlayerProfileData
{
    [Key(0)]
    public string PlayerName { get; set; } = $"DEFAULT_PLAYER";

    [Key(1)]
    public Color PlayerColor { get; set; } = Colors.White;

    [Key(2)]
    public string PlayerFaceId { get; set; } = "TinySmile";

    public override string ToString()
    {
        return $"[color={PlayerColor.ToHtml()}] name:{PlayerName} colour:{PlayerColor.ToHtml()}[/color] faceID:{PlayerFaceId}";
    }

    public void PrintProperties(string message = "")
    {
        //using the british spelling on debug logs to avoid rich text conflict lol
        NcLogger.Log($"{message} : {ToString()}");
        // byte[] binaryData = MPacker.Pack(this);
        // GD.Print("Raw bytes: ", BitConverter.ToString(binaryData).ToLower().Replace("-", ""));
        // GD.Print($"when packed as a byte array, this has {binaryData.Length} bytes");
    }

    public void RandomizeProfile()
    {
        GD.Randomize();
        PlayerName = FillNameField();
        PlayerColor = new(GD.Randf(), GD.Randf(), GD.Randf());
        PlayerFaceId = GameRegistries.Instance.PlayerFaceRegistry.GetRandomEntry().Value.playerFaceId;

    }

    private string FillNameField()
    {
        NameGenerator nameGen = NameGenerator.Create();
        if (GD.Randf() <= 0.7)
        {
            nameGen.UseWesternPatterns();
        }
        else
        {
            nameGen.UseSimplePatterns();
        }
        string name = nameGen.GenerateName();
        string nameCorrected = char.ToUpper(name[0]) + name.Substring(1);
        NcLogger.Log($"Randomly generated name: {nameCorrected}");
        return nameCorrected;
    }
}

