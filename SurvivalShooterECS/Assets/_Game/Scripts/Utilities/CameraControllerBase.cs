using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities {
    public class CameraControllerBase : SingletonMonoBehaviour<CameraControllerBase> {
        public float widthInUnits;
        public Camera camera;

        public virtual void Start() {
            camera = GetComponent<Camera>();
            Miscellaneous.SetCameraOrthographicSizeByWidth(camera, widthInUnits);
        }
    }
}