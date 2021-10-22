using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TavernaGSUtilities {
    public static class BezierUtils {

        public static Vector2 GetBezierQuadraticPoint(Vector2 start, Vector2 end, Vector2 control, float t) {
            Vector2 v1 = Vector2.Lerp(start, control, t);
            Vector2 v2 = Vector2.Lerp(control, end, t);
            return Vector2.Lerp(v1, v2, t);
        }

        public static float BezierLenght(Vector2 p1, Vector2 p2, Vector2 control) {
            float lenght = 0;
            Vector2 previousStep = p1;
            for (int i = 0; i <= 10; ++i) {
                Vector2 step = BezierUtils.GetBezierQuadraticPoint(p1, p2, control, i / 10f);
                lenght += Vector2.Distance(step, previousStep);
                previousStep = step;
            }
            return lenght;
        }

        public static BezierQuadraticPoint[] GetBezierQuadraticPoints(Vector2 start, Vector2 end, Vector2 control, int numPoints) {
            BezierQuadraticPoint[] points = new BezierQuadraticPoint[numPoints + 1];
            Vector2 last = Vector2.zero;
            float distance = 0;
            for (int t = 0; t <= numPoints; t++) {
                Vector2 current = GetBezierQuadraticPoint(start, end, control, (float) t / (float) numPoints);
                if (t > 0) {
                    points[t] = new BezierQuadraticPoint();
                    distance = points[t].SetPoint(last, current, distance);
                }
                last = current;
            }

            return points;
        }

        public static Vector2 TravelInBezierQuadratic(BezierQuadraticPoint[] points, float distance) {
            BezierQuadraticPoint current = null;
            int index = 1;

            while (points.Length > index && points[index].distanceTraveled < distance) {
                current = points[index];
                index++;
            }

            if (current == null || distance < 0) return points[0].current;

            var amount = Mathf.InverseLerp(0, current.distance, distance - current.distanceTraveled);
            return Vector2.Lerp(current.current, current.next, amount);
        }

        public static BezierQuadraticLength[] GetBezierQuadraticLengths(Vector2 start, Vector2 end, Vector2 control, int numPoints) {
            BezierQuadraticLength[] points = new BezierQuadraticLength[numPoints + 1];
            Vector2 last = start;
            float distance = 0;
            for (int t = 0; t <= numPoints; t++) {
                float normalized = (float) t / (float) numPoints;
                Vector2 current = GetBezierQuadraticPoint(start, end, control, normalized);
                distance += Vector2.Distance(current, last);
                points[t] = new BezierQuadraticLength(distance, normalized);
                last = current;
            }

            return points;
        }

        public static float GetDistanceNormalized(BezierQuadraticLength[] normals, float distance, ref int lastId) {
            BezierQuadraticLength normal = null, nextNormal = null;
            for (int i = lastId; i < normals.Length; ++i) {
                if (normals[i].length > distance) {
                    nextNormal = normals[i];
                    lastId = i;
                    break;
                }
                normal = normals[i];
            }
            if (distance < 0) return normals[lastId].normalized;
            else if (distance > normals[(normals.Length - 1)].length) return normals[(normals.Length - 1)].normalized;
            if (normal == null) normal = normals[lastId - 1];
            var amount = Mathf.InverseLerp(normal.length, nextNormal.length, distance);
            return Mathf.Lerp(normal.normalized, nextNormal.normalized, amount);
        }
    }

    [System.Serializable]
    public class BezierQuadraticPoint {
        public Vector2 current;
        public Vector2 next;
        public float distance;
        public float distanceTraveled;

        public float SetPoint(Vector2 current, Vector2 next, float distanceTraveled) {
            this.current = current;
            this.next = next;
            this.distance = (next - current).magnitude;
            this.distanceTraveled = distanceTraveled + this.distance;
            return distanceTraveled + this.distance;
        }
    }

    [System.Serializable]
    public class BezierQuadraticLength {
        public float length, normalized;

        public BezierQuadraticLength(float length, float normalized) {
            this.length = length;
            this.normalized = normalized;
        }

        public BezierQuadraticLength Copy() {
            return new BezierQuadraticLength(length, normalized);
        }
    }
}

public class Test2 : MonoBehaviour {
    public int NumPoints = 100;
    public Point[] points;

    public Vector3 startPos;
    public Vector3 controlPoint;
    public Vector3 wayPoint;

    void Start() {
        points = new Point[NumPoints + 1];
        Vector3 last = Vector3.zero;
        float distance = 0;
        for (int t = 0; t <= NumPoints; t++) {
            Vector3 current = GetBezierPoint((float) t / (float) NumPoints);
            if (t > 0) {
                points[t] = new Point();
                distance = points[t].SetPoint(last, current, distance);
            }
            last = current;
        }
    }

    public Vector3 Travel(float distance) {
        Point current = null;
        int index = 0;

        while (points.Length < index && points[index].distanceTraveled < distance) {
            current = points[index];
            index++;
        }

        if (current == null || distance < 0) return points[NumPoints].current;

        var amount = Mathf.InverseLerp(0, current.distance, distance - current.distanceTraveled);

        return Vector3.Lerp(current.current, current.next, amount);
    }

    private Vector3 GetBezierPoint(float t) {
        float x = (((1 - t) * (1 - t)) * startPos.x) + (2 * t * (1 - t) * controlPoint.x) + ((t * t) * wayPoint.x);
        float y = (((1 - t) * (1 - t)) * startPos.y) + (2 * t * (1 - t) * controlPoint.y) + ((t * t) * wayPoint.y);
        return new Vector3(x, y, 0);
    }
}

public class Point {
    public Vector3 current;
    public Vector3 next;
    public float distance;
    public float distanceTraveled;

    public float SetPoint(Vector3 current, Vector3 next, float distanceTraveled) {
        this.current = current;
        this.next = next;
        this.distance = (next - current).magnitude;
        this.distanceTraveled = distanceTraveled + this.distance;
        return distanceTraveled + this.distance;
    }
}