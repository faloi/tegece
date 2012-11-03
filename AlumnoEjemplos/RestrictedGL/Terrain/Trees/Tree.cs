using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain.Trees
{
    public class Tree {
        private static TgcMesh baseTree;
        private static int count;
        private static readonly Vector3 scale = new Vector3(5, 5, 5);
        public TgcMesh mesh;
        public TgcBoundingSphere boundingSphere;

        private static void createBaseTree() {
            var scene = new TgcSceneLoader().loadSceneFromFile(Path.PalmScene);
            baseTree = scene.Meshes[0];
        }

        public static Tree create(Vector3 initialPosition,Terrain terrain) {
            if (baseTree == null)
                createBaseTree();

            count++;
            
            var newMesh = baseTree.createMeshInstance(baseTree.Name + count);
            newMesh.Position = initialPosition;
            newMesh.Scale = scale;
            newMesh.AlphaBlendEnable = true;

            var newTree = new Tree();
            newTree.mesh = newMesh;
            
            var newYValue = terrain.getYValueFor(newMesh.BoundingBox.calculateBoxCenter().X,newMesh.BoundingBox.calculateBoxCenter().Z);
            newTree.boundingSphere = new TgcBoundingSphere(new Vector3(newMesh.BoundingBox.calculateBoxCenter().X, newYValue+100f, newMesh.BoundingBox.calculateBoxCenter().Z), 150f);

            return newTree;
        }

        public static float baseSize {
            get {
                if (baseTree == null)
                    createBaseTree();

                return baseTree.BoundingBox.calculateSize().X * scale.X;
            }
        }
    }
}
