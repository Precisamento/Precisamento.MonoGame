using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public class PolygonCollider : Collider
    {
        private float _rotation = 0;
        private RectangleF _boundingBox;
        private Vector2[] _edgeNormals;
        private Vector2[] _points;
        private Vector2 _center;
        private float _scale = 1;
        protected bool _dirty = true;
        protected Vector2[] _originalPoints;
        private Vector2 _originalCenter;

        public Vector2[] Points
        {
            get
            {
                if (_dirty)
                    Clean();
                return _points;
            }
        }

        public override RectangleF BoundingBox
        {
            get
            {
                if (_dirty)
                    Clean();
                return new RectangleF(_boundingBox.Position + Position, _boundingBox.Size);
            }
        }

        public Vector2[] EdgeNormals
        {
            get
            {
                if (_dirty)
                    Clean();
                return _edgeNormals;
            }
        }

        public override Vector2 Position { get; set; }

        public override float Rotation
        {
            get => _rotation;
            set
            {
                if(value != _rotation)
                {
                    _rotation = value;
                    _dirty = true;
                }
            }
        }

        public Vector2 OriginalCenter
        {
            get => _originalCenter;
            set
            {
                if(value != _originalCenter)
                {
                    _originalCenter = value;
                    _dirty = true;
                }
            }
        }

        public Vector2 Center
        {
            get => _originalCenter * _scale;
        }

        public override float Scale
        {
            get => _scale;
            set
            {
                if(value != _scale)
                {
                    AssertScale(value);
                    _scale = value;
                    _dirty = true;
                }
            }
        }

        public bool IsUnrotated => _rotation == 0;

        public override ColliderType ColliderType => ColliderType.Polygon;

        public PolygonCollider(Vector2[] points)
            : this(points, Vector2.Zero) 
        { 
        }

        public PolygonCollider(Vector2[] points, Vector2 center)
        {
            _originalPoints = new Vector2[points.Length];
            _points = new Vector2[points.Length];
            _edgeNormals = new Vector2[points.Length];
            _center = _originalCenter = center;
            Array.Copy(points, _originalPoints, points.Length);
        }

        private void Clean()
        {
            _dirty = false;
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var maxX = float.MinValue;
            var maxY = float.MinValue;

            // The branches here are used to avoid having to iterate over the points array more than once.
            // It makes sure to perform all operations at the same.

            if(_scale != 1f && _rotation != 0)
            {
                var cos = MathF.Cos(_rotation);
                var sin = MathF.Sin(_rotation);
                _center = _originalCenter * _scale;
                for(var i = 0; i < _originalPoints.Length; i++)
                {
                    var p = (_originalPoints[i] * _scale) - _center;
                    p = new Vector2((cos * p.X) - (sin * p.Y), (sin * p.X) + (cos * p.Y));
                    _points[i] = p + _center;

                    if (_points[i].X < minX)
                        minX = _points[i].X;
                    if (maxX < _points[i].X)
                        maxX = _points[i].X;
                    if (_points[i].Y < minY)
                        minY = _points[i].Y;
                    if (maxY < _points[i].Y)
                        maxY = _points[i].Y;

                    SetEdgeNormal(i);
                }
            }
            else if(_scale != 1f)
            {
                _center = _originalCenter * _scale;

                for (var i = 0; i < _points.Length; i++)
                {
                    _points[i] *= _scale;

                    if (_points[i].X < minX)
                        minX = _points[i].X;
                    if (maxX < _points[i].X)
                        maxX = _points[i].X;
                    if (_points[i].Y < minY)
                        minY = _points[i].Y;
                    if (maxY < _points[i].Y)
                        maxY = _points[i].Y;

                    SetEdgeNormal(i);
                }
            }
            else if(_rotation != 0)
            {
                var cos = MathF.Cos(_rotation);
                var sin = MathF.Sin(_rotation);
                for (var i = 0; i < _points.Length; i++)
                {
                    var p = _points[i] - _center;
                    p = new Vector2((cos * p.X) - (sin * p.Y), (sin * p.X) + (cos * p.Y));
                    _points[i] = p + _center;

                    if (_points[i].X < minX)
                        minX = _points[i].X;
                    if (maxX < _points[i].X)
                        maxX = _points[i].X;
                    if (_points[i].Y < minY)
                        minY = _points[i].Y;
                    if (maxY < _points[i].Y)
                        maxY = _points[i].Y;

                    SetEdgeNormal(i);
                }
            }
            else
            {
                for (var i = 0; i < _originalPoints.Length; i++)
                {
                    _points[i] = _originalPoints[i];

                    if (_points[i].X < minX)
                        minX = _points[i].X;
                    if (maxX < _points[i].X)
                        maxX = _points[i].X;
                    if (_points[i].Y < minY)
                        minY = _points[i].Y;
                    if (maxY < _points[i].Y)
                        maxY = _points[i].Y;

                    SetEdgeNormal(i);
                }
            }

            _boundingBox = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetEdgeNormal(int index)
        {
            if (index == 0)
                return;

            var perp = Vector2Ext.Perpendicular(ref _points[index - 1], ref _points[index]);
            Vector2Ext.Normalize(ref perp);
            _edgeNormals[index - 1] = perp;

            if(index == _points.Length - 1)
            {
                perp = Vector2Ext.Perpendicular(ref _points[index], ref _points[0]);
                Vector2Ext.Normalize(ref perp);
                _edgeNormals[index] = perp;
            }
        }

        public override void DebugDraw(SpriteBatch spriteBatch, Color color)
        {
            Primitives2D.DrawLine(spriteBatch, Points[_points.Length - 1] + Position, _points[0] + Position, color);

            for (var i = 1; i < _points.Length; i++)
                Primitives2D.DrawLine(spriteBatch, _points[i - 1] + Position, _points[i] + Position, color);
        }

        public static Vector2[] BuildBox(float width, float height)
        {
            return new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(width, 0),
                new Vector2(width, height),
                new Vector2(0, height)
            };
        }

        public static Vector2 FindPolygonCenter(Vector2[] points)
        {
            float x = 0, y = 0;

            for (var i = 0; i < points.Length; i++)
            {
                x += points[i].X;
                y += points[i].Y;
            }

            return new Vector2(x / points.Length, y / points.Length);
        }

        public static Vector2 GetFarthestPointInDirection(PolygonCollider poly, Vector2 direction)
        {
            var points = poly.Points;
            int index = 0;
            Vector2.Dot(ref points[index], ref direction, out float maxDot);

            for (int i = 0; i < points.Length; i++)
            {
                Vector2.Dot(ref points[i], ref direction, out float dot);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    index = i;
                }
            }

            return points[index];
        }

        public static Vector2 GetClosestPointOnPolygonToPoint(Vector2[] points, Vector2 point, out float distanceSquared)
        {
            distanceSquared = float.MaxValue;
            var closestPoint = Vector2.Zero;

            for (var i = 0; i < points.Length; i++)
            {
                var j = i + 1;
                if (j == points.Length)
                    j = 0;

                var closest = Collisions.ClosestPointOnLine(points[i], points[j], point);
                Vector2.DistanceSquared(ref point, ref closest, out float tempDistanceSquared);

                if (tempDistanceSquared < distanceSquared)
                {
                    distanceSquared = tempDistanceSquared;
                    closestPoint = closest;
                }
            }

            return closestPoint;
        }

        public static Vector2 GetClosestPointOnPolygonToPoint(Vector2[] points, Vector2 point, out float distanceSquared, out Vector2 edgeNormal)
        {
            distanceSquared = float.MaxValue;
            edgeNormal = Vector2.Zero;
            var closestPoint = Vector2.Zero;

            for (var i = 0; i < points.Length; i++)
            {
                var j = i + 1;
                if (j == points.Length)
                    j = 0;

                var closest = Collisions.ClosestPointOnLine(points[i], points[j], point);
                Vector2.DistanceSquared(ref point, ref closest, out float tempDistanceSquared);

                if (tempDistanceSquared < distanceSquared)
                {
                    distanceSquared = tempDistanceSquared;
                    closestPoint = closest;

                    // get the normal of the line
                    var line = points[j] - points[i];
                    edgeNormal.X = -line.Y;
                    edgeNormal.Y = line.X;
                }
            }

            Vector2Ext.Normalize(ref edgeNormal);

            return closestPoint;
        }

        public static PolygonCollider CreateConvexFromPoints(Vector2[] points)
        {
            Vector2 yMin = points[0];
            int location = 0;
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].Y < yMin.Y || (points[i].Y == yMin.Y && points[i].X < yMin.X))
                {
                    yMin = points[i];
                    location = i;
                }
            }

            var temp = points[0];
            points[0] = yMin;
            points[location] = temp;

            List<KeyValuePair<Vector2, float>> order = new List<KeyValuePair<Vector2, float>>();

            for (int i = 1; i < points.Length; i++)
            {
                float angle = (MathF.Atan2(points[i].Y, points[i].X) - MathF.Atan2(yMin.Y, yMin.X)) * MathF.Rad2Deg;
                order.Add(new KeyValuePair<Vector2, float>(points[i], angle));
            }

            order = (from kvp in order
                     orderby kvp.Value
                     select kvp).ToList();

            int m = 1;
            for (int i = 0; i < order.Count; i++)
            {
                int cur = i;
                float curDis = CalculateDistance(yMin, order[i].Key);
                while (i < order.Count - 1 && order[i].Value == order[i + 1].Value)
                {
                    i++;
                    var d = CalculateDistance(yMin, order[i].Key);
                    if (d > curDis)
                    {
                        curDis = d;
                        cur = i;
                    }
                }
                points[m] = order[cur].Key;
                m++;
            }

            if (m < 3)
            {
                return null;
            }

            Stack<Vector2> make = new Stack<Vector2>();

            make.Push(points[0]);
            make.Push(points[1]);
            make.Push(points[2]);

            for (int i = 3; i < m; i++)
            {
                while (Orientation(NextToTop(make), make.Peek(), points[i]) != 2)
                {
                    make.Pop();
                }
                make.Push(points[i]);
            }

            return new PolygonCollider(make.ToArray());

            float CalculateDistance(Vector2 p1, Vector2 p2)
            {
                return (((p2.X - p1.X) * (p2.X - p1.X)) + ((p2.Y - p1.Y) * (p2.Y - p1.Y)));
            }

            int Orientation(Vector2 p, Vector2 q, Vector2 r)
            {
                float val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
                if (val == 0)
                    return 0;
                return (val > 0) ? 1 : 2;

            }

            Vector2 NextToTop(Stack<Vector2> s)
            {
                Vector2 p = s.Peek();
                s.Pop();
                Vector2 ret = s.Peek();
                s.Push(p);
                return ret;
            }
        }

        public override bool Overlaps(Collider other)
        {
            switch(other.ColliderType)
            {
                case ColliderType.Point:
                    var point = (PointCollider)other;
                    if (point.InternalCollider != point)
                        return Overlaps(point.InternalCollider);
                    return ContainsPoint(point.Position);
                case ColliderType.Line:
                    return Collisions.LineToPoly((LineCollider)other, this);
                case ColliderType.Circle:
                    return Collisions.CircleToPolygon((CircleCollider)other, this);
                case ColliderType.Box:
                case ColliderType.Polygon:
                    return Collisions.PolygonToPolygon(this, (PolygonCollider)other);
            }

            throw new NotImplementedException($"Overlaps of Polygon to {other.GetType()} are not supported.");
        }

        public override bool CollidesWithShape(Collider other, out CollisionResult collision, out RaycastHit ray)
        {
            ray = default;
            switch (other.ColliderType)
            {
                case ColliderType.Point:
                    var point = (PointCollider)other;
                    if (point.InternalCollider != point)
                        return CollidesWithShape(point.InternalCollider, out collision, out ray);
                    return Collisions.PointToPoly(point.Position, this, out collision);
                case ColliderType.Line:
                    collision = default;
                    return Collisions.LineToPoly((LineCollider)other, this, out ray);
                case ColliderType.Circle:
                    return Collisions.CircleToPolygon((CircleCollider)other, this, out collision);
                case ColliderType.Box:
                case ColliderType.Polygon:
                    return Collisions.PolygonToPolygon(this, (PolygonCollider)other, out collision);
            }

            throw new NotImplementedException($"Collisions of Polygon to {other.GetType()} are not supported.");
        }

        public override bool CollidesWithLine(Vector2 start, Vector2 end)
        {
            return Collisions.LineToPoly(start, end, this);
        }

        public override bool CollidesWithLine(Vector2 start, Vector2 end, out RaycastHit hit)
        {
            return Collisions.LineToPoly(start, end, this, out hit);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            return Collisions.PointToPoly(point, this);
        }

        public override bool CollidesWithPoint(Vector2 point, out CollisionResult result)
        {
            return Collisions.PointToPoly(point, this, out result);
        }
    }
}
