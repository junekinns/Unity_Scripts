using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class J_EnemyCarController : J_CarController
    {
        public override void Move(float steering, float accel, float footbrake, float handbrake)
        {
            base.Move(steering, accel, footbrake, handbrake);
        }
    }
}
