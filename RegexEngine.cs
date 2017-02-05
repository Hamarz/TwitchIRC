using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace TwitchIRC
{
    public delegate void DelegateString(Match m, IrcClient c);
    public class RegexEngine
    {
        // {"Name", "MethodName", "Regex"}
        private string filePath = @"Data/RegexOptions.conf";

        public struct RegexAction
        {
            public string Name;
            public Action<Match, IrcClient> method;
        }
        private Dictionary<RegexAction, Regex> m_regex;

        public void LoadRegex()
        {
            int count = 0;
            m_regex = new Dictionary<RegexAction, Regex>();
            var lines = File.ReadAllLines(filePath);
            foreach(var line in lines)
            {
                if (line.StartsWith("#")) // It's a comment. Ping, 
                    continue;
                // Split the strings
                var options = line.Split(',');
                // Name
                var name = options[0];
                // MethodName
                var methodName = options[1].Remove(0, 1);
                // Regex
                var rgxStr = options[2].Remove(0, 1);
                m_regex.Add(new RegexAction() { Name = name, method = new Action<Match, IrcClient>(GetDelegateFromString(methodName))}, new Regex(rgxStr, RegexOptions.Compiled));
                count++;
                Console.WriteLine($"{name}: {methodName}(), with pattern: {rgxStr}");
            }
        }

        public RegexEngine()
        {
            if (!File.Exists(filePath))
                File.Create(filePath);
        }

        private DelegateString GetDelegateFromString(string methodName)
        {
            DelegateString function = Handlers.HandleNotImplemented;

            Type inf = typeof(Handlers);
            foreach (var method in inf.GetMethods())
                if (method.Name == methodName)
                    function = (DelegateString)Delegate.CreateDelegate(typeof(DelegateString), method);

            return function;
        }

        public Dictionary<RegexAction, Regex> GetRegex() { return m_regex; }
    }
}
