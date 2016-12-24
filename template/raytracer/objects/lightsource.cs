using OpenTK;

namespace template
{
    class lightsource
    {
        public Vector3 position;
        public Vector3 color;

        public lightsource(Vector3 position, Vector3 color)
        {
            this.position = position;
            this.color = color;
        }
    }
}
