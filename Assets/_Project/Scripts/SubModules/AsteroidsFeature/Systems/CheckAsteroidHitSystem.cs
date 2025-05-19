using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using Asteroids.StartshipsFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class CheckAsteroidHitSystem : IEcsRun //GAME_RULES
    {
        [DI] RuntimeData _runtimeData;
        [DI] StaticData _staticData;
        [DI] PoolService _poolService;
        [DI] EntityGraph _graph;
        List<int> _rels = new List<int>(32);


        class OtherAspect : EcsAspect
        {
            public EcsPool<Asteroid> Asteroids = Exc;
        }
        class RelAspect : EcsAspect
        {
            public EcsPool<HitRequest> HitRequests = Inc;
            public EcsPool<HitAnswer> HitAnswers = Exc;
            public EcsPool<HitImmunity> HitImmunities = Opt;
        }
        class AsteroidAspect : EcsAspect
        {
            public EcsPool<Asteroid> Asteroids = Inc;
            public EcsPool<TransformData> TransformDatas = Inc;
            public EcsPool<KillSignal> killSignals = Opt;
        }

        public void Run()
        {
            _graph.World.GetAspects(out AsteroidAspect asteroidA, out OtherAspect otherA);
            _graph.GraphWorld.GetAspects(out RelAspect relA);

            var map = _graph.GraphWorld.Where(relA).Join(JoinMode.End);
            foreach (var asteroidE in map.Nodes.Where(asteroidA))
            {
                ref var asteroid = ref asteroidA.Asteroids[asteroidE];
                ref var transformData = ref asteroidA.TransformDatas[asteroidE];
                _rels.Clear();

                if (map.GetRelationsCount(asteroidE) > 0)
                {
                    Vector3 directionNormalsSum = default;
                    foreach (var relE in map.GetRelations(asteroidE))
                    {
                        var otherE = _graph.GetRelationOpposite(relE, asteroidE);
                        if(_graph.World.IsMatchesMask(otherA, otherE))
                        {
                            directionNormalsSum += relA.HitRequests[relE].directionNormal;
                            _rels.Add(relE);
                            relA.HitAnswers.TryAddOrGet(relE).directionNormal = -directionNormalsSum;
                        } 
                    }
                }
                
                if (_rels.Count > 0)
                {
                    Vector3 directionNormalsSum = default;
                    _rels.Clear();
                    foreach (var relE in map.GetRelations(asteroidE))
                    {
                        directionNormalsSum += relA.HitRequests[relE].directionNormal;
                        _rels.Add(relE);
                        relA.HitAnswers.TryAddOrGet(relE).directionNormal = -directionNormalsSum;
                    }

                    var explosion = _poolService.Get(_staticData.AsteroidExplosionPrefab, out var instanceID);
                    explosion.transform.position = transformData.position;
                    explosion.Play(_poolService, instanceID);

                    _runtimeData.Score++;
                    asteroidA.killSignals.TryAddOrGet(asteroidE);



                    if (asteroid.DeathsLeft <= 0) { continue; }
                    asteroid.DeathsLeft--;

                    var forward = Vector3.forward;
                    if (directionNormalsSum != Vector3.zero)
                    {
                        forward = directionNormalsSum.normalized;
                    }
                    var spawnPool = _graph.World.GetPool<SpawnAsteroidSignal>();
                    for (var i = 0; i < 2; i++)
                    {
                        var startForward = Quaternion.Euler(0, 90 + 180 * i, 0) * forward;

                        ref var spawnAsteroid = ref spawnPool.NewEntity(out int newAsteroidE);
                        spawnAsteroid.DeathsLeft = asteroid.DeathsLeft;
                        spawnAsteroid.Position = transformData.position;
                        spawnAsteroid.Rotation = Quaternion.LookRotation(startForward);
                        spawnAsteroid.StartRadius = asteroid.Radius / 2f;

                        foreach (var relE in _rels)
                        {
                            var newRelE = _graph.GetOrNewRelation(_graph.GetRelationStart(relE), newAsteroidE);
                            relA.HitImmunities.TryAddOrGet(newRelE).TimeLeft = 0.2f;
                        }
                    }
                }
            }

        }
    }
}