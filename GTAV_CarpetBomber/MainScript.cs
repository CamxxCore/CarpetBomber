using System;
using GTA;
using GTA.Math;
using GTA.Native;

namespace GTAV_CarpetBomber
{
    public class MainScript : Script
    {
        CarpetBomb cBomb, cBomb1;
        BombingVehicle mainVehicle;

        public MainScript()
        {
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            var player = Game.Player.Character;

            if (mainVehicle != null && !player.IsInVehicle(mainVehicle.Vehicle))
            {
                if (mainVehicle.BombCam != null)
                    mainVehicle.ToggleBombBayView();

                cBomb.DestroyActiveSounds();
                cBomb1.DestroyActiveSounds();
                mainVehicle = default(BombingVehicle);
            }

            if (player.IsInVehicle() && Function.Call<bool>(Hash.IS_THIS_MODEL_A_PLANE, player.CurrentVehicle.Model))
            {
                if ((mainVehicle == null || mainVehicle.Handle != player.CurrentVehicle.Handle))
                {
                    mainVehicle = new BombingVehicle(player.CurrentVehicle);
                    Vector3 leftOffset, rightOffset;

                    switch ((VehicleHash)player.CurrentVehicle.Model.Hash)
                    {
                        case VehicleHash.Cuban800:
                            leftOffset = new Vector3(-0.12f, -0.12f, -0.9f);
                            rightOffset = new Vector3(0.12f, -0.12f, -0.9f);
                            break;

                        case VehicleHash.Lazer:
                            leftOffset = new Vector3(-0.6f, 0, 0);
                            rightOffset = new Vector3(0.6f, 0, 0);
                            break;

                        default:
                            leftOffset = new Vector3(-0.12f, -0.12f, -1.1f);
                            rightOffset = new Vector3(0.12f, -0.12f, -1.1f);
                            break;
                    }

                    cBomb = new CarpetBomb(mainVehicle.Vehicle, leftOffset);
                    cBomb1 = new CarpetBomb(mainVehicle.Vehicle, rightOffset);
                }

                else if (mainVehicle.Vehicle.IsDriveable)
                {
                    cBomb.Update();
                    cBomb1.Update();

                    Control ctlToggleDoor, ctlToggleView, ctlAttack;
                    ctlToggleDoor = ctlToggleView = ctlAttack = default(Control);

                    if (Function.Call<bool>(Hash._GET_LAST_INPUT_METHOD, 2))
                    {
                        ctlToggleDoor = Control.SelectWeaponMelee;
                        ctlToggleView = Control.SelectWeaponUnarmed;
                        ctlAttack = Control.VehicleFlyAttack2;
                    }

                    else
                    {
                        ctlToggleDoor = Control.ScriptRS;
                        ctlToggleView = Control.ScriptPadRight;
                        ctlAttack = Control.ScriptRDown;
                    }

                    if (Game.IsControlJustReleased(0, ctlToggleDoor))
                    {
                        mainVehicle.ToggleBombBay();
                    }

                    if (Game.IsControlJustPressed(0, ctlToggleView))
                    {
                        mainVehicle.ToggleBombBayView();
                    }

                    if (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, (int)ctlAttack) && mainVehicle.BombCam != null)
                    {
                        cBomb.StartBombingSequence();
                        Wait(150);
                        cBomb1.StartBombingSequence();
                    }
                }
            }
        }

        protected override void Dispose(bool A_0)
        {
            World.RenderingCamera = null;
            Function.Call(Hash.DISPLAY_RADAR, true);
            Function.Call(Hash.DISPLAY_HUD, true);
            Function.Call(Hash.DESTROY_ALL_CAMS, false);

            if (cBomb != null)
            {
                cBomb.DestroyActiveBombs();
                cBomb.DestroyActiveSounds();
            }

            if (cBomb1 != null)
            {
                cBomb1.DestroyActiveBombs();
                cBomb1.DestroyActiveSounds();
            }

            base.Dispose(A_0);
        }
    }
}
