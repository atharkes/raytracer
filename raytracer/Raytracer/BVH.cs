using OpenTK;
using System;
using System.Collections.Generic;

namespace Raytracer {
    /// <summary> A node of a bounding volume hierarchy tree
    /// (Ordered traversal can be added)
    /// (Can be optimized using two arrays for primitives and BHV nodes, and using only a Left index and Count) </summary>
    class BVHNode {
        /// <summary> The left child node if it has one </summary>
        public BVHNode Left { get; private set; }
        /// <summary> The right child node if it has one </summary>
        public BVHNode Right { get; private set; }
        /// <summary> The primitives that are contained in this node or any of it's children </summary>
        public readonly List<Primitive> Primitives;
        /// <summary> The bounds of the axis alinged bounding box of this node </summary>
        public readonly List<Vector3> Bounds;
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

        /// <summary> Intersect this Node and it's children </summary>
        /// <param name="ray">The ray to calculate intersections for</param>
        /// <returns>A pair of the distance and the primitive that it intersects</returns>
        public Intersection IntersectTree(Ray ray) {
            if (!IntersectAABB(ray)) {
                return null;
            } else if (!Leaf) {
                Intersection intersectionLeft = Left.IntersectTree(ray);
                Intersection intersectionRight = Right.IntersectTree(ray);
                if (intersectionLeft == null) {
                    return intersectionRight;
                } else if (intersectionRight == null) {
                    return intersectionLeft;
                } else if (intersectionLeft.Distance < intersectionRight.Distance) {
                    return intersectionLeft;
                } else {
                    return intersectionRight;
                }
            } else {
                float intersectionDistance = float.MaxValue;
                Primitive intersectionPrimitive = null;
                foreach (Primitive primitive in Primitives) {
                    float distance = primitive.Intersect(ray);
                    if (distance > 0 && distance < intersectionDistance) {
                        intersectionPrimitive = primitive;
                        intersectionDistance = distance;
                    }
                }
                if (intersectionPrimitive == null) return null;
                else return new Intersection(ray, intersectionPrimitive, intersectionDistance);
            }
        }

        /// <summary> Intersect this Node and it's children </summary>
        /// <param name="ray">The ray to calculate intersections for</param>
        /// <returns>Whether there is an intersection</returns>
        public bool IntersectTreeBool(Ray ray) {
            bool intersectBool = IntersectAABB(ray);
            if (intersectBool && !Leaf) {
                bool intersectLeft = Left.IntersectTreeBool(ray);
                bool intersectRight = Right.IntersectTreeBool(ray);
                if (intersectLeft || intersectRight) {
                    return true;
                } else {
                    return false;
                }
            } else if (intersectBool && Leaf) {
                foreach (Primitive primitive in Primitives) {
                    if (primitive.IntersectBool(ray)) return true;
                }
                return false;
            } else {
                return false;
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

            if ((tmin > tymax) || (tymin > tmax)) return false;
            if (tymin > tmin) tmin = tymin;
            if (tymax < tmax) tmax = tymax;

            float tz1 = (Bounds[ray.Sign[2]].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            float tz2 = (Bounds[1 - ray.Sign[2]].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            float tzmin = Math.Min(tz1, tz2);
            float tzmax = Math.Max(tz1, tz2);

            tmin = Math.Max(tmin, Math.Min(tz1, tz2));
            tmax = Math.Min(tmax, Math.Max(tz1, tz2));

            if ((tmin > tzmax) || (tzmin > tmax)) return false;
            if (tzmin > tmin) tmin = tzmin;
            if (tzmax < tmax) tmax = tzmax;

            return tmin > 0 || tmax > 0;
        }

        /// <summary> Try split the Node into 2 smaller Nodes </summary>
        void Split() {
            if (Primitives.Count < 3) return;
            (List<Primitive> primitivesLeft, List<Primitive> primitivesRight) = CalculateSplit();
            if (primitivesLeft == null || primitivesRight == null) return;
            Left = new BVHNode(primitivesLeft);
            Right = new BVHNode(primitivesRight);
            Leaf = false;
        }

        /// <summary> Calculate Cheapest Split </summary>
        /// <returns>A pair with two sides of the split</returns>
        (List<Primitive> left, List<Primitive> right) CalculateSplit() { 
            float cost = CalculateSurfaceArea(Bounds) * Primitives.Count;
            (List<Primitive>, List<Primitive>) splitPrimitives = (null, null);
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
        /// <param name="primitive">The primitive to split on</param>
        /// <returns>A pair with primitives on both sides</returns>
        (List<Primitive> left, List<Primitive> right) SplitX(Primitive primitive) {
            float split = primitive.GetCenter().X;
            List<Primitive> primitivesLeft = new List<Primitive>();
            List<Primitive> primitivesRight = new List<Primitive>();
            foreach (Primitive primitiveCheck in Primitives) {
                if (primitiveCheck.GetCenter().X <= split) {
                    primitivesLeft.Add(primitiveCheck);
                } else {
                    primitivesRight.Add(primitiveCheck);
                }
            }
            return (primitivesLeft, primitivesRight);
        }

        /// <summary> Split on Y-axis </summary>
        /// <param name="primitive">The primitive to split on</param>
        /// <returns>A pair with primitives on both sides</returns>
        (List<Primitive> left, List<Primitive> right) SplitY(Primitive primitive) {
            float split = primitive.GetCenter().Y;
            List<Primitive> primitivesLeft = new List<Primitive>();
            List<Primitive> primitivesRight = new List<Primitive>();
            foreach (Primitive primitiveCheck in Primitives) {
                if (primitiveCheck.GetCenter().Y <= split) {
                    primitivesLeft.Add(primitiveCheck);
                } else {
                    primitivesRight.Add(primitiveCheck);
                }
            }
            return (primitivesLeft, primitivesRight);
        }

        /// <summary> Split on Z-axis </summary>
        /// <param name="primitive">The primitive to split on</param>
        /// <returns>A pair with primitives on both sides</returns>
        (List<Primitive> left, List<Primitive> right) SplitZ(Primitive primitive) {
            float split = primitive.GetCenter().Z;
            List<Primitive> primitivesLeft = new List<Primitive>();
            List<Primitive> primitivesRight = new List<Primitive>();
            foreach (Primitive primitiveCheck in Primitives) {
                if (primitiveCheck.GetCenter().Z <= split) {
                    primitivesLeft.Add(primitiveCheck);
                } else {
                    primitivesRight.Add(primitiveCheck);
                }
            }
            return (primitivesLeft, primitivesRight);
        }

        /// <summary> Calculate the cost of a split </summary>
        /// <param name="split">The split to calculate the cost for</param>
        /// <returns>The cost of the split</returns>
        float CalculateCost((List<Primitive>, List<Primitive>) split) {
            List<Vector3> boundsLeft = CalculateBounds(split.Item1);
            float surfaceArea1 = CalculateSurfaceArea(boundsLeft);
            List<Vector3> boundsRight = CalculateBounds(split.Item2);
            float surfaceArea2 = CalculateSurfaceArea(boundsRight);
            return surfaceArea1 * split.Item1.Count + surfaceArea2 * split.Item2.Count;
        }

        /// <summary> Calculate the bounds of a List of Primitives </summary>
        /// <param name="primitives">The primitives the calculate the bounds for</param>
        /// <returns>The bounds of the primitives</returns>
        List<Vector3> CalculateBounds(List<Primitive> primitives) {
            List<Vector3> bounds = new List<Vector3>(2);
            Vector3 boundMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 boundMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (Primitive primitive in primitives) {
                List<Vector3> boundPrimitives = primitive.GetBounds();
                boundMin = Vector3.ComponentMin(boundPrimitives[0], boundMin);
                boundMax = Vector3.ComponentMax(boundPrimitives[1], boundMax);
            }
            bounds.Add(boundMin);
            bounds.Add(boundMax);
            return bounds;
        }

        /// <summary> Calculate the surface area for some bounds </summary>
        /// <param name="bounds">The bounds to calculate the surface area for</param>
        /// <returns>The surface area of the bounds</returns>
        float CalculateSurfaceArea(List<Vector3> bounds) {
            float a = bounds[1].X - bounds[0].X;
            float b = bounds[1].Y - bounds[0].Y;
            float c = bounds[1].Z - bounds[0].Z;
            float surfaceArea = 2 * (a * b + b * c + a * c);
            return surfaceArea;
        }
    }
}