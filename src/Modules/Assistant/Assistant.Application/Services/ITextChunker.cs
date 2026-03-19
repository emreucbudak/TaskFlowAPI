namespace Assistant.Application.Services;

public interface ITextChunker
{
    IReadOnlyList<string> Split(string text, int targetSize, int overlapSize);
}
