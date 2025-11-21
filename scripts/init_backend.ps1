param(
    [switch]$SkipTests,
    [switch]$SkipSeed,
    [string]$Configuration = 'Debug'
)
$ErrorActionPreference = 'Stop'
Write-Host '== Backend initialization ==' -ForegroundColor Cyan

$solutionPath = "$PSScriptRoot\..\Teste_Brainz_Group.sln"
$projectPath = "$PSScriptRoot\..\backend\StudentEventsAPI\StudentEventsAPI.csproj"
$testsPath = "$PSScriptRoot\..\backend\StudentEventsAPI.Tests\StudentEventsAPI.Tests.csproj"

if (-not (Test-Path $solutionPath)) { Write-Error 'Solution not found'; exit 1 }

Write-Host 'Restoring solution...' -ForegroundColor Yellow
& dotnet restore $solutionPath

Write-Host 'Building backend...' -ForegroundColor Yellow
& dotnet build $projectPath -c $Configuration

Write-Host 'Applying EF Core migrations...' -ForegroundColor Yellow
& dotnet ef database update --project $projectPath

if (-not $SkipTests) {
  Write-Host 'Running tests...' -ForegroundColor Yellow
  & dotnet test $testsPath -c $Configuration --no-build
}

if (-not $SkipSeed) {
  . "$PSScriptRoot/util_common.ps1"
  if (Wait-ApiReady -TimeoutSeconds 20) {
    $token = Get-JwtToken
    if ($token) {
      Write-Host 'Seeding sample data...' -ForegroundColor Yellow
      $r = Invoke-Api -Path 'api/dev/seed-sample' -Method POST -Token $token -Raw
      if ($r) { Write-Host 'Seed completed' -ForegroundColor Green }
    } else {
      Write-Warning 'Skipping seed (no token)'
    }
  } else {
    Write-Warning 'API not ready for seeding'
  }
}

Write-Host 'Starting backend (Ctrl+C to stop)...' -ForegroundColor Cyan
& dotnet run --project $projectPath -c $Configuration
