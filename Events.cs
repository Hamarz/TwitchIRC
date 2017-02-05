using System;

namespace TwitchIRC
{
    public delegate void ConnectionHandler(object s, EventArgs e);
    public delegate void RawMessageHandler(object s, RawMessageEventArgs e);
    public delegate void MessageHandler(object s, MessageEventArgs e);
    public delegate void JoinHandler(object s, JoinEventArgs e);
    public delegate void LeaveHandler(object s, LeaveEventArgs e);
    public delegate void OperatorHandler(object s, OperatorEventArgs e);
    public delegate void CodeHandler(object s, CodeEventArgs e);

    public class MessageEventArgs : JoinEventArgs
    {
        public string Message { set; get; }
    }

    public class JoinEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
    }

    public class LeaveEventArgs : JoinEventArgs{}

    public class RawMessageEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class OperatorEventArgs : JoinEventArgs
    {
        public string Change { get; set; }
    }

    public class CodeEventArgs : MessageEventArgs
    {
        public string Code { get; set; }
    }
}
