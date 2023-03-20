using System;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Vehicles.Car
{
    public class CarSelfRighting : MonoBehaviour
    {
        // Automatically put the car the right way up, if it has come to rest upside-down.
        [SerializeField] private float m_WaitTime = 3f;           // time to wait before self righting
        [SerializeField] private float m_WaitOffTrack = 10f;           // time to wait before self righting
        [SerializeField] private float m_WaitTryingToMoveTime = 1f;           // time car should be trying to move
        [SerializeField] private float m_VelocityThreshold = 1f;  // the velocity below which the car is considered stationary for self-righting
        private bool isActive = false;

        private float m_LastOkTime; // the last time that the car was in an OK state
        private float m_LastOkProgress; // the last time that the car was in an OK state
        private float m_LastProgressing;

        private Rigidbody m_Rigidbody;
        private CarController m_CarController;
        private WaypointProgressTracker m_waypointProgressTracker;

        private bool tryingToMove = false;
        private bool shouldRight = false;


        private void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_CarController = GetComponent<CarController>();
            m_waypointProgressTracker = GetComponent<WaypointProgressTracker>();
        }


        private void Update()
        {
            if (isActive)
            {
                float progress_change = m_waypointProgressTracker.GetProgress() - m_LastOkProgress;
                tryingToMove = m_CarController.IsTryingToDrive();
                // is the car is the right way up
                if (!shouldRight && (transform.up.y > 0f || m_Rigidbody.velocity.magnitude > m_VelocityThreshold) && (!tryingToMove || m_Rigidbody.velocity.magnitude > m_VelocityThreshold))
                {
                    m_LastOkProgress = m_waypointProgressTracker.GetProgress();
                    m_LastOkTime = Time.time;
                }
                if (progress_change != 0 || m_Rigidbody.velocity.magnitude < m_VelocityThreshold)
                {
                    m_LastProgressing = Time.time;
                }
                if (Time.time > m_LastOkTime + m_WaitTryingToMoveTime)
                {
                    shouldRight = true;
                    Debug.Log("1");
                }
                if (Time.time > m_LastOkTime + m_WaitTryingToMoveTime && m_Rigidbody.velocity.magnitude > m_VelocityThreshold)
                {
                    shouldRight = false;
                    Debug.Log("1 - negate");
                }
                if (Time.time > m_LastProgressing + m_WaitOffTrack)
                {
                    shouldRight = true;
                }

                if (Time.time > m_LastOkTime + m_WaitTime)
                {
                    shouldRight = false;
                    Debug.Log("3");
                    RightCar();
                }
            }
            else 
            {
                m_LastOkProgress = m_waypointProgressTracker.GetProgress();
                m_LastOkTime = Time.time;
                m_LastProgressing = Time.time;
            }
        }

        public void SetActive(bool active) 
        {
            isActive = active;
        }

        // put the car back the right way up:
        private void RightCar()
        {
            Debug.Log("BOUNCE");
            // set the correct orientation for the car, and lift it off the ground a little
            m_CarController.resetIt();
            transform.position = m_waypointProgressTracker.progressPoint.position;
            transform.rotation = Quaternion.LookRotation(m_waypointProgressTracker.progressPoint.direction);
            transform.position += (Vector3.up/2);
            //transform.rotation = Quaternion.LookRotation(transform.forward);
        }
    }
}
