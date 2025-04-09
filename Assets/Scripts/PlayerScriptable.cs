using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerScriptable : ScriptableObject
{
   [Header("Movement Stats")]
   public float Speed;
   public float turnSmoothTime = 0.2f;
}
