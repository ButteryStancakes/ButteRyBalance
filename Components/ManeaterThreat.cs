using UnityEngine;

namespace ButteRyBalance.Components
{
    public class ManeaterThreat : MonoBehaviour, IVisibleThreat
    {
        internal CaveDwellerAI caveDwellerAI;

        void Update()
        {
            if (caveDwellerAI == null)
                caveDwellerAI = GetComponent<CaveDwellerAI>();
        }

        // based on MouthDog

        ThreatType IVisibleThreat.type
        {
            get
            {
                return ThreatType.EyelessDog;
            }
        }

        int IVisibleThreat.GetThreatLevel(Vector3 seenByPosition)
        {
            int threatLevel = 1;
            if (caveDwellerAI != null)
            {
                if (caveDwellerAI.currentBehaviourStateIndex == 0) // baby
                    return threatLevel;

                threatLevel += 2;

                if (caveDwellerAI.enemyHP > 2)
                    threatLevel += 2;

                if (caveDwellerAI.creatureAnimator.GetBool("Leaping"))
                    threatLevel += 3;
            }
            return threatLevel;
        }

        int IVisibleThreat.GetInterestLevel()
        {
            return 0;
        }

        Transform IVisibleThreat.GetThreatLookTransform()
        {
            if (caveDwellerAI != null)
                return caveDwellerAI.eye;

            return transform;
        }

        Transform IVisibleThreat.GetThreatTransform()
        {
            return transform;
        }

        Vector3 IVisibleThreat.GetThreatVelocity()
        {
            if (caveDwellerAI != null && caveDwellerAI.IsOwner)
                return caveDwellerAI.agent.velocity;

            return Vector3.zero;
        }

        float IVisibleThreat.GetVisibility()
        {
            if (caveDwellerAI == null || caveDwellerAI.isEnemyDead)
                return 0f;

            if (caveDwellerAI.currentBehaviourStateIndex == 0) // baby
                return 0.5f;

            if (caveDwellerAI.creatureAnimator.GetBool("Leaping"))
                return 1f;

            return 0.75f;
        }

        int IVisibleThreat.SendSpecialBehaviour(int id)
        {
            return 0;
        }

        GrabbableObject IVisibleThreat.GetHeldObject()
        {
            return null;
        }

        bool IVisibleThreat.IsThreatDead()
        {
            return caveDwellerAI != null && caveDwellerAI.isEnemyDead;
        }
    }
}
