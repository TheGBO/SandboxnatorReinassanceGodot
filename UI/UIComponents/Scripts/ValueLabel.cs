using Godot;
namespace NullGarel.Sandboxnator.UI;

public partial class ValueLabel : Label
{
	[Export] private Range _displayValue;
	[Export] private string _displayPrefix = "";

	public override void _Process(double delta)
	{
		LabelDisplayValue();
	}

	private void LabelDisplayValue()
	{
		Text = $"{_displayPrefix}{_displayValue.Value}";
	}
}
