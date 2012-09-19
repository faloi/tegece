using TgcViewer;

namespace AlumnoEjemplos.RestrictedGL.GuiWrappers
{
    public abstract class GuiControllerWrapper
    {
        protected static GuiController Gui
        {
            get { return GuiController.Instance; }
        }
    }
}
