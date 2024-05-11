using NaughtyAttributes;
using UnityEngine;

namespace Quaker2D {
    /// <summary>
    /// ScriptableObject of a Quaker preset.
    /// </summary>
    [CreateAssetMenu(fileName = "New Quake Preset", menuName = "Quaker/Quake Preset")]
    public class QuakePreset : ScriptableObject {
        [InfoBox(
            "There are 3 types of quakes:\n" +
            "1. One-Shot: Starts a single camera quake, that fades over time.\n" +
            "2. Sustained: Continuously quakes the camera, until you execute quaker.EndConstQuake().\n" +
            "3. Bump: Will bump the camera in a direction, or towards a target.")]

#pragma warning disable 0414
        [TextArea]
        [SerializeField]
        [Tooltip("This doesn't do anything. It's just for comments and notes on this quake preset.")]
        private string quakeNotes = "Write notes/description here.";
#pragma warning restore 0414


        [Header("Quake Parameters")] [Tooltip("The type of Quake this preset is.")] [SerializeField]
        private QuakeType quakeType;

        [Tooltip("How intense the quakes are.")] [SerializeField] [MinValue(0.001f)]
        private float magnitude = 0.3f;

        [Tooltip("How much time it takes the camera to move between shakes (0 = instant).")] [SerializeField] [MinValue(0)]
        private float travelTime = 1;


        [Tooltip("If the quake has a fade-in.")] [Header("Fade In Parameters")] [SerializeField] [ShowIf("IfQuake")]
        private bool fadeIn;

        [Tooltip("The fade-in's animation curve.")]
        [CurveRange(0, 0, 1, 1, EColor.Red)]
        [SerializeField]
        [ShowIf(EConditionOperator.And, "IfQuake", "fadeIn")]
        private AnimationCurve fadeInCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] [ShowIf(EConditionOperator.And, "IfQuake", "fadeIn")] [MinValue(0.001f)]
        private float fadeInLength;

        [SerializeField] [Header("Sustain")] [ShowIf("IfOneShot")][MinValue(0)]
        private float sustainLength;

        [Tooltip("If the quake has a fade-out.")] [Header("Fade Out Parameters")] [SerializeField] [ShowIf("IfQuake")]
        private bool fadeOut = true;

        [Tooltip("The fade-out's animation curve.")]
        [CurveRange(0, 0, 1, 1, EColor.Red)]
        [SerializeField]
        [ShowIf(EConditionOperator.And, "IfQuake", "fadeOut")]
        private AnimationCurve fadeOutCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] [ShowIf(EConditionOperator.And, "IfQuake", "fadeOut")] [MinValue(0.001f)]
        private float fadeOutLength = 1f;


        [Tooltip("The type of bump.")] [SerializeField] [BoxGroup("Bump Parameters")] [ShowIf("IfBump")]
        private BumpType bumpType;

        [Tooltip("Bumps in the direction of a vector (normalized).")]
        [SerializeField]
        [BoxGroup("Bump Parameters")]
        [ShowIf(EConditionOperator.And, "IfBump", "IfBumpVector")]
        private Vector2 bumpDirection;

        [Tooltip("Bumps in the direction in degrees (0 = right).")]
        [SerializeField]
        [BoxGroup("Bump Parameters")]
        [ShowIf(EConditionOperator.And, "IfBump", "IfBumpDegrees")]
        private float bumpDegrees;

        [Tooltip("Bumps towards a transform.")]
        [SerializeField]
        [BoxGroup("Bump Parameters")]
        [ShowIf(EConditionOperator.And, "IfBump", "IfBumpTarget")]
        [NaughtyAttributes.ReadOnly]private Transform bumpTarget;

        [Tooltip("The bump's animation curve.")]
        [BoxGroup("Bump Parameters")]
        [CurveRange(0, 0, 1, 1, EColor.Red)]
        [SerializeField]
        [ShowIf("IfBump")]
        private AnimationCurve bumpAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        #region Properties

        /// <summary>
        /// The type of Quake this preset is.
        /// </summary>
        public QuakeType QuakeType {
            get => quakeType;
            set => quakeType = value;
        }

        /// <summary>
        /// How intense the quakes are. Has to be positive.
        /// </summary>
        public float Magnitude {
            get => magnitude;
            set => magnitude = value;
        }

        /// <summary>
        /// How much time it takes the camera to move between shakes (0 = instant).
        /// </summary>
        public float TravelTime {
            get => travelTime;
            set => travelTime = value;
        }

        /// <summary>
        /// If the quake has a fade-in.
        /// </summary>
        public bool FadeIn {
            get => fadeIn;
            set => fadeIn = value;
        }

        /// <summary>
        /// Animation curve of the quake's fade-in.
        /// </summary>
        public AnimationCurve FadeInCurve {
            get => fadeInCurve;
            set => fadeInCurve = value;
        }

        /// <summary>
        /// How long the quake's fade-in is.
        /// </summary>
        public float FadeInLength {
            get => fadeInLength;
            set => fadeInLength = value;
        }

        /// <summary>
        /// How long the quake's sustain is.
        /// </summary>
        public float SustainLength {
            get => sustainLength;
            set => sustainLength = value;
        }

        /// <summary>
        /// If the quake has a fade-out.
        /// </summary>
        public bool FadeOut {
            get => fadeOut;
            set => fadeOut = value;
        }

        /// <summary>
        /// Animation curve of the quake's fade-out.
        /// </summary>
        public AnimationCurve FadeOutCurve {
            get => fadeOutCurve;
            set => fadeOutCurve = value;
        }

        /// <summary>
        /// How long the quake's fade-Out is.
        /// </summary>
        public float FadeOutLength {
            get => fadeOutLength;
            set => fadeOutLength = value;
        }


        //Bump settings
        /// <summary>
        /// The type of bump.
        /// </summary>
        public BumpType BumpType {
            get => bumpType;
            set => bumpType = value;
        }

        /// <summary>
        /// Bumps in the direction of a vector (normalized).
        /// </summary>
        public Vector2 BumpDirection {
            get => bumpDirection;
            set => bumpDirection = value;
        }

        /// <summary>
        /// Bumps in the direction in degrees (0 = right).
        /// </summary>
        public float BumpDegrees {
            get => bumpDegrees;
            set => bumpDegrees = value;
        }

        /// <summary>
        /// Bumps towards a transform.
        /// </summary>
        public Transform BumpTarget {
            get => bumpTarget;
            set => bumpTarget = value;
        }

        /// <summary>
        /// The bump's animation curve.
        /// </summary>
        public AnimationCurve BumpAnimationCurve {
            get => bumpAnimationCurve;
            set => bumpAnimationCurve = value;
        }

        #endregion

        #region Inspector Checkers

        private bool IfQuake() {
            return quakeType == QuakeType.OneShot || quakeType == QuakeType.Sustained;
        }

        private bool IfOneShot() {
            return quakeType == QuakeType.OneShot;
        }

        private bool IfFade() {
            return fadeIn || fadeOut;
        }

        private bool IfBump() {
            return quakeType == QuakeType.Bump;
        }

        private bool IfBumpTarget() {
            return bumpType == BumpType.Target;
        }

        private bool IfBumpDegrees() {
            return bumpType == BumpType.Degrees;
        }

        private bool IfBumpVector() {
            return bumpType == BumpType.Vector;
        }

        #endregion
    }

    /// <summary>
    /// Defines the behavior of a quake.
    /// </summary>
    public enum QuakeType {
        /// <summary>
        /// Starts a single camera quake, that fades over time.
        /// </summary>
        OneShot,

        /// <summary>
        /// Continuously quakes the camera, until you execute quaker.EndConstQuake().
        /// </summary>
        Sustained,

        /// <summary>
        /// Will bump the camera in a direction.
        /// </summary>
        Bump
    }

    /// <summary>
    /// Defines the direction of a bump.
    /// </summary>
    public enum BumpType {
        /// <summary>
        /// Give bump direction in degrees
        /// </summary>
        Degrees,

        /// <summary>
        /// Give bump direction in a Vector2
        /// </summary>
        Vector,

        /// <summary>
        /// Bump direction is towards a Transform
        /// </summary>
        Target
    }
}