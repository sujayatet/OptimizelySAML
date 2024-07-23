namespace OptimizelySAML.Models.Pages
{
    [ContentType(DisplayName = "AccountPage",
        GUID = "0bc38c4e-c37d-4e05-b80b-6f54b9393beb",
        Description = "Page with Azure AD login button")]
    [AvailableContentTypes(
    Availability = Availability.Specific,
    IncludeOn = new[] { typeof(StartPage) })]
    public class AccountPage : SitePageData
    {
    }
}
