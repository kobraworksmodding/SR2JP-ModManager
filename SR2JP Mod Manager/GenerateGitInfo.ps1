
$gitHash = git -C ".." rev-parse --short HEAD 2>$null
if (-not $gitHash) { $gitHash = "000000" }

@"
namespace SR2JP_Mod_Manager
{
    public static class GitInfo
    {
        public const string Hash = "$gitHash";
    }
}
"@ | Set-Content -Encoding UTF8 ".\GitInfo.cs"