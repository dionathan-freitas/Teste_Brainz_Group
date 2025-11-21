$ErrorActionPreference = 'Stop'
$baseUrl = 'http://localhost:5035'
$body = @{ username = 'admin'; password = 'admin123' } | ConvertTo-Json
$login = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $body -ContentType 'application/json'
$token = $null

if ($login -is [System.Collections.IDictionary]) {
  if ($login.ContainsKey('token')) { $token = $login['token'] }
  elseif ($login.ContainsKey('accessToken')) { $token = $login['accessToken'] }
} else {
  if ($null -ne $login.token) { $token = $login.token }
  elseif ($null -ne $login.accessToken) { $token = $login.accessToken }
}

if (-not $token) { Write-Output 'NO_TOKEN'; exit 1 }

Write-Output "TOKEN_LEN: $($token.Length)"
$headers = @{ Authorization = "Bearer $token" }
try {
  $r = Invoke-RestMethod -Uri "$baseUrl/api/dev/seed-sample" -Method Post -Headers $headers
  Write-Output 'SEED_OK'
  $r | ConvertTo-Json
} catch {
  Write-Output 'SEED_FAILED'
  Write-Output $_.Exception.Message
}
