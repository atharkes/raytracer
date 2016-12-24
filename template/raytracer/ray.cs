using OpenTK;

namespace template
{
    class ray
    {
        public Vector3 origin;
        public Vector3 direction;
        public Vector3 directionInverted;
        public float length;
        public float beginLength = float.MaxValue;
        float epsilon = 0.00001f;

        public ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction.Normalized();
            directionInverted = new Vector3(1 / direction.X, 1 / direction.Y, 1 / direction.Z);
            this.origin += this.direction * epsilon;
            length = beginLength;
        }
    }
}
