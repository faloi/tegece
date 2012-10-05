using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public class Terrain : IRenderObject {
        private readonly List<IRenderObject> components;
        private readonly AdaptativeHeightmap adaptativeHeightmap;

        private static readonly string PathHeightMap = Shared.MediaFolder + "\\Terreno\\Heightmap.jpg";
        private static readonly string PathTexture = Shared.MediaFolder + "\\Terreno\\Mapa.jpg";
        private const float SKYBOX_DEPTH = 9000f;
        private const float INITIAL_SCALE_XZ = 20f;
        private const float INITIAL_SCALE_Y = 0.8f;
        private const float INITIAL_THRESHOLD = 0.075f;
        private const int TREES_COUNT = 30;

        public int[,] HeightmapData { get { return this.adaptativeHeightmap.HeightmapData; } }
        public float ScaleXZ { get { return this.adaptativeHeightmap.ScaleXZ; } }
        public float ScaleY { get { return this.adaptativeHeightmap.ScaleXZ; } }
        private int HeightmapSize { get { return this.HeightmapData.GetLength(0); } }

        public Terrain() {
            this.adaptativeHeightmap = new AdaptativeHeightmap(INITIAL_SCALE_XZ, INITIAL_SCALE_Y, INITIAL_THRESHOLD);
            this.adaptativeHeightmap.loadHeightmap(PathHeightMap, new Vector3(0, 0, 0));
            this.adaptativeHeightmap.loadTexture(PathTexture);

            var treeFactory = new TreeFactory(INITIAL_SCALE_XZ, INITIAL_SCALE_Y);
            treeFactory.createTrees(TREES_COUNT, HeightmapSize / 2, adaptativeHeightmap.HeightmapData);

            this.components = new List<IRenderObject> {
               adaptativeHeightmap,
               treeFactory,
               new SkyBox(new Vector3(0, 0, 0), new Vector3(SKYBOX_DEPTH, SKYBOX_DEPTH, SKYBOX_DEPTH))
            };

            createModifiers();
        }

        private void createModifiers() {
            GuiController.Instance.Modifiers.addFloat("Scale Y", 0f, 1f, INITIAL_SCALE_Y);
            GuiController.Instance.Modifiers.addFloat("ROAM Threshold", 0f, 1f, INITIAL_THRESHOLD);
        }

        private void updateModifiers() {
            var scaleYNew = Modifiers.get<float>("Scale Y");
            if (this.adaptativeHeightmap.ScaleY != scaleYNew) {
                this.adaptativeHeightmap.scaleMap(this.ScaleXZ, scaleYNew);
            }

            this.adaptativeHeightmap.Threshold = Modifiers.get<float>("ROAM Threshold");
        }

        public void render() {
            updateModifiers();
            this.components.ForEach(o => o.render());
        }

        public void dispose() {
            this.components.ForEach(o => o.dispose());
        }

        public bool AlphaBlendEnable { get; set; }
    }
}