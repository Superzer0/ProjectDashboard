namespace Dashboard.Infrastructure.Services.Abstract
{
    public interface IEnvironment
    {
        string AppRootPath { get; }
        string MapPath(string relativePath);
        string BaseAddress { get; }
        string AppVersion { get; }
    }
}
