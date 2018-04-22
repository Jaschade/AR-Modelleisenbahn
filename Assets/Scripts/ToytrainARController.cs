﻿//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.HelloAR
{
    using System;
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Controls the HelloAR example.
    /// </summary>
    public class ToytrainARController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject TrackedPlanePrefab;

        /// <summary>
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        public GameObject SearchingForPlaneUI;

        /// <summary>
        /// A list to hold new planes ARCore began tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<TrackedPlane> m_NewPlanes = new List<TrackedPlane>();

        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<TrackedPlane> m_AllPlanes = new List<TrackedPlane>();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;


        /// <summary>
        /// ////////////////////////////
        /// ////////////////////////////
        /// </summary>
        public GameObject trainPrefab;
        GameObject railwayTrack;
        GameObject railwayObject;
        GameObject trainObject;
        GameObject btnDrive;

        Train train;
        float trainSpeed = 0f;

        public Button btnSpawnTrain;
        public Slider slider;

        bool b_ObjectInstantiated = false;
        bool b_ClickedOnce = false;

        String ScaleScript = "Lean.Touch.LeanScale";
        String RotateScript = "Lean.Touch.LeanRotate";
        String TranslateScript = "Lean.Touch.LeanTranslate";
        String SelectableScript = "Lean.Touch.LeanSelectable";

        /// <summary>
        /// The Unity Start() method.
        /// </summary>
        private void Start()
        {
            railwayTrack = GameObject.Find("Track(Clone)");
            btnDrive = GameObject.Find("BtnDriveTrain");

            if (railwayTrack != null)
            {
                railwayTrack.SetActive(false);
            }

            btnSpawnTrain.gameObject.SetActive(false);
            slider.gameObject.SetActive(false);
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {

            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            _QuitOnConnectionErrors();

            // Check that motion tracking is tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
                if (!m_IsQuitting && Session.Status.IsValid())
                {
                    SearchingForPlaneUI.SetActive(true);
                }

                return;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
            Session.GetTrackables<TrackedPlane>(m_NewPlanes, TrackableQueryFilter.New);
            for (int i = 0; i < m_NewPlanes.Count; i++)
            {
                // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
                // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
                // coordinates.
                GameObject planeObject = Instantiate(TrackedPlanePrefab, Vector3.zero, Quaternion.identity,
                    transform);
                planeObject.GetComponent<TrackedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
            }

            // Disable the snackbar UI when no planes are valid.
            Session.GetTrackables<TrackedPlane>(m_AllPlanes);
            //bool showSearchingUI = true;
            for (int i = 0; i < m_AllPlanes.Count; i++)
            {
                if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
                {
                    //showSearchingUI = false;
                    break;
                }
            }

            //SearchingForPlaneUI.SetActive(showSearchingUI);


            if (slider.IsActive() == true)
            {
                train.Drive(slider.value);
            }


            // If the player has not touched the screen, we are done with this update.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }


            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && b_ObjectInstantiated == false)
            {
                ///////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////
                railwayTrack.SetActive(true);
                railwayObject = Instantiate(railwayTrack, hit.Pose.position, hit.Pose.rotation);
                railwayTrack.SetActive(false);

                //// Andy should look at the camera but still be flush with the plane.
                //if ((hit.Flags & TrackableHitFlags.PlaneWithinPolygon) != TrackableHitFlags.None)
                //{
                //    // Get the camera position and match the y-component with the hit position.
                //    Vector3 cameraPositionSameY = FirstPersonCamera.transform.position;
                //    cameraPositionSameY.y = hit.Pose.position.y;

                //    // Have Andy look toward the camera respecting his "up" perspective, which may be from ceiling.
                //    railwayObject.transform.LookAt(cameraPositionSameY, railwayObject.transform.up);
                //}

                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                railwayObject.transform.rotation = Quaternion.Euler(90f, 
                                                    railwayObject.transform.rotation.eulerAngles.y, 
                                                    railwayObject.transform.rotation.eulerAngles.z);

                // Make Andy model a child of the anchor.
                railwayObject.transform.parent = anchor.transform;

                var scaleType = Type.GetType(ScaleScript + ",Assembly-CSharp");
                var rotateType = Type.GetType(RotateScript + ",Assembly-CSharp");
                var translateType = Type.GetType(TranslateScript + ",Assembly-CSharp");
                var selectableType = Type.GetType(SelectableScript + ",Assembly-CSharp");

                //railwayObject.AddComponent(scaleType);
                //railwayObject.AddComponent(rotateType);
                //railwayObject.AddComponent(translateType);
                //railwayObject.AddComponent(selectableType);

                SearchingForPlaneUI.GetComponentInChildren<Text>().text = " Now spawn the train...";

                b_ObjectInstantiated = true;
                btnSpawnTrain.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Quit the application if there was a connection error for the ARCore session.
        /// </summary>
        private void _QuitOnConnectionErrors()
        {
            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }

        // set locomotive on track
        public void BtnSpawnTrain()
        {
            if (!b_ClickedOnce)
            {
                trainObject = Instantiate(trainPrefab, railwayObject.transform.GetChild(0).position + new Vector3(0f, 0.05f, 0f), railwayObject.transform.GetChild(0).rotation);
                trainObject.transform.Rotate(0f, 180f, 0f);
                train = trainObject.GetComponent<Train>();
                trainObject.transform.parent = railwayObject.transform;
                train.Setup(railwayObject.transform.GetChild(0).GetComponent<Rail>().direction);

                slider.gameObject.SetActive(true);
                b_ClickedOnce = true;

                SearchingForPlaneUI.GetComponentInChildren<Text>().text = "You may drive now...";
            }
            else
            {
                Destroy(trainObject);

                trainObject = Instantiate(trainPrefab, railwayObject.transform.GetChild(0).position + new Vector3(0f, 0f, -0.05f), railwayObject.transform.GetChild(0).rotation);
                trainObject.transform.Rotate(0f, 180f, 0f);
                train = trainObject.GetComponent<Train>();
                train.Setup(railwayObject.transform.GetChild(0).GetComponent<Rail>().direction);
            }
        }
    }
}
