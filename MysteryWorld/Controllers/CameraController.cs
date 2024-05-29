using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace MysteryWorld.Controllers
{
    public sealed class CameraController
    {
        private const int MapBoundaryOffset = 3;
        private const int SpeedLow = 12;
        private const int SpeedMedium = 15;
        private const int SpeedHigh = 20;
        private const int SpeedMax = 25;
        private const int EdgeOffset = 100;

        private const float Half = 2f;
        private const float ZoomThreshold1 = 0.4f;
        private const float ZoomThreshold2 = 2.4f;
        private const float ZoomClampLower = 0.3f;
        private const float ZoomClampUpper = 2.5f;

        public Rectangle Bounds { get; private set; }
        public Matrix Transform { get; private set; }
        public Vector2 Position { get; private set; }
        public float Zoom { get; private set; }
        public float CameraSpeed { get; private set; }
        public bool IsLocked { get; set; }

        internal Rectangle VisibleArea;
        internal Rectangle VisibleMap;
        private Matrix Inverse;

        public CameraController(Vector2 initPos)
        {
            UpdateBounds();
            VisibleMap = new Rectangle();
            VisibleArea = new Rectangle();
            CameraSpeed = SpeedLow;
            Zoom = 1f;
            Position = initPos;
            Transform = new Matrix();
            UpdateTransform();
            UpdateVisible();
        }

        private void UpdateTransform()
        {
            Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) * Matrix.CreateScale(Zoom, Zoom, 1) *
                        Matrix.CreateTranslation(new Vector3(Bounds.Width / Half, Bounds.Height / Half, 0));
            Inverse = Matrix.Invert(Transform);
        }

        private void UpdateVisible()
        {
            var topLeft = Vector2.Transform(Vector2.Zero, Inverse);
            var bottomRight = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), Inverse);
            VisibleArea = new Rectangle(new Point((int)topLeft.X, (int)topLeft.Y), new Point((int)(bottomRight.X - topLeft.X), (int)(bottomRight.Y - topLeft.Y)));
            VisibleMap = new Rectangle(new Point((int)(topLeft.X / GameController.ScaledPixelSize - 1), (int)(topLeft.Y / GameController.ScaledPixelSize) - 1),
                new Point((int)((bottomRight.X - topLeft.X) / GameController.ScaledPixelSize + MapBoundaryOffset), (int)((bottomRight.Y - topLeft.Y) / GameController.ScaledPixelSize + MapBoundaryOffset)));
        }

        private void MoveCamera(Vector2 pos)
        {
            Position += pos;
        }

        private void NormalizeMovement(ref float normalizedMovementX, ref float normalizedMovementY)
        {

            normalizedMovementX = Math.Clamp(normalizedMovementX, -CameraSpeed, CameraSpeed);
            normalizedMovementY = Math.Clamp(normalizedMovementY, -CameraSpeed, CameraSpeed);
        }

        private void AdjustMouse(Point mousePosition, ref Vector2 cameraMovement, float nX, float nY)
        {
            if (mousePosition.X <= EdgeOffset)
            {
                cameraMovement.X = -CameraSpeed;
                cameraMovement.Y = nY;
            }
            else if (mousePosition.X >= Bounds.Width - EdgeOffset)
            {
                cameraMovement.X = CameraSpeed;
                cameraMovement.Y = nY;
            }
            if (mousePosition.Y <= EdgeOffset)
            {
                cameraMovement.Y = -CameraSpeed;
                cameraMovement.X = nX;
            }
            else if (mousePosition.Y >= Bounds.Height - EdgeOffset)
            {
                cameraMovement.Y = CameraSpeed;
                cameraMovement.X = nX;
            }
        }

        private void SetCameraSpeed(float zoom)
        {
            CameraSpeed = zoom switch
            {
                <= ZoomThreshold1 => SpeedMax,
                > ZoomThreshold1 and < ZoomThreshold2 => SpeedHigh,
                >= ZoomThreshold2 => SpeedMedium,
                _ => SpeedLow
            };
        }

        internal void UpdateCamera()
        {
            UpdateBounds();
            UpdateTransform();
            UpdateVisible();

            if (!IsLocked)
            {
                var cameraMovement = Vector2.Zero;
                var mousePosition = Mouse.GetState().Position;

                var normalizedX = (float)((-1.0 + 2.0 * mousePosition.X / GameController.ScreenWidth) * CameraSpeed);
                var normalizedY = (float)(-(1.0 - 2.0 * mousePosition.Y / GameController.ScreenHeight) * CameraSpeed);

                NormalizeMovement(ref normalizedX, ref normalizedY);
                SetCameraSpeed(Zoom);
                AdjustMouse(mousePosition, ref cameraMovement, normalizedX, normalizedY);
                MoveCamera(cameraMovement);
            }
        }

        public static Vector2 TileCenterToWorld(Vector2 gridVector)
        {
            gridVector.X = gridVector.X * GameController.ScaledPixelSize + GameController.ScaledPixelSize / Half;
            gridVector.Y = gridVector.Y * GameController.ScaledPixelSize + GameController.ScaledPixelSize / Half;
            return gridVector;
        }

        public Vector2 CameraToWorld(Vector2 screenPosition) =>
            Vector2.Transform(screenPosition, Matrix.Invert(Transform));

        public void AdjustZoom(float zoomDelta) { Zoom = Math.Clamp(Zoom + zoomDelta, ZoomClampLower, ZoomClampUpper); }
        internal void SetCameraPosition(Vector2 position) { Position = position; }
        private void UpdateBounds() { Bounds = new Rectangle(new Point(0, 0), new Point(GameController.ScreenWidth, GameController.ScreenHeight)); }
    }
}