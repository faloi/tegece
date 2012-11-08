using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Utils;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain.Trees
{
    public class TreeFactory : IRenderObject
    {
        private readonly Terrain terrain;
        private readonly List<Tree> trees;

        public TreeFactory(Terrain terrain) {
            this.terrain = terrain;
            this.trees = new List<Tree>();
        }

        private void addTree(Tree newTree) {
            this.trees.Add(newTree);
        }

        public void createTrees(int count, int maxRadius) {
            var baseSize = Tree.baseSize;

            var maxValue = (int)(maxRadius - 2 * baseSize / terrain.ScaleXZ);
            var randomizer = new Randomizer(5, maxValue);

            for (var i = 1; i < count; i++) {
                var offsetX = randomizer.getNext();
                var offsetZ = randomizer.getNext();

                var adjust = (int)(maxRadius + baseSize / terrain.ScaleXZ);
                var offsetY = terrain.getYValueFor(offsetX + adjust - 100, offsetZ + adjust - 100);

                var initialPosition = new Vector3(offsetX * terrain.ScaleXZ, offsetY * terrain.ScaleY, offsetZ * terrain.ScaleXZ);
                
                var newTree = Tree.create(initialPosition,terrain);
                newTree.mesh.rotateY(offsetX);

                this.addTree(newTree);
            }
        }

        public bool isAnyCollidingWith(TgcBoundingSphere tankSphere){
            foreach (var tree in trees) {
                if (TgcCollisionUtils.testSphereSphere(tree.boundingSphere, tankSphere))
                    return true;
            }
            return false;
        }

        public void render() {
            this.trees.ForEach(t => {
               t.mesh.render(); 
               
               if (Modifiers.showBoundingBox())
                    t.boundingSphere.render();
            });
        }

        public void dispose() {
            this.trees.ForEach(t => t.mesh.dispose());
        }

        public bool AlphaBlendEnable { get; set; }
    }
}