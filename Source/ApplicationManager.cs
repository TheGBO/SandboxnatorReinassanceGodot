using Godot;
using NullGarel.Util.GodotHelpers;
using NullGarel.Util.Log;
namespace NullGarel.Sandboxnator;

/// <summary>
/// An autoload script to manage os events like closing the app.
/// </summary>
public partial class ApplicationManager : Singleton<ApplicationManager>
{
	private const string Msg = @"
	So long, it's been good to know ya
	So long, it's been good to know ya
	So long, it's been good to know ya";

	public override void _Ready()
	{
		GetTree().Root.CloseRequested += HandleCloseRequest;
	}

	public void HandleCloseRequest()
	{
		NcLogger.Log("ApplicationManager :: close requested.");

		//This is required to bypass godot's memory cleanup of resources. Otherwise some weird shit would be ganning on........
		if (OS.HasFeature("standalone"))
		{
			NcLogger.Log("https://www.youtube.com/watch?v=zqiblXFlZuk");
			NcLogger.Log(Msg);
			System.Environment.Exit(0);
			return;
		}
		GetTree().Quit();
	}

	public override void _ExitTree()
	{
		GetTree().Root.CloseRequested -= HandleCloseRequest;
	}
}
