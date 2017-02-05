// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IrcClient.cs" company="HamHam">
//   
// </copyright>
// <summary>
//   The irc client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TwitchIRC
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Sockets;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// The irc client.
    /// </summary>
    public class IrcClient
    {
        public event ConnectionHandler OnConnected;
        public event MessageHandler OnRawMessage;
        private RegexEngine _engine;
       /* #region "Regex"
        public static readonly Regex PingMatch = new Regex(@"PING :(?<ip>\S+)", RegexOptions.Compiled);
        public static readonly Regex MessageMatch = new Regex(@":(?<name>\S+)!\S+\.tmi\.twitch\.tv PRIVMSG #(?<channel>\w+) :(?<message>.+)$", RegexOptions.Compiled);
        public static readonly Regex JoinMatch = new Regex(@":\S+!\S+@(?<name>\S+)\.tmi\.twitch\.tv JOIN #(?<channel>\w+)", RegexOptions.Compiled);
        public static readonly Regex PartMatch = new Regex(@":\S+!\S+@(?<name>\S+)\.tmi\.twitch\.tv PART #(?<channel>\w+)", RegexOptions.Compiled);
        public static readonly Regex OperatorMatch = new Regex(@":jtv MODE #(?<channel>\S+) (?<change>[+-])o (?<name>\S+)", RegexOptions.Compiled);
        public static readonly Regex CodeMatch = new Regex(@":(?<name>\S+)\.tmi\.twitch\.tv (?<code>\d{3}) \S+ = #(?<channel>\w+) :(?<message>.+)", RegexOptions.Compiled);
        public static readonly Regex NoticeMatch = new Regex(@"@msg-id=(?<msgID>\S+) :tmi\.twitch\.tv NOTICE #(?<channel>\S+) :(?<response>.+)", RegexOptions.Compiled);
        #endregion */

        public TcpClient Session;

        private StreamWriter writer;
        private StreamReader reader;

        private readonly bool debug = false;

        public IrcClient()
        {
            _engine = new RegexEngine();
            _engine.LoadRegex();
        }

        private void PrintIfDebug(string message)
        {
            if (this.debug)
            {
                Debug.Print(message);
            }
        }

        public bool IsConnected()
        {
            return this.Session.Connected;
        }

        public void Connect(string username, string oAuth)
        {
            this.Session = new TcpClient("irc.twitch.tv", 6667);
            this.writer = new StreamWriter(this.Session.GetStream()){ AutoFlush = true };
            this.reader = new StreamReader(this.Session.GetStream());

            this.writer.WriteLine($"PASS {oAuth}{Environment.NewLine} NICK {username.ToLower()}{Environment.NewLine}");
            // Raise OnConnected event!
            OnConnected?.Invoke(this, new EventArgs());

            new Thread(this.Listen)
                {
                    IsBackground = true
                }.Start();
        }

        private void Listen()
        {
            while (this.Session.Connected)
            {
                if (this.Session.Available <= 0 && this.reader.Peek() < 0)
                {
                    continue;
                }
                var message = this.reader.ReadLine();
                this.HandleData(message);

                OnRawMessage?.Invoke(this, new MessageEventArgs(){ Message = message});
            }
        }

        public void SendOther(string input)
        {
            if (!this.IsConnected())
            {
                return;
            }

            this.writer.WriteLine(input);
        }

        public void SendCap(string cap)
        {
            this.SendOther($"CAP REQ :{cap}");
        }

        public void JoinChannel(string channel)
        {
            string first = channel.Substring(0, 1);
            if(first == "#") channel = channel.Remove(0, 1);

            SendOther($"JOIN #{channel}");
        }

        public void SendMessage(string channel, string message)
        {
            var prefix = $"PRIVMSG #{channel} :{message}";

            this.SendOther(prefix);
        }

        private void HandleData(string message)
        {
            foreach(var option in _engine.GetRegex())
            {
                var regexMatch = option.Value.Match(message);

                if(!regexMatch.Success)
                    continue;

                option.Key.method.Invoke(regexMatch, this);
                break;
            }
        }
    }
}
