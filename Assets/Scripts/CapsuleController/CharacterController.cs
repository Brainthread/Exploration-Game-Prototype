using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterController : MonoBehaviour
{
    internal abstract void Initialize();
    internal abstract void ReadInput();
    internal abstract void SendInput();
}
