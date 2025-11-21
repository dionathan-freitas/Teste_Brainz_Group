param([switch]$SkipInstall,[switch]$SkipSeed)
$ErrorActionPreference = 'Stop'
Write-Host '== Full dev environment ==' -ForegroundColor Cyan

$backendProject = "$PSScriptRoot\..\backend\StudentEventsAPI\StudentEventsAPI.csproj"
$frontendPath = "$PSScriptRoot\..\frontend"

if (-not (Test-Path $backendProject)) { Write-Error 'Backend project missing'; exit 1 }
if (-not (Test-Path $frontendPath)) { Write-Error 'Frontend folder missing'; exit 1 }

# Start backend
Write-Host '[1/2] Starting backend...' -ForegroundColor Yellow
$backendJob = Start-Job -ScriptBlock {
    param($proj,$skipSeed,$scriptRoot)
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    & dotnet run --project $proj
} -ArgumentList $backendProject,$SkipSeed,$PSScriptRoot
Start-Sleep -Seconds 5

# Start frontend
Write-Host '[2/2] Starting frontend...' -ForegroundColor Yellow
if (-not $SkipInstall) {
  if (Test-Path "$frontendPath\pnpm-lock.yaml") { pnpm install } else { npm install }
}
Push-Location $frontendPath
$frontendJob = Start-Job -ScriptBlock { npm run dev }
Pop-Location

Write-Host 'Backend Job Id: ' $backendJob.Id -ForegroundColor Cyan
Write-Host 'Frontend Job Id: ' $frontendJob.Id -ForegroundColor Cyan
Write-Host 'Use Receive-Job -Id <id> to view output. Use Stop-Job to stop.' -ForegroundColor Green
