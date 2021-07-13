using Microsoft.Xna.Framework;

namespace Precisamento.MonoGame.Collisions
{
    public struct RaycastHit
    {
        public Collider Shape;
        public float Fraction;
        public float Distance;
        public Vector2 Point;
        public Vector2 Normal;
        public Vector2 Centroid;

        public RaycastHit(Collider shape, float fraction, float distance, Vector2 point, Vector2 normal)
        {
            Shape = shape;
            Fraction = fraction;
            Distance = distance;
            Point = point;
            Normal = normal;
            Centroid = Vector2.Zero;
        }

        internal void SetValues(Collider shape, float fraction, float distance, Vector2 point)
        {
            Shape = shape;
            Fraction = fraction;
            Distance = distance;
            Point = point;
        }

        internal void SetValues(float fraction, float distance, Vector2 point, Vector2 normal)
        {
            Fraction = fraction;
            Distance = distance;
            Point = point;
            Normal = normal;
        }

        internal void Reset()
        {
            Shape = null;
            Fraction = Distance = 0;
        }

        public override string ToString()
        {
            return $"[RaycastHit] Fraction: {Fraction}, Distance: {Distance}, Normal: {Normal}, Centroid: {Centroid}, Point: {Point}";
        }
    }
}