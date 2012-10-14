namespace AlumnoEjemplos.RestrictedGL.GuiWrappers
{
    class UserVars : GuiControllerWrapper
    {
        public void add(string key) {
            Gui.UserVars.addVar(key);
        }

        public void addMany(params string[] keys) {
            foreach (var key in keys)
                this.add(key);
        }

        public UserVars set(string key, object value) {
            Gui.UserVars.setValue(key, value);
            return this;
        }
    }
}
