using Microsoft.Xna.Framework.Input;
using MysteryWorld.Models.Enums;

namespace MysteryWorld.Models;

internal sealed class KeyEventModel
{
    public readonly Keys key;
    public readonly KeyEventTypeEnum type;

    public KeyEventModel(Keys key, KeyEventTypeEnum type = KeyEventTypeEnum.OnButtonPressed)
    {
        this.key = key;
        this.type = type;
    }
}
