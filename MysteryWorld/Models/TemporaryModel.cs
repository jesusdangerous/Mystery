﻿using Microsoft.Xna.Framework;

namespace MysteryWorld.Models;

public class TemporaryModel
{
    public string Message { get; }
    public float Duration { get; }
    public float Timer { get; set; }
    public Color TextColor { get; }
    public bool Unlock { get; }
    public Vector2 Position { get; }

    public TemporaryModel(string message, Color textColor, Vector2 position, float duration = 1, bool unlock = false)
    {
        Message = message;
        TextColor = textColor;
        Duration = duration;
        Unlock = unlock;
        Position = position;
        Timer = 0;
    }
}