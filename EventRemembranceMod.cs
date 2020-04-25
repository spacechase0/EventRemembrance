using Farmhand;
using Farmhand.API.Dialogues;
using Farmhand.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventRemembrance
{
    class EventRemembranceMod : Farmhand.Mod
    {
        public override void Entry()
        {
            EventEvents.OnEventFinished += afterEvent;
            GameEvents.OnAfterLoadedContent += afterContentLoaded;

            DialogueResultInformation dreamResult = new DialogueResultInformation(this, new Dialogue.AnswerResult(dream));
            DialogueAnswerInformation dreamAnswer = new DialogueAnswerInformation(this, "SleepDream", "Dream", dreamResult);
            Dialogue.RegisterNewAnswer(Questions.Sleep, dreamAnswer);
        }

        public static string filterEventCommands( string str )
        {
            // Did something else instead
            return str;

            List<string> commands = new List<string>(str.Split('/'));
            for ( int i = 0; i < commands.Count; ++i )
            {
                string cmd = commands[i];
                // ...
            }
        }

        internal static PreEventState preEvent;
        private void afterEvent( object sender, EventArgs args )
        {
            if (preEvent != null)
                preEvent.apply();
            preEvent = null;
        }

        private void afterContentLoaded( object sender, EventArgs args )
        {
            Dictionary<string, Dictionary<string, string>> eventLocs = Util.LoadContentFolder<Dictionary<string, string>>("Data/Events");
            Farmhand.Logging.Log.Verbose($"[EventRemembrance] Total event locations: {eventLocs.Count}");

            // Cache all the event stuff.
            // Key: ID
            // Value: <Location>/<Event Data>
            foreach ( var events in eventLocs )
            {
                foreach (var ev in events.Value)
                {
                    // Events without an ID tend to be 'forks' from choices during another event
                    // Also, the ID of events seen in the save is how we know to show
                    int slash = ev.Key.IndexOf('/');
                    if (slash == -1)
                        continue;

                    var currEv = new EventData(events.Key.Substring(0, events.Key.Length - 4), ev.Key, ev.Value);
                    eventData.Add( currEv );
                }
            }
            Farmhand.Logging.Log.Verbose($"[EventRemembrance] Total events: {eventData.Count}");

            // Load event names
            foreach ( string line in File.ReadLines( Path.Combine( ModSettings.ModDirectory, "eventnames.txt" ) ) )
            {
                if (line == "") continue;

                int eq = line.IndexOf('=');
                if ( eq == -1 )
                {
                    Farmhand.Logging.Log.Verbose("[EventRemembrance] Incorrectly formatted line in eventnames.txt");
                    continue;
                }

                int id = Convert.ToInt32(line.Substring(0, eq));
                string name = line.Substring(eq + 1);
                eventNames.Add(id, name);
            }
            Farmhand.Logging.Log.Verbose($"[EventRemembrance] Total event names: {eventNames.Count}");

            // Filter out extra so I know which to remove
            /*
            var names = new Dictionary<int, string>(eventNames);
            foreach( var ev in eventData)
                names.Remove(ev.Id);
            foreach (var ev in names)
                Farmhand.Logging.Log.Verbose("EXTRA EVENT " + ev.Key + "=" + ev.Value);
            //*/
        }

        private void dream( ref bool doDefault )
        {
            StardewValley.Game1.activeClickableMenu = new ChooseEventMenu();
            doDefault = false;
        }

        public static List<EventData> eventData = new List<EventData>();
        public static Dictionary<int, string> eventNames = new Dictionary<int, string>();
    }
}
