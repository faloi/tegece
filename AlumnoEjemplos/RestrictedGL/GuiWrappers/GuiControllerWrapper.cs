using TgcViewer;

namespace AlumnoEjemplos.RestrictedGL.GuiWrappers
{
    public abstract class Gui
    {
        public static GuiController I
        {
            get { return GuiController.Instance; }
        }
    }
}
