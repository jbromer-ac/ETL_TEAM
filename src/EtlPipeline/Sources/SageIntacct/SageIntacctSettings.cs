namespace EtlPipeline.Sources.SageIntacct;

public class SageIntacctSettings
{
    public string SenderId { get; set; } = string.Empty;
    public string SenderPassword { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = "https://api.intacct.com/ia/xml/xmlgw.phtml";

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(SenderId)    && !SenderId.Contains('<') &&
        !string.IsNullOrWhiteSpace(CompanyId)   && !CompanyId.Contains('<') &&
        !string.IsNullOrWhiteSpace(UserId)      && !UserId.Contains('<') &&
        !string.IsNullOrWhiteSpace(UserPassword) && !UserPassword.Contains('<');
}
