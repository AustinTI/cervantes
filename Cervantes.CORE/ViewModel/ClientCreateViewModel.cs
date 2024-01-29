namespace Cervantes.CORE.ViewModel;

public class ClientCreateViewModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string ContactName { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhone { get; set; }
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
}