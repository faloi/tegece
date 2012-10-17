using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.Utils;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain.Trees
{
    public class TreeFactory : IRenderObject
    {
        private readonly Terrain terrain;
        private readonly List<TgcMesh> trees;

        public TreeFactory(Terrain terrain) {
            this.terrain = terrain;
            this.trees = new List<TgcMesh>();
        }

        private void addTree(TgcMesh instance) {
            this.trees.Add(instance);
        }

        public void createTrees(int count, int maxRadius) {
            var treeSize = Tree.baseSize;
            var maxValue = (int)(maxRadius - 2 * Tree.baseSize / terrain.ScaleXZ);
            var randomizer = new Randomizer(5, maxValue);

            for (var i = 1; i < count; i++) {
                var offsetX = randomizer.getNext();
                var offsetZ = randomizer.getNext();

                var adjust = (int)(maxRadius + treeSize / terrain.ScaleXZ);
                var offsetY = terrain.getYValueFor(offsetX + adjust, offsetZ + adjust);

                var initialPosition = new Vector3(offsetX * terrain.ScaleXZ, offsetY * terrain.ScaleY, offsetZ * terrain.ScaleXZ);
                
                var newTree = Tree.create(initialPosition);
                newTree.rotateY(offsetX);

                this.addTree(newTree);
            }
        }

        public void render() {
            this.trees.ForEach(t => t.render());
        }

        public void dispose() {
            this.trees.ForEach(t => t.dispose());
        }

        public bool AlphaBlendEnable { get; set; }
    }
}