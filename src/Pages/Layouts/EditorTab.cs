namespace GTDialogueViewer.Layout;

public class EditorTab
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FilePath { get; set; } = string.Empty;
    public string FileName => System.IO.Path.GetFileName(FilePath);
    public string Content { get; set; } = string.Empty;
    public bool IsModified { get; set; }
}