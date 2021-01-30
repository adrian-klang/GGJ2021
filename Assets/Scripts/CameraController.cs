using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform FollowTarget;
    public Transform Parent;
    public GameConfig Config;

    private void LateUpdate() {
        transform.localPosition = new Vector3(0, 0, Config.Distance);
        transform.LookAt(Parent);

        Vector3 desiredPos = FollowTarget.position;
        Vector3 actualPos = Parent.position;

        Parent.position = Vector3.Slerp(actualPos, desiredPos, Config.FollowDamp);
    }
}
