namespace studio_api.Contracts;

public record EmbeddingsMoment(
    string Title,
    string Subtitle,
    string Destination,
    string Vibe
);

public record EmbeddingsResult(
    List<float> Vector
);