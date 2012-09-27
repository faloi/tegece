using System;
using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.Utils;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public static class TreeFactory {
        
        public static IEnumerable<IRenderObject> createTrees(int count, int maxRadius)
        {
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(Shared.MediaFolder + "#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");

            var originalTree = scene.Meshes[1];
            var treeSize = Convert.ToInt32(originalTree.BoundingBox.calculateSize().X);

            var randomizer = new Randomizer(0, maxRadius - 2 * treeSize);

            for (var i = 1; i < count; i++)
                yield return createTree(randomizer, originalTree, i);
            
        }

        private static TgcMesh createTree(Randomizer randomizer, TgcMesh tree, int i) {
            var instance = tree.createMeshInstance(tree.Name + i);

            var offsetX = randomizer.getNext();
            var offsetZ = randomizer.getNext();

            instance.move(offsetX, 100, offsetZ);
            return instance;
        }
    }
}
