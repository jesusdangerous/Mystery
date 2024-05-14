﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Manager;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.GameEvent
{
    class GameEventMessage : IGameEvent
    {
        public bool Done { get; set; }
        public string Text { get; private set; }

        public GameEventType EventType
        {
            get { return GameEventType.Message; }
        }

        public GameEventMessage(string text)
        {
            Text = text;
        }

        public void Initialize()
        {
            var window = new WindowMessage(Text);
            ManagerWindow.NewWindow("gameEventMessage", window);
            ManagerInput.ThrottleInput = true;
            Done = false;
        }

        public void Update(double gameTime)
        {
            if (!ManagerWindow.Contains("gameEventMessage"))
            {
                Done = true;
                ManagerInput.ThrottleInput = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}