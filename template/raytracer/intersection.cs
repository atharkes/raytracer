using OpenTK;

namespace template
{
    class intersection
    {
        public Vector3 position;
        public primitive primitive;

        public intersection(Vector3 position, primitive primitive)
        {
            this.position = position;
            this.primitive = primitive;
        }
    }
}
