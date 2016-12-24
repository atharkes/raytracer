using OpenTK;
using System;

namespace template
{
    class camera
    {
        public Vector3 position, direction;
        public Vector3 planeCenter;
        public Vector3 planeCorner1, planeCorner2, planeCorner3, planeCorner4;
        public Vector3 up = new Vector3(0, -1, 0);
        public Vector3 left;
        public float fov = 90;
        public float sensitivity = 0.004f;
        public float moveSpeed = 0.1f;

        public camera(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
            UpdatePlane();
        }
        
        void UpdatePlane()
        {
            planeCenter = position + direction * 1 / ((float)Math.Tan(fov / 360 * Math.PI));
            left = new Vector3(-direction.Z, 0, direction.X).Normalized();
            up = Vector3.Cross(direction, left);
            planeCorner1 = planeCenter + left + up;
            planeCorner2 = planeCenter - left + up;
            planeCorner3 = planeCenter + left - up;
            planeCorner4 = planeCenter - left - up;
        }

        public void Move(Vector3 move)
        {
            position += move * moveSpeed;
            UpdatePlane();
        }

        public void Turn(Vector3 turn)
        {
            direction += turn * sensitivity;
            direction.Normalize();
            UpdatePlane();
        }
    }
}
