using OpenTK;
using System;
using System.Collections.Generic;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing.AccelerationStructures {
    /// <summary> A node of a bounding volume hierarchy tree
    /// TODO:
    /// - Refitting (enable animation/movement)
    /// - Binning SAH while splitting (increase splitting performance)
    /// - Geometricly ordered traversal (increase performance)
    /// - Top-level BHV's for static and non-static parts
    /// - Use two arrays for primitives and BHV nodes, and using only a Left index and Count (decrease storage and increase performance) </summary>
    class BVHNode : IAccelerationStructure {
        /// <summary> The left child node if it has one </summary>
        public BVHNode Left { get; private set; }
        /// <summary> The right child node if it has one </summary>
        public BVHNode Right { get; private set; }
        /// <summary> The primitives that are contained in this node or any of it's children </summary>
        public readonly List<Primitive> Primitives;
        /// <summary> The bounds of the axis alinged bounding box of this node </summary>
        public readonly Vector3[] Bounds = new Vector3[2];
        /// <summary> Whether this node is a leaf or not </summary>
        public bool Leaf { get; private set; }

        /// <summary> Construct a bounding volume hierarchy tree, it will split into smaller nodes if needed </summary>
        /// <param name="primitives">The primitives in the tree</param>
        public BVHNode(List<Primitive> primitives) {
            Primitives = primitives;
            Bounds = CalculateBounds(primitives);
            Leaf = true;
            Split();
        }

        /// <summary> Intersect and traverse the BVH with a ray </summary>
        /// <param name="ray">The ray to intersect the BVH with</param>
        /// <returns>The intersection in the BVH</returns>
        public Intersection Intersect(Ray ray) {
            if (!IntersectAABB(ray)) {
                return null;
            } else if (Leaf) {
                return IntersectPrimitives(ray);
            } else {
                return IntersectChildren(ray); 
            }
        }

        /// <summary> Intersect and traverse the BVH with a ray </summary>
        /// <param name="ray">The ray to intersect the BVH with</param>
        /// <returns>Whether there is an intersection with the BVH and the ray</returns>
        public bool IntersectBool(Ray ray) {
            if (!IntersectAABB(ray)) {
                return false;
            } else if (Leaf) {
                foreach (Primitive primitive in Primitives) {
                    if (primitive.IntersectBool(ray)) return true;
                }
                return false;
            } else {
                return Left.IntersectBool(ray) || Right.IntersectBool(ray);
            }
        }

        /// <summary> Intersect the axis aligned bounding box of this Node (Amy Williams's An Efficient and Robust Ray–Box Intersection Algorithm) </summary>
        /// <param name="ray">The ray to calculate intersection for</param>
        /// <returns>Whether the ray intersects the bounding box</returns>
        public bool IntersectAABB(Ray ray) {
            float tmin = (Bounds[ray.Sign[0]].X - ray.Origin.X) * ray.DirectionInverted.X;
            float tmax = (Bounds[1 - ray.Sign[0]].X - ray.Origin.X) * ray.DirectionInverted.X;

            float tymin = (Bounds[ray.Sign[1]].Y - ray.Origin.Y) * ray.DirectionInverted.Y;
            float tymax = (Bounds[1 - ray.Sign[1]].Y - ray.Origin.Y) * ray.DirectionInverted.Y;
            if ((tmin > tymax) || (tmax < tymin)) return false;
            tmin = Math.Max(tmin, tymin);
            tmax = Math.Min(tmax, tymax);

            float tzmin = (Bounds[ray.Sign[2]].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            float tzmax = (Bounds[1 - ray.Sign[2]].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            if ((tmin > tzmax) || (tmax < tzmin)) return false;
            tmin = Math.Max(tmin, tzmin);
            tmax = Math.Min(tmax, tzmax);

            return tmin > 0 || tmax > 0;
        }

        

        /// <summary> Intersect the children of this BHV node </summary>
        /// <param name="ray">The ray to intersect the children with</param>
        /// <returns>The intersection in the children if there is any</returns>
        Intersection IntersectChildren(Ray ray) {
            Intersection intersectionLeft = Left.Intersect(ray);
            Intersection intersectionRight = Right.Intersect(ray);
            if (intersectionLeft == null) {
                return intersectionRight;
            } else if (intersectionRight == null) {
                return intersectionLeft;
            } else if (intersectionLeft.Distance < intersectionRight.Distance) {
                return intersectionLeft;
            } else {
                return intersectionRight;
            }
        }

        /// <summary> Intersect the primitives of this BHV node </summary>
        /// <param name="ray">The ray to intersect the primitives with</param>
        /// <returns>The intersection if there is any</returns>
        Intersection IntersectPrimitives(Ray ray) {
            float intersectionDistance = ray.Length;
            Primitive intersectionPrimitive = null;
            foreach (Primitive primitive in Primitives) {
                float distance = primitive.Intersect(ray);
                if (0 < distance && distance < intersectionDistance) {
                    intersectionPrimitive = primitive;
                    intersectionDistance = distance;
                }
            }
            if (intersectionPrimitive == null) return null;
            else return new Intersection(ray, intersectionPrimitive, intersectionDistance);
        }

        /// <summary> Try split the BVH Node into 2 smaller Nodes </summary>
        void Split() {
            (List<Primitive> left, List<Primitive> right)? split = CalculateSplit();
            if (!split.HasValue) return;
            Left = new BVHNode(split.Value.left);
            Right = new BVHNode(split.Value.right);
            Leaf = false;
        }

        /// <summary> Calculate Cheapest Split </summary>
        /// <returns>A pair with two sides of the split</returns>
        (List<Primitive> left, List<Primitive> right)? CalculateSplit() { 
            float cost = SurfaceArea() * Primitives.Count;
            (List<Primitive>, List<Primitive>)? splitPrimitives = null;
            foreach (Primitive primitive in Primitives) {
                List<(List<Primitive>, List<Primitive>)> splits = new List<(List<Primitive>, List<Primitive>)>(3)
                {
                    SplitX(primitive),
                    SplitY(primitive),
                    SplitZ(primitive)
                };

                foreach ((List<Primitive>, List<Primitive>) split in splits) {
                    float splitCost = CalculateCost(split);
                    if (splitCost < cost) {
                        cost = splitCost;
                        splitPrimitives = split;
                    }
                }
            }
            return splitPrimitives;
        }

        /// <summary> Split on X-axis </summary>
        /// <param name="splitPrimitive">The primitive to split on</param>
        /// <returns>A pair with primitives on both sides</returns>
        (List<Primitive> left, List<Primitive> right) SplitX(Primitive splitPrimitive) {
            float split = splitPrimitive.GetCenter().X;
            List<Primitive> primitivesLeft = new List<Primitive>();
            List<Primitive> primitivesRight = new List<Primitive>();
            foreach (Primitive primitive in Primitives) {
                if (primitive.GetCenter().X <= split) {
                    primitivesLeft.Add(primitive);
                } else {
                    primitivesRight.Add(primitive);
                }
            }
            return (primitivesLeft, primitivesRight);
        }

        /// <summary> Split on Y-axis </summary>
        /// <param name="splitPrimitive">The primitive to split on</param>
        /// <returns>A pair with primitives on both sides</returns>
        (List<Primitive> left, List<Primitive> right) SplitY(Primitive splitPrimitive) {
            float split = splitPrimitive.GetCenter().Y;
            List<Primitive> primitivesLeft = new List<Primitive>();
            List<Primitive> primitivesRight = new List<Primitive>();
            foreach (Primitive primitive in Primitives) {
                if (primitive.GetCenter().Y <= split) {
                    primitivesLeft.Add(primitive);
                } else {
                    primitivesRight.Add(primitive);
                }
            }
            return (primitivesLeft, primitivesRight);
        }

        /// <summary> Split on Z-axis </summary>
        /// <param name="splitPrimitive">The primitive to split on</param>
        /// <returns>A pair with primitives on both sides</returns>
        (List<Primitive> left, List<Primitive> right) SplitZ(Primitive splitPrimitive) {
            float split = splitPrimitive.GetCenter().Z;
            List<Primitive> primitivesLeft = new List<Primitive>();
            List<Primitive> primitivesRight = new List<Primitive>();
            foreach (Primitive primitive in Primitives) {
                if (primitive.GetCenter().Z <= split) {
                    primitivesLeft.Add(primitive);
                } else {
                    primitivesRight.Add(primitive);
                }
            }
            return (primitivesLeft, primitivesRight);
        }

        /// <summary> Calculate the cost of a split. Uses the Surface Area Heuristic </summary>
        /// <param name="split">The split to calculate the cost for</param>
        /// <returns>The cost of the split</returns>
        float CalculateCost((List<Primitive> left, List<Primitive> right) split) {
            Vector3[] boundsLeft = CalculateBounds(split.left);
            float surfaceArea1 = CalculateSurfaceArea(boundsLeft);
            Vector3[] boundsRight = CalculateBounds(split.right);
            float surfaceArea2 = CalculateSurfaceArea(boundsRight);
            return surfaceArea1 * split.left.Count + surfaceArea2 * split.right.Count;
        }

        /// <summary> Calculate the bounds of a List of Primitives </summary>
        /// <param name="primitives">The primitives the calculate the bounds for</param>
        /// <returns>The bounds of the primitives</returns>
        Vector3[] CalculateBounds(List<Primitive> primitives) {
            Vector3 boundMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 boundMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (Primitive primitive in primitives) {
                (Vector3 primitiveMin, Vector3 primitiveMax) = primitive.GetBounds();
                boundMin = Vector3.ComponentMin(primitiveMin, boundMin);
                boundMax = Vector3.ComponentMax(primitiveMax, boundMax);
            }
            return new Vector3[] { boundMin, boundMax };
        }

        /// <summary> Calculate the surface area of the AABB of this BVH node </summary>
        /// <returns>The surface area of the AABB</returns>
        public float SurfaceArea() {
            return CalculateSurfaceArea(Bounds);
        }

        /// <summary> Calculate the surface area for some bounds </summary>
        /// <param name="bounds">The bounds to calculate the surface area for</param>
        /// <returns>The surface area of the bounds</returns>
        public static float CalculateSurfaceArea(Vector3[] bounds) {
            float a = bounds[1].X - bounds[0].X;
            float b = bounds[1].Y - bounds[0].Y;
            float c = bounds[1].Z - bounds[0].Z;
            return 2 * (a * b + b * c + a * c);
        }

        /// <summary> Get the distance from a point to the AABB </summary>
        /// <param name="point">To point to get the distance from the AABB to</param>
        /// <returns>The distance from the point to the AABB</returns>
        public float DistanceSquared(Vector3 point) {
            float dx = Math.Max(Bounds[0].X - point.X, point.X - Bounds[1].X);
            float dy = Math.Max(Bounds[0].Y - point.Y, point.Y - Bounds[1].Y);
            float dz = Math.Max(Bounds[0].Z - point.Z, point.Z - Bounds[1].Z);
            return dx * dx + dy * dy + dz * dz;
        }
    }
}