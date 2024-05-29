using System;
using MysteryWorld.Models;
using MysteryWorld.Models.Interfaces;

namespace MysteryWorld.Controllers;

public sealed class EventController
{
    public event Action<INavigationEvent> OnScreenRequest;
    public event Action<ResolutionEventModel> OnResolutionRequest;
    public event Action OnFullScreenRequest;
    public event Action OnExit;

    internal void SendScreenRequest(INavigationEvent screen)
    {
        OnScreenRequest?.Invoke(screen);
    }

    internal void SendResolutionRequest(ResolutionEventModel resolutionEvent)
    {
        OnResolutionRequest?.Invoke(resolutionEvent);
    }

    internal void SendFullScreenRequest()
    {
        OnFullScreenRequest?.Invoke();
    }

    internal void CloseGame()
    {
        OnExit?.Invoke();
    }

    public event Action<IPopupEvent> OnPopupEvent;

    public void SendPopupEvent(IPopupEvent popupEvent)
    {
        OnPopupEvent?.Invoke(popupEvent);
    }
}