# Common utility functions for StudentEvents project
# Requires: PowerShell 5.1+
# Usage: . .\util_common.ps1

$Script:BaseUrl = if ($env:STUDENT_EVENTS_API) { $env:STUDENT_EVENTS_API } else { 'http://localhost:5035' }

function Get-JwtToken {
    [Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSAvoidUsingPlainTextForPassword', '', Justification='Dev credentials')]
    param(
        [string]$Username = 'admin',
        [string]$UserPassword = 'admin123'
    )
    $body = @{ username = $Username; password = $UserPassword } | ConvertTo-Json
    try {
        $login = Invoke-RestMethod -Uri "$Script:BaseUrl/api/auth/login" -Method Post -Body $body -ContentType 'application/json'
    } catch {
        Write-Error "Login request failed: $($_.Exception.Message)"
        return $null
    }
    $token = $null
    if ($login -is [System.Collections.IDictionary]) {
        if ($login.ContainsKey('token')) { $token = $login['token'] }
        elseif ($login.ContainsKey('accessToken')) { $token = $login['accessToken'] }
    } else {
        try { $token = $login.token } catch {}
        try { if (-not $token) { $token = $login.accessToken } } catch {}
    }
    if (-not $token) { Write-Warning 'No token returned'; return $null }
    return $token
}

function Invoke-Api {
    param(
        [Parameter(Mandatory)][string]$Path,
        [ValidateSet('GET','POST','PUT','DELETE','PATCH')][string]$Method = 'GET',
        [object]$Body = $null,
        [string]$Token = $null,
        [int]$Depth = 5,
        [switch]$Raw
    )
    $headers = @{}
    if ($Token) { $headers['Authorization'] = "Bearer $Token" }
    $uri = ("{0}/{1}" -f $Script:BaseUrl.TrimEnd('/'), $Path.TrimStart('/'))
    try {
        if ($Body) { $json = $Body | ConvertTo-Json -Depth $Depth } else { $json = $null }
        $result = Invoke-RestMethod -Uri $uri -Method $Method -Headers $headers -Body $json -ContentType 'application/json'
        if ($Raw) { return $result } else { return ($result | ConvertTo-Json -Depth $Depth) }
    } catch {
        Write-Error "API call failed ($Method $uri): $($_.Exception.Message)"
        return $null
    }
}

function Wait-ApiReady {
    param([int]$TimeoutSeconds = 30)
    $stopAt = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $stopAt) {
        try {
            $r = Invoke-WebRequest -Uri "$Script:BaseUrl/health" -UseBasicParsing -TimeoutSec 5
            if ($r.StatusCode -ge 200 -and $r.StatusCode -lt 300) { Write-Host 'API ready'; return $true }
        } catch { Start-Sleep -Milliseconds 500 }
    }
    Write-Warning 'API not ready within timeout'
    return $false
}
