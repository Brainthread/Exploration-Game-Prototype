using System.Collections;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerStateFactory
    {
        PlayerMovementStateMachine m_context;
        private PlayerGroundState m_groundState;
        private PlayerAerialState m_aerialState;
        private PlayerJumpState m_jumpState;
        public PlayerStateFactory(PlayerMovementStateMachine context)
        {
            m_context = context;
            m_groundState = new PlayerGroundState(m_context, this);
            m_aerialState = new PlayerAerialState(m_context, this);
            m_jumpState = new PlayerJumpState(m_context, this);
        }
        public PlayerBaseState Grounded()
        {
            return m_groundState;
        }
        public PlayerBaseState Aerial()
        {
            return m_aerialState;
        }
        public PlayerBaseState Jump()
        {
            return m_jumpState;
        }
    }
}