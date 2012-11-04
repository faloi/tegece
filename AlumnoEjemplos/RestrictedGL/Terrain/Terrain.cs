using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.Interfaces;
using AlumnoEjemplos.RestrictedGL.Terrain.Trees;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public class Terrain : IRenderObject, ITerrainCollision {
        public readonly TreeFactory treeFactory;
        private readonly List<IRenderObject> components;
        private readonly AdaptativeHeightmap adaptativeHeightmap;

        private const float INITIAL_THRESHOLD = 0.075f;
        private const int TREES_COUNT = 10;

        public int[,] heightmapData { get { return this.adaptativeHeightmap.HeightmapData; } }
        public readonly float ScaleXZ;
        public float ScaleY { get; private set; }

        private int heightmapSize { get { return this.heightmapData.GetLength(0); } }
        public float heightmapSizeScaled {
            get { return this.heightmapSize * this.ScaleXZ; }
        }

        public Terrain(float scaleXZ, float scaleY) {
            //Cargar heightmap
            this.ScaleXZ = scaleXZ;
            this.ScaleY = scaleY;
            
            this.adaptativeHeightmap = new AdaptativeHeightmap(this.ScaleXZ, this.ScaleY, INITIAL_THRESHOLD);
            this.adaptativeHeightmap.loadHeightmap(Path.MapHeightmap, new Vector3(0, 0, 0));
            this.adaptativeHeightmap.loadTexture(Path.MapTexture);

            //Crear TREES_COUNT pinos de forma random a lo largo del tererno
            this.treeFactory = new TreeFactory(this);
            this.treeFactory.createTrees(TREES_COUNT, this.heightmapSize / 2);

            this.components = new List<IRenderObject> { //lista de componentes a renderizar
               adaptativeHeightmap,
               new SkyBox(new Vector3(0, 0, 0), new Vector3(this.heightmapSizeScaled * 3f, this.heightmapSizeScaled, this.heightmapSizeScaled * 3f)),
               new Water()
            };

            this.createModifiers();
        }

        private void createModifiers() {
            Gui.I.Modifiers.addFloat("ROAM Threshold", 0f, 1f, INITIAL_THRESHOLD);
        }

        private void updateModifiers() {
            this.adaptativeHeightmap.Threshold = Modifiers.get<float>("ROAM Threshold");
        }

        private float transformCoordenate(float originalValue) {
            return (float) (originalValue / this.ScaleXZ + this.heightmapSize / 2);
        }

        public float getYValueFor(float x, float z) {
            var realX = this.transformCoordenate(x);
            var realZ = this.transformCoordenate(z);

            var u = checkIndex((int)realX);
            var v = checkIndex((int)realZ);

            var s = realX - u;
            var t = realZ - v;
            var heightA = heightmapData[u, v] + s * (heightmapData[u+1, v] - heightmapData[u, v]);
            var heightB = heightmapData[u, v+1] + s * (heightmapData[u+1, v+1] - heightmapData[u, v+1]);
            var finalHeight = heightA + t * (heightB - heightA);

            return finalHeight;
        }

        private static int checkIndex(int value){
            if (value < 0){
                return 0;
            } else if (value >= 127){
                return 126;
            }
            return value;
        }

        public bool isOutOfBounds(ITransformObject tankOrMissile){
            return tankOrMissile.Position.X > this.heightmapSizeScaled / 2 || tankOrMissile.Position.X < -this.heightmapSizeScaled / 2 ||
                   tankOrMissile.Position.Z > this.heightmapSizeScaled / 2 || tankOrMissile.Position.Z < -this.heightmapSizeScaled / 2;
        }

        public void deform(float x, float z, float radius, int power) {
            //Deforma el heightmap en (x,z) creando un agujero de un radio
            //con una determinada "potencia" (qué tan profundo se hace el agujero)
            var realX = this.transformCoordenate(x);
            var realZ = this.transformCoordenate(z);

            var realRadius = (int)(radius / this.ScaleXZ);

            this.adaptativeHeightmap.deform((int)realX, (int)realZ, realRadius, power);
        }

        public void render() {
            this.updateModifiers();
            this.treeFactory.render();
            this.components.ForEach(o => o.render());
        }

        public void dispose() {
            this.components.ForEach(o => o.dispose());
        }

        public bool AlphaBlendEnable { get; set; }
    }
}