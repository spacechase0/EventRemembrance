using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using System.Collections.Generic;

namespace EventRemembrance
{
    class ChooseEventMenu : IClickableMenu
    {
        private static int WIDTH = IClickableMenu.borderWidth * 2 + 800;
        private static int HEIGHT = IClickableMenu.borderWidth * 2 + 600;

        public ChooseEventMenu()
        : base((Game1.viewport.Width - WIDTH) / 2, (Game1.viewport.Height - HEIGHT) / 2, WIDTH, HEIGHT)
        {
            exitFunction = restoreMovement;
            seenEvents.AddRange(EventRemembranceMod.eventData.FindAll( ev => Game1.player.eventsSeen.Contains(ev.Id) && !ev.Name.StartsWith("null") ));
            seenEvents.Sort( (a, b) => a.Name.CompareTo(b.Name) );
        }

        bool didLeftClick = false;
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            didLeftClick = true;
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void receiveScrollWheelAction( int dir )
        {
            currScroll -= dir / 3;
            if (currScroll < 0)
                currScroll = 0;
            if (currScroll > ITEM_HEIGHT * seenEvents.Count - height + ITEM_HEIGHT)
                currScroll = ITEM_HEIGHT * seenEvents.Count - height + ITEM_HEIGHT;
        }

        EventData clickedEvent = null;
        public override void update(GameTime time)
        {
            base.update(time);
            didLeftClick = false;

            if (clickedEvent != null)
            {
                Game1.exitActiveMenu();

                string commands = EventRemembranceMod.filterEventCommands(clickedEvent.Commands);
                if (Game1.getLocationFromName(clickedEvent.Location).currentEvent == null)
                {
                    EventRemembranceMod.preEvent = new PreEventState();
                    Game1.getLocationFromName(clickedEvent.Location).currentEvent = new Event(commands);
                    Game1.warpFarmer(clickedEvent.Location, 8, 8, false);
                }
                else Farmhand.API.Dialogues.Dialogue.OpenStatement("There is a pending event in that area.");
            }
        }
        
        public override void draw(SpriteBatch b)
        {
            drawTextureBox(b, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, clip);
            {
                b.GraphicsDevice.ScissorRectangle = new Rectangle(xPositionOnScreen + IClickableMenu.borderWidth / 2, yPositionOnScreen + IClickableMenu.borderWidth / 2, width - IClickableMenu.borderWidth, height - IClickableMenu.borderWidth);
                
                for ( int i = currScroll / ITEM_HEIGHT - 2; i <= currScroll / ITEM_HEIGHT + height / ITEM_HEIGHT + 2; ++i )
                {
                    if (i < 0 || i >= seenEvents.Count)
                        continue;
                    EventData ev = seenEvents[i];

                    string str = ev.Name;
                    Vector2 pos = new Vector2(xPositionOnScreen + IClickableMenu.borderWidth, yPositionOnScreen + IClickableMenu.borderWidth + i * ITEM_HEIGHT - currScroll);
                    Color col = Color.Black;
                    if (!Game1.player.eventsSeen.Contains(i))
                    {
                        //str = "???";
                        col = NOTSEEN_COLOR;
                    }
                    /*else*/ if (Game1.getMouseX() >= xPositionOnScreen + IClickableMenu.borderWidth &&
                             Game1.getMouseX() < xPositionOnScreen + width - IClickableMenu.borderWidth &&
                             Game1.getMouseY() >= yPositionOnScreen + IClickableMenu.borderWidth + i * ITEM_HEIGHT - currScroll &&
                             Game1.getMouseY() < yPositionOnScreen + IClickableMenu.borderWidth + (i + 1) * ITEM_HEIGHT - currScroll &&
                             Game1.getMouseY() >= yPositionOnScreen + IClickableMenu.borderWidth &&
                             Game1.getMouseY() < yPositionOnScreen + height - IClickableMenu.borderWidth * 2)
                    {
                        col = HIGHLIGHT_COLOR;
                        if (didLeftClick)
                            clickedEvent = ev;
                    }
                    b.DrawString(Game1.dialogueFont, str, pos, col);

                    str = ev.Location;
                    pos.X = xPositionOnScreen + width - IClickableMenu.borderWidth - Game1.dialogueFont.MeasureString(str).X;
                    b.DrawString(Game1.dialogueFont, str, pos, col);
                }
            }
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState());

            base.draw(b);
            drawMouse(b);
        }

        private void restoreMovement()
        {
            Game1.player.CanMove = true;
        }

        private int currScroll = 0;
        private List<EventData> seenEvents = new List<EventData>();
        
        private const int ITEM_HEIGHT = 60;
        private static Color NOTSEEN_COLOR = new Color(128, 64, 64);
        private static Color HIGHLIGHT_COLOR = new Color(64, 64, 64);

        private static RasterizerState clip = new RasterizerState() { ScissorTestEnable = true };
    }
}
