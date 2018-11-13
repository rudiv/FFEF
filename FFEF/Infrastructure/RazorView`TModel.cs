namespace FFEF.Infrastructure
{
    public abstract class RazorView<TModel> : RazorView
    {
        public TModel Model { get; set; } = default(TModel);
    }
}
