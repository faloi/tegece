using System;
using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Utils;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public class Terrain : IRenderObject {
        private const float SKYBOX_DEPTH = 9000f;
        private readonly List<IRenderObject> components;

        private const int HEIGHTMAP_SIZE = 64;
        private const float INITIAL_SCALE_XZ = 20f;
        private const float INITIAL_SCALE_Y = 0.8f;
        private const float INITIAL_THRESHOLD = 0.075f;

        private const int TREES_COUNT = 100;

        public Terrain() {
            this.components = new List<IRenderObject> {
               new AdaptativeHeightmap(INITIAL_SCALE_XZ, INITIAL_SCALE_Y, INITIAL_THRESHOLD),
               new SkyBox(new Vector3(0, 0, 0), new Vector3(SKYBOX_DEPTH, SKYBOX_DEPTH, SKYBOX_DEPTH))
            };

            this.createTrees(TREES_COUNT);
        }

        private void createTrees(int count) {
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(Shared.MediaFolder + "#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");

            var tree = scene.Meshes[1];
            var treeSize = Convert.ToInt32(tree.BoundingBox.calculateSize().X);

            var randomizer = new Randomizer(0, HEIGHTMAP_SIZE * Convert.ToInt32(INITIAL_SCALE_XZ) - 2 * treeSize);

            for (var i = 1; i < count; i++) {
                var instance = tree.createMeshInstance(tree.Name + i);

                var offsetX = randomizer.getNext();
                var offsetZ = randomizer.getNext();

                instance.move(offsetX, 100, offsetZ);

                this.components.Add(instance);
            }
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