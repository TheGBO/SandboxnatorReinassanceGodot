using Godot;
namespace NullCyan.Sandboxnator.UI;

public partial class ValueLabel : Label
{
	[Export] private Range displayValue;
	[Export] private string displayPrefix = "";

	public override void _Process(double delta)
	{
		LabelDisplayValue();
	}

	private void LabelDisplayValue()
	{
		Text = $"{displayPrefix}{displayValue.Value}";
	}
}
