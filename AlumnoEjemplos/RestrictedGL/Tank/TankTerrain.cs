using System;
using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Interfaces;
using AlumnoEjemplos.RestrictedGL.Utils;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.RestrictedGL.Tank {
    
    class TankTerrain : IAlumnoRenderObject {
        
        private TgcBox surface;
        private List<TgcMesh> trees;

        const int SURFACE_SIZE = 500;

        private void createTrees(string alumnoMediaFolder) {
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(alumnoMediaFolder + "RestrictedGL\\#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");

            var tree = scene.Meshes[1];
            var treeSize = Convert.ToInt32(tree.BoundingBox.calculateSize().X);

            this.trees = new List<TgcMesh>();
            var randomizer = new Randomizer(50, SURFACE_SIZE / 2 - 2 * treeSize);

            for (var i = 1; i < 10; i++)
            {
                var instance = tree.createMeshInstance(tree.Name + i);

                var offsetX = randomizer.getNext();
                var offsetZ = randomizer.getNext();
                instance.move(offsetX, 0, offsetZ);

                this.trees.Add(instance);
            }
        }

        public void init(string alumnoMediaFolder){
            var d3DDevice = GuiController.Instance.D3dDevice;
            var surfaceTexture = TgcTexture.createTexture(d3DDevice, alumnoMediaFolder + "RestrictedGL\\#TankExample\\Textures\\tierra.jpg");            
            this.surface = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(SURFACE_SIZE, 0, SURFACE_SIZE), surfaceTexture);

            this.createTrees(alumnoMediaFolder);
        }

        public void render(float elapsedTime) {
            surface.render();
            
            var showBoundingBox = Modifiers.get<bool>("showBoundingBox");
            foreach (var tree in trees) {
                tree.render();
                if (showBoundingBox)
                    tree.BoundingBox.render();
            }

        }
    
        public void dispose() {
            surface.dispose();
            trees.ForEach(t => t.dispose());
        }
    }   
}
