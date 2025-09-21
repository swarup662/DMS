﻿using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class PermissionHtmlProcessor
{
    private readonly PermissionService _permissionService;

    public PermissionHtmlProcessor(PermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// Processes HTML string and removes <has-permission module-id="x" action-id="y"> blocks
    /// if the user doesn't have permission
    /// </summary>
    public async Task<string> ProcessAsync(string html)
    {
        if (string.IsNullOrEmpty(html)) return html;

        // Regex to match <has-permission module-id="1" action-id="2">...</has-permission>
        var regex = new Regex(
            @"<has-permission\s+module-id=""(?<module>\d+)""\s+action-id=""(?<action>\d+)"">(?<inner>.*?)</has-permission>",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        var matches = regex.Matches(html);

            foreach (Match match in matches)
            {
                var moduleId = int.Parse(match.Groups["module"].Value);
                var actionId = int.Parse(match.Groups["action"].Value);
                var innerHtml = match.Groups["inner"].Value;

                var hasPerm = await _permissionService.HasPermissionAsync(moduleId, actionId);

                if (hasPerm)
                {
                    // replace full tag with inner content
                    html = html.Replace(match.Value, innerHtml);
                }
                else
                {
                    // remove entire tag
                    html = html.Replace(match.Value, string.Empty);
                }
            }

        return html;
    }
}
