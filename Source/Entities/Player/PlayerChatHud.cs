using Godot;
using System;
using NullCyan.Sandboxnator.Chat;
using NullCyan.Util.ComponentSystem;
namespace NullCyan.Sandboxnator.Entity;

public partial class PlayerChatHud : AbstractComponent<Player>
{
    [Export] public Control chatRoot;
    [Export] public LineEdit messageEdit;
    [Export] public RichTextLabel messageBox;
    [Export] public AudioStreamPlayer notificationSound;

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
        ComponentParent.playerInput.IsChatOpen = chatRoot.Visible;
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
        chatRoot.Visible = true;
        messageEdit.FocusMode = Control.FocusModeEnum.All;
        messageEdit.CallDeferred(Control.MethodName.GrabFocus);
    }

    private void HideChat()
    {
        chatRoot.Visible = false;
    }

    private void ReceiveMessage(ChatMessage message, PlayerProfileData senderData)
    {
        //-1: System notifications
        if (message.PlayerId != -1)
        {
            messageBox.Text += $"[color={senderData.PlayerColor.ToHtml()}](@{senderData.PlayerName}) [/color] : {message.Content}\n";
        }
        else
        {
            messageBox.Text += $"[color=yellow][System]:[/color]{message.Content}\n";
        }
        notificationSound.Play();
    }

    private void SendMessage()
    {
        string msg = messageEdit.Text;
        if (!string.IsNullOrEmpty(msg) && !string.IsNullOrWhiteSpace(messageEdit.Text) && chatRoot.Visible)
        {
            ChatManager.Instance.RequestSendMessageToServer(msg);
            messageEdit.Text = "";
        }
        messageEdit.FocusMode = Control.FocusModeEnum.All;
        messageEdit.CallDeferred(Control.MethodName.GrabFocus);
    }

    public void _on_send_btn_pressed()
    {
        SendMessage();
    }
}
