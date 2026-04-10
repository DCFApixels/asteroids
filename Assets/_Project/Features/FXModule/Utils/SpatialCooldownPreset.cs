using System.Collections.Generic;
using UnityEngine;

namespace Modules.FX
{
    [CreateAssetMenu]
    public class SpatialCooldownPreset : ScriptableObject
    {
        public int CircularSegments = 6;
        public int RadialSegments = 6;
        public float RadialSegmentMultiplier = 2f;
    }
    public class SpatialCooldownManager
    {
        public Dictionary<SpatialCooldownPreset, Stack<SpatialCooldownTable>> TablesPool = new();
        public Dictionary<SFXAsset, SpatialCooldownTable> Tables = new();

        private SpatialCooldownTable TakeTableFromPool(SpatialCooldownPreset preset)
        {
            if (TablesPool.TryGetValue(preset, out var pool) == false)
            {
                pool = new Stack<SpatialCooldownTable>();
                TablesPool.Add(preset, pool);
            }
            if (pool.TryPop(out var result))
            {
                return result;
            }
            return new SpatialCooldownTable(preset);
        }
        private void ReturnTableToPool(SpatialCooldownTable table)
        {
            if (TablesPool.TryGetValue(table.Preset, out var pool) == false)
            {
                pool = new Stack<SpatialCooldownTable>();
                TablesPool.Add(table.Preset, pool);
            }
            pool.Push(table); 
        }


        public void Update()
        {
            
        }
    }
    public class SpatialCooldownTable
    {
        const float Radians = 360f * Mathf.Deg2Rad;

        public readonly SpatialCooldownPreset Preset;
        public readonly Segment[] Grid;

        public SFXAsset ProcessedSFX;
        public float LastTime;

        public SpatialCooldownTable(SpatialCooldownPreset preset)
        {
            Preset = preset;
            Grid = new Segment[CircularSize + RadialSize];
        }

        public int CircularSize => Preset.CircularSegments;
        public int RadialSize => Preset.RadialSegments;

        public bool IsAnyCooldownActive(float currentTime)
        {
            return (currentTime - LastTime) < ProcessedSFX.Cooldown;
        }

        public bool CheckSegment(AudioListener listener, Vector3 point, float currentTime)
        {
            ref var s = ref GetSegment(listener, point);
            return s.Check(currentTime, ProcessedSFX.Cooldown);
        }
        public ref Segment GetSegment(int circularIndex, int radialIndex)
        {
            radialIndex = Mathf.Min(radialIndex, RadialSize - 1);
            return ref Grid[RadialSize * radialIndex + circularIndex];
        }
        public ref Segment GetSegment(AudioListener listener, Vector3 point)
        {
            var center = listener.transform.position;
            var direction = (point - center);

            float radialRaw = direction.sqrMagnitude * Preset.RadialSegmentMultiplier;
            float angle = Mathf.Atan2(direction.y, direction.x);
            float circularRaw = (angle / Radians) * RadialSize;

            int radial = Mathf.FloorToInt(radialRaw);
            radial >>= 2;
            radial = radial < 1 ? 1 : radial;
            int circular = Mathf.FloorToInt(circularRaw);

            return ref GetSegment(circular, radial);
        }
        public struct Segment
        {
            public float LastTime;
        }
    }

    internal static class SegmentExtensions
    {
        public static bool Check(this ref SpatialCooldownTable.Segment self, float currentTime, float cooldown)
        {
            if ((currentTime - self.LastTime) < cooldown)
            {
                return false;
            }
            self.LastTime = currentTime;
            return true;
        }
    }
}

