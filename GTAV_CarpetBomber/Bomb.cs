using GTA;
using GTA.Native;
using GTA.Math;

namespace GTAV_CarpetBomber
{
    public class Bomb : Entity
    {
        public Bomb(Vector3 position, Vector3 rotation) : base(Create(position, rotation))
        { }

        private static int Create(Vector3 position, Vector3 rotation)
        {
            var model = new Model("prop_ld_bomb_01");
            if (!model.IsLoaded)
                model.Request(1000);

            var mBomb = World.CreateProp(model, position, false, false);
            Function.Call(Hash.SET_ENTITY_RECORDS_COLLISIONS, mBomb.Handle, true);
            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, mBomb.Handle, true);
            Function.Call(Hash.SET_ENTITY_LOD_DIST, mBomb.Handle, 1000);
            mBomb.Rotation = rotation;
            return mBomb.Handle;
        }
    }
}
