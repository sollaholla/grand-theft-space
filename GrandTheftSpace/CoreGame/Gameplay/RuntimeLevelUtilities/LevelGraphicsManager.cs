using GTA;
using GTA.Math;
using System;

namespace GrandTheftSpace.CoreGame.Gameplay.RuntimeLevelUtilities
{
    internal class LevelGraphicsManager : RuntimeLevelUtility
    {
        public LevelGraphicsManager(RuntimeLevel runtimeLevel) : base(runtimeLevel)
        { }

        #region IUpdatable

        public override void Init()
        {
            SetTime();
        }

        public override void Tick()
        {
            UpdateTimecycleModifier();
            UpdateTime();
        }

        public override void Stop()
        {
            ClearTimecycleModifier();
            ResetTime();
        }

        public override void Abort()
        { }

        #endregion

        private void UpdateTimecycleModifier()
        {
            var timecycleModifier = LevelMetadata.DefaultTimecycleModifier;

            if (LevelMetadata.TimecycleAreas != null)
            {
                var camera = World.RenderingCamera;

                Vector3 cameraPosition;

                if (!Camera.Exists(camera))
                {
                    cameraPosition = GameplayCamera.Position;
                }
                else
                {
                    cameraPosition = camera.Position;
                }

                foreach (var modifierArea in LevelMetadata.TimecycleAreas)
                {
                    var distance = Vector3.Distance(cameraPosition, modifierArea.Offset);

                    if (distance > modifierArea.Radius)
                    {
                        continue;
                    }

                    timecycleModifier = modifierArea.TimecycleModifier;
                }
            }

            if (string.IsNullOrEmpty(timecycleModifier))
            {
                return;
            }

            GTA.Native.Function.Call(GTA.Native.Hash.SET_TIMECYCLE_MODIFIER, timecycleModifier);
        }

        private void ClearTimecycleModifier()
        {
            GTA.Native.Function.Call(GTA.Native.Hash.CLEAR_TIMECYCLE_MODIFIER);
        }

        private void UpdateTime()
        {
            if (LevelMetadata.FreezeTime)
            {
                SetTime();
            }
        }

        private void SetTime()
        {
            World.CurrentDayTime = new TimeSpan(World.CurrentDayTime.Days, LevelMetadata.Hour, LevelMetadata.Minute, 0);
        }

        private void ResetTime()
        {
            World.CurrentDayTime = new TimeSpan(World.CurrentDayTime.Days, 12, 0, 0);
        }
    }
}
