using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackFollow : MonoBehaviour
{
   [SerializeField] private Transform stackpos;

   private void Update()
   {
      transform.position = stackpos.position;
      transform.rotation = stackpos.rotation;
      
   }
}
