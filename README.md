Whitted Raytracer for the course Advanced Graphics in 2019-2020 at Utrecht University

Created by:
Theo Harkes (5672120)

Required:
- .Net Framework 4.7.2

Features:
- Raytracing architecture
- Camera: Position, Orientation, FOV, Aspect ratio
- Input handling (see controls)
- Primitives: Planes, Spheres, Triangles
- Materials: Diffuse, Specular, Dielectric, Glossy (floats)
- Scene showing off all primitives and materials

Extra:
- Dielectrics: Snell & Fresnel
- Multithreading: Threadpool with worker threads
- Acceleration Structure: BVH
- Glossyness: Phong Shading
- Debug drawing
- Triangle primitives

References:
- Course Slides (Graphics & Advanced Graphics)
- OpenTK: https://github.com/opentk/opentk
- Triangle Intersection: https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
- Sphere Intersection: https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection
- AABB Intersection: https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-box-intersection

Controls:
- Arrow keys:   Rotate Camera
- W,A,S,D:      Move Camera
- Space:        Move Camera Up
- Shift:        Move Camera Down
- F1:           Disable/Enable Debug
- Numpad '+':   Increase FOV
- Numpad '-':   Decrease FOV