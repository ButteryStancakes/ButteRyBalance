using ButteRyBalance.Network;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(RadMechAI))]
    class OldBirdPatches
    {
        [HarmonyPatch(nameof(RadMechAI.Start))]
        [HarmonyPostfix]
        static void RadMechAI_Post_Start(RadMechAI __instance)
        {
            if (BRBNetworker.Instance.RobotRider.Value)
            {
                Transform headPhysicsRegion = new GameObject("HeadPhysicsRegion").transform;
                headPhysicsRegion.transform.SetParent(__instance.flyingModeEye.parent, false);
                headPhysicsRegion.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, -90f, 0f));
                headPhysicsRegion.transform.localScale = Vector3.one;
                headPhysicsRegion.gameObject.layer = 11; // Colliders

                Transform itemRegion = new GameObject("ItemRegion").transform;
                itemRegion.transform.SetParent(headPhysicsRegion, false);
                itemRegion.transform.SetLocalPositionAndRotation(new(0.0839999989f, 4.21999979f, -0.0160000008f), Quaternion.Euler(0f, -90f, 0f));
                itemRegion.transform.localScale = new(2.65949273f, 2.36880684f, 6.71592999f);
                itemRegion.gameObject.layer = 11; // Colliders

                BoxCollider headTrigger = headPhysicsRegion.gameObject.AddComponent<BoxCollider>();
                headTrigger.isTrigger = true;
                headTrigger.center = new(0.0399663262f, 4.37110519f, -0.203689113f);
                headTrigger.size = new(6.759974f, 2.87486291f, 2.94662189f);

                BoxCollider headCollider = headPhysicsRegion.gameObject.AddComponent<BoxCollider>();
                headCollider.center = new(0.000214454092f, 4.23026705f, -0.0562505536f);
                headCollider.size = new(1.40990055f, 0.206586823f, 2.70570874f);

                BoxCollider leftCollider = headPhysicsRegion.gameObject.AddComponent<BoxCollider>();
                leftCollider.center = new(-2.03359866f, 3.09493351f, -0.0739010125f);
                leftCollider.size = new(2.14170933f, 0.447652966f, 1.87487447f);

                BoxCollider rightCollider = headPhysicsRegion.gameObject.AddComponent<BoxCollider>();
                rightCollider.center = new(2.02334881f, 3.09493351f, -0.0739012584f);
                rightCollider.size = new(2.1624248f, 0.447652966f, 1.87487447f);

                Rigidbody headRigidbody = headPhysicsRegion.gameObject.AddComponent<Rigidbody>();
                headRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                headRigidbody.isKinematic = true;

                BoxCollider itemCollider = itemRegion.gameObject.AddComponent<BoxCollider>();
                itemCollider.isTrigger = true;

                Rigidbody itemRigidbody = itemRegion.gameObject.AddComponent<Rigidbody>();
                itemRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                itemRigidbody.isKinematic = true;

                PlayerPhysicsRegion playerPhysicsRegion = headPhysicsRegion.gameObject.AddComponent<PlayerPhysicsRegion>();
                playerPhysicsRegion.physicsTransform = headPhysicsRegion;
                playerPhysicsRegion.parentNetworkObject = __instance.GetComponent<NetworkObject>();
                playerPhysicsRegion.allowDroppingItems = true;
                playerPhysicsRegion.priority = 1;
                playerPhysicsRegion.physicsCollider = headCollider;
                playerPhysicsRegion.itemDropCollider = itemCollider;
                playerPhysicsRegion.maxTippingAngle = 65f;
            }
        }
    }
}
