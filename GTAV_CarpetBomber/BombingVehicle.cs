using GTA;
using GTA.Math;
using GTA.Native;

namespace GTAV_CarpetBomber
{
    public class BombingVehicle : Entity
    {
        private Camera bombCam;
        private bool bombBayState;
        private readonly Vehicle vehicle;

        public Vehicle Vehicle { get { return vehicle; } }
        public Camera BombCam { get { return bombCam; } }

        public BombingVehicle(int handle) : base(handle)
        {
            this.bombCam = null;
            this.vehicle = new Vehicle(handle);
        }

        public BombingVehicle(Vehicle vehicle) : this(vehicle.Handle)
        { }

        public void ToggleBombBayView()
        {
            if (bombCam == null)
            {
                ControlMonitor.DisableControl(Control.VehicleFlyAttack);
                ControlMonitor.DisableControl(Control.VehicleFlyAttack2);
                int boneIndex = Function.Call<int>(Hash._GET_ENTITY_BONE_INDEX, vehicle.Handle, "chassis_dummy");
                var boneCoord = Function.Call<Vector3>(Hash._GET_ENTITY_BONE_COORDS, vehicle.Handle, boneIndex);
                bombCam = new Camera(Function.Call<int>(Hash.CREATE_CAMERA_WITH_PARAMS, 0x19286a9, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 65.0f, 0, 2));
                bombCam.AttachTo(vehicle, new Vector3(0f, 1.3324f, -0.9f));
                bombCam.PointAt(vehicle, vehicle.GetOffsetFromWorldCoords(boneCoord - new Vector3(0, 0, 1.3f)));
                bombCam.Shake(CameraShake.RoadVibration, 0.4f);
                bombCam.FieldOfView = 65.0f;
                bombCam.IsActive = true;
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 1, 0, 3000, 1, 0, 0);
                Function.Call(Hash.DISPLAY_RADAR, 0);
                Function.Call(Hash.DISPLAY_HUD, 0);
                SetAutopilot(true);
            }

            else
            {
                World.RenderingCamera = null;
                bombCam.IsActive = false;
                bombCam.Destroy();
                bombCam = null;
                Function.Call(Hash.DISPLAY_RADAR, 1);
                Function.Call(Hash.DISPLAY_HUD, 1);
                ControlMonitor.EnableControl(Control.VehicleFlyAttack);
                ControlMonitor.EnableControl(Control.VehicleFlyAttack2);
                SetAutopilot(false);
            }
        }

        public void ToggleBombBay()
        {
            if (bombBayState)
                Function.Call(Hash._0x3556041742A0DC74, vehicle.Handle);
            else
                Function.Call(Hash._0x87E7F24270732CB1, vehicle.Handle);

            Script.Wait(20);
            Scripts.RequestScriptAudioBank(@"SCRIPT\DRUG_TRAFFIC_AIR");
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "DRUG_TRAFFIC_AIR_BAY_DOOR_OPEN_MASTER", 0, 1);

            bombBayState = !bombBayState;
        }

        public void SetAutopilot(bool toggle)
        {
            if (toggle)
            {
                if (!Function.Call<bool>(Hash.IS_WAYPOINT_ACTIVE))
                    return;

                var wpBlip = new Blip(Function.Call<int>(Hash.GET_FIRST_BLIP_INFO_ID, 8));
                var wpVec = Function.Call<Vector3>(Hash.GET_BLIP_COORDS, wpBlip);
                Function.Call(Hash.TASK_VEHICLE_DRIVE_TO_COORD, vehicle.GetPedOnSeat(VehicleSeat.Driver), vehicle, wpVec.X, wpVec.Y, wpVec.Z + 150f, vehicle.Speed, 1, vehicle.Model.Hash, 262144, -1.0, -1.0);
            }
            else
                vehicle.GetPedOnSeat(VehicleSeat.Driver).Task.ClearAll();
        }
    }
}