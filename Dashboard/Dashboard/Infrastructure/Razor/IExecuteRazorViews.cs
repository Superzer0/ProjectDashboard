namespace Dashboard.Infrastructure.Razor
{
    public interface IExecuteRazorViews
    {
        string Execute(string viewPath, object model, dynamic viewBag, string layoutPath);
        string ExecutePartial(string viewPath, object model, dynamic viewBag);
    }
}
