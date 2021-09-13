# Pathtracing Workbench
**by Theo Harkes**

*Disclaimer: This framework is by no means efficient at pathtracing. It's main goal is to be general enough to be able to support a multitude of different (monte carlo) rendering algorithms, while breaking them into smaller pieces to avoid blobs and code duplication.*

#### Required:
Visual Studio 2019 with:
- .Net 5
- Class Designer (to view the class diagram)

#### Features:
- Pathtracing architecture
- Camera: Position, Orientation, FOV, Aspect ratio
- Input handling: see controls
- Primitives: Planes, Spheres, Triangles, etc.
- Materials: Diffuse, Specular, Dielectric (Snell & Fresnel), Glossy (Phong Shading), etc.
- Acceleration Structure: SBVH, BVH (SAH, Binning, Split-Ordered-Traversal & BVH rendering)
- Multithreading: Threadpool with worker threads

#### Controls:
- Arrow keys:   Rotate Camera
- W,A,S,D:      Move Camera
- Space:        Move Camera Up
- Shift:        Move Camera Down
- F1:           Toggle debug information
- F2:           Cycle through viewing mode
- Numpad '+':   Increase FOV
- Numpad '-':   Decrease FOV
- ESC           Exit the application

#### References:
- Many Parts: Course Slides of Graphics & Advanced Graphics at the Utrecht University
- OpenTK: https://github.com/opentk/opentk
- Triangle Intersection: https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
- Sphere Intersection: https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection
- AABB Intersection: https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-box-intersection
- Binning: On fast Construction of SAH-based Bounding Volume Hierarchies - I. Wald
- Triangle Clipping: Efficient Triangle and Quadrilateral Clipping within Shaders - M. McGuire
- SBVH Idea: Spatial Splits in Bounding Volume Hierarchies - M. Stich, H. Friedrich, A. Dietrich
- SBVH Implementation: Parallel Spatial Splits in Bounding Volume Hierarchies - V. Fuetterling, C. Lojewski, F.-J Pfreundt and A. Ebert

## Notes to Self
#### Monte Carlo integration required for:
- Area Lights (Random point on area of light)
- Indirect Illumination (Random bounce over hemisphere)
- Depth of Field (Introducing a lense somewhere)
- Anti-Aliasing (Random point on screenplane pixels)
- Motion Blur (Random point in time)
