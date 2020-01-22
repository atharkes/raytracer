Whitted Raytracer for the course Advanced Graphics in 2019-2020 at Utrecht University

Created by:
Theo Harkes (5672120)

Required:
- .Net Framework 4.7.2

Features:
- Raytracing architecture
- Camera: Position, Orientation, FOV, Aspect ratio
- Input handling: see controls
- Primitives: Planes, Spheres, Triangles
- Materials: Diffuse, Specular, Dielectric (Snell & Fresnel), Glossy (Phong Shading)
- Acceleration Structure: Bounding Volume Hierachy (SAH & Binning)
- Multithreading: Threadpool with worker threads
- Scene showing off all primitives and materials
- Debug drawing

References:
- Course Slides (Graphics & Advanced Graphics)
- OpenTK: https://github.com/opentk/opentk
- Triangle Intersection: https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
- Sphere Intersection: https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection
- AABB Intersection: https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-box-intersection
- Binning: http://www.sci.utah.edu/~wald/Publications/2007/ParallelBVHBuild/fastbuild.pdf

Controls:
- Arrow keys:   Rotate Camera
- W,A,S,D:      Move Camera
- Space:        Move Camera Up
- Shift:        Move Camera Down
- F1:           Toggle Debug Drawing
- F2:           Toggle BVHTraversal Color Scale
- Numpad '+':   Increase FOV
- Numpad '-':   Decrease FOV

Notes to self:
- No area lights with deterministic/whitted raytracing