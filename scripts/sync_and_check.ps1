$ErrorActionPreference = 'Stop'
$baseUrl = 'http://localhost:5035'
try {
  $health = Invoke-RestMethod "$baseUrl/health"
  Write-Output 'HEALTH_OK'
  $health | ConvertTo-Json -Depth 5
} catch {
  Write-Output 'HEALTH_FAILED'
  Write-Output $_.Exception.Message
  exit 0
}

try {
  $body = @{ username = 'admin'; password = 'admin123' } | ConvertTo-Json
  $login = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $body -ContentType 'application/json'
  Write-Output 'LOGIN_OK'
  $login | ConvertTo-Json -Depth 5
} catch {
  Write-Output 'LOGIN_FAILED'
  Write-Output $_.Exception.Message
  exit 0
}

$token = $null
if ($login -is [System.Collections.IDictionary]) {
  if ($login.ContainsKey('token')) { $token = $login['token'] }
  elseif ($login.ContainsKey('accessToken')) { $token = $login['accessToken'] }
} else {
  try { $token = $login.token } catch { }
  try { if (-not $token) { $token = $login.accessToken } } catch { }
}

if (-not $token) {
  Write-Output 'NO_TOKEN'
  exit 0
}

$headers = @{ Authorization = "Bearer $token" }

try {
  $syncStudents = Invoke-RestMethod -Uri "$baseUrl/api/sync/students" -Method Post -Headers $headers
  Write-Output 'SYNC_STUDENTS_OK'
  $syncStudents | ConvertTo-Json -Depth 5
} catch {
  Write-Output 'SYNC_STUDENTS_FAILED'
  Write-Output $_.Exception.Message
}

try {
  $syncEvents = Invoke-RestMethod -Uri "$baseUrl/api/sync/events" -Method Post -Headers $headers
  Write-Output 'SYNC_EVENTS_OK'
  $syncEvents | ConvertTo-Json -Depth 5
} catch {
  Write-Output 'SYNC_EVENTS_FAILED'
  Write-Output $_.Exception.Message
}

try {
  $students = Invoke-RestMethod -Uri "$baseUrl/api/students?page=1&pageSize=10" -Method Get -Headers $headers
  Write-Output 'STUDENTS_OK'
  $students | ConvertTo-Json -Depth 5
} catch {
  Write-Output 'STUDENTS_FAILED'
  Write-Output $_.Exception.Message
}
