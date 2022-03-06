using System.Collections;
using UnityEngine;

namespace Dp.Rig
{
    public static class Calibration
    {
        private static bool _started = false;
        public static bool Continue = false; 
        public static IEnumerator StartCalibration(VRRig rig, Terminal terminal, GameObject[] defaultControllers) 
        {
            if (!_started)
            {
                _started = true;
                EnableControllers(defaultControllers);

                terminal?.NewText("Spušta sa kalibrácia.. <br>Pre správnu kalibráciu je potrebné stáť <b>vzpriamene</b> s pózov v tvare <b>T</b> a <b>pozerať pred seba.</b>. " +
                    "Pózu je nutné držať až do skončenia kalibrácie." +
                    "<br><br>Pre pokračovanie stlač potvrdit alebo daj like");

                if (terminal != null)
                {
                    Continue = false;
                    while (!Continue) yield return null;
                }
                ResetRig(rig);

                terminal?.NewText("Kalibrácia začne za 5 sekúnd.");
                yield return new WaitForSeconds(5f);
                Stage1(rig, terminal);
                yield return new WaitForSeconds(1f);
                Stage2(rig, terminal);
                yield return new WaitForSeconds(1f);
                Stage3(rig, terminal);
                yield return new WaitForSeconds(1f);

                terminal?.NewText("Kalibrácia skončená.");
                yield return new WaitForSeconds(2f);

                terminal?.Clear();
                terminal?.gameObject.SetActive(false);

                EnableControllers(defaultControllers, false);
            }
        }

        private static void EnableControllers(GameObject[] controllers, bool enable = true) 
        {
            foreach (var item in controllers)
            {
                item.SetActive(enable);
            }
        }

        /// <summary>
        /// Stage 1 > Align user with model 
        /// </summary>
        private static void Stage1(VRRig rig, Terminal terminal) 
        {
            terminal?.NewText("Prebieha zarovnanie pozicie používateľa a avatara...");
            rig.AlignAvatar();
            terminal?.AddText(" <color=#005500>Hotovo</color>");
        }

        /// <summary>
        /// Stage 2 > enable animator and scale camera to avatar height 
        /// </summary>
        private static void Stage2(VRRig rig, Terminal terminal) 
        {
            terminal?.AddText("Prebieha prisposobenie na výšku avatara...", true);
            rig.UpdateScale();
            terminal?.AddText(" <color=#005500>Hotovo</color>");
        }

        /// <summary>
        /// Stage 2 > Initialize avatar model
        /// </summary>
        private static void Stage3(VRRig rig, Terminal terminal) 
        {
            terminal?.NewText("Inicializujem avatar...");

            rig.Model.Initialize();

            rig.Calibrated = true;

            terminal?.AddText(" <color=#005500>Hotovo</color>");
            _started = false;
        }

        private static void ResetRig(VRRig rig) 
        {
            var transforms = rig.Model.CameraRig.GetComponentsInChildren<Transform>();
            foreach (var item in transforms)
            {
                item.localPosition = Vector3.zero;
                item.localScale = Vector3.one;
            }
        }

    }
}
