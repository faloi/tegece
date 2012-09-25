using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils;

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
        Vector3 apexPos;

        int tInd;
        int lInd;
        int rInd;

        public bool splitted = false;
        public bool addedToMergeList = false;

        public Triangle(Triangle parent, Vector2 tPoint, Vector2 lPoint, Vector2 rPoint, int[,] heightData) {
            //Crea el triángulo
            int resolution = heightData.GetLength(0);
            tInd = (int)(tPoint.X + tPoint.Y * resolution);
            lInd = (int)(lPoint.X + lPoint.Y * resolution);
            rInd = (int)(rPoint.X + rPoint.Y * resolution);

            lPos = new Vector3(lPoint.X, heightData[(int)lPoint.X, (int)lPoint.Y], -lPoint.Y);
            Vector2 apex = (lPoint + rPoint) * (1/2);
            apexPos = new Vector3(apex.X, heightData[(int)apex.X, (int)apex.Y], -apex.Y);

            this.parent = parent;
            //Splitea el triángulo recursivamente hasta lograr que la altura de cada uno sea <= 1
            //(para usar los valores del heightmap)
            if (Vector2Distance(lPoint, tPoint) > 1) {
                lChild = new Triangle(this, apex, tPoint, lPoint, heightData);
                rChild = new Triangle(this, apex, rPoint, tPoint, heightData);
            }
        }

        public void AddNeighs(Triangle lNeigh, Triangle rNeigh, Triangle bNeigh) {
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

                lChild.AddNeighs(rChild, bNeighRightChild, lNeighRightChild);
                rChild.AddNeighs(bNeighLeftChild, lChild, rNeighLeftChild);
            }
        }

        /*public void CreateSplitList(ref List<Triangle> splitList, ref List<Triangle> remainderList, ref Matrix wvp, ref BoundingFrustum bf) {
            //since this method is called for all triangles drawn in the previous frame, this line is used to reset the addedToMergeList variable
            addedToMergeList = false;

            bool hasSplit = false;
            if ((lChild != null) && (!splitted)) {
                if (ShouldSplit(ref wvp, ref bf)) {
                    PropagateSplit(ref splitList);
                    hasSplit = true;
                }
            }

            if (!hasSplit)
                remainderList.Add(this);
        }

        public void PropagateSplit(ref List<Triangle> splitList) {
            if (!splitted) {
                splitted = true;
                splitList.Add(this);
                if (bNeigh != null)
                    bNeigh.PropagateSplit(ref splitList);
                if (parent != null)
                    parent.PropagateSplit(ref splitList);
            }
        }

        public void ProcessSplitList(ref List<Triangle> toDrawList) {
            if (!rChild.splitted)
                toDrawList.Add(rChild);
            if (!lChild.splitted)
                toDrawList.Add(lChild);
        }

        public bool CheckMerge(ref List<Triangle> mergeList, ref Matrix wvp, ref BoundingFrustum bf) {
            bool shouldMerge = false;
            if (!addedToMergeList) {
                if (CanMerge()) {
                    if (!ShouldSplit(ref wvp, ref bf)) {
                        shouldMerge = true;
                        if (bNeigh != null)
                            if (bNeigh.ShouldSplit(ref wvp, ref bf))
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

        public void CreateMergeList(ref List<Triangle> mergeList, ref List<Triangle> leftoverList, ref Matrix wvp, ref BoundingFrustum bf) {
            bool cannotMerge = true;
            if (parent != null)
                cannotMerge = !parent.CheckMerge(ref mergeList, ref wvp, ref bf);

            if (cannotMerge)
                leftoverList.Add(this);
        }

        public void ProcessMergeList(ref List<Triangle> toDrawList, ref Matrix wvp, ref BoundingFrustum bf) {
            splitted = false;
            toDrawList.Add(this);
        }

        public void ProcessLeftovers(ref List<Triangle> toDrawList) {
            if (!splitted)
                toDrawList.Add(this);
        }

        public bool CanMerge() {
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

        public bool ShouldSplit(ref Matrix wvp, ref BoundingFrustum bf) {
            bool shouldSplit = false;
            if (bf.Contains(apexPos) != ContainmentType.Disjoint) {
                Vector4 lScreenPos = Vector4.Transform(lPos, wvp);
                Vector4 aScreenPos = Vector4.Transform(apexPos, wvp);
                lScreenPos /= lScreenPos.W;
                aScreenPos /= aScreenPos.W;

                Vector4 difference = lScreenPos - aScreenPos;
                Vector2 screenDifference = new Vector2(difference.X, difference.Y);

                float threshold = 0.1f;
                if (screenDifference.Length() > threshold)
                    shouldSplit = true;
            }

            return shouldSplit;
        }*/

        public void AddIndices(ref List<int> indicesList) {
            indicesList.Add(lInd);
            indicesList.Add(tInd);
            indicesList.Add(rInd);
        }

        private float Vector2Distance(Vector2 v1, Vector2 v2) {
            return (float) Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2));
        }
    }
}