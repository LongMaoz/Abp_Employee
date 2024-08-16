namespace GrpcService.Model;

public class EmailRequestSimpleModel
{
    public string ToAddress { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string HtmlBody { get; set; } = string.Empty;

    public string Remark { get; set; } = string.Empty;
}