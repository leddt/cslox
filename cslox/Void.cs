namespace cslox
{
    public sealed class Void
    {
        public static readonly Void Instance = new Void();
        private Void() {}
    }
}