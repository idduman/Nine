using System;
using System.Collections;
using System.Collections.Generic;
using Garawell.Managers;
using Garawell.Managers.Events;
using Garawell.Managers.Game;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CharacterMovement : ECMonoBehaviour {
    
    private float rotAngle = 0;
    float turnnSmoothVelocity;
    
    CharacterController controller;
    private float inputHorizontal;
    private float inputVertical;
   
    [Header("Joysticks")]
    public DynamicJoystick dynamicJoystick;
    
    [Header("PLAYER DATA")]
    [SerializeField] private PlayerScriptable _playerScriptable;
    
    
    [SerializeField] private Animator anim;
    private bool canPlay;



    public override void OnSceneLoadComplete()
    {
        
      //  dynamicJoystick = MainManager.Instance.MenuManager.GamePanel.Joystick;
        MainManager.Instance.EventManager.Register( EventTypes.LevelStart,StartPlayer,true);


    }

    void StartPlayer (EventArgs args)
    {
        controller = GetComponent<CharacterController>();
        canPlay = true;

    }
    void GetInTurret(EventArgs args)
    {
        this.enabled = false;
    }
    void GetOutTurret(EventArgs args)
    {
        this.enabled = true;
    }
  

    void Update () {
        
        if (canPlay)
        {
            inputHorizontal = Input.GetAxisRaw("Vertical");

            Vector3 from = new Vector3(0f,0f,1f);
            Vector3 to = new Vector3(dynamicJoystick.Horizontal,0f, dynamicJoystick.Vertical);

       

            if (dynamicJoystick.Horizontal != 0 && dynamicJoystick.Vertical != 0)
            {
                anim.SetBool("Run",true);
                anim.gameObject.transform.localEulerAngles= new Vector3(0,0,0);
                float angle = Vector3.SignedAngle(from, to, Vector3.up);
                rotAngle = angle;

          
                if (dynamicJoystick.gameObject.activeInHierarchy)
                {
                    controller.Move(Vector3.forward * dynamicJoystick.Vertical * Time.deltaTime * _playerScriptable.Speed / 5 + Vector3.right * dynamicJoystick.Horizontal * Time.deltaTime * _playerScriptable.Speed / 5);
                    transform.position = new Vector3(transform.position.x, 0.85f, transform.position.z);
                    
                    Vector3 horizontalVelocity = controller.velocity;
                    horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);

                    // The speed on the x-z plane ignoring any speed
                    float horizontalSpeed = horizontalVelocity.magnitude;
        
                    anim.SetFloat("Speed", horizontalVelocity.magnitude/4);
                }
               
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnnSmoothVelocity, _playerScriptable.turnSmoothTime);

            }
            else
            {
                anim.SetBool("Run",false);
                anim.gameObject.transform.localEulerAngles= new Vector3(0,0,0);
                
            }


        }
        

    }

}
