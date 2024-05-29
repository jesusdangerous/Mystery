using Microsoft.Xna.Framework;
using MysteryWorld.Models.Enums;
using MysteryWorld.Views;

namespace MysteryWorld.Controllers
{
    sealed class EnvironmentIemController : GameObjectView
    {
        private const float EnvironmentalItemLayer = 0.9f;
        private readonly bool animationIsLooped;
        private readonly bool stayOnMap;
        public EnvironmentalAnimations Type;

        public EnvironmentIemController(Vector2 position, EnvironmentalAnimations type, EnvironmentalMode mode) : base(position)
        {
            Type = type;
            State = InstanceState.Pending;
            LayerDepth = EnvironmentalItemLayer;
            switch (mode)
            {
                case EnvironmentalMode.SingleSprite:
                    isNotAnimated = true;
                    animationIsLooped = false;
                    stayOnMap = true;
                    break;
                case EnvironmentalMode.SingleAnimation:
                    isNotAnimated = false;
                    animationIsLooped = false;
                    stayOnMap = false;
                    break;
                case EnvironmentalMode.AnimationStop:
                    isNotAnimated = false;
                    animationIsLooped = false;
                    stayOnMap = true;
                    break;
                case EnvironmentalMode.FullAnimation:
                    isNotAnimated = false;
                    animationIsLooped = true;
                    stayOnMap = true;
                    break;
            }
            SpriteId = (int)type;
            CurrentSpriteId = SpriteId;
        }
        private new void UpdateAnimation(float deltaTime)
        {
            if (isNotAnimated) return;

            if (animationTimer > TimeToNextFrame)
            {
                animationTimer = 0f;
                currentFrame += 1;

                if (currentFrame > TotalFrames)
                {
                    if (animationIsLooped) currentFrame = 0;
                    else
                    {
                        if (!stayOnMap)
                            State = InstanceState.LimitReached;

                        isNotAnimated = true;
                        currentFrame = TotalFrames;
                    }
                }
            }
            animationTimer += deltaTime;
        }

        public override void Update(float deltaTime)
        {
            UpdateAnimation(deltaTime);
        }
    }
}