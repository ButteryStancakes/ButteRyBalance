using GameNetcodeStuff;
using ScandalsTweaks.Scripts;
using UnityEngine;

namespace ButteRyBalance.Components
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(VehicleController))]
    public class ButteryCruiserManager : SVehicleEnemyManager
    {
        static readonly LayerMask VEHICLE_MASK = 1 << 30;

        VehicleController cruiser;

        void Awake()
        {
            cruiser = GetComponent<VehicleController>();
        }

        public override bool ShouldAllowSightThroughVehicle(EnemyAI enemyScript, PlayerControllerB playerScript)
        {
            if (cruiser == null || (cruiser.currentDriver != playerScript && cruiser.currentPassenger != playerScript))
                return false;

            if (enemyScript.eye == null || playerScript.gameplayCamera.transform == null)
                return false;

            // enemy has unbroken line-of-sight to the player's head
            if (!Physics.Linecast(enemyScript.eye.position, playerScript.gameplayCamera.transform.position, VEHICLE_MASK))
                return true;

            // let Old Birds stay locked onto the Cruiser if they were already targeting driver/passenger
            if (enemyScript is RadMechAI radMechAI && radMechAI.isAlerted && radMechAI.focusedThreatTransform == playerScript.transform)
                return true;

            return false;
        }
    }
}