﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Manager;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.GameEvent
{
    class GameEventSwitch : IGameEvent
    {
        public bool Done { get; set; }
        public int EventSwitchId { get; set; }
        public bool Value { get; set; }
        public GameEventType EventType { get { return GameEventType.EventSwitch; } }

        public GameEventSwitch(int eventSwitchId, bool value)
        {
            EventSwitchId = eventSwitchId;
            Value = value;
        }
        public void Initialize()
        {
            Done = false;
        }

        public void Update(double gameTime)
        {
            ManagerLists.SetEventSwitchValue(EventSwitchId, Value);
            Done = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
