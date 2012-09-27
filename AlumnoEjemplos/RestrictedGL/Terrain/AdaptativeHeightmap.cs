using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
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
        TgcFrustum frustum;
        List<Triangle> triangleList;
        List<int> indicesList;
        VertexBuffer vbTerrain;
        IndexBuffer ibTerrain;
        Texture terrainTexture;
        int totalVertices;

        int[,] heightmapData; //valor de y para cada (x,z) <- la matriz debe ser cuadrada
        public int[,] HeightmapData {
            get { return heightmapData; }
        }
        bool enabled; //si se renderiza o no
        public bool Enabled {
            get { return enabled; }
            set { enabled = value; }
        }
        bool alphaBlendEnable;
        public bool AlphaBlendEnable {
            get { return alphaBlendEnable; }
            set { alphaBlendEnable = value; }
        }
        Vector3 position; //posición de la esquina (se arma en X+ y Z+)
        public Vector3 Position { //centro de la malla
            get { return position; }
        }
        public Vector3 Center { //centro de la malla
            get { return position + new Vector3(heightmapData.GetLength(0) * scaleXZ / 2, position.Y, heightmapData.GetLength(0) * scaleXZ / 2); }
        }
        float scaleXZ; //factor de escala a lo largo
        public float ScaleXZ {
            get { return scaleXZ; }
        }
        float scaleY; //factor de escala de altura
        public float ScaleY {
            get { return scaleY; }
        }
        float threshold;
        public float Threshold {
            get { return threshold; }
            set { threshold = value; }
        }

        public AdaptativeHeightmap() {
            enabled = true;
            alphaBlendEnable = false;
            frustum = new TgcFrustum();
            threshold = 1f;
        }

        public void loadHeightmap(string heightmapPath, float scaleXZ, float scaleY, Vector3 center) {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            this.scaleXZ = scaleXZ;
            this.scaleY = scaleY;

            //Dispose de vb y ib si había
            if (vbTerrain != null && !vbTerrain.Disposed) {
                vbTerrain.Dispose();
            }
            if (ibTerrain != null && !ibTerrain.Disposed) {
                ibTerrain.Dispose();
            }

            //Cargar heightmap
            heightmapData = loadHeightmapValues(d3dDevice, heightmapPath);
            totalVertices = 2 * 3 * (heightmapData.GetLength(0) - 1) * (heightmapData.GetLength(1) - 1); //por cada puntito tengo dos triángulos de 3 vértices
            int width = heightmapData.GetLength(0);
            int length = heightmapData.GetLength(1);

            //Convertir de centro a esquina:
            this.position = center - new Vector3(width * scaleXZ / 2, position.Y, length * scaleXZ/ 2);

            //Crear vértices
            CustomVertex.PositionNormalTextured[] terrainVertices = createTerrainVertices(totalVertices);

            //Bajar vértices al VertexBuffer
            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), totalVertices, d3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);
            vbTerrain.SetData(terrainVertices, 0, LockFlags.None);

            //Crear los dos triángulos root, el piso
            float terrainSize = width - 1;
            Triangle leftTriangle = new Triangle(null, new Vector2(0, 0), new Vector2(terrainSize, 0), new Vector2(0, terrainSize), this);
            Triangle rightTriangle = new Triangle(null, new Vector2(terrainSize, terrainSize), new Vector2(0, terrainSize), new Vector2(terrainSize, 0), this);
            leftTriangle.addNeighs(null, null, rightTriangle);
            rightTriangle.addNeighs(null, null, leftTriangle);

            //Inicializar listas de triángulos e índices
            triangleList = new List<Triangle>();
            triangleList.Add(leftTriangle);
            triangleList.Add(rightTriangle);
            indicesList = new List<int>();
            foreach (Triangle t in triangleList) {
                t.addIndices(ref indicesList);
            }

            //Bajar índices al IndexBuffer
            ibTerrain = new IndexBuffer(typeof(int), totalVertices, d3dDevice, Usage.WriteOnly, Pool.Default);
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

        protected CustomVertex.PositionNormalTextured[] createTerrainVertices(int totalVertices) {
            //Devuelve un array con posición, normal (fija), y coord. de textura de cada vértice
            int width = heightmapData.GetLength(0);
            int length = heightmapData.GetLength(1);
            CustomVertex.PositionNormalTextured[] terrainVertices = new CustomVertex.PositionNormalTextured[width * length];

            int i = 0;
            for (int z = 0; z < length; z++) {
                for (int x = 0; x < width; x++) {
                    //Vector3 position = new Vector3(x, heightmapData[x, z], z);
                    Vector3 position = new Vector3(this.position.X + x * scaleXZ, this.position.Y + heightmapData[x, z] * scaleY, this.position.Z + z * scaleXZ);
                    Vector3 normal = new Vector3(0, 0, 1);
                    Vector2 texCoord = new Vector2((float)x / 30.0f, (float)z / 30.0f);

                    terrainVertices[i++] = new CustomVertex.PositionNormalTextured(position, normal, texCoord.X, texCoord.Y);
                }
            }

            return terrainVertices;
        }

        private void updateTriangles() {
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

        private void updateIndexBuffer() {
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

        public void render() {
            if (!enabled) return;

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
    }
}