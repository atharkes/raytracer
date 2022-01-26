# Pathtracing Workbench
**by Theo Harkes**

*Disclaimer: This framework is by no means efficient at pathtracing. It's main goal is to be general enough to be able to support a multitude of different (monte carlo) rendering algorithms, while breaking them into smaller pieces to avoid blobs and code duplication.*

#### Required:
Visual Studio 2021 with:
- .Net 6

#### Features:
- Pathtracing architecture
- Camera: Position, Rotation, FOV, Aspect ratio
- Input handling: see controls
- Primitives: Planes, Spheres, Triangles, etc.
- Materials: Profiled to allow for modular designs
- Density Profiles: Uniform (volumetric), Delta (surface), Exponential (surface with depth)
- Orientation Profiles: Flat (surface), Uniform (volumetric), surface-GGX, surface-SGGX
- Reflection Profiles: Diffuse, Specular
- Acceleration Structure: SBVH, BVH (SAH, Binning, Split-Ordered-Traversal & BVH rendering)
- Multithreading: Threadpool with worker threads

#### Controls:
- Arrow keys:   Rotate Camera
- W,A,S,D:      Move Camera
- Space:        Move Camera Up
- Shift:        Move Camera Down
- L:            Lock Camera Movement
- F1:           Cycle through Viewing Mode
- F2:           Cycle through Debug information
- F3:           Cycle through Debug information color
- =:            Increase FOV
- -:            Decrease FOV
- ESC:          Exit the application

#### References:
- Inspiration: Graphics (2015-2016) & Advanced Graphics (2019-2020) courses at the Utrecht University
- OpenTK: https://github.com/opentk/opentk
- SimpleImageIO: https://github.com/pgrit/SimpleImageIO
- Triangle-Ray Intersection: https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
- Sphere-Ray Intersection: https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection
- AABB-Ray Intersection: https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-box-intersection
- BVH Binning: On fast Construction of SAH-based Bounding Volume Hierarchies - I. Wald
- Triangle Clipping: Efficient Triangle and Quadrilateral Clipping within Shaders - M. McGuire
- SBVH: Spatial Splits in Bounding Volume Hierarchies - M. Stich, H. Friedrich, A. Dietrich
- SBVH implementation: Parallel Spatial Splits in Bounding Volume Hierarchies - V. Fuetterling, C. Lojewski, F.-J Pfreundt and A. Ebert
- GGX: The Ellipsoid Normal Distribution Function - B. Walter, Z. Dong, S. Marschner and D. P. Greenberg
- GGX implementation: A Simpler and Exact Sampling Routine for the GGX Distribution of Visible Normals - E. Heitz
- SGGX: The SGGX microflake distribution - E. Heitz, J. Dupuy, C. Crassin and C. Dachsbacher
