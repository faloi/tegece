using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;

public class Missile : ITransformObject
{
    private bool isExploded ;
    private float flightTime;
    private TgcMesh mesh;
    private Vector3 currentVelocity;
    private Vector3 currentPosition;
    private float shootAngle;
    private const float GRAVITY = -0.125f;


    public Matrix Transform { get; set; }
    public bool AutoTransformEnable { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 Scale { get; set; }
    
    public Missile(Vector3 tankPosition,float currentAngle) {

        var alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
        var loader = new TgcSceneLoader();
        var scene = loader.loadSceneFromFile(alumnoMediaFolder + "RestrictedGL\\#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");

        this.mesh = scene.Meshes[2];
        //Tengo que settiar la velocidad Inicial
        this.currentVelocity = new Vector3(0,2,0);
        this.currentPosition = new Vector3(tankPosition.X, tankPosition.Y + 5, tankPosition.Z);
        this.shootAngle = Geometry.DegreeToRadian(currentAngle);
        this.isExploded = false;
    }

    //Velocidad en X y en Z son CONSTANTES (no hay gravedad), solo cambia en Y
    private void calcVelocity(float elapsedTime) {
        this.currentVelocity.Y += GRAVITY*elapsedTime;
    }

    private void calcPosition(float elapsedTime) {
        var initialPosition = this.currentPosition;
        //Formulas sacadas de http://ingciv-sandrus.blogspot.com.ar/2008/05/tiro-parablico-en-tres-dimensiones.html
        //Alfa es el angulo Verticual, supongo que es 45 degree (seria tipo la inclinacion en la que esta el cañon respecto el eje XZ)
        //Beta es el angulo Horizontal
        //x(t) = xo + v cos (alfa) * cos(beta) * t
        currentPosition.X = (float) (currentPosition.X + this.currentVelocity.X*Math.Cos(Geometry.DegreeToRadian(45))*Math.Cos(this.shootAngle)*this.flightTime);
        //y(t) = Zo + v sen (alfa) * t - gt^2 /2
        currentPosition.Y = (float) (currentPosition.Y + this.currentVelocity.Y*Math.Sin(Geometry.DegreeToRadian(45))*this.flightTime -
                                     (GRAVITY*Math.Pow(this.flightTime,2))*0.5);
        //z(t) = yo + v cos (alfa) * sen(beta) * t
        currentPosition.Z = (float) (currentPosition.Z +
                                     this.currentVelocity.Z*Math.Cos(Geometry.DegreeToRadian(45))*Math.Sin(this.shootAngle)*this.flightTime);
    }

    public void move(Vector3 v) {
        this.mesh.move(this.currentPosition.X,this.currentPosition.Y,this.currentPosition.Z);
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

    public void render(float elapsedTime) {
        this.flightTime += elapsedTime;
        this.calcVelocity(elapsedTime);
        this.calcPosition(elapsedTime);
        this.move(this.currentPosition);
        this.mesh.render();
    }

    public void dispose() {
        this.mesh.dispose(); ;
    }
}