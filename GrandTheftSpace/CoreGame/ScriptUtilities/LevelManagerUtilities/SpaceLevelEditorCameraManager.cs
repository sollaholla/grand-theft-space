using GrandTheftSpace.CoreGame.Gameplay.Interfaces;
using GrandTheftSpace.CoreGame.Interfaces;
using GrandTheftSpace.CoreGame.Library;
using GTA;
using GTA.Math;
using System;
using System.Drawing;

namespace GrandTheftSpace.CoreGame.ScriptUtilities.LevelManagerUtilities
{
    public class SpaceLevelEditorCameraManager : ISettingsReader, IUpdatable
    {
        private float xRotation;                // The x rotation of the camera.
        private float yRotation;                // The y rotation of the camera.
        private float cameraSensitivity = 180;  // The rotation speed (in degrees) of the camera per second.
        private float cameraFieldOfView = 60;   // The field of view of the camera.
        private float movementSpeed = 150;      // The movement speed of the camera (meters per second).

        /// <summary>
        /// The camera we use to move around the level and view stuff.
        /// </summary>
        public Camera LevelEditorCamera { get; private set; }

        /// <summary>
        /// True if the custom hud is hidden.
        /// </summary>
        public bool HideHud { get; private set; }

        #region IUpdatable

        public void Init()
        {
            LevelEditorCamera = World.CreateCamera(GameplayCamera.Position, GameplayCamera.Rotation, 70);

            World.RenderingCamera = LevelEditorCamera;

            Game.Player.CanControlCharacter = false;
            Game.Player.Character.IsVisible = false;
        }

        public void Tick()
        {
            GTA.Native.Function.Call(GTA.Native.Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);

            RotateCamera();
            MoveCamera();
        }

        public void Stop()
        {
            if (LevelEditorCamera != null)
            {
                if (World.RenderingCamera == LevelEditorCamera)
                {
                    World.RenderingCamera = null;
                }

                LevelEditorCamera.Destroy();

                Game.Player.CanControlCharacter = true;
                Game.Player.Character.IsVisible = true;
            }
        }

        public void Abort()
        { }

        #endregion

        #region ISettingsReader

        public void ReadSettings(ScriptSettings scriptSettings)
        {
            cameraSensitivity = scriptSettings.GetValue("editor_camera", "camera_sensitivity", cameraSensitivity);
            scriptSettings.SetValue("editor_camera", "camera_sensitivity", cameraSensitivity);

            cameraFieldOfView = scriptSettings.GetValue("editor_camera", "camera_field_of_view", cameraFieldOfView);
            scriptSettings.SetValue("editor_camera", "camera_field_of_view", cameraFieldOfView);

            movementSpeed = scriptSettings.GetValue("editor_camera", "movement_speed", movementSpeed);
            scriptSettings.SetValue("editor_camera", "movement_speed", movementSpeed);
        }

        #endregion

        private void RotateCamera()
        {
            if (!LevelEditorCamera.IsActive)
            {
                return;
            }

            var sensitivity = Game.LastFrameTime * cameraSensitivity;
            var mouseInput = GetMouseInput();

            xRotation -= sensitivity * mouseInput.X;
            yRotation -= sensitivity * mouseInput.Z;

            xRotation = MathUtil.Clamp(xRotation, -90, 90);

            LevelEditorCamera.Rotation = new Vector3(xRotation, 0, yRotation);
        }

        private Vector3 GetMouseInput()
        {
            return new Vector3(Game.GetControlNormal(2, Control.LookUpDown), 0, Game.GetControlNormal(2, Control.LookLeftRight));
        }

        private void MoveCamera()
        {
            var scrollSpeed = Game.LastFrameTime * movementSpeed * 2f;
            movementSpeed += GetScrollInput() * scrollSpeed;

            var movement = GetMovementInput();
            var speed = Game.LastFrameTime * movementSpeed;

            var leftRightMovement = Quaternion.Euler(0, 0, LevelEditorCamera.Rotation.Z) * new Vector3(movement.X, 0, 0);
            var forwardBackMovement = LevelEditorCamera.Direction * movement.Y;

            movement = leftRightMovement + forwardBackMovement;
            LevelEditorCamera.Position += movement * speed;

            DrawMoveSpeedText();
        }

        private Vector3 GetMovementInput()
        {
            return new Vector3(Game.GetControlNormal(2, Control.MoveLeftRight), -Game.GetControlNormal(2, Control.MoveUpDown), 0);
        }

        private float GetScrollInput()
        {
            return Game.GetControlNormal(2, Control.CursorScrollUp) - Game.GetControlNormal(2, Control.CursorScrollDown);
        }

        private void DrawMoveSpeedText()
        {
            if (!HideHud)
            {
                var text = new UIText("Speed: " + movementSpeed.ToString("N0") + " m/s", new Point(UI.WIDTH / 2, 0), 0.5f, Color.White, GTA.Font.ChaletLondon, true, false, true);
                text.Draw();
            }
        }
    }
}
