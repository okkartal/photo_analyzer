namespace Photos;

public class PhotosAnalyzer
{
    private readonly IAnalyzerService analyzerService;

    public PhotosAnalyzer(IAnalyzerService analyzerService)
    {
        this.analyzerService = analyzerService;
    }

    [FunctionName("PhotosAnalyzer")]
    public async Task<dynamic> Run([ActivityTrigger] List<byte> image)
    {
        return await analyzerService.AnalyzeAsync(image.ToArray());
    }
}