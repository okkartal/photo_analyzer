namespace Photos.Models;

public class PhotoUploadModel
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("tags")] public string Tags { get; set; }

    [JsonPropertyName("photo")] public string Photo { get; set; }
}