﻿namespace Photos.AnalyzerService.Abstractions;

public interface IAnalyzerService
{
    Task<dynamic> AnalyzeAsync(byte[] image);
}