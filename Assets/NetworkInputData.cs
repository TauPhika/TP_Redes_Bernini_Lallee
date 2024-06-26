using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public float movementX;
    public float movementY;
    public Vector3 position;
    public NetworkBool waiting;
    public Quaternion rotation;
    public NetworkBool isFiring;
    public NetworkBool isFirePressed;
    public NetworkBool isAirborne;
    public NetworkBool isJumpPressed;
    public NetworkBool isJetpackPressed;
    public NetworkBool isDashPressed;
    public NetworkBool isRechargingJetpack;
    public Vector3 dashDir;
}
