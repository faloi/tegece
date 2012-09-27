using System;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;

public class Missile : ITransformObject
{
    private bool isExploded ;
    private TgcMesh mesh;

    public Matrix Transform { get; set; }
    public bool AutoTransformEnable { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 Scale { get; set; }
    
    public Missile(Vector3 tankPosition) {

        var alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
        var loader = new TgcSceneLoader();
        var scene = loader.loadSceneFromFile(alumnoMediaFolder + "RestrictedGL\\#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");

        this.mesh = scene.Meshes[2];
        this.mesh.move(tankPosition.X+20,tankPosition.Y,tankPosition.Z+20);

        this.isExploded = false;
    }

    public void move(Vector3 v) {
            throw new NotImplementedException();
    }
    public void move(float x, float y, float z) {
        throw new NotImplementedException();
    }
    public void moveOrientedY(float movement) {
        throw new NotImplementedException();
    }
    public void getPosition(Vector3 pos) {
        throw new NotImplementedException();
    }
    public void rotateX(float angle) {
        throw new NotImplementedException();
    }
    public void rotateY(float angle) {
        throw new NotImplementedException();
    }
    public void rotateZ(float angle) {
        throw new NotImplementedException();
    }

    public void render() {
        this.mesh.render();
    }
    public void dispose() {
        this.mesh.dispose(); ;
    }
}