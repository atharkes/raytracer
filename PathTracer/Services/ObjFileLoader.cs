using ObjParser;
using ObjParser.Types;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Services;

public class ObjFileLoader {
    public static Aggregate Load(string path) {
        var obj = new ObjModel();
        obj.Load(path);

        var triangles = obj.Faces.Select(face => {
            var v1 = ToPos3(obj.Vertices[face.VertexIndexList[0] - 1]);
            var v2 = ToPos3(obj.Vertices[face.VertexIndexList[1] - 1]);
            var v3 = ToPos3(obj.Vertices[face.VertexIndexList[2] - 1]);

            return new Triangle(v1, v2, v3);
        });

        var primitives = triangles.Select(triangle => new Primitive(triangle, Pathtracing.SceneDescription.Materials.Material.Diffuse(RGBColors.Red)));

        return [.. primitives];
    }

    private static Position3 ToPos3(Vertex vertex) => new((float)vertex.X, (float)vertex.Y, (float)vertex.Z);
}
