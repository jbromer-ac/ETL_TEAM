using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtlPipeline.Sources.SageIntacct;

public class SageIntacctClient
{
    private readonly SageIntacctSettings _settings;
    private readonly HttpClient _http = new();
    private readonly ILogger<SageIntacctClient> _logger;

    public SageIntacctClient(IOptions<SageIntacctSettings> settings, ILogger<SageIntacctClient> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string?> GetSessionAsync(CancellationToken ct = default)
    {
        if (!_settings.IsConfigured)
        {
            _logger.LogWarning("[SageIntacct] Credentials not configured — skipping.");
            return null;
        }

        var controlId = Guid.NewGuid().ToString("N")[..8];
        var functionId = Guid.NewGuid().ToString();

        var xml = $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <request>
                <control>
                    <senderid>{_settings.SenderId}</senderid>
                    <password>{_settings.SenderPassword}</password>
                    <controlid>{controlId}</controlid>
                    <uniqueid>false</uniqueid>
                    <dtdversion>3.0</dtdversion>
                    <includewhitespace>false</includewhitespace>
                </control>
                <operation>
                    <authentication>
                        <login>
                            <userid>{_settings.UserId}</userid>
                            <companyid>{_settings.CompanyId}</companyid>
                            <password>{_settings.UserPassword}</password>
                        </login>
                    </authentication>
                    <content>
                        <function controlid="{functionId}">
                            <getAPISession>
                                <locationid></locationid>
                            </getAPISession>
                        </function>
                    </content>
                </operation>
            </request>
            """;

        try
        {
            var response = await _http.PostAsync(
                _settings.ApiUrl,
                new StringContent(xml, Encoding.UTF8, "text/xml"),
                ct);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync(ct);
            var doc = XDocument.Parse(body);

            var error = doc.Descendants("error").FirstOrDefault();
            if (error != null)
            {
                var errNo   = error.Element("errorno")?.Value;
                var desc    = error.Element("description")?.Value;
                var desc2   = error.Element("description2")?.Value;
                var correct = error.Element("correction")?.Value;
                _logger.LogError(
                    "[SageIntacct] API error {ErrNo}: {Desc} {Desc2} | Correction: {Correction}",
                    errNo, desc, desc2, correct);
                return null;
            }

            var sessionId = doc.Descendants("sessionid").FirstOrDefault()?.Value;

            if (!string.IsNullOrWhiteSpace(sessionId))
                _logger.LogInformation("[SageIntacct] Session obtained successfully for company: {CompanyId}", _settings.CompanyId);
            else
                _logger.LogWarning("[SageIntacct] Response succeeded but no session ID found.");

            return sessionId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SageIntacct] Failed to obtain session: {Message}", ex.Message);
            return null;
        }
    }
}
