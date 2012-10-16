using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.Interfaces;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public class Terrain : IRenderObject, ITerrainCollision {
        private readonly List<IRenderObject> components;
        private readonly AdaptativeHeightmap adaptativeHeightmap;

        private const float INITIAL_THRESHOLD = 0.075f;
        private const int TREES_COUNT = 30;

        public int[,] heightmapData { get { return this.adaptativeHeightmap.HeightmapData; } }
        public readonly float ScaleXZ;
        public float ScaleY { get; private set; }

        private int heightmapSize { get { return this.heightmapData.GetLength(0); } }
        private float heightmapSizeScaled {
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
            var treeFactory = new TreeFactory(this);
            treeFactory.createTrees(TREES_COUNT, this.heightmapSize / 2);

            this.components = new List<IRenderObject> { //lista de componentes a renderizar
               adaptativeHeightmap,
               treeFactory,
               new SkyBox(new Vector3(0, 0, 0), new Vector3(this.heightmapSizeScaled * 3f, this.heightmapSizeScaled, this.heightmapSizeScaled * 3f)),
               new Water()
            };

            this.createModifiers();
        }

        private void createModifiers() {
            GuiController.Instance.Modifiers.addFloat("ROAM Threshold", 0f, 1f, INITIAL_THRESHOLD);
        }

        private void updateModifiers() {
            this.adaptativeHeightmap.Threshold = Modifiers.get<float>("ROAM Threshold");
        }

        private int transformCoordenate(float originalValue) {
            return (int) (originalValue / this.ScaleXZ + this.heightmapSize / 2);
        }

        public int getYValueFor(float x, float z) {
            var realX = this.transformCoordenate(x);
            var realZ = this.transformCoordenate(z);

            return this.heightmapData[realX, realZ];
        }

        public void deform(float x, float z, float radius, int power) {
            //Deforma el heightmap en (x,z) creando un agujero de un radio
            //con una determinada "potencia" (qué tan profundo se hace el agujero)
            var realX = this.transformCoordenate(x);
            var realZ = this.transformCoordenate(z);

            var realRadius = (int)(radius / this.ScaleXZ);

            this.adaptativeHeightmap.deform(realX, realZ, realRadius, power);
        }

        public void render() {
            this.updateModifiers();
            this.components.ForEach(o => o.render());
        }

        public void dispose() {
            this.components.ForEach(o => o.dispose());
        }

        public bool AlphaBlendEnable { get; set; }
    }
}