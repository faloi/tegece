using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public class Terrain : IRenderObject {
        private readonly List<IRenderObject> components;
        private readonly AdaptativeHeightmap adaptativeHeightmap;

        private const float SKYBOX_DEPTH = 9000f;
        private const int HEIGHTMAP_SIZE = 128;
        private const float INITIAL_SCALE_XZ = 20f;
        private const float INITIAL_SCALE_Y = 0.8f;
        private const float INITIAL_THRESHOLD = 0.075f;

        private const int TREES_COUNT = 30;

        public int[,] HeightmapData { get { return this.adaptativeHeightmap.HeightmapData; } }

        public Terrain() {
            this.adaptativeHeightmap = new AdaptativeHeightmap(INITIAL_SCALE_XZ, INITIAL_SCALE_Y, INITIAL_THRESHOLD);

            var treeFactory = new TreeFactory(INITIAL_SCALE_XZ, INITIAL_SCALE_Y);
            treeFactory.createTrees(TREES_COUNT, HEIGHTMAP_SIZE / 2, adaptativeHeightmap.HeightmapData);

            this.components = new List<IRenderObject> {
               this.adaptativeHeightmap,
               treeFactory,
               new SkyBox(new Vector3(0, 0, 0), new Vector3(SKYBOX_DEPTH, SKYBOX_DEPTH, SKYBOX_DEPTH))
            };

        }

        public void render() {
            this.components.ForEach(o => o.render());
        }

        public void dispose() {
            this.components.ForEach(o => o.dispose());
        }

        public bool AlphaBlendEnable { get; set; }
    }
}