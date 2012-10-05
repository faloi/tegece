using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL
{
    public class AdaptativeHeightmap : IRenderObject
    {
        private TgcFrustum frustum;
        private List<Triangle> triangleList;
        private List<int> indicesList;
        private VertexBuffer vbTerrain;
        private IndexBuffer ibTerrain;
        private Texture terrainTexture;
        private int totalVertices;
        private string lastHeightmapPath;

        public int[,] HeightmapData { get; private set; }
        public bool Enabled { get; set; }
        public Vector3 Position { get; private set; }
        public Vector3 Center { //centro de la malla
            get { return Position + new Vector3(HeightmapData.GetLength(0) * ScaleXZ / 2, Position.Y, HeightmapData.GetLength(0) * ScaleXZ / 2); }
        }
        public float ScaleXZ { get; private set; }
        public float ScaleY { get; private set; }
        public float Threshold { get; set; }

        public AdaptativeHeightmap(float initialScaleXZ, float initialScaleY, float initialThreshold) {
            this.Enabled = true;
            this.AlphaBlendEnable = false;
            this.frustum = new TgcFrustum();

            this.Threshold = initialThreshold;
            this.ScaleY = initialScaleY;
            this.ScaleXZ = initialScaleXZ;
        }

        public void loadHeightmap(string heightmapPath, Vector3 center) {
            var d3dDevice = GuiController.Instance.D3dDevice;

            //Dispose de vb y ib si había
            if (vbTerrain != null && !vbTerrain.Disposed) {
                vbTerrain.Dispose();
            }
            if (ibTerrain != null && !ibTerrain.Disposed) {
                ibTerrain.Dispose();
            }

            //Cargar heightmap
            lastHeightmapPath = heightmapPath;
            HeightmapData = loadHeightmapValues(d3dDevice, heightmapPath);
            totalVertices = 2 * 3 * (HeightmapData.GetLength(0) - 1) * (HeightmapData.GetLength(1) - 1); //por cada puntito tengo dos triángulos de 3 vértices
            int width = HeightmapData.GetLength(0);
            int length = HeightmapData.GetLength(1);

            //Convertir de centro a esquina:
            this.Position = center - new Vector3(width * ScaleXZ / 2, Position.Y, length * ScaleXZ / 2);

            //Crear vértices
            CustomVertex.PositionNormalTextured[] terrainVertices = createTerrainVertices(totalVertices);

            //Bajar vértices al VertexBuffer
            this.vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), totalVertices, d3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);
            vbTerrain.SetData(terrainVertices, 0, LockFlags.None);

            //Crear los dos triángulos root, el piso
            float terrainSize = width - 1;
            Triangle leftTriangle = new Triangle(null, new Vector2(0, 0), new Vector2(terrainSize, 0), new Vector2(0, terrainSize), this);
            Triangle rightTriangle = new Triangle(null, new Vector2(terrainSize, terrainSize), new Vector2(0, terrainSize), new Vector2(terrainSize, 0), this);
            leftTriangle.addNeighs(null, null, rightTriangle);
            rightTriangle.addNeighs(null, null, leftTriangle);

            //Inicializar listas de triángulos e índices
            this.triangleList = new List<Triangle>();
            triangleList.Add(leftTriangle);
            triangleList.Add(rightTriangle);
            this.indicesList = new List<int>();
            foreach (Triangle t in triangleList) {
                t.addIndices(ref indicesList);
            }

            //Bajar índices al IndexBuffer
            this.ibTerrain = new IndexBuffer(typeof(int), totalVertices, d3dDevice, Usage.WriteOnly, Pool.Default);
            ibTerrain.SetData(indicesList.ToArray(), 0, LockFlags.None);
        }

        protected int[,] loadHeightmapValues(Device d3dDevice, string path) {
            //Cargar los valores en una matriz
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(path);
            int width = bitmap.Size.Width;
            int length = bitmap.Size.Height;
            int[,] heightmap = new int[width + 1, length + 1];
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < length; j++) {
                    //(j, i) invertido para primero barrer filas y despues columnas
                    Color pixel = bitmap.GetPixel(j, i);
                    float intensity = pixel.R * 0.299f + pixel.G * 0.587f + pixel.B * 0.114f;
                    heightmap[i, j] = (int)intensity;
                }
            }
            for (int i = 0; i < width + 1; i++) {
                //copia última fila/columna para que la dimensión sea (potencia de 2)+1
                heightmap[width, i] = heightmap[width - 1, i];
                heightmap[i, width] = heightmap[i, width - 1];
            }

            bitmap.Dispose();
            return heightmap;
        }

        public void loadTexture(string path) {
            //Dispose textura anterior, si habia
            Device d3dDevice = GuiController.Instance.D3dDevice;

            if (terrainTexture != null && !terrainTexture.Disposed) {
                terrainTexture.Dispose();
            }

            //Rotar e invertir textura
            Bitmap b = (Bitmap)Bitmap.FromFile(path);
            b.RotateFlip(RotateFlipType.Rotate90FlipX);
            terrainTexture = Texture.FromBitmap(d3dDevice, b, Usage.None, Pool.Managed);
        }

        public void scaleMap(float scaleXZ, float scaleY) {
            //Recarga el mapa con valores nuevos de escala
            this.ScaleXZ = scaleXZ;
            this.ScaleY = scaleY;
            loadHeightmap(lastHeightmapPath, Center);
        }

        protected CustomVertex.PositionNormalTextured[] createTerrainVertices(int totalVertices) {
            //Devuelve un array con posición, normal (fija), y coord. de textura de cada vértice
            int width = HeightmapData.GetLength(0);
            int length = HeightmapData.GetLength(1);
            CustomVertex.PositionNormalTextured[] terrainVertices = new CustomVertex.PositionNormalTextured[width * length];

            int i = 0;
            for (int z = 0; z < length; z++) {
                for (int x = 0; x < width; x++) {
                    Vector3 position = new Vector3(Position.X + x * ScaleXZ, Position.Y + HeightmapData[x, z] * ScaleY, Position.Z + z * ScaleXZ);
                    Vector3 normal = new Vector3(0, 0, 1);
                    Vector2 texCoord = new Vector2((float)x / 30.0f, (float)z / 30.0f);

                    terrainVertices[i++] = new CustomVertex.PositionNormalTextured(position, normal, texCoord.X, texCoord.Y);
                }
            }

            return terrainVertices;
        }

        protected void updateTriangles() {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            List<Triangle> splitList = new List<Triangle>(); //triángulos que deben ser divididos
            List<Triangle> mergeList = new List<Triangle>(); //triángulos padres de dos que se unieron
            List<Triangle> remainderList = new List<Triangle>(); //triángulos que no deben ser divididos
            List<Triangle> leftoverList = new List<Triangle>(); //triángulos que quedan
            List<Triangle> newTriangleList = new List<Triangle>(triangleList.Count); //triángulos ya divididos

            Matrix worldViewProjectionMatrix = d3dDevice.Transform.View * d3dDevice.Transform.Projection;

            foreach (Triangle t in triangleList) //agregar triángulos a dividir
                t.createSplitList(ref splitList, ref remainderList, ref worldViewProjectionMatrix, ref frustum);

            foreach (Triangle t in splitList) //agregar los ya divididos en newTriangleList (lista definitiva)
                t.processSplitList(ref newTriangleList);

            foreach (Triangle t in remainderList) //agregar los que se tengan que unir en la mergeList
                t.createMergeList(ref mergeList, ref leftoverList, ref worldViewProjectionMatrix, ref frustum);

            foreach (Triangle t in mergeList) //agregar los de la mergeList a la newTriangleList
                t.processMergeList(ref newTriangleList, ref worldViewProjectionMatrix, ref frustum);

            foreach (Triangle t in leftoverList) //procesar los que quedan dependiendo si se dividieron o no
                t.processLeftovers(ref newTriangleList);

            triangleList = newTriangleList;
            triangleList.TrimExcess();
        }

        protected void updateIndexBuffer() {
            //Actualiza el Index Buffer pidiendole a cada triángulo que por favor agregue sus índice a la lista
            Device d3dDevice = GuiController.Instance.D3dDevice;

            indicesList.Clear();
            foreach (Triangle t in triangleList)
                t.addIndices(ref indicesList);

            if (ibTerrain.SizeInBytes / sizeof(int) < indicesList.Count) {
                ibTerrain.Dispose(); //si hay que agregar índices los cargamos de nuevo
                ibTerrain = new IndexBuffer(typeof(int), totalVertices, d3dDevice, Usage.WriteOnly, Pool.Default);
            }
            ibTerrain.SetData(indicesList.ToArray(), 0, LockFlags.None);
        }

        public void render() {
            if (!Enabled) return;

            Device d3dDevice = GuiController.Instance.D3dDevice;
            d3dDevice.Transform.World = Matrix.Identity;
            frustum.updateVolume(d3dDevice.Transform.View, d3dDevice.Transform.Projection);

            updateTriangles();
            updateIndexBuffer();

            d3dDevice.SetTexture(0, terrainTexture);
            d3dDevice.SetTexture(1, null);
            d3dDevice.Material = TgcD3dDevice.DEFAULT_MATERIAL;

            d3dDevice.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            d3dDevice.Indices = ibTerrain;
            d3dDevice.SetStreamSource(0, vbTerrain, 0);
            d3dDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, totalVertices, 0, indicesList.Count / 3);

            d3dDevice.Indices = null;
        }

        public void dispose() {
            if (vbTerrain != null) {
                vbTerrain.Dispose();
            }
            if (ibTerrain != null) {
                ibTerrain.Dispose();
            }

            if (terrainTexture != null) {
                terrainTexture.Dispose();
            }
        }

        public bool AlphaBlendEnable { get; set; }
    }
}