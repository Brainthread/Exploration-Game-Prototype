﻿using System.Collections;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerStateFactory
    {
        PlayerStateMachine m_context;
        private PlayerGroundState m_groundState;
        private PlayerAerialState m_aerialState;
        public PlayerStateFactory(PlayerStateMachine context)
        {
            m_context = context;
            m_groundState = new PlayerGroundState(m_context, this);
            m_aerialState = new PlayerAerialState(m_context, this);

        }
        public PlayerBaseState Grounded()
        {
            return m_groundState;
        }
        public PlayerBaseState Aerial()
        {
            return m_aerialState;
        }
    }
}