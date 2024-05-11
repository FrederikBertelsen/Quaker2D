using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Quaker2D {
    public class Quaker : MonoBehaviour {
        /// <summary>
        /// The transform of the main camera.
        /// </summary>
        public static Transform CamTransform;

        #region Pause Functionality

        /// <summary>
        /// returns true if Quaker is paused.
        /// </summary>
        public static bool IsPaused { get; private set; }

        /// <summary>
        /// Will pause Quaker.
        /// </summary>
        public static void Pause() {
            IsPaused = true;
        }

        /// <summary>
        /// Will unpause Quaker.
        /// </summary>
        public static void UnPause() {
            IsPaused = false;
        }

        /// <summary>
        /// Will toggle between pausing and unpausing Quaker.
        /// </summary>
        public static void TogglePause() {
            IsPaused = !IsPaused;
        }

        #endregion

        private static readonly QuakeInstance[] CntQuakes = new QuakeInstance[2];
        public static bool Overwritable = true;

        /// <summary>
        /// Will play the Quaker preset that is specified.
        /// </summary>
        /// <param name="quakePreset">The Quaker preset.</param>
        public static void PlayQuake(QuakePreset quakePreset) {
            if (quakePreset.QuakeType == QuakeType.Bump && quakePreset.BumpType == BumpType.Target &&
                quakePreset.BumpTarget is null) {
                Debug.LogError(
                    "When using Target as Bump direction, call PlayQuake() with the target's transform as the seconds argument.");
            }

            switch (quakePreset.QuakeType) {
                case QuakeType.OneShot:
                case QuakeType.Bump:
                    //Debug.Log("Playing Quake Preset: " + quakePreset.name);
                    if (CntQuakes[1].Finished || Overwritable) CntQuakes[1].SetParameters(quakePreset);
                    break;
                case QuakeType.Sustained:
                    //Debug.Log("Playing Quake Preset: " + quakePreset.name);
                    if (CntQuakes[0].Finished || Overwritable) CntQuakes[0].SetParameters(quakePreset);
                    break;
            }
        }

        /// <summary>
        /// Will play the Quaker preset that is specified.
        /// </summary>
        /// <param name="quakePreset">The Quaker preset.</param>
        /// <param name="bumpTarget">The target that the camera will bump towards.</param>
        public static void PlayQuake(QuakePreset quakePreset, Transform bumpTarget) {
            quakePreset.BumpTarget = bumpTarget;
            PlayQuake(quakePreset);
        }

        private void Awake() {
            FindAndResetCamera();
            for (int i = 0; i < CntQuakes.Length; i++) {
                CntQuakes[i] = new QuakeInstance();
            }
        }

        private void Update() {
            if (IsPaused) return;
            foreach (var quakeInstance in CntQuakes) {
                if (quakeInstance.Finished) continue;

                switch (quakeInstance.QuakeType) {
                    case QuakeType.OneShot:
                        UpdateOneShotOrSustained(quakeInstance, Time.deltaTime);
                        break;
                    case QuakeType.Sustained:
                        UpdateOneShotOrSustained(quakeInstance, Time.deltaTime);
                        break;
                    case QuakeType.Bump:
                        UpdateBump(quakeInstance, Time.deltaTime);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void FindAndResetCamera() {
            foreach (Transform child in transform)
                if (child.GetComponent<Camera>()) {
                    CamTransform = child;
                    CamTransform.localPosition = new Vector3(0, 0, 0);
                    break;
                }

            if (CamTransform is null) throw new Exception("Camera child not found. Please add a camera as a child.");
        }

        private static void UpdateOneShotOrSustained(QuakeInstance quakeI, float deltaTime) {
            //Fade-in
            if (quakeI.FadeInLengthLeft > 0 && quakeI.FadeIn) {
                float p = quakeI.FadeInLengthLeft / quakeI.FadeInLength;

                quakeI.FadeInLengthLeft -= deltaTime;

                if (quakeI.Lerp <= -1) {
                    quakeI.StartPos = quakeI.Destination;
                    quakeI.Destination = Random.insideUnitCircle *
                                         (quakeI.Magnitude * (1 - quakeI.FadeInCurve.Evaluate(p)));
                    quakeI.Distance = Vector3.Distance(quakeI.StartPos, quakeI.Destination);
                    quakeI.Lerp = 0;
                }
                else if (quakeI.Lerp < 1) {
                    if (quakeI.MoveRate <= 0)
                        quakeI.Lerp = 1;
                    else
                        quakeI.Lerp += (quakeI.MoveRate / quakeI.Distance) * deltaTime;
                }
                else {
                    quakeI.Lerp = -1;
                    return;
                }

                CamTransform.localPosition = Vector3.Lerp(quakeI.StartPos, quakeI.Destination,
                    quakeI.Lerp);
            }
            //Sustain
            else if (quakeI.SustainLengthLeft > 0 || quakeI.Sustained) {
                float p = quakeI.SustainLengthLeft / quakeI.SustainLength;

                quakeI.SustainLengthLeft -= deltaTime;

                if (quakeI.Lerp <= -1) {
                    quakeI.StartPos = quakeI.Destination;
                    quakeI.Destination = Random.insideUnitCircle * quakeI.Magnitude;
                    quakeI.Distance = Vector3.Distance(quakeI.StartPos, quakeI.Destination);
                    quakeI.Lerp = 0;
                }
                else if (quakeI.Lerp < 1) {
                    if (quakeI.MoveRate <= 0)
                        quakeI.Lerp = 1;
                    else
                        quakeI.Lerp += (quakeI.MoveRate / quakeI.Distance) * deltaTime;
                }
                else {
                    quakeI.Lerp = -1;
                    return;
                }

                CamTransform.localPosition = Vector3.Lerp(quakeI.StartPos, quakeI.Destination,
                    quakeI.Lerp);
            }
            //Fade-out
            else if (quakeI.FadeOutLengthLeft > 0 && quakeI.FadeOut) {
                float p = quakeI.FadeOutLengthLeft / quakeI.FadeOutLength;

                quakeI.FadeOutLengthLeft -= deltaTime;

                if (quakeI.Lerp <= -1) {
                    quakeI.StartPos = quakeI.Destination;
                    quakeI.Destination = Random.insideUnitCircle *
                                         (quakeI.Magnitude * quakeI.FadeOutCurve.Evaluate(p));
                    quakeI.Distance = Vector3.Distance(quakeI.StartPos, quakeI.Destination);
                    quakeI.Lerp = 0;
                }
                else if (quakeI.Lerp < 1) {
                    if (quakeI.MoveRate <= 0)
                        quakeI.Lerp = 1;
                    else
                        quakeI.Lerp += (quakeI.MoveRate / quakeI.Distance) * deltaTime;
                }
                else {
                    quakeI.Lerp = -1;
                    return;
                }

                CamTransform.localPosition = Vector3.Lerp(quakeI.StartPos, quakeI.Destination,
                    quakeI.Lerp);
            }
            else {
                quakeI.Finished = true;

                if (CntQuakes.All(quakeInstance => quakeInstance.Finished == false))
                    CamTransform.localPosition = new Vector2(0, 0);
            }
        }

        /// <summary>
        /// This will stop any running Sustain Quakes.
        /// </summary>
        public static void StopSustainQuake() {
            if (CntQuakes[0].QuakeType == QuakeType.Sustained) CntQuakes[0].Sustained = false;
        }

        /// <summary>
        /// This will cancel any running quakes.
        /// </summary>
        public static void CancelRunningQuakes() {
            foreach (var quakeInstance in CntQuakes) {
                quakeInstance.Finished = true;
            }
        }

        private static void UpdateBump(QuakeInstance quakeInstance, float deltaTime) {
            if (quakeInstance.Lerp < 2) {
                CamTransform.localPosition = Vector3.Lerp(new Vector3(0, 0, 0), quakeInstance.Destination,
                    quakeInstance.BumpAnimationCurve.Evaluate(quakeInstance.Lerp <= 1
                        ? quakeInstance.Lerp
                        : 1 - quakeInstance.Lerp + 1));

                quakeInstance.Lerp += quakeInstance.MoveRate * deltaTime;
                if (quakeInstance.MoveRate <= 0) quakeInstance.Lerp++;
            }
            else {
                CamTransform.localPosition = new Vector2(0, 0);

                quakeInstance.Finished = true;
            }
        }
    }
}