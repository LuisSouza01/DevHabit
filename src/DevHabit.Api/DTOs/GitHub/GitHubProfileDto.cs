using DevHabit.Api.DTOs.Commons;

namespace DevHabit.Api.DTOs.GitHub;

public sealed record GitHubProfileDto : ILinksResponse
{
    public string Login { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }
    public string Bio { get; set; }
    public string PublicRepos { get; set; }
    public string Followers { get; set; }
    public string Following { get; set; }
    public List<LinkDto> Links { get; set; }
}
