using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using UnityEngine;

namespace Asteroids.GameFieldFeature
{
    [CreateAssetMenu(menuName = "Asteroids/Modules/Game Field Config")]
    public class GameFieldModuleConfig : ScriptableObject, IInjectionBlock
    {
        public float ScreenBorderOffset = 0.5f;

        [Range(1.1f, 2f)]
        public float AdditionalKillOffset = 1.1f;

        public void InjectTo(Injector inj)
        {
            var boundsOverlapsRuntime = new BoundsOverlapsRuntime();
            inj.Inject(new GameFieldRuntimeData
            {
                BoundsOverlapsRuntime = boundsOverlapsRuntime
            });
            inj.Inject(boundsOverlapsRuntime);
        }
    }
}
