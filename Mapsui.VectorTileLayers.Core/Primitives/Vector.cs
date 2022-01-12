using System.Diagnostics;
using System.Globalization;

namespace Mapsui.VectorTileLayers.Core.Primitives
{
    [DebuggerDisplay("X={X}, Y={Y}")]
    public struct Vector
    {
        public static Vector Empty = new Vector(0f, 0f, false);

        public Vector(double x, double y)
        {
            _x = (float)x;
            _y = (float)y;
            _defined = true;
        }

        public Vector(float x, float y)
        {
            _x = x;
            _y = y;
            _defined = true;
        }

        public Vector(float x, float y, bool defined)
        {
            _x = x;
            _y = y;
            _defined = defined;
        }

        private float _x;
        private float _y;
        private bool _defined;

        public bool IsDefined { get => _defined; }

        public float X
        {
            get => _x;
            set
            {
                _x = value;
                _defined = true;
            }
        }

        public float Y
        {
            get => _y;
            set
            {
                _y = value;
                _defined = true;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Vector other && other.IsDefined && _x == other.X && _y == other.Y;
        }

        public override int GetHashCode()
        {
            int result = 7;
            result = 31 * result + (int)(_x * 100000);
            result = 31 * result + (int)(_y * 100000);
            return result;
        }

        public Vector Clone()
        {
            return new Vector(_x, _y);
        }

        public override string ToString()
        {
            return $"X={_x.ToString(CultureInfo.InvariantCulture)}, Y={_y.ToString(CultureInfo.InvariantCulture)}";
        }

        public static Vector operator +(Vector first, Vector second) => new Vector(first.X + second.X, first.Y + second.Y);
        public static Vector operator -(Vector first, Vector second) => new Vector(first.X - second.X, first.Y - second.Y);
        public static Vector operator *(Vector first, float scalar) => new Vector(first.X * scalar, first.Y * scalar);
        public static Vector operator *(float scalar, Vector first) => new Vector(first.X * scalar, first.Y * scalar);
        public static Vector operator /(Vector first, float scalar) => new Vector(first.X / scalar, first.Y / scalar);
        public static Vector operator /(float scalar, Vector first) => new Vector(first.X / scalar, first.Y / scalar);
        public static bool operator ==(Vector first, Vector second) => first.IsDefined && !second.IsDefined && first.X == second.X && first.Y == second.Y;
        public static bool operator !=(Vector first, Vector second) => !(first == second);
    }
}
