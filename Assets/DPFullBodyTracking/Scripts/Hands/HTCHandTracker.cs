using Dp.Rig;
using System.Collections;
using UnityEngine;
using ViveHandTracking;
using System.Linq;

namespace Dp.Hands
{
    /// <summary>
    /// Modified ModelRenderer.cs from Vive Hand Tracking SDK.. we dont need palm position
    /// </summary>
    public class HTCHandTracker : MonoBehaviour
    {
        public Vector3 InitialRotation;
        public bool IsLeft;
        private Transform _hand;
       

        [Header("Thumb twist correction")]
        [SerializeField] private Vector3 _thumbTwistProximal;
        [SerializeField] private Vector3 _thumbTwistIntermediate;
        [SerializeField] private Vector3 _thumbTwistDistal;
        
        private Transform[] _fingersRoot;
        private Quaternion[] _jointRotation;
        private VRRig _vrRig;

        private Quaternion[] _rotations;
        private Transform[] _transforms;

        private void Awake()
        {
            _vrRig = GetComponent<VRRig>();
            HandData handData = IsLeft ? _vrRig.Model.LeftHand : _vrRig.Model.RightHand;
            _hand = IsLeft ? _vrRig.Model.LeftArm.Target : _vrRig.Model.RightArm.Target;
            _fingersRoot = new Transform[]
            {
                handData.Thumb,
                handData.Index,
                handData.Middle,
                handData.Ring,
                handData.Pinky
            };
            InitializeModel();       
        }

        IEnumerator Start()
        {
            GetFingersTransform();

            while (GestureProvider.Status == GestureStatus.NotStarted) yield return null;
            if (!GestureProvider.HaveSkeleton) this.enabled = false;
        }

        void Update()
        {
            if (!HandsManager.Instance.IsHTCHandTrackingEnabled)
                return;

            if (!_vrRig.Calibrated)
                return;
            
            GestureResult result = IsLeft ? GestureProvider.LeftHand : GestureProvider.RightHand;
            if (result == null)
                RestoreHand();
            else
                TrackHand(result);
        }

        private void GetFingersTransform()
        {
            _transforms = _fingersRoot[0].parent.GetComponentsInChildren<Transform>();
            _rotations = _transforms.Select(item => item.localRotation).ToArray();
        }

        private void RestoreHand() 
        {
            for (int i = 1; i < _transforms.Length; i++)
            {
                _transforms[i].localRotation = Quaternion.RotateTowards(_transforms[i].localRotation, _rotations[i], Time.deltaTime * 500f);
            }
        }

        private void TrackHand(GestureResult result)
        {
            _hand.rotation = result.rotation * Quaternion.Euler(InitialRotation);
            
            // 0 - Thumb , 1 - Index, 2 - Middle, 3 - Ring, 4 - Pinky 
            int resultIndex = 1;
            int jointIndex = 0;
            for (int i = 0; i < _fingersRoot.Length; i++, resultIndex += 4, jointIndex += 3)
            {
                _fingersRoot[i].rotation = result.rotations[resultIndex] * _jointRotation[jointIndex];
                _fingersRoot[i].GetChild(0).rotation = result.rotations[resultIndex + 1] * _jointRotation[jointIndex + 1];
                _fingersRoot[i].GetChild(0).GetChild(0).rotation = result.rotations[resultIndex + 2] * _jointRotation[jointIndex + 2];
            }

            _fingersRoot[0].localRotation = _fingersRoot[0].localRotation * Quaternion.Euler(_thumbTwistProximal);
            _fingersRoot[0].GetChild(0).localRotation = (Quaternion.Inverse(Quaternion.Euler(_thumbTwistProximal)) * _fingersRoot[0].GetChild(0).localRotation) * Quaternion.Euler(_thumbTwistIntermediate);
            _fingersRoot[0].GetChild(0).GetChild(0).localRotation = (Quaternion.Inverse(Quaternion.Euler(_thumbTwistIntermediate)) * _fingersRoot[0].GetChild(0).GetChild(0).localRotation) * Quaternion.Euler(_thumbTwistDistal);
        }

        #region Model axis detection

        private void InitializeModel()
        {
            // find local normal vector in node local axis, assuming all finger nodes have same local axis
            var right = FindLocalNormal(_fingersRoot[2]);
            // get initial finger direction and length in local axis
            _jointRotation = new Quaternion[15];

            int jointIndex = 0;
            for (int i = 0; i < _fingersRoot.Length; i++, jointIndex += 3)
            {
                var up = _fingersRoot[i].GetChild(0).localPosition; // get next segment
                _jointRotation[jointIndex] = Quaternion.Inverse(Quaternion.LookRotation(Vector3.Cross(right, up), up));

                up = _fingersRoot[i].GetChild(0).GetChild(0).localPosition;
                _jointRotation[jointIndex + 1] = Quaternion.Inverse(Quaternion.LookRotation(Vector3.Cross(right, up), up));

                up = _fingersRoot[i].GetChild(0).GetChild(0).GetChild(0).localPosition;
                _jointRotation[jointIndex + 2] = Quaternion.Inverse(Quaternion.LookRotation(Vector3.Cross(right, up), up));
            }
        }

        private Vector3 FindLocalNormal(Transform node)
        {
            var rotation = node.rotation * Quaternion.Euler(InitialRotation);
            if (_fingersRoot[0].parent != null)
                 rotation = Quaternion.Inverse(_fingersRoot[0].parent.rotation) * rotation;
            var axis = Vector3.zero;
            var minDistance = 0f;
            var dot = Vector3.Dot(rotation * Vector3.forward, Vector3.right);
            if (dot > minDistance)
            {
                minDistance = dot;
                axis = Vector3.forward;
            }
            else if (-dot > minDistance)
            {
                minDistance = -dot;
                axis = Vector3.back;
            }

            dot = Vector3.Dot(rotation * Vector3.right, Vector3.right);
            if (dot > minDistance)
            {
                minDistance = dot;
                axis = Vector3.right;
            }
            else if (-dot > minDistance)
            {
                minDistance = -dot;
                axis = Vector3.left;
            }

            dot = Vector3.Dot(rotation * Vector3.up, Vector3.right);
            if (dot > minDistance)
            {
                minDistance = dot;
                axis = Vector3.up;
            }
            else if (-dot > minDistance)
            {
                minDistance = -dot;
                axis = Vector3.down;
            }
            return axis;
        }

        #endregion
    }
}