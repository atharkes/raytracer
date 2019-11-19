using OpenTK;
using System;

namespace Raytracer {
    class Camera {
        public Vector3 Position, Direction;
        public Vector3 PlaneCenter;
        public Vector3 PlaneCorner1, PlaneCorner2, PlaneCorner3, PlaneCorner4;
        public Vector3 Up = new Vector3(0, -1, 0);
        public Vector3 Left;
        public float Fov = 90;
        public float Sensitivity = 0.004f;
        public float MoveSpeed = 0.1f;

        public Camera(Vector3? position = null, Vector3? direction = null) {
            Position = position ?? new Vector3(0, -1, -1);
            Direction = direction ?? new Vector3(0, 0, 1);
            UpdatePlane();
        }

        void UpdatePlane() {
            PlaneCenter = Position + Direction * 1 / ((float)Math.Tan(Fov / 360 * Math.PI));
            Left = new Vector3(-Direction.Z, 0, Direction.X).Normalized();
            Up = Vector3.Cross(Direction, Left);
            PlaneCorner1 = PlaneCenter + Left + Up;
            PlaneCorner2 = PlaneCenter - Left + Up;
            PlaneCorner3 = PlaneCenter + Left - Up;
            PlaneCorner4 = PlaneCenter - Left - Up;
        }

        public void Move(Vector3 move) {
            Position += move * MoveSpeed;
            UpdatePlane();
        }

        public void Turn(Vector3 turn) {
            Direction += turn * Sensitivity;
            Direction.Normalize();
            UpdatePlane();
        }
    }
}