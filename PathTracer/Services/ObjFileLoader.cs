using ObjParser;
using ObjParser.Types;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Services;

public class ObjFileLoader {
    /// <summary> Load a model, uniformly scaled to <paramref name="height"/> and standing with the center of its base on <paramref name="position"/> </summary>
    /// <param name="path">The path of the .obj file</param>
    /// <param name="position">Where the center of the bottom of the model is placed</param>
    /// <param name="height">The height of the model after scaling</param>
    /// <returns>An <see cref="Aggregate"/> with a <see cref="Primitive"/> per face</returns>
    public static Aggregate Load(string path, Position3 position, float height) {
        var obj = new ObjModel();
        obj.Load(path);

        var vertices = obj.Vertices.Select(ToPos3).ToArray();
        var min = vertices.Aggregate(Position3.PositiveInfinity, Position3.ComponentMin);
        var max = vertices.Aggregate(Position3.NegativeInfinity, Position3.ComponentMax);
        var scale = height / (float)(max.Y - min.Y);
        var baseCenter = new Position3((min.X + max.X) / 2, min.Y, (min.Z + max.Z) / 2);
        Position3 Place(int index) => position + (vertices[index - 1] - baseCenter) * scale;

        var triangles = obj.Faces.Select(face => new Triangle(Place(face.VertexIndexList[0]), Place(face.VertexIndexList[1]), Place(face.VertexIndexList[2])));

        var primitives = triangles.Select(triangle => new Primitive(triangle, Pathtracing.SceneDescription.Materials.Material.Diffuse(RGBColors.Red)));

        return [.. primitives];
    }

    private static Position3 ToPos3(Vertex vertex) => new((float)vertex.X, (float)vertex.Y, (float)vertex.Z);
}
