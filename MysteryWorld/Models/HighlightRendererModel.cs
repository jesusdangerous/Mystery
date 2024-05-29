using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MysteryWorld.Controllers;
using MysteryWorld.Views;

namespace MysteryWorld.Models;

public class HighlightRendererModel
{
    private const int OutlineScale = 2;
    private const float DragSensitivity = 8f;

    private bool isDragging;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private readonly MouseController mouseListener;
    private readonly CameraController camera;

    public HighlightRendererModel(CameraController camera)
    {
        mouseListener = new MouseController();
        this.camera = camera;
    }

    public void HandleInput()
    {
        mouseListener.Update();
        if (mouseListener.WasPressedLmb())
        {
            startPoint = camera.CameraToWorld(mouseListener.GetMousePosition());
            endPoint = camera.CameraToWorld(mouseListener.GetMousePosition());
        }
        else if (mouseListener.IsHeldLmb())
        {
            endPoint = camera.CameraToWorld(mouseListener.GetMousePosition());
            if (Vector2.Distance(startPoint, endPoint) > DragSensitivity)
                isDragging = true;
        }
        else if (mouseListener.WasReleasedLmb())
            isDragging = false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!isDragging) return;
        spriteBatch.DrawRectangle(CalculateSelectionBounds(), (int)(OutlineScale / camera.Zoom) + 1, Color.Red);
    }

    public Rectangle CalculateSelectionBounds()
    {
        return new Rectangle((int)Math.Min(startPoint.X, endPoint.X), (int)Math.Min(startPoint.Y, endPoint.Y),
            (int)Math.Max(startPoint.X, endPoint.X) - (int)Math.Min(startPoint.X, endPoint.X),
            (int)Math.Max(startPoint.Y, endPoint.Y) - (int)Math.Min(startPoint.Y, endPoint.Y));
    }
}