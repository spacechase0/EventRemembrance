using System;

namespace EventRemembrance
{
    public class EventData
    {
        public EventData( string loc, string reqs, string cmds )
        {
            int slash = reqs.IndexOf('/');

            location = loc;
            id = slash == -1 ? -1 : Convert.ToInt32(reqs.Substring(0, slash));
            requirements = slash == -1 ? reqs : reqs.Substring(slash + 1);
            commands = cmds;
        }

        private int id;
        private string location;
        private string requirements;
        private string commands;

        public int Id { get { return id; } }
        public string Location { get { return location; } }
        public string Requirements { get { return id == -1 ? "" : requirements; } }
        public string Commands { get { return commands; } }
        public string Name
        {
            get
            {
                if (id == -1)
                    return requirements;
                if (EventRemembranceMod.eventNames.ContainsKey(id))
                    return EventRemembranceMod.eventNames[id];
                return "? " + id;
            }
        }
    }
}
