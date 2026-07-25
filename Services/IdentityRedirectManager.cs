using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace blazor_todo_list.Services;

/// <summary>
/// Account pages (Register/Login/Logout/ProfileEdit) render as static server-side
/// components so they have a real HttpContext to sign the Identity cookie. A plain
/// NavigationManager.NavigateTo during that phase needs "forceLoad: true" and needs the
/// current request to stop immediately afterwards - this helper does both consistently.
/// </summary>
internal sealed class IdentityRedirectManager(NavigationManager navigationManager)
{
    [DoesNotReturn]
    public void RedirectTo(string? uri)
    {
        uri ??= "";

        // Prevent open redirects.
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = "/";
        }

        var newUri = navigationManager.ToAbsoluteUri(uri).ToString();
        navigationManager.NavigateTo(newUri, forceLoad: true);
        throw new InvalidOperationException($"{nameof(IdentityRedirectManager)} can only be used during static server-side rendering.");
    }

    [DoesNotReturn]
    public void RedirectTo(string uri, IReadOnlyDictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        RedirectTo(newUri);
    }
}
