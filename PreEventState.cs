using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventRemembrance
{
    class PreEventState
    {
        public GameLocation loc;
        public Vector2 pos;
        public List<Item> inv = new List<Item>();
        public int money;
        public SerializableDictionary<string, int[]> friendship = new SerializableDictionary<string, int[]>();
        public List<string> mailSeen = new List<string>();
        public List<string> mailTomorrow = new List<string>();
        public List<int> answers = new List<int>();
        public List<Quest> quests = new List<Quest>();
        public int farmScore;

        public PreEventState()
        {
            loc = Game1.player.currentLocation;
            pos = Game1.player.position;
            foreach (var entry in Game1.player.items)
                inv.Add(entry); // TODO: Clone for stack size isn't adjusted?
            money = Game1.player.money;
            foreach (var entry in Game1.player.friendships)
                friendship.Add(entry.Key, (int[]) entry.Value.Clone());
            foreach (var entry in Game1.player.mailReceived)
                mailSeen.Add(entry);
            foreach (var entry in Game1.player.mailForTomorrow)
                mailTomorrow.Add(entry);
            foreach (var entry in Game1.player.dialogueQuestionsAnswered)
                answers.Add(entry);
            foreach (var entry in Game1.player.questLog)
                quests.Add(entry);
            farmScore = Game1.getFarm().grandpaScore;
        }

        public void apply()
        {
            Game1.locationAfterWarp = loc;
            Game1.xLocationAfterWarp = (int)pos.X / Game1.tileSize;
            Game1.yLocationAfterWarp = (int)pos.Y / Game1.tileSize;
            Game1.player.items = inv;
            Game1.player.money = money;
            Game1.player.friendships = friendship;
            Game1.player.mailReceived = mailSeen;
            Game1.player.mailForTomorrow = mailTomorrow;
            Game1.player.dialogueQuestionsAnswered = answers;
            Game1.player.questLog = quests;
            Game1.getFarm().grandpaScore = farmScore;
        }
    }
}
