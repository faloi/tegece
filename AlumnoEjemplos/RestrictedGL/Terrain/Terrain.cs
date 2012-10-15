﻿using System.Collections.Generic;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public class Terrain : IRenderObject {
        private readonly List<IRenderObject> components;
        private readonly AdaptativeHeightmap adaptativeHeightmap;

        private const float INITIAL_THRESHOLD = 0.075f;
        private const int TREES_COUNT = 30;

        public int[,] heightmapData { get { return this.adaptativeHeightmap.HeightmapData; } }
        public readonly float ScaleXZ;
        public readonly float ScaleY;

        private int heightmapSize { get { return this.heightmapData.GetLength(0); } }
        private float heightmapSizeScaled {
            get { return this.heightmapSize * this.ScaleXZ; }
        }

        public Terrain(float scaleXZ, float scaleY) {
            //Cargar heightmap
            this.ScaleXZ = scaleXZ;
            this.ScaleY = scaleY;
            
            this.adaptativeHeightmap = new AdaptativeHeightmap(this.ScaleXZ, this.ScaleY, INITIAL_THRESHOLD);
            this.adaptativeHeightmap.loadHeightmap(Path.HeightMap, new Vector3(0, 0, 0));
            this.adaptativeHeightmap.loadTexture(Path.Texture);

            //Crear TREES_COUNT pinos de forma random a lo largo del tererno
            var treeFactory = new TreeFactory(this);
            treeFactory.createTrees(TREES_COUNT, this.heightmapSize / 2);

            this.components = new List<IRenderObject> { //lista de componentes a renderizar
               adaptativeHeightmap,
               treeFactory,
               new SkyBox(new Vector3(0, 0, 0), new Vector3(this.heightmapSizeScaled * 3f, this.heightmapSizeScaled, this.heightmapSizeScaled * 3f))
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

        public void deform(float x, float z, float radius, int power, int count) {
            //Deforma el heightmap en (x,z) creando un agujero de un radio
            //con una determinada "potencia" (qué tan profundo se hace el agujero)
            var realX = this.transformCoordenate(x);
            var realZ = this.transformCoordenate(z);

            var realRadius = (int)(radius / this.ScaleXZ);

            this.adaptativeHeightmap.deform(realX, realZ, realRadius, power, count);
        }

        public void render(List<Missile> missilesShooted) {
            this.updateModifiers();
            missilesShooted.ForEach(this.deformIfCollidingWith);
            
            this.components.ForEach(o => o.render());
        }

        private void deformIfCollidingWith(Missile missile) {
            if (missile.isCollidingWith(this))            
                this.deform(missile.Position.X, missile.Position.Z, 150, 10, 1);
        }

        public void render() {
            throw new System.NotImplementedException();
        }

        public void dispose() {
            this.components.ForEach(o => o.dispose());
        }

        public bool AlphaBlendEnable { get; set; }
    }
}