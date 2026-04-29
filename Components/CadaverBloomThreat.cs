using UnityEngine;

namespace ButteRyBalance.Components
{
    public class CadaverBloomThreat : MonoBehaviour, IVisibleThreat
    {
        internal CadaverBloomAI cadaverBloomAI;

        void Update()
        {
            if (cadaverBloomAI == null)
                cadaverBloomAI = GetComponent<CadaverBloomAI>();
        }

        // based on MaskedPlayerEnemy

        ThreatType IVisibleThreat.type
        {
            get
            {
                return ThreatType.RadMech;
            }
        }

        int IVisibleThreat.GetThreatLevel(Vector3 seenByPosition)
        {
            return 3;
        }

        int IVisibleThreat.GetInterestLevel()
        {
            return 0;
        }

        Transform IVisibleThreat.GetThreatLookTransform()
        {
            if (cadaverBloomAI != null)
                return cadaverBloomAI.eye;

            return transform;
        }

        Transform IVisibleThreat.GetThreatTransform()
        {
            return transform;
        }

        Vector3 IVisibleThreat.GetThreatVelocity()
        {
            if (cadaverBloomAI != null && cadaverBloomAI.IsOwner)
                return cadaverBloomAI.agent.velocity;

            return Vector3.zero;
        }

        float IVisibleThreat.GetVisibility()
        {
            if (cadaverBloomAI == null || cadaverBloomAI.isEnemyDead)
                return 0f;

            return 1f;
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
            return cadaverBloomAI != null && cadaverBloomAI.isEnemyDead;
        }
    }
}
