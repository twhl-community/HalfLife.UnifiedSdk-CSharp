using JeremyAnsel.Media.WavefrontObj;
using Sledge.Formats.Bsp;
using Sledge.Formats.Bsp.Objects;

namespace HalfLife.UnifiedSdk.Bsp2Obj
{
    internal sealed class BspToObjConverter
    {
        private readonly BspFile _bspFile;

        private readonly ObjFile _objFile = new();

        private BspToObjConverter(BspFile bspFile)
        {
            _bspFile = bspFile;
        }

        public static ObjFile Convert(BspFile bspFile)
        {
            BspToObjConverter converter = new(bspFile);

            return converter.ConvertCore();
        }

        private ObjFile ConvertCore()
        {
            // Sledge checks for this so make sure to add it at the start. The exact format matters!
            _objFile.HeaderText = " Scale: 1";

            foreach (var model in _bspFile.Models)
            {
                ConvertModel(model);
            }

            return _objFile;
        }

        private void ConvertModel(Model model)
        {
            foreach (var face in Enumerable
                .Range(model.FirstFace, model.NumFaces)
                .Select(i => _bspFile.Faces[i]))
            {
                ObjFace objFace = new();

                foreach (var vertex in GetEdgeVertices(face))
                {
                    objFace.Vertices.Add(vertex);
                }

                _objFile.Faces.Add(objFace);
            }
        }

        private IEnumerable<ObjTriplet> GetEdgeVertices(Face face)
        {
            // Vertices need to be reversed to match the OBJ format.
            foreach (var surfEdgeIndex in Enumerable
                .Range(face.FirstEdge, face.NumEdges)
                .Reverse())
            {
                var edgeIndex = _bspFile.Surfedges[surfEdgeIndex];
                var edge = _bspFile.Edges[Math.Abs(edgeIndex)];
                var vertex = _bspFile.Vertices[edgeIndex > 0 ? edge.Start : edge.End];

                var objVertex = new ObjVertex(vertex.X, vertex.Y, vertex.Z);

                var index = _objFile.Vertices.IndexOf(objVertex);

                if (index == -1)
                {
                    _objFile.Vertices.Add(objVertex);
                    index = _objFile.Vertices.Count - 1;
                }

                // Indices are 1-based.
                yield return new ObjTriplet(index + 1, 0, 0);
            }
        }
    }
}
