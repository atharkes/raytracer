using OpenTK;
using System;
using System.Collections.Generic;

namespace template {
    class BVHNode {
        BVHNode left, right;
        readonly List<Vector3> bounds;
        bool leaf;
        List<Primitive> primitives;

        public BVHNode(List<Primitive> primitives) {
            this.primitives = primitives;
            bounds = CalculateBounds(primitives);
            leaf = true;
            Split();
        }

        // Intersect this Node and it's children
        public Tuple<float, Primitive> IntersectTree(Ray ray) {
            bool intersectBool = IntersectAABB(ray);
            if (intersectBool && !leaf) {
                Tuple<float, Primitive> intersectionLeft = left.IntersectTree(ray);
                Tuple<float, Primitive> intersectionRight = right.IntersectTree(ray);
                if (intersectionRight.Item2 == null)
                    return intersectionLeft;
                else if (intersectionLeft.Item2 == null)
                    return intersectionRight;
                else if (intersectionLeft.Item1 < intersectionRight.Item1)
                    return intersectionLeft;
                else
                    return intersectionRight;
            } else if (intersectBool && leaf) {
                Tuple<float, Primitive> intersection = new Tuple<float, Primitive>(ray.Length, null);
                foreach (Primitive primitive in primitives) {
                    float intersectDistance = primitive.Intersect(ray);
                    if (intersectDistance > 0 && intersectDistance < intersection.Item1)
                        intersection = new Tuple<float, Primitive>(intersectDistance, primitive);
                }
                if (intersection.Item1 == ray.Length)
                    intersection = new Tuple<float, Primitive>(-1f, null);
                return intersection;
            } else {
                return new Tuple<float, Primitive>(-1f, null);
            }
        }

        // Intersect this Node and it's children; returns bool
        public bool IntersectTreeBool(Ray ray) {
            bool intersectBool = IntersectAABB(ray);
            if (intersectBool && !leaf) {
                bool intersectLeft = left.IntersectTreeBool(ray);
                bool intersectRight = right.IntersectTreeBool(ray);
                if (intersectLeft || intersectRight)
                    return true;
                else
                    return false;
            } else if (intersectBool && leaf) {
                foreach (Primitive primitive in primitives) {
                    if (primitive.IntersectBool(ray))
                        return true;
                }
                return false;
            } else
                return false;
        }

        // Intersect the AABB of the Node
        public bool IntersectAABB(Ray ray) {
            float tx1 = (bounds[0].X - ray.Origin.X) * ray.DirectionInverted.X;
            float tx2 = (bounds[1].X - ray.Origin.X) * ray.DirectionInverted.X;
            float tmin = Math.Min(tx1, tx2);
            float tmax = Math.Max(tx1, tx2);

            float ty1 = (bounds[0].Y - ray.Origin.Y) * ray.DirectionInverted.Y;
            float ty2 = (bounds[1].Y - ray.Origin.Y) * ray.DirectionInverted.Y;
            float tymin = Math.Min(ty1, ty2);
            float tymax = Math.Max(ty1, ty2);

            if ((tmin > tymax) || (tymin > tmax))
                return false;
            if (tymin > tmin)
                tmin = tymin;
            if (tymax < tmax)
                tmax = tymax;

            float tz1 = (bounds[0].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            float tz2 = (bounds[1].Z - ray.Origin.Z) * ray.DirectionInverted.Z;
            float tzmin = Math.Min(tz1, tz2);
            float tzmax = Math.Max(tz1, tz2);

            tmin = Math.Max(tmin, Math.Min(tz1, tz2));
            tmax = Math.Min(tmax, Math.Max(tz1, tz2));

            if ((tmin > tzmax) || (tzmin > tmax))
                return false;
            if (tzmin > tmin)
                tmin = tzmin;
            if (tzmax < tmax)
                tmax = tzmax;

            return true;
        }

        // Try split the Node into 2 smaller Nodes
        void Split() {
            if (primitives.Count < 3)
                return;
            Tuple<List<Primitive>, List<Primitive>> splitPrimitives = CalculateSplit();
            if (splitPrimitives == null)
                return;
            List<Primitive> primitivesLeft = splitPrimitives.Item1;
            List<Primitive> primitivesRight = splitPrimitives.Item2;
            left = new BVHNode(primitivesLeft);
            right = new BVHNode(primitivesRight);
        }

        // Calculate Cheapest Split
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
            leaf = false;
            return splitPrimitives;
        }

        // Split on X-axis
        Tuple<List<Primitive>, List<Primitive>> SplitX(Primitive primitive) {
            float split = primitive.GetCenter().X;
            List<Primitive> primitivesLeft = new List<Primitive>();
            List<Primitive> primitivesRight = new List<Primitive>();
            foreach (Primitive primitiveCheck in primitives) {
                if (primitiveCheck.GetCenter().X <= split)
                    primitivesLeft.Add(primitiveCheck);
                else
                    primitivesRight.Add(primitiveCheck);
            }
            return new Tuple<List<Primitive>, List<Primitive>>(primitivesLeft, primitivesRight);
        }

        // Split on Y-axis
        Tuple<List<Primitive>, List<Primitive>> SplitY(Primitive primitive) {
            float split = primitive.GetCenter().Y;
            List<Primitive> primitivesLeft = new List<Primitive>();
            List<Primitive> primitivesRight = new List<Primitive>();
            foreach (Primitive primitiveCheck in primitives) {
                if (primitiveCheck.GetCenter().Y <= split)
                    primitivesLeft.Add(primitiveCheck);
                else
                    primitivesRight.Add(primitiveCheck);
            }
            return new Tuple<List<Primitive>, List<Primitive>>(primitivesLeft, primitivesRight);
        }

        // Split on Z-axis
        Tuple<List<Primitive>, List<Primitive>> SplitZ(Primitive primitive) {
            float split = primitive.GetCenter().Z;
            List<Primitive> primitivesLeft = new List<Primitive>();
            List<Primitive> primitivesRight = new List<Primitive>();
            foreach (Primitive primitiveCheck in primitives) {
                if (primitiveCheck.GetCenter().Z <= split)
                    primitivesLeft.Add(primitiveCheck);
                else
                    primitivesRight.Add(primitiveCheck);
            }
            return new Tuple<List<Primitive>, List<Primitive>>(primitivesLeft, primitivesRight);
        }

        // Calculate Cost of a Split
        float CalculateCost(Tuple<List<Primitive>, List<Primitive>> split) {
            List<Vector3> boundsLeft = CalculateBounds(split.Item1);
            float surfaceArea1 = CalculateSurfaceArea(boundsLeft);
            List<Vector3> boundsRight = CalculateBounds(split.Item2);
            float surfaceArea2 = CalculateSurfaceArea(boundsRight);
            return surfaceArea1 * split.Item1.Count + surfaceArea2 * split.Item2.Count;
        }

        // Calculate Bounds of a List of Primitives
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

        // Calculate Surface Area from Bounds
        float CalculateSurfaceArea(List<Vector3> bounds) {
            float a = bounds[1].X - bounds[0].X;
            float b = bounds[1].Y - bounds[0].Y;
            float c = bounds[1].Z - bounds[0].Z;
            float surfaceArea = 2 * (a * b + b * c + a * c);
            return surfaceArea;
        }
    }
}