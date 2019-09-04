﻿using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// This class provides an example control script for a space fighter.
    /// </summary>
    public class PlayerCapitalShipFlightControls : VehicleInput
    {
      
        [Header("Capital Ship Control")]

        [SerializeField]
        protected CustomInput yawInput = new CustomInput("Capital Ships", "Rotate Horizontally", "Roll");

        [Header("Throttle")]

        [SerializeField]
        private Vector3 throttleSensitivity = new Vector3(1f, 1f, 0.5f);

        [SerializeField]
        protected CustomInput throttleUpInput = new CustomInput("Capital Ships", "Throttle Up", KeyCode.Z);

        [SerializeField]
        protected CustomInput throttleDownInput = new CustomInput("Capital Ships", "Throttle Down", KeyCode.X);

        [SerializeField]
        protected CustomInput strafeVerticalInput = new CustomInput("Capital Ships", "Strafe Vertical", "Strafe Vertical");

        [SerializeField]
        protected CustomInput strafeHorizontalInput = new CustomInput("Capital Ships", "Strafe Horizontal", "Strafe Horizontal");
  
        [SerializeField]
        protected CustomInput boostInput = new CustomInput("Capital Ships", "Boost", KeyCode.Tab);

        [Header("Roll Correction")]

        [SerializeField]
        protected ShipPIDController shipPIDController;

        protected VehicleEngines3D engines;



        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle)) return false;
            
            engines = vehicle.GetComponent<VehicleEngines3D>();

            return (engines != null);
        }
       

        // Set the control values for the vehicle
        void SetControlValues()
        {
            
            // Values to be passed to the ship
            float pitch = 0;
            float yaw = 0;
            float roll = 0;
            
            Vector3 flattenedForward = new Vector3(engines.transform.forward.x, 0f, engines.transform.forward.z).normalized;
            Maneuvring.TurnToward(engines.transform, engines.transform.position + flattenedForward, new Vector3(0f, 360f, 0f), shipPIDController.steeringPIDController);

            pitch = shipPIDController.steeringPIDController.GetControlValue(PIDController3D.Axis.X);
            roll = shipPIDController.steeringPIDController.GetControlValue(PIDController3D.Axis.Z);

            yaw = -yawInput.FloatValue();
            

            // ************************** Throttle ******************************

            Vector3 nextTranslationInputs = engines.TranslationThrottleValues;

            if (throttleUpInput.Pressed())
            {
                nextTranslationInputs.z += throttleSensitivity.z * Time.deltaTime;
            }
            else if (throttleDownInput.Pressed())
            {
                nextTranslationInputs.z -= throttleSensitivity.z * Time.deltaTime;
            }

            // Left / right movement
            nextTranslationInputs.x = strafeHorizontalInput.FloatValue();

            // Up / down movement
            nextTranslationInputs.y = strafeVerticalInput.FloatValue();
            engines.SetTranslationThrottleValues(nextTranslationInputs);


            if (boostInput.Down())
            {
                engines.SetBoostThrottleValues(new Vector3(0f, 0f, 1f));
            }
            else if (boostInput.Up())
            {
                engines.SetBoostThrottleValues(Vector3.zero);
            }

            engines.SetRotationThrottleValues(new Vector3(pitch, yaw, roll));
            
        }

        
        // Called every frame
        protected override void InputUpdate ()
        {
            SetControlValues();
        }
    }
}