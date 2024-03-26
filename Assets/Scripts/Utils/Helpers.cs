using System;
using UnityEngine;

namespace TurboTowers.Helpers
{
    public static class Helpers
    {
        public static void CreateSmallCube(Vector3 position)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Small Helper Cube";
            cube.GetComponent<Collider>().enabled = false;
            cube.transform.localScale = new Vector3(1f, 1f, 1f);
            cube.transform.position = position;
            cube.GetComponent<Renderer>().material.color = Color.red;
        }
    }
}