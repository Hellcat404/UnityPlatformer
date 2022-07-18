using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts {
    class Platform : MonoBehaviour {

        private static System.Random rand = new System.Random();

        public static Material StartPlatformMat;
        public static Material checkpointOffMat;
        public static Material checkpointOnMat;
        public static Material PlatMat;
        public static Material DropPlatMat;
        public static Material FinishPlatMat;

        public static GameObject createPlatform(PlatformType type) {
            GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            switch(type) {
                case PlatformType.Normal:
                    float platSize = 2f + (float)(rand.Next(5) + rand.NextDouble());
                    platform.transform.localScale += new Vector3(platSize,0f,0f);
                    platform.GetComponent<Renderer>().material = PlatMat;
                    platform.AddComponent<Rigidbody>();
                    platform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    break;
                case PlatformType.Start:
                    platform.transform.localScale += new Vector3(2f,0f,0f);
                    platform.transform.position = new Vector3(0f,0f,0f);
                    platform.GetComponent<Renderer>().material = StartPlatformMat;
                    platform.AddComponent<Rigidbody>();
                    platform.GetComponent<Rigidbody>().useGravity = false;
                    platform.GetComponent<Rigidbody>().isKinematic = false;
                    platform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    break;
                case PlatformType.Drop:
                    float dropPlatSize = (float)(rand.Next(2) + rand.NextDouble());
                    platform.transform.localScale += new Vector3(dropPlatSize,0f,0f);
                    platform.GetComponent<Renderer>().material = DropPlatMat;
                    platform.AddComponent<Rigidbody>();
                    platform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    break;
                case PlatformType.Checkpoint:
                    float checkpointSize = (float)(rand.Next(3) + rand.NextDouble());
                    platform.transform.localScale += new Vector3(checkpointSize,0f,0f);
                    platform.GetComponent<Renderer>().material = checkpointOffMat;
                    platform.AddComponent<Rigidbody>();
                    platform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    break;
                case PlatformType.Finish:
                    platform.transform.localScale += new Vector3(5f,0f,0f);
                    platform.GetComponent<Renderer>().material = FinishPlatMat;
                    platform.AddComponent<Rigidbody>();
                    platform.GetComponent<Rigidbody>().useGravity = false;
                    platform.GetComponent<Rigidbody>().isKinematic = false;
                    platform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    break;
                default:
                    break;
            }
            return platform;
        }
    }
}
