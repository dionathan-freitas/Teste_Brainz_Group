param([switch]$SkipInstall)
$ErrorActionPreference = 'Stop'
Write-Host '== Frontend run ==' -ForegroundColor Cyan
$frontendPath = "$PSScriptRoot\..\frontend"
if (-not (Test-Path $frontendPath)) { Write-Error 'Frontend folder not found'; exit 1 }
Push-Location $frontendPath
if (-not $SkipInstall) {
  if (Test-Path 'pnpm-lock.yaml') {
    Write-Host 'Installing dependencies (pnpm)...' -ForegroundColor Yellow
    & pnpm install
  } elseif (Test-Path 'package-lock.json') {
    Write-Host 'Installing dependencies (npm)...' -ForegroundColor Yellow
    & npm install
  } else {
    Write-Host 'Installing dependencies (npm default)...' -ForegroundColor Yellow
    & npm install
  }
}
Write-Host 'Starting Vite dev server...' -ForegroundColor Cyan
& npm run dev
Pop-Location
