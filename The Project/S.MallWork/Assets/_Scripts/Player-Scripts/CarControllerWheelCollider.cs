using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Player_Scripts
{
    [Serializable]
    public class _WheelWheelCollider
    {
        public bool          power;
        public bool          steer;
        public Transform     wheelVisual;
        public WheelCollider wheelCollider;
    }

    [RequireComponent(typeof(Rigidbody))]
    public class CarControllerWheelCollider : Vehicle
    {
        [SerializeField]
        public List<_WheelWheelCollider> wheels;

        private Rigidbody _rigidbody;


        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void FixedUpdate()
        {
            if (!vehicleIsActive) return;
            xAxis = Input.GetAxis("Horizontal");
            yAxis = Input.GetAxis("Vertical");

            float absYAxis = yAxis < 0 ? -yAxis : yAxis;

            velocity = transform.InverseTransformDirection(_rigidbody.velocity).z;
            smoothXAxis = Mathf.SmoothDamp(smoothXAxis, xAxis, ref xAxisVelocity, 0.12f);

            float motorTorque = torqueSpeedCurve.Evaluate(Mathf.Abs(velocity * 0.02f)) * maxMotorTorque;

            foreach (var w in wheels)
            {
                w.wheelCollider.brakeTorque = 0f;
                w.wheelCollider.motorTorque = 0f;

                if (Input.GetKey(KeyCode.Space)) w.wheelCollider.brakeTorque = maxBrakeTorque;

                if (velocity < -0.4f && yAxis > 0.1f || velocity > 0.4f && yAxis < -0.1f)
                    w.wheelCollider.brakeTorque = maxBrakeTorque * Mathf.Abs(yAxis);

                if (w.wheelCollider.brakeTorque < 0.01f)
                    if (velocity >= -0.5f && yAxis > 0.1f || velocity <= 0.5f && yAxis < -0.1f)
                        w.wheelCollider.motorTorque = motorTorque * yAxis;


                if (w.steer)
                    w.wheelCollider.steerAngle =
                        Mathf.Lerp(maxSteeringAngle, minSteeringAngle, Mathf.Abs(velocity) * 0.05f) * xAxis;

                if (w.wheelVisual == null) continue;
                Vector3 position;
                Quaternion rotation;
                w.wheelCollider.GetWorldPose(out position, out rotation);
                w.wheelVisual.SetPositionAndRotation(position, rotation);
            }
        }


        


        public void OnBrakeValueChanged(float a) => maxBrakeTorque = a;
        public void OnMotorValueChanged(float v) => maxMotorTorque = v;
    }
}