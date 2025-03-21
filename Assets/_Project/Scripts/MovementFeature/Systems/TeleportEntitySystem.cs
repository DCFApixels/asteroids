using DCFApixels.DragonECS;
using DCFApixels.DragonECS.RunnersCore;

namespace Asteroids.MovementFeature
{
    [MetaGroup(nameof(MovementModule), EcsConsts.PROCESSES_GROUP)]
    [MetaColor(MetaColor.Cyan)]
    public interface ITeleportProcess : IEcsProcess
    {
        public void Run();
    }
    [MetaGroup(nameof(MovementModule), EcsConsts.PROCESSES_GROUP)]
    [MetaColor(MetaColor.Cyan)]
    public class ITeleportProcessRunner : EcsRunner<ITeleportProcess>, ITeleportProcess
    {
        public void Run()
        {
            foreach (var system in Process)
            {
                system.Run();
            }
        }
    }
    [MetaGroup(nameof(MovementModule), EcsConsts.SYSTEMS_GROUP)]
    [MetaColor(MetaColor.Cyan)]
    public class TeleportEntitySystem : ITeleportProcess, IEcsModule, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.POST_END_LAYER;

        class Aspect : EcsAspect
        {
            public EcsPool<TeleportSignal> teleportSignals = Inc;
            public EcsPool<TransformData> transforms = Inc;
        }

        [DI] EcsDefaultWorld _world;
        public void Run()
        {
            var es = _world.Where(out Aspect a);
            foreach (var e in es)
            {
                ref var transform = ref a.transforms.Get(e);
                ref var teleportSignal = ref a.teleportSignals.Get(e);

                transform.position = teleportSignal.newPosition;
                transform.rotation = teleportSignal.newRotation;

                a.teleportSignals.Del(e);
            }
        }

        public void Import(EcsPipeline.Builder b)
        {
            b.AddRunner<ITeleportProcessRunner>();
        }
    }
}
