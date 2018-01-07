using GTA;
using GTA.Math;

namespace GrandTheftSpace.CoreGame.Library
{
    internal static class GTAUtil
    {
        public static Prop CreateProp(string modelName, Vector3 spawnPos)
        {
            var model = new Model(modelName);

            if (!model.IsValid)
            {
                return null;
            }

            model.Request(10000);

            return World.CreateProp(model, spawnPos, false, false);
        }
    }
}
