using Dp.Enums;
using Dp.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dp.ActiveRagdoll
{
    public class CollisionHandler : MonoBehaviour
    {
        private ActiveRagdoll _controller;
        private LayerMask _collisionMask;
        private ConfigurableJointMotion _motion;
        // Start is called before the first frame update
        void Start()
        {
            _controller = GetComponentInParent<ActiveRagdoll>();
            _collisionMask = _controller.CollisionMask;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_controller == null)
                return;

            var layer = collision.gameObject.layer;
            if (_collisionMask == (_collisionMask | (1 << layer)))
            {
                GetComponent<Rigidbody>().FreezeRotationOnAxis(_controller.JointDirection);
                _controller.CollisionDetected++;
            }
               
        }

        private void OnCollisionExit(Collision collision)
        {
            if (_controller == null)
                return;
            var layer = collision.gameObject.layer;
            if (_collisionMask == (_collisionMask | (1 << layer)))
            {
                GetComponent<Rigidbody>().FreezeRotationOnAxis(Axis.None);
                _controller.CollisionDetected--;
            }
        }
    }
}
