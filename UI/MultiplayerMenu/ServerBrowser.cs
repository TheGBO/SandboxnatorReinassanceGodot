using Godot;
using NullCyan.Util.Log;
using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
//TODO: separation of concerns between networking and server browser UI
namespace NullCyan.Sandboxnator.UI;

public partial class ServerBrowser : Panel
{

    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

}
