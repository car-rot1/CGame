using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CGame
{
    public static class AnimationCurveExtension
    {
        public static float GetTime(this AnimationCurve curve) => curve.keys[^1].time - curve.keys[0].time;

        public static Vector2 GetFirstPoint(this AnimationCurve curve) => new(curve.keys[0].time, curve.keys[0].value);
        
        public static Vector2 GetLastPoint(this AnimationCurve curve) => new(curve.keys[^1].time, curve.keys[^1].value);
        
        public static AnimationCurve Normalize(this AnimationCurve curve)
        {
            var curveOffsetX = 0 - curve.GetFirstPoint().x;
            var curveScaleX = 1f / curve.GetTime();

            var keyFrames = curve.keys;
            for (var i = 0; i < keyFrames.Length; i++)
            {
                keyFrames[i].time += curveOffsetX;
                keyFrames[i].time *= curveScaleX;
            }

            return new AnimationCurve(keyFrames);
        }

        // 准确性存在问题
        public static AnimationCurve Symmetry(this AnimationCurve curve, float symmetryValue, int symmetryDirection = 1)
        {
            var addKeys = new List<Keyframe>();
            
            var lastPoint = new Vector2(symmetryValue - 0.05f, curve.Evaluate(symmetryValue - 0.05f));
            var nowPoint = new Vector2(symmetryValue, curve.Evaluate(symmetryValue));
            var nextPoint = new Vector2(symmetryValue + 0.05f, curve.Evaluate(symmetryValue + 0.05f));
            
            var symmetryKey = new Keyframe(nowPoint.x, nowPoint.y);

            var keyList = curve.keys.ToList();
            if (symmetryDirection > 0)
            {
                symmetryKey.inTangent = (nowPoint.y - lastPoint.y) / (nowPoint.x - lastPoint.x);
                symmetryKey.outTangent = -symmetryKey.inTangent;
                addKeys.Add(symmetryKey);

                for (var i = keyList.Count - 1; i >= 0; i--)
                {
                    var key = keyList[i];
                    if (key.time >= symmetryValue)
                    {
                        keyList.RemoveAt(i);
                        continue;
                    }
                    var newKey = new Keyframe(
                        symmetryValue + symmetryValue - key.time,
                        key.value,
                        -key.outTangent,
                        -key.inTangent,
                        key.outWeight,
                        key.inWeight);
                    newKey.weightedMode = key.weightedMode switch
                    {
                        WeightedMode.None => WeightedMode.None,
                        WeightedMode.Both => WeightedMode.Both,
                        WeightedMode.In => WeightedMode.Out,
                        WeightedMode.Out => WeightedMode.In,
                        _ => newKey.weightedMode
                    };
                    addKeys.Add(newKey);
                }
            }
            else
            {
                symmetryKey.outTangent = (nextPoint.y - nowPoint.y) / (nextPoint.x - nowPoint.x);
                symmetryKey.inTangent = -symmetryKey.outTangent;
                addKeys.Add(symmetryKey);
                
                for (var i = keyList.Count - 1; i >= 0; i--)
                {
                    var key = keyList[i];
                    if (key.time <= symmetryValue)
                    {
                        keyList.RemoveAt(i);
                        continue;
                    }
                    
                    var newKey = new Keyframe(
                        symmetryValue + symmetryValue - key.time,
                        key.value,
                        -key.outTangent,
                        -key.inTangent,
                        key.outWeight,
                        key.inWeight);
                    newKey.weightedMode = key.weightedMode switch
                    {
                        WeightedMode.None => WeightedMode.None,
                        WeightedMode.Both => WeightedMode.Both,
                        WeightedMode.In => WeightedMode.Out,
                        WeightedMode.Out => WeightedMode.In,
                        _ => newKey.weightedMode
                    };
                    addKeys.Add(newKey);
                }
            }
            
            keyList.AddRange(addKeys);
            return new AnimationCurve(keyList.ToArray());
        }

        public static float GetCurveLength(this AnimationCurve curve, int accuracy = 100)
        {
            if (accuracy > 1000_000)
                accuracy = 1000_000;
            
            var curveLength = 0f;
            
            var value = curve.keys[0].time;
            var valueOffset = curve.GetTime() / accuracy;
            
            var lastPoint = curve.GetFirstPoint();
            for (var i = 1; i <= accuracy; i++)
            {
                value += valueOffset;
                var nowPoint = new Vector2(value, curve.Evaluate(value));
                
                curveLength += (nowPoint - lastPoint).magnitude;
                lastPoint = nowPoint;
            }

            return curveLength;
        }

        public static float GetCurveArea(this AnimationCurve curve, int accuracy = 100)
        {
            if (accuracy > 1000_000)
                accuracy = 1000_000;
            
            var curveArea = 0f;
            
            var value = curve.keys[0].time;
            var valueOffset = curve.GetTime() / accuracy;
            
            var lastPoint = curve.GetFirstPoint();
            for (var i = 1; i <= accuracy; i++)
            {
                value += valueOffset;
                var nowPoint = new Vector2(value, curve.Evaluate(value));

                // y = ax + b;
                var a = (nowPoint.y - lastPoint.y) / (nowPoint.x - lastPoint.x); 
                var b = nowPoint.y - a * nowPoint.x;
                
                if (lastPoint.x * nowPoint.x < 0)
                {
                    var zeroPoint = new Vector2(-b / a, 0);
                    curveArea += (zeroPoint.x - lastPoint.x) * Mathf.Abs(lastPoint.y) * 0.5f;
                    curveArea += (nowPoint.x - zeroPoint.x) * Mathf.Abs(nowPoint.y) * 0.5f;
                }
                else
                {
                    curveArea += (nowPoint.x - lastPoint.x) * Mathf.Min(Mathf.Abs(lastPoint.y), Mathf.Abs(nowPoint.y));
                    curveArea += (nowPoint.x - lastPoint.x) * Mathf.Abs(nowPoint.y - lastPoint.y) * 0.5f;
                }
                lastPoint = nowPoint;
            }

            return curveArea;
        }

        public static Vector2 GetPositionForProcess(this AnimationCurve curve, float process)
        {
            switch (process)
            {
                case <= 0:
                    return curve.GetFirstPoint();
                case >= 1:
                    return curve.GetLastPoint();
            }

            var x = Mathf.Lerp(curve.keys[0].time, curve.keys[^1].time, process);
            return new Vector2(x, curve.Evaluate(x));
        }
        
        public static Vector2 GetPositionForCurveProcess(this AnimationCurve curve, float curveProcess, int accuracy = 100)
        {
            if (accuracy > 1000_000)
                accuracy = 1000_000;
            
            switch (curveProcess)
            {
                case <= 0:
                    return curve.GetFirstPoint();
                case >= 1:
                    return curve.GetLastPoint();
            }

            var realLength = curve.GetCurveLength(accuracy);
            var processLength = realLength * curveProcess;
            
            var nowLength = 0f;
            
            var value = curve.keys[0].time;
            var valueOffset = curve.GetTime() / accuracy;
            var lastPoint = curve.GetFirstPoint();
            for (var i = 1; i <= accuracy; i++)
            {
                value += valueOffset;
                var nowPoint = new Vector2(value, curve.Evaluate(value));
                var length = (nowPoint - lastPoint).magnitude;
                if (nowLength < processLength && processLength <= nowLength + length)
                {
                    var process = Mathf.InverseLerp(nowLength, nowLength + length, processLength);
                    return Vector2.Lerp(lastPoint, nowPoint, process);
                }
                nowLength += length;
                lastPoint = nowPoint;
            }

            return lastPoint;
        }
    }
}