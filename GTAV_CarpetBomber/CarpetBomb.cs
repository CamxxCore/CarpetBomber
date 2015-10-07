using System.Collections.Generic;
using GTA.Native;
using GTA.Math;
using GTA;

namespace GTAV_CarpetBomber
{
    public class CarpetBomb
    {
        private int bombSoundID;
        private int bombsDropped;
        private Timer dropTimer;
        private Vector3 dropOffset;
        private Vector3 updatedVel;
        private List<Bomb> bombs;
        private readonly Vehicle vehicle;

        public CarpetBomb(Vehicle vehicle, Vector3 dropOffset)
        {
            this.vehicle = vehicle;
            this.dropTimer = new Timer(300);
            this.dropOffset = dropOffset;
            this.updatedVel = new Vector3();
            this.bombs = new List<Bomb>();
            this.bombsDropped = 0;
            this.bombSoundID = -1;
        }

        public void StartBombingSequence()
        {
            if ((!vehicle.IsOnAllWheels || vehicle.Speed > 0.0f || !vehicle.IsInWater) && !dropTimer.Enabled)
            {
                DestroyActiveBombs();
                DestroyActiveSounds();
                Scripts.RequestScriptAudioBank(@"SCRIPT\DRUG_TRAFFIC_AIR");
                bombSoundID = Function.Call<int>(Hash.GET_SOUND_ID);
                Function.Call(Hash.PLAY_SOUND_FRONTEND, bombSoundID, "DRUG_TRAFFIC_AIR_BOMB_CAM_MASTER", 0, 1);
                dropTimer.Start();
            }
        }

        public void AddSequentialBomb()
        {
            if (bombs.Count < 1)
            {
                var rot = new Vector3(80.0f, 0.0f, vehicle.Heading + 180.0f);
                var mBomb = new Bomb(vehicle.GetOffsetInWorldCoords(dropOffset), rot);
                mBomb.Velocity = vehicle.Velocity + new Vector3(0, 0, -5f);
                bombs.Add(mBomb);
            }

            else
            {
                var prevRot = bombs[bombs.Count - 1].Rotation;

                if (prevRot.X < 79.1)
                {
                    prevRot.X += 4.0f;
                    if (prevRot.X > 79.1)
                        prevRot.X = 79.1f;
                    bombs[bombs.Count - 1].Rotation = prevRot;
                }

                var rot = new Vector3(80.0f, 0.0f, vehicle.Heading + 180.0f);
                var mBomb = new Bomb(vehicle.GetOffsetInWorldCoords(dropOffset), rot);
                mBomb.Velocity = bombs[bombs.Count - 1].Velocity;
                bombs.Add(mBomb);
            }
        }

        public void Update()
        {
            if (dropTimer.Enabled && Game.GameTime > dropTimer.Waiter)
            {
                if (bombsDropped < 10)
                {
                    AddSequentialBomb();
                    bombsDropped++;

                    if (bombsDropped == 10)
                        bombs[bombs.Count - 1].IsVisible = false;

                    dropTimer.Reset();
                }

                else
                {
                    dropTimer.Enabled = false;
                    bombsDropped = 0;
                }
            }

            if (bombs.Count < 2) return;

            for (int i = 0; i < bombs.Count; i++)
            {
                if (!Function.Call<bool>((Hash)0xE9676F61BC0B3321, bombs[i].Handle))
                {
                    Function.Call(Hash.REQUEST_COLLISION_AT_COORD, bombs[i].Position.X, bombs[i].Position.Y, bombs[i].Position.Z);
                }

                if (Function.Call<bool>(Hash.HAS_ENTITY_COLLIDED_WITH_ANYTHING, bombs[i].Handle) || bombs[i].IsInWater)
                {
                    DestroyActiveSounds();
                    if (bombs[i].Position.DistanceTo(vehicle.Position) > 15f)
                        PlayExplosion(bombs[i]);                               
                    bombs[i].Delete();
                    bombs.RemoveAt(i);
                    continue;
                }

                if (i == bombs.Count - 1)
                    break;

                OutputArgument arg1, arg2, arg3, arg4;
                arg1 = arg2 = arg3 = arg4 = new OutputArgument();

                float velocityZ = -60.0f;
                float velMultiplier = -5.0f;

                if (bombs[i + 1].HeightAboveGround > 150.0f)
                {
                    velocityZ = -70.0f;
                    velMultiplier = -10.0f;
                }

                if (!bombs[i + 1].IsDead)
                {
                    updatedVel = bombs[i + 1].Velocity;
                    Function.Call(Hash.GET_ENTITY_MATRIX, bombs[i + 1].Handle, arg1, arg2, arg3, arg4);
                }

                var result = arg3.GetResult<Vector3>();

                var vMag = Function.Call<float>(Hash.VMAG, result.X, result.Y, result.Z);
                if (vMag != 0.0)
                    result *= 1.0f / vMag;

                updatedVel += result * velMultiplier;

                if (updatedVel.Z <= velocityZ)
                    updatedVel.Z = velocityZ;

                if (Function.Call<bool>(Hash.DOES_ENTITY_HAVE_PHYSICS, bombs[i].Handle))
                    Function.Call(Hash.SET_ENTITY_MAX_SPEED, bombs[i].Handle, 90.0f);

                bombs[i].Velocity = updatedVel;
            }
        }

        public void DestroyActiveSounds()
        {
            if (bombSoundID != -1)
            {
                Function.Call(Hash.STOP_SOUND, bombSoundID);
                Function.Call(Hash.RELEASE_SOUND_ID, bombSoundID);
                bombSoundID = -1;
            }
        }

        public void DestroyActiveBombs()
        {
            bombs.ForEach(x => x.Delete());
            bombs.Clear();
        }

        private void PlayExplosion(Entity ent)
        {
            Scripts.RequestPTFXAsset("scr_oddjobtraffickingair");
            Function.Call(Hash._SET_PTFX_ASSET_NEXT_CALL, "scr_oddjobtraffickingair");

            if (ent.IsInWater)
                Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, "scr_ojdg4_water_exp", ent.Position.X, ent.Position.Y, ent.Position.Z, 0.0, 0.0, 0.0, 3.0, 0, 0, 0);
            else
                Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, "scr_drug_grd_train_exp", ent.Position.X, ent.Position.Y, ent.Position.Z, 0.0, 0.0, 0.0, 3.0, 0, 0, 0);

            World.AddExplosion(ent.Position, ExplosionType.Train, 1.0f, 1.0f);
            Script.Wait(20);
            World.AddExplosion(ent.Position + ent.Position.LeftVector(Vector3.WorldUp) * 3, (ExplosionType)17, 30f, 1.5f);
            Script.Wait(20);
            World.AddExplosion(ent.Position + ent.Position.RightVector(Vector3.WorldUp) * 3, (ExplosionType)26, 30f, 1.5f);
            Script.Wait(20);
            World.AddExplosion(ent.Position + ent.ForwardVector * 3, (ExplosionType)17, 30f, 1.5f);
            Script.Wait(20);
            World.AddExplosion(ent.Position - ent.ForwardVector * 3, (ExplosionType)26, 30f, 1.5f);
        }
    }
}
