using NetsphereScnTool.Scene.Chunks;
using ObjParser;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace NetsphereScnTool.Utility
{
    public class Obj_Importer
    {
        public void ReplaceModel(string fileName, ModelChunk model, string textureName)
        {
            var VertexList = new List<Vector3>();
            var NormalList = new List<Vector3>();
            var UVList = new List<Vector2>();
            var FacesList = new List<Vector3>();

            var obj = new Obj();
            obj.LoadObj(fileName);

            if (obj == null)
                return;

            var _matrix = new Matrix4x4();
            Matrix4x4.Invert(model.Matrix, out _matrix);

            for (int index = 0; index < obj.VertexList.Count; index++)
                VertexList.Add(new Vector3(float.Parse(obj.VertexList[index].X.ToString()),
                                           float.Parse(obj.VertexList[index].Y.ToString()),
                                           float.Parse(obj.VertexList[index].Z.ToString())));

            for (int index = 0; index < obj.NormalList.Count; index++)
                NormalList.Add(new Vector3(float.Parse(obj.NormalList[index].X.ToString()),
                                           float.Parse(obj.NormalList[index].Y.ToString()),
                                           float.Parse(obj.NormalList[index].Z.ToString())));

            for (int index = 0; index < obj.TextureList.Count; index++)
                UVList.Add(new Vector2(float.Parse(obj.TextureList[index].X.ToString()), 0.0f -
                                       float.Parse(obj.TextureList[index].Y.ToString())));

            var vector_temp = new List<Vector3>();
            for (int index = 0; index < VertexList.Count; index++)
            {
                var temp = Vector3.Transform(VertexList[index], _matrix);
                vector_temp.Add(temp);
            }

            for (int index = 0; index < obj.FaceList.Count; index++)
            {
                string face = obj.FaceList[index].ToString();
                string[] face_array = face.Split(' ');

                string X = Convert.ToString(face_array[1]);
                string Y = Convert.ToString(face_array[2]);
                string Z = Convert.ToString(face_array[3]);

                string[] _X = X.Split('/');
                string[] _Y = Y.Split('/');
                string[] _Z = Z.Split('/');

                FacesList.Add(new Vector3(Convert.ToInt32(_X[0]) - 1,
                                          Convert.ToInt32(_Y[0]) - 1,
                                          Convert.ToInt32(_Z[0]) - 1));
            }

            var texture = new TextureEntry();
            texture.FaceCount = obj.FaceList.Count;
            texture.FaceCounter = 0;

            texture.FileName = string.IsNullOrEmpty(textureName) ? model.TextureData.Textures[0].FileName : textureName;
            texture.FileName2 = model.TextureData.Textures[0].FileName2;

            if (model.TextureData.Textures.Count != 0)
                model.TextureData.Textures[0] = texture;
            else
                model.TextureData.Textures.Add(texture);

            model.Mesh.Vertices = vector_temp;
            model.Mesh.Normals = NormalList;
            model.Mesh.UV = UVList;
            model.Mesh.Faces = FacesList;
        }
    }
}
