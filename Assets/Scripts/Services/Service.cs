namespace Services {
    public abstract class Service {
        public abstract void SetupService();

        public virtual void CloseService() {}
    }
}