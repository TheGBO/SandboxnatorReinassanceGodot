using Godot;
using System;
using NullGarel.Sandboxnator.Chat;
using NullGarel.Util.ComponentSystem;
namespace NullGarel.Sandboxnator.Entity;

public partial class PlayerChatHud : AbstractComponent<Player>
{

    [ExportCategory("Full chat menu")]
    [Export] private LineEdit _messageEdit;
    [Export] private RichTextLabel _messageBox;
    [ExportCategory("Notification")]
    [Export] private AudioStreamPlayer _notificationSound;
    [ExportCategory("Latest message")]
    [Export] private RichTextLabel _quickMessageBox;
    private string _latestMessage = "";
    public string LatestMessage
    {
        get => _latestMessage;
        set
        {
            _latestMessage = value;
            _quickMessageBox.Text = _latestMessage;
        }
    }

    public override void _Ready()
    {
        if (!IsMultiplayerAuthority()) return;
        ComponentParent.playerInput.OnShowChat += ShowChat;
        ComponentParent.playerInput.OnUiEscape += HideChat;
        ChatManager.Instance.OnMessageReceived += ReceiveMessage;
    }

    public override void _Process(double delta)
    {
        if (!IsMultiplayerAuthority()) return;
        ComponentParent.playerHud.IsChatOpen = ComponentParent.playerHud.chatRoot.Visible;
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (!IsMultiplayerAuthority()) return;
        if (inputEvent is InputEventKey eventKey)
        {
            if (eventKey.Pressed && eventKey.Keycode == Key.Enter)
            {
                SendMessage();
                GetViewport().SetInputAsHandled();
            }
        }
    }

    private void ShowChat()
    {
        ComponentParent.playerHud.chatRoot.Visible = true;
        _messageEdit.FocusMode = Control.FocusModeEnum.All;
        _messageEdit.CallDeferred(Control.MethodName.GrabFocus);
    }

    private void HideChat()
    {
        ComponentParent.playerHud.chatRoot.Visible = false;
    }

    private void ReceiveMessage(ChatMessage message, PlayerProfileData senderData)
    {
        //-1: System notifications
        if (message.PlayerId != -1)
        {
            string computedText = $"[color={senderData.PlayerColor.ToHtml()}](@{senderData.PlayerName}) [/color] : {message.Content}\n";
            _messageBox.Text += computedText;
            LatestMessage = computedText;
        }
        else
        {
            string computedText = $"[color=yellow][System]:[/color]{message.Content}\n";
            _messageBox.Text += computedText;
        }
        _notificationSound.Play();
    }

    private void SendMessage()
    {
        string msg = _messageEdit.Text;
        if (!string.IsNullOrEmpty(msg) && !string.IsNullOrWhiteSpace(_messageEdit.Text) && ComponentParent.playerHud.chatRoot.Visible)
        {
            ChatManager.Instance.RequestSendMessageToServer(msg);
            _messageEdit.Text = "";
        }
        _messageEdit.FocusMode = Control.FocusModeEnum.All;
        _messageEdit.CallDeferred(Control.MethodName.GrabFocus);
    }

    public void _on_send_btn_pressed()
    {
        SendMessage();
    }
}
