using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL
{
    public class Triangle
    {
        Triangle lNeigh;
        Triangle rNeigh;
        Triangle bNeigh;

        Triangle parent;
        Triangle lChild;
        Triangle rChild;

        Vector3 lPos;
        Vector3 centerPos;

        int tInd;
        int lInd;
        int rInd;

        public bool splitted = false;
        public bool addedToMergeList = false;
        AdaptativeHeightmap map; //Mapa al que pertenecen (para obtener el threshold)

        public Triangle(Triangle parent, Vector2 tPoint, Vector2 lPoint, Vector2 rPoint, int[,] heightData, float scaleXZ, float scaleY, AdaptativeHeightmap map) {
            //Crea el triángulo
            this.map = map;

            int resolution = heightData.GetLength(0);
            tInd = (int)(tPoint.X + tPoint.Y * resolution);
            lInd = (int)(lPoint.X + lPoint.Y * resolution);
            rInd = (int)(rPoint.X + rPoint.Y * resolution);

            lPos = new Vector3(map.Position.X + lPoint.X * scaleXZ, map.Position.Y + heightData[(int)lPoint.X, (int)lPoint.Y] * scaleY, map.Position.Z + lPoint.Y * scaleXZ);
            //centro: promedio de cada componente
            Vector2 center = new Vector2((lPoint.X + rPoint.X) / 2, (lPoint.Y + rPoint.Y) / 2);
            centerPos = new Vector3(map.Position.X + center.X * scaleXZ, map.Position.Y + heightData[(int)center.X, (int)center.Y] * scaleY, map.Position.Z + center.Y * scaleXZ);

            this.parent = parent;
            //Splitea el triángulo hasta que queden triángulos medianamente chicos
            //(para dejar el árbol armado con la mayor calidad posible)
            if (Vector2Distance(lPoint, tPoint) > 1) {
                lChild = new Triangle(this, center, tPoint, lPoint, heightData, scaleXZ, scaleY, map);
                rChild = new Triangle(this, center, rPoint, tPoint, heightData, scaleXZ, scaleY, map);
            }
        }

        public void addNeighs(Triangle lNeigh, Triangle rNeigh, Triangle bNeigh) {
            //Agrega referencias a los vecinos
            this.lNeigh = lNeigh;
            this.rNeigh = rNeigh;
            this.bNeigh = bNeigh;

            //Encuentra y asigna las referencias a vecinos de sus hijos
            if (lChild != null) {
                Triangle bNeighRightChild = null;
                Triangle bNeighLeftChild = null;
                Triangle lNeighRightChild = null;
                Triangle rNeighLeftChild = null;

                if (bNeigh != null) {
                    bNeighLeftChild = bNeigh.lChild;
                    bNeighRightChild = bNeigh.rChild;
                }
                if (lNeigh != null)
                    lNeighRightChild = lNeigh.rChild;
                if (rNeigh != null)
                    rNeighLeftChild = rNeigh.lChild;

                lChild.addNeighs(rChild, bNeighRightChild, lNeighRightChild);
                rChild.addNeighs(bNeighLeftChild, lChild, rNeighLeftChild);
            }
        }

        public void createSplitList(ref List<Triangle> splitList, ref List<Triangle> remainderList, ref Matrix wvp, ref TgcFrustum bf) {
            addedToMergeList = false; //este flag se resetea acá ya que se llama por cada render

            bool hasSplit = false;
            if ((lChild != null) && (!splitted)) { //si todavía no se dividió
                if (shouldSplit(ref wvp, ref bf)) { //y se puede
                    propageSplit(ref splitList); //dale!!!
                    hasSplit = true;
                }
            }

            if (!hasSplit)
                remainderList.Add(this); //sino va a remainderList
        }

        public void propageSplit(ref List<Triangle> splitList) {
            //Si dividimos un triángulo, hay que dividir también al bottom neighbor y al papá
            //(para evitar crackkks)
            if (!splitted) {
                splitted = true; //avisamos rápido que lo pensamos dividir, para evitar que otro con el que comparta lado lo haga
                splitList.Add(this);
                if (bNeigh != null)
                    bNeigh.propageSplit(ref splitList);
                if (parent != null)
                    parent.propageSplit(ref splitList);
            }
        }

        public void processSplitList(ref List<Triangle> toDrawList) {
            //Agrega a los hijos a la lista próxima a dibujar
            if (!rChild.splitted)
                toDrawList.Add(rChild);
            if (!lChild.splitted)
                toDrawList.Add(lChild);
        }

        public bool canMerge() {
            //Determina si puede unirse o no, mas allá de si realmente deba hacerlo
            //(no podría en caso de que sus hijos o los de su vecino estén divididos)
            bool cannotMerge = false;
            if (lChild != null)
                cannotMerge |= lChild.splitted;
            if (rChild != null)
                cannotMerge |= rChild.splitted;
            if (bNeigh != null) {
                if (bNeigh.lChild != null)
                    cannotMerge |= bNeigh.lChild.splitted;
                if (bNeigh.rChild != null)
                    cannotMerge |= bNeigh.rChild.splitted;
            }

            return !cannotMerge;
        }

        public bool checkMerge(ref List<Triangle> mergeList, ref Matrix wvp, ref TgcFrustum bf) {
            //Determina si debe unirse o no, y en caso de poderse se agrega a la mergeList
            bool shouldMerge = false;
            if (!addedToMergeList) {
                if (canMerge()) {
                    if (!shouldSplit(ref wvp, ref bf)) {
                        shouldMerge = true;
                        if (bNeigh != null)
                            if (bNeigh.shouldSplit(ref wvp, ref bf)) //él y su vecino deben ponerse de acuerdo para mergear
                                shouldMerge = false;
                    }
                }
            }

            if (shouldMerge) {
                addedToMergeList = true;
                mergeList.Add(this);
                if (bNeigh != null) {
                    bNeigh.addedToMergeList = true;
                    mergeList.Add(bNeigh);
                }
            }

            return addedToMergeList;
        }

        public void createMergeList(ref List<Triangle> mergeList, ref List<Triangle> leftoverList, ref Matrix wvp, ref TgcFrustum bf) {
            //Crea la mergeList, los restantes van a lestoverList
            bool cannotMerge = true;
            if (parent != null)
                cannotMerge = !parent.checkMerge(ref mergeList, ref wvp, ref bf);

            if (cannotMerge)
                leftoverList.Add(this);
        }

        public void processMergeList(ref List<Triangle> toDrawList, ref Matrix wvp, ref TgcFrustum bf) {
            //Los triángulos que se unen simplemente avisan que no están divididos y se agregan a la lista
            splitted = false;
            toDrawList.Add(this);
        }

        public void processLeftovers(ref List<Triangle> toDrawList) {
            //Por la propagación, hace falta verificar que no se hayan dividido, para agregarlos a la lista
            if (!splitted)
                toDrawList.Add(this);
        }

        public bool shouldSplit(ref Matrix wvp, ref TgcFrustum bf) {
            /*Determina si un triángulo debe dividirse o no, midiendo la distancia
             *(screen distance) entre su left point y su center point. Cerca del frustum, esta
             *distancia es más grande. Si supera un threshold se considera suficiente para dividirlo.
            */
            bool shouldSplit = false;
            if (TgcCollisionUtils.testPointFrustum(bf, centerPos)) {
                Vector4 lScreenPos = Vector4.Transform(new Vector4(lPos.X, lPos.Y, lPos.Z, 1), wvp);
                Vector4 aScreenPos = Vector4.Transform(new Vector4(centerPos.X, centerPos.Y, centerPos.Z, 1), wvp);
                lScreenPos = lScreenPos * (1/lScreenPos.W);
                aScreenPos = aScreenPos * (1/aScreenPos.W);

                Vector4 difference = lScreenPos - aScreenPos;
                Vector2 screenDifference = new Vector2(difference.X, difference.Y);

                //(menos tolerancia => más calidad)
                if (screenDifference.Length() > map.Threshold)
                    shouldSplit = true;
            }

            return shouldSplit;
        }

        public void addIndices(ref List<int> indicesList) {
            //Agrega índices de los vértices a la lista que luego se pasa al Index Buffer
            indicesList.Add(lInd);
            indicesList.Add(tInd);
            indicesList.Add(rInd);
        }

        private float Vector2Distance(Vector2 v1, Vector2 v2) {
            //Distancia entre puntos 2D
            return (float) Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2));
        }
    }
}