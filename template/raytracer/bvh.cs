using OpenTK;
using System;
using System.Collections.Generic;

namespace raytracer {
    class BVHNode {
        BVHNode left, right;
        List<Primitive> primitives;
        readonly List<Vector3> bounds;
        bool leaf;

        /// <summary> Construct a bounding volume hierarchy tree, it will split into smaller nodes if needed </summary>
        /// <param name="primitives">The primitives in the tree</param>
        public BVHNode(List<Primitive> primitives) {
            this.primitives = primitives;
            bounds = CalculateBounds(primitives);
            leaf = true;
            Split();
        }

        /// <summary> Intersect this Node and it's children </summary>
        /// <param name="ray">The ray to calculate intersections for</param>
        /// <returns>A tuple of the distance and the primitive that it intersects</returns>
        public Tuple<float, Primitive> IntersectTree(Ray ray) {
            if (!IntersectAABB(ray)) {
                return new Tuple<float, Primitive>(-1f, null);
            } else if (!leaf) {
                Tuple<float, Primitive> intersectionLeft = left.IntersectTree(ray);
                Tuple<float, Primitive> intersectionRight = right.IntersectTree(ray);
                if (intersectionRight.Item2 == null) {
                    return intersectionLeft;
                } else if (intersectionLeft.Item2 == null) {
                    return intersectionRight;
                } else if (intersectionLeft.Item1 < intersectionRight.Item1) {
                    return intersectionLeft;
                } else {
                    return intersectionRight;
                }
            } else {
                Tuple<float, Primitive> intersection = new Tuple<float, Primitive>(ray.Length, null);
                foreach (Primitive primitive in primitives) {
                    float intersectDistance = primitive.Intersect(ray);
                    if (intersectDistance > 0 && intersectDistance < intersection.Item1)
                        intersection = new Tuple<float, Primitive>(intersectDistance, primitive);
                }
                if (intersection.Item1 == ray.Length)
                    intersection = new Tuple<float, Primitive>(-1f, null);
                return intersection;
            }
        }

        /// <summary> Intersect this Node and it's children </summary>
        /// <param name="ray">The ray to calculate intersections for</param>
        /// <returns>Whether there is an intersection</returns>
        public bool IntersectTreeBool(Ray ray) {
            bool intersectBool = IntersectAABB(ray);
            if (intersectBool && !leaf) {
                bool intersectLeft = left.IntersectTreeBool(ray);
                bool intersectRight = right.IntersectTreeBool(ray);
                if (intersectLeft || intersectRight) {
                    return true;
                } else {
                    return false;
                }
            } else if (intersectBool && leaf) {
                foreach (Primitive primitive in primitives) {
                    if (primitive.IntersectBool(ray)) return true;
                }
                return false;
            } else {
                return false;
            }   
        }

        /// <summary> Intersect the axis aligned bounding box of this Node </summary>
        /// <param name="ray">The ray to calculate intersection for</param>
        /// <returns>Whether the ray intersects the bounding box</returns>
        public bool IntersectAABB(Ray ray) {
            float tx1 = (bounds[0].X - ray.Origin.X) * ray.DirectionInverted.X;
            float tx2 = (bounds[1].X - ray.Origin.X) * ray.DirectionInverted.X;
            float tmin = Math.Min(tx1, tx2);
            float tmax = Math.Max(tx1, tx2);

            float ty1 = (bounds[0].Y - ray.Origin.Y) * ray.DirectionInverted.Y;
            float ty2 = (bounds[1].Y - ray.Origin.Y) * ray.DirectionInverted.Y;
            float tymin = Math.Min(ty1, ty2);
            float tymax = Math.Max(ty1, ty2);

            if ((tmin > tymax) || (tymin > tmax)) return false;
            if (tymin > tmin) tmin = tymin;
            if (tymax < tmax) tmax = tymax;

            float tz1 = (bounds[0].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            float tz2 = (bounds[1].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            float tzmin = Math.Min(tz1, tz2);
            float tzmax = Math.Max(tz1, tz2);

            tmin = Math.Max(tmin, Math.Min(tz1, tz2));
            tmax = Math.Min(tmax, Math.Max(tz1, tz2));

            if ((tmin > tzmax) || (tzmin > tmax)) return false;
            if (tzmin > tmin) tmin = tzmin;
            if (tzmax < tmax) tmax = tzmax;

            return true;
        }

        /// <summary> Try split the Node into 2 smaller Nodes </summary>
        void Split() {
            if (primitives.Count < 3) return;
            Tuple<List<Primitive>, List<Primitive>> splitPrimitives = CalculateSplit();
            if (splitPrimitives == null) return;
            List<Primitive> primitivesLeft = splitPrimitives.Item1;
            List<Primitive> primitivesRight = splitPrimitives.Item2;
            left = new BVHNode(primitivesLeft);
            right = new BVHNode(primitivesRight);
            leaf = false;
        }

        /// <summary> Calculate Cheapest Split </summary>
        /// <returns>Tuple with two sides of the split</returns>
        Tuple<List<Primitive>, List<Primitive>> CalculateSplit() {
            float costBegin = CalculateSurfaceArea(bounds) * primitives.Count;
            float cost = costBegin;
            Tuple<List<Primitive>, List<Primitive>> splitPrimitives = null;
            foreach (Primitive primitive in primitives) {
                List<Tuple<List<Primitive>, List<Primitive>>> splits = new List<Tuple<List<Primitive>, List<Primitive>>>(3)
                {
                    SplitX(primitive),
                    SplitY(primitive),
                    SplitZ(primitive)
                };

                foreach (Tuple<List<Primitive>, List<Primitive>> split in splits) {
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
        /// <returns>Tuple with primitives on both sides</returns>
        Tuple<List<Primitive>, List<Primitive>> SplitX(Primitive primitive) {
            float split = primitive.GetCenter().X;
            List<Primitive> primitivesLeft = new List<Primitive>();
            List<Primitive> primitivesRight = new List<Primitive>();
            foreach (Primitive primitiveCheck in primitives) {
                if (primitiveCheck.GetCenter().X <= split) {
                    primitivesLeft.Add(primitiveCheck);
                } else {
                    primitivesRight.Add(primitiveCheck);
                }
            }
            return new Tuple<List<Primitive>, List<Primitive>>(primitivesLeft, primitivesRight);
        }

        /// <summary> Split on Y-axis </summary>
        /// <param name="primitive">The primitive to split on</param>
        /// <returns>Tuple with primitives on both sides</returns>
        Tuple<List<Primitive>, List<Primitive>> SplitY(Primitive primitive) {
            float split = primitive.GetCenter().Y;
            List<Primitive> primitivesLeft = new List<Primitive>();
            List<Primitive> primitivesRight = new List<Primitive>();
            foreach (Primitive primitiveCheck in primitives) {
                if (primitiveCheck.GetCenter().Y <= split) {
                    primitivesLeft.Add(primitiveCheck);
                } else {
                    primitivesRight.Add(primitiveCheck);
                }
            }
            return new Tuple<List<Primitive>, List<Primitive>>(primitivesLeft, primitivesRight);
        }

        /// <summary> Split on Z-axis </summary>
        /// <param name="primitive">The primitive to split on</param>
        /// <returns>Tuple with primitives on both sides</returns>
        Tuple<List<Primitive>, List<Primitive>> SplitZ(Primitive primitive) {
            float split = primitive.GetCenter().Z;
            List<Primitive> primitivesLeft = new List<Primitive>();
            List<Primitive> primitivesRight = new List<Primitive>();
            foreach (Primitive primitiveCheck in primitives) {
                if (primitiveCheck.GetCenter().Z <= split) {
                    primitivesLeft.Add(primitiveCheck);
                } else {
                    primitivesRight.Add(primitiveCheck);
                }
            }
            return new Tuple<List<Primitive>, List<Primitive>>(primitivesLeft, primitivesRight);
        }

        /// <summary> Calculate the cost of a split </summary>
        /// <param name="split">The split to calculate the cost for</param>
        /// <returns>The cost of the split</returns>
        float CalculateCost(Tuple<List<Primitive>, List<Primitive>> split) {
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