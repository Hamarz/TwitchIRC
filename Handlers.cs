using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TwitchIRC
{
    public static class Handlers
    {
        
        public static event MessageHandler OnPing;
        public static event JoinHandler OnJoin;
        public static event LeaveHandler OnLeave;
        public static event OperatorHandler OnOperator;
        public static event CodeHandler OnCodeMessage;
        public static event MessageHandler OnChannelMessage;

        public static void HandlePing(Match m, IrcClient c)
        {
            OnPing?.Invoke(c, new MessageEventArgs() { Message = m.Groups["ip"].Value });
        }

        public static void HandleMessage(Match m, IrcClient c)
        {
            var username = m.Groups["name"].Value;
            var message = m.Groups["message"].Value;
            //Console.WriteLine($"{username}: {message}");

            OnChannelMessage?.Invoke(c, new MessageEventArgs() { User = username, Channel = m.Groups["channel"].Value, Message = message });
        }

        public static void HandleJoin(Match m, IrcClient c)
        {
            OnJoin?.Invoke(c, new JoinEventArgs() { User = m.Groups["name"].Value, Channel = m.Groups["channel"].Value });
        }

        public static void HandlePart(Match m, IrcClient c)
        {
            OnLeave?.Invoke(c, new LeaveEventArgs() { User = m.Groups["name"].Value, Channel = m.Groups["channel"].Value });
        }

        public static void HandleCode(Match m, IrcClient c)
        {

        }

        public static void HandleNotice(Match m, IrcClient c)
        {

        }

        public static void HandleNotImplemented(Match m, IrcClient c)
        {
            throw new NotImplementedException($"There was no handler found for, check Regex configuration for any spelling error.");
        }
    }
}
