using Godot;
using NullCyan.Util.Log;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Util;
using Godot.Collections;

namespace NullCyan.Sandboxnator.Entity;

[GlobalClass]
public partial class PlayerProfileData : Resource
{
    private string _playerName = "DEFAULT_PLAYER";
    [Export]
    public string PlayerName
    {
        get => _playerName;
        set => _playerName = value;
    }

    private Color _playerColor = Colors.White;
    [Export]
    public Color PlayerColor
    {
        get => _playerColor;
        set => _playerColor = value;
    }

    private string _playerFaceId = "TinySmile";
    [Export]
    public string PlayerFaceId
    {
        get => _playerFaceId;
        set => _playerFaceId = value;
    }

    public PlayerProfileData() { }

    public PlayerProfileData(string name, Color color, string faceId)
    {
        PlayerName = name;
        PlayerColor = color;
        PlayerFaceId = faceId;
    }

    public void RandomizeProfile()
    {
        GD.Randomize();
        PlayerName = FillNameField();
        PlayerColor = new Color(GD.Randf(), GD.Randf(), GD.Randf());
        PlayerFaceId = GameRegistries.Instance.PlayerFaceRegistry.GetRandomEntry().Value.playerFaceId;
    }

    private string FillNameField()
    {
        NameGenerator nameGen = NameGenerator.Create();
        if (GD.Randf() <= 0.7f) nameGen.UseDictedPatterns();
        else nameGen.UseSimplePatterns();

        string name = nameGen.GenerateName();
        return char.ToUpper(name[0]) + name.Substring(1);
    }
    public override string ToString()
    {
        return $"[color={PlayerColor.ToHtml()}] name:{PlayerName} colour:{PlayerColor.ToHtml()}[/color] faceID:{PlayerFaceId}";
    }

    public void PrintProperties(string message = "")
    {
        NcLogger.Log($"{message} : {ToString()}");
    }
}