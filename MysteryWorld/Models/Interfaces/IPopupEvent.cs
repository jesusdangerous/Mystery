using Microsoft.Xna.Framework;

namespace MysteryWorld.Models.Interfaces;

public interface IPopupEvent
{
    public class NotificationPopup : IPopupEvent
    {
        public readonly string Notification;
        public readonly Color Color;
        public readonly float DisplayDuration;

        public NotificationPopup(string notification, Color color, float duration = 5f)
        {
            Notification = notification;
            Color = color;
            DisplayDuration = duration;
        }
    }

    public class SavePopUp : IPopupEvent
    {
    }
}
