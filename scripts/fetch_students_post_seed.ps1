$ErrorActionPreference = 'Stop'
$base = 'http://localhost:5035'
$body = @{ username = 'admin'; password = 'admin123' } | ConvertTo-Json
$login = Invoke-RestMethod -Uri "$base/api/auth/login" -Method Post -Body $body -ContentType 'application/json'
$token = $null
if ($login -is [System.Collections.IDictionary]) {
  if ($login.ContainsKey('token')) { $token = $login['token'] }
  elseif ($login.ContainsKey('accessToken')) { $token = $login['accessToken'] }
} else {
  try { $token = $login.token } catch { }
  try { if (-not $token) { $token = $login.accessToken } } catch { }
}

if (-not $token) { Write-Output 'NO_TOKEN'; exit 1 }

$headers = @{ Authorization = "Bearer $token" }
try {
  $students = Invoke-RestMethod -Uri "$base/api/students?page=1&pageSize=10" -Method Get -Headers $headers
  Write-Output 'STUDENTS:'
  $students | ConvertTo-Json -Depth 5
} catch {
  Write-Output 'FAILED:'
  Write-Output $_.Exception.Message
}
