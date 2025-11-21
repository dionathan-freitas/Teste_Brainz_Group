$ErrorActionPreference = 'Stop'
$baseUrl = 'http://localhost:5035'
$body = @{ username = 'admin'; password = 'admin123' } | ConvertTo-Json
$login = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $body -ContentType 'application/json'
Write-Output 'LOGIN:'; $login | ConvertTo-Json -Depth 5
$token = $null
if ($login -is [System.Collections.IDictionary]) {
  if ($login.ContainsKey('token')) { $token = $login['token'] }
  elseif ($login.ContainsKey('accessToken')) { $token = $login['accessToken'] }
} else { try { $token = $login.token } catch {} ; try { if (-not $token) { $token = $login.accessToken } } catch {} }
Write-Output 'TOKEN_LEN:'; if ($token) { Write-Output $token.Length } else { Write-Output 'NO_TOKEN' ; exit 0 }
$headers = @{ Authorization = "Bearer $token" }
try {
  $r = Invoke-WebRequest -Uri "$baseUrl/api/students?page=1&pageSize=1" -Method Get -Headers $headers -UseBasicParsing
  Write-Output 'STUDENTS_OK'; Write-Output $r.StatusCode; Write-Output $r.Content
} catch {
  if ($_.Exception.Response) {
    $resp = $_.Exception.Response
    $sr = New-Object System.IO.StreamReader($resp.GetResponseStream())
    Write-Output 'STATUS_EXCEPTION:'; try { Write-Output $resp.StatusCode } catch { }
    Write-Output 'CONTENT_EXCEPTION:'; Write-Output $sr.ReadToEnd()
    Write-Output 'HEADERS:'; try { $resp.Headers.AllKeys | ForEach-Object { "$($_): $($resp.Headers[$_])" } } catch { }
  } else { Write-Output 'NO_RESPONSE'; Write-Output $_.Exception.Message }
}
