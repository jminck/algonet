rm ~\_vix_under_$vixMax.txt
rm ~\vix_under_$vixMax.txt
$r = $null
$vixMax = 15

$url = "https://finance.yahoo.com/quote/%5EVIX?p=%5EVIX" 
$webrequest = Invoke-WebRequest -Uri $url -SessionVariable websession 
$cookies = $websession.Cookies.GetCookies($url) 
 
# Here, you can output all of $cookies, or you can go through them one by one. 
 
foreach ($cookie in $cookies) { 
     # You can get cookie specifics, or just use $cookie 
     # This gets each cookie's name and value 
     Write-Host "$($cookie.name) = $($cookie.value)" 
}
$spl = $webrequest.ToString()
$s = $spl.Split(",")
foreach ($line in $s) 
{
    if($line -like "*CrumbStore*")
    { write-host $line
      $splitline = $line.Split(":")
      $crumb = $splitline[2].Substring(1, $splitline[2].Length - 3)
    }
}
write-host crumb is $crumb

$url = "https://query1.finance.yahoo.com/v7/finance/download/%5EVIX?period1=1293948000&period2=1513663200&interval=1d&events=history&crumb=$crumb"
$webrequest = Invoke-WebRequest -Uri $url -WebSession $websession 
$webrequest.Content | Out-File ~\vix_history.csv

$VIX = import-csv ~\vix_history.csv


   for ($i=8; $i -le $vixMax; $i++)
    {
        foreach($rec in $VIX) 
        {
 
        if (([math]::Round($rec.Low, 1) -le [int]$i) -and ([math]::Round($rec.Low, 1) -ge [int]$i-1))
        {
            if ($rec.Low -lt $vixMax)
                {
                $matchingRecords += $rec.Date 
                $d = "new DateTime(" + $rec.Date.Split("-")[0] + ", " + $rec.Date.Split("-")[1] + ", " + $rec.Date.Split("-")[2] + "),"
                write-host $d
                $d |  out-file -FilePath ~\_vix_under_$vixMax.txt -Append -Encoding utf8
            }
        }
    
    }
    $i |  out-file -FilePath ~\_vix_under_$vixMax.txt -Append -Encoding utf8
}
Write-Host VIX max set to $vixMax
Write-Host total count $matchingRecords.count

#rewrite the file into one line wrapped every 7 items
$dates = Get-Content ~\_vix_under_$vixMax.txt

$count = 0
$line = $null

	
#for ($i=8; $i -le $vixMax; $i++) 
#{
    foreach ($day in $dates) {
        $count++
        $line += $day
        if($count -ge 7)
            {
                Write-host $line
                $line | out-file ~\vix_under_$vixMax.txt -Append -Encoding utf8
                $count = 0
                $line = $null
            }
    }
    $end = "****** END OF " + $i + " ******"
    $end | out-file ~\vix_under_$vixMax.txt -Append -Encoding utf8
    $count = 0
    $line = $null
#}
