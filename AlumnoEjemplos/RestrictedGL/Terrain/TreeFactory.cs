using System;
using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.Utils;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public class TreeFactory {
        private readonly float scaleXZ;
        private readonly float scaleY;

        public TreeFactory(float scaleXZ, float scaleY) {
            this.scaleXZ = scaleXZ;
            this.scaleY = scaleY;
        }

        public IEnumerable<IRenderObject> createTrees(int count, int maxRadius, int [,] yValues) {
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(Shared.MediaFolder + "#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");

            var originalTree = scene.Meshes[1];
            var treeSize = Convert.ToInt32(originalTree.BoundingBox.calculateSize().X);

            var maxValue = (int) (maxRadius - 2 * treeSize / this.scaleXZ);
            var randomizer = new Randomizer(0, maxValue);

            for (var i = 1; i < count; i++) {
                var instance = originalTree.createMeshInstance(originalTree.Name + i);

                var offsetX = randomizer.getNext();
                var offsetZ = randomizer.getNext();

                var adjust = (int) (maxRadius + treeSize / this.scaleXZ);
                var offsetY = yValues[offsetX + adjust, offsetZ + adjust];

                instance.move(offsetX * this.scaleXZ, offsetY * this.scaleY, offsetZ * this.scaleXZ);
                
                yield return instance;
            }
        }
    }
}
