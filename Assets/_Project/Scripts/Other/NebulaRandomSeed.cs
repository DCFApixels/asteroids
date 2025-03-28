using UnityEngine;

namespace Asteroids
{
    public class NebulaRandomSeed : MonoBehaviour
    {
        private static readonly int _seedProp = Shader.PropertyToID("_Seed");
        private void Awake()
        {
            Renderer renderer = GetComponent<Renderer>();
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(mpb);
            mpb.SetFloat(_seedProp, Random.Range(0, 1000));
            renderer.SetPropertyBlock(mpb);
        }
    }
}
