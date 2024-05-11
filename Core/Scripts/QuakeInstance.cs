using System;
using UnityEngine;

namespace Quaker2D {
    public class QuakeInstance {
        /// <summary>
        /// True if the quake is finished
        /// </summary>
        public bool Finished { get; set; } = true;

        /// <summary>
        /// If true, quake is paused
        /// </summary>
        public bool Paused { get; set; } = false;

        /// <summary>
        /// If true, quake will
        /// </summary>
        public bool Sustained { get; set; }

        /// <summary>
        /// The type of Quake this preset is.
        /// </summary>
        public QuakeType QuakeType { get; set; }

        /// <summary>
        /// How intense the quakes are. Has to be positive.
        /// </summary>
        public float Magnitude { get; set; }

        /// <summary>
        /// How much time it takes the camera to move between shakes (0 = instant).
        /// </summary>
        public float TravelTime { get; set; }

        /// <summary>
        /// If the quake has a fade-in.
        /// </summary>
        public bool FadeIn { get; set; }

        /// <summary>
        /// Animation curve of the quake's fade-in.
        /// </summary>
        public AnimationCurve FadeInCurve { get; set; }

        /// <summary>
        /// How long the quake's fade-in is.
        /// </summary>
        public float FadeInLength { get; private set; }

        /// <summary>
        /// How much time the quake's fade-in has left is.
        /// </summary>
        public float FadeInLengthLeft { get; set; }

        /// <summary>
        /// How long the quake's sustain is.
        /// </summary>
        public float SustainLength { get; private set; }

        /// <summary>
        /// How much time the quake's sustain has left is.
        /// </summary>
        public float SustainLengthLeft { get; set; }

        /// <summary>
        /// If the quake has a fade-out.
        /// </summary>
        public bool FadeOut { get; set; }

        /// <summary>
        /// Animation curve of the quake's fade-out.
        /// </summary>
        public AnimationCurve FadeOutCurve { get; set; }

        /// <summary>
        /// How long the quake's fade-Out is.
        /// </summary>
        public float FadeOutLength { get; private set; }

        /// <summary>
        /// How much time the quake's fade-Out has left is.
        /// </summary>
        public float FadeOutLengthLeft { get; set; }

        
        public float MoveRate { get; set; }
        public float Lerp { get; set; } 
        
        //Bump parameters
        
        /// <summary>
        /// The destination of the current shake.
        /// </summary>
        public Vector2 Destination { get; set; }
        /// <summary>
        /// The start position of the current shake.
        /// </summary>
        public Vector2 StartPos { get; set; }
        /// <summary>
        /// The distance between the start position and the destination.
        /// </summary>
        public float Distance { get; set; }
        /// <summary>
        /// the animation curve of the bump.
        /// </summary>
        public AnimationCurve BumpAnimationCurve { get; set; }


        /// <summary>
        /// Set quake instance's parameter to a quake preset's parameters.
        /// </summary>
        /// <param name="quakePreset">The Quaker preset that the Quaker instance will copy its variables from.</param>
        public void SetParameters(QuakePreset quakePreset) {
            Finished = false;
        
            QuakeType = quakePreset.QuakeType;
            Magnitude = quakePreset.Magnitude;
            TravelTime = quakePreset.TravelTime;
            
            MoveRate = TravelTime == 0 ? 0 : 1 / (TravelTime / 100);
            Lerp = -1;
        
            switch (quakePreset.QuakeType) {
                case QuakeType.OneShot:
                    Destination = new Vector2();
                    
                    FadeIn = quakePreset.FadeIn;
                    FadeInCurve = quakePreset.FadeInCurve;
                    FadeInLength = quakePreset.FadeInLength;
                    FadeInLengthLeft = quakePreset.FadeInLength;

                    SustainLength = quakePreset.SustainLength;
                    SustainLengthLeft = quakePreset.SustainLength;
        
                    FadeOut = quakePreset.FadeOut;
                    FadeOutCurve = quakePreset.FadeOutCurve;
                    FadeOutLength = quakePreset.FadeOutLength;
                    FadeOutLengthLeft = quakePreset.FadeOutLength;
                    break;
                case QuakeType.Sustained:
                    Sustained = true;
                    Destination = new Vector2();

                    FadeIn = quakePreset.FadeIn;
                    FadeInCurve = quakePreset.FadeInCurve;
                    FadeInLength = quakePreset.FadeInLength;
                    FadeInLengthLeft = quakePreset.FadeInLength;

                    FadeOut = quakePreset.FadeOut;
                    FadeOutCurve = quakePreset.FadeOutCurve;
                    FadeOutLength = quakePreset.FadeOutLength;
                    FadeOutLengthLeft = quakePreset.FadeOutLength;
                    break;
                case QuakeType.Bump:
                    BumpAnimationCurve = quakePreset.BumpAnimationCurve;

                    switch (quakePreset.BumpType) {
                        case BumpType.Degrees:
                            Destination = Quaternion.Euler(0, 0, quakePreset.BumpDegrees) * Vector2.right;
                            break;
                        case BumpType.Target:
                            Destination = quakePreset.BumpTarget.transform.position - Quaker.CamTransform.position;
                            break;
                        case BumpType.Vector:
                            Destination = quakePreset.BumpDirection;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    Destination = Destination.normalized * Magnitude;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}