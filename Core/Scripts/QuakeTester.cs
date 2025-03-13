using NaughtyAttributes;
using UnityEngine;

namespace Quaker2D {
    [RequireComponent(typeof(Quaker))]
    public class QuakeTester : MonoBehaviour {
        [SerializeField]  [Expandable] private QuakePreset quakePreset;
        //[HorizontalLine]
        [BoxGroup][SerializeField] [EnableIf("IfTarget")] private Transform target;

        [Button("Test Quaker preset", enabledMode: EButtonEnableMode.Playmode)]
        private void QuakeTest() {
            if (quakePreset == null) Debug.LogError("Please select a Quake preset.");
            Quaker.PlayQuake(quakePreset);
        }

        [ShowIf("IfSustained")]
        [Button("Stop Quaker preset", enabledMode: EButtonEnableMode.Playmode)]
        private void StopQuakeTest() {
            if (quakePreset == null) Debug.LogError("Please select a Quake preset.");
            Quaker.StopSustainQuake();
        }

        [Button("Toggle Quake Pause", enabledMode: EButtonEnableMode.Playmode)]
        private void TogglePause() {
            Quaker.TogglePause();
            Debug.Log(Quaker.IsPaused);
        }
        [Button("Cancel running quakes", enabledMode: EButtonEnableMode.Playmode)]
        private void CancelQuakes() {
            Quaker.CancelRunningQuakes();
        }

        private bool IfSustained() {
            if (quakePreset == null)
                return false;
            return quakePreset.QuakeType == QuakeType.Sustained;
        }
        private bool IfTarget() {
            if (quakePreset == null)
                return false;
            return quakePreset.QuakeType == QuakeType.Bump && quakePreset.BumpType == BumpType.Target;
        }
    }
}
