namespace RMD.Movil.Services.Pdf
{
    public interface IPdfService
    {
        Task<string> SaveHtmlToPdfAsync(string html, string fileNameNoExt, CancellationToken ct = default);
    }
}
