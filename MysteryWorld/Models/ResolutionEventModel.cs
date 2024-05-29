namespace MysteryWorld.Models;

public class ResolutionEventModel
{
    public int Width { get; }
    public int Height { get; }

    public ResolutionEventModel(int width, int height)
    {
        Width = width;
        Height = height;
    }
}