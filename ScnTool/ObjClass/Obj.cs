using ObjParser.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ObjParser
{
    public class Obj
    {
        public List<Vertex> VertexList;
        public List<Face> FaceList;
        public List<TextureVertex> TextureList;
        public List<Normal> NormalList;

        public Extent Size { get; set; }

        public string UseMtl { get; set; }
        public string Mtl { get; set; }

        public Obj()
        {
            VertexList = new List<Vertex>();
            FaceList = new List<Face>();
            TextureList = new List<TextureVertex>();
            NormalList = new List<Normal>();
        }

        public void LoadObj(string path)
        {
            LoadObj(File.ReadAllLines(path));
        }

        public void LoadObj(Stream data)
        {
            using (var reader = new StreamReader(data))
            {
                LoadObj(reader.ReadToEnd().Split(Environment.NewLine.ToCharArray()));
            }
        }

        public void LoadObj(IEnumerable<string> data)
        {
            foreach (string line in data)
            {
                processLine(line);
            }

            updateSize();
        }

        public void WriteObjFile(string path, string[] headerStrings)
        {
            using (var outStream = File.OpenWrite(path))
            using (var writer = new StreamWriter(outStream))
            {
                // Write some header data
                WriteHeader(writer, headerStrings);

                if (!string.IsNullOrEmpty(Mtl))
                {
                    writer.WriteLine("mtllib " + Mtl);
                }

                VertexList.ForEach(v => writer.WriteLine(v));
                TextureList.ForEach(tv => writer.WriteLine(tv));
                string lastUseMtl = "";
                foreach (var face in FaceList)
                {
                    if (face.UseMtl != null && !face.UseMtl.Equals(lastUseMtl))
                    {
                        writer.WriteLine("usemtl " + face.UseMtl);
                        lastUseMtl = face.UseMtl;
                    }
                    writer.WriteLine(face);
                }
            }
        }

        private void WriteHeader(StreamWriter writer, string[] headerStrings)
        {
            if (headerStrings == null || headerStrings.Length == 0)
            {
                writer.WriteLine("# Generated by ObjParser");
                return;
            }

            foreach (string line in headerStrings)
            {
                writer.WriteLine("# " + line);
            }
        }

        private void updateSize()
        {
            // If there are no vertices then size should be 0.
            if (VertexList.Count == 0)
            {
                Size = new Extent
                {
                    XMax = 0,
                    XMin = 0,
                    YMax = 0,
                    YMin = 0,
                    ZMax = 0,
                    ZMin = 0
                };

                return;
            }

            Size = new Extent
            {
                XMax = VertexList.Max(v => v.X),
                XMin = VertexList.Min(v => v.X),
                YMax = VertexList.Max(v => v.Y),
                YMin = VertexList.Min(v => v.Y),
                ZMax = VertexList.Max(v => v.Z),
                ZMin = VertexList.Min(v => v.Z)
            };
        }

        private void processLine(string line)
        {
            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                switch (parts[0])
                {
                    case "usemtl":
                        UseMtl = parts[1];
                        break;
                    case "mtllib":
                        Mtl = parts[1];
                        break;
                    case "v":
                        var v = new Vertex();
                        v.LoadFromStringArray(parts);
                        VertexList.Add(v);
                        v.Index = VertexList.Count();
                        break;
                    case "f":
                        var f = new Face();
                        f.LoadFromStringArray(parts);
                        f.UseMtl = UseMtl;
                        FaceList.Add(f);
                        break;
                    case "vt":
                        var vt = new TextureVertex();
                        vt.LoadFromStringArray(parts);
                        TextureList.Add(vt);
                        vt.Index = TextureList.Count();
                        break;
                    case "vn":
                        var vn = new Normal();
                        vn.LoadFromStringArray(parts);
                        NormalList.Add(vn);
                        vn.Index = TextureList.Count();
                        break;
                }
            }
        }
    }
}