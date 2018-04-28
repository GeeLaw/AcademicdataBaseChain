Function New-ABCEntity
{
    Param
    (
        [string]$Name,
        [string]$AccountAddress
    )
    Process
    {
        $local:result = 0 | Select-Object -Property 'Name', 'AccountAddress', 'IsEntity';
        $local:result.Name = $Name;
        $local:result.AccountAddress = $AccountAddress;
        $local:result.IsEntity = $true;
        Return $local:result;
    }
}

$script:ChainName = '<chain name here>';
$script:ContractName = '<contract name here>';
$script:ApiKey = '<API key here>';

$global:BoEducation = New-ABCEntity -Name 'Bureau of Education' -AccountAddress '<bureau of education address>';
$global:TsinghuaUniv = New-ABCEntity -Name 'Tsinghua University' -AccountAddress '<school 1 address>';
$global:UoWaterloo = New-ABCEntity -Name 'University of Waterloo' -AccountAddress '<school 2 address>';
$global:GeeLaw = New-ABCEntity -Name 'Gee Law' -AccountAddress '<person 1 address>';
$global:TingfungLau = New-ABCEntity -Name 'Ting Fung Lau' -AccountAddress '<person 2 address>';
$global:WangbinSun = New-ABCEntity -Name 'Wangbin Sun' -AccountAddress '<person 3 address>';
$global:CongrongMa = New-ABCEntity -Name 'Congrong Ma' -AccountAddress '<person 4 address>';
$global:JingyiWang = New-ABCEntity -Name 'Jingyi Wang' -AccountAddress '<person 5 address>';
$global:ChelseaLiu = New-ABCEntity -Name 'Chelsea Liu' -AccountAddress '<person 6 address>';

Function Resolve-ABCEntity
{
    [CmdletBinding()]
    Param
    (
        [object]$InputObject
    )
    Process
    {
        Write-Verbose "Resolving: $InputObject.";
        If ($InputObject.IsEntity)
        {
            Write-Verbose 'No need to resolve.';
            Return $InputObject;
        }
        If ($InputObject -is [string])
        {
            $local:resolved = @(@($global:BoEducation, $global:TsinghuaUniv, $global:UoWaterloo,
                $global:GeeLaw, $global:TingfungLau, $global:WangbinSun,
                $global:CongrongMa, $global:JingyiWang, $global:ChelseaLiu) |
                Where-Object Name -like "*$InputObject*");
            If ($resolved.Count -ne 1)
            {
                Write-Error -Message "Could not resolve $InputObject." -Category ObjectNotFound -TargetObject $resolved;
                Return $null;
            }
            $InputObject = $resolved[0];
            Write-Verbose "Resolved as $($InputObject.Name) [$($InputObject.AccountAddress)].";
            Return $InputObject;
        }
        Write-Error -Message 'Invalid entity to be resolved.' -Category InvalidArgument -TargetObject $InputObject;
        Return $null;
    }
}

Function Register-ABCSchool
{
    [CmdletBinding()]
    Param
    (
        [object]$School,
        [object]$As = $global:BoEducation
    )
    Process
    {
        $School = Resolve-ABCEntity -InputObject $School;
        $As = Resolve-ABCEntity -InputObject $As;
        $local:payloadQ = @{ 'func' = 'markAsSchool'; 'params' = @($School.AccountAddress) };
        $payloadQ = $payloadQ | ConvertTo-Json -Compress -Depth 32;
        Write-Host $payloadQ;
        $payloadQ = [System.Text.Encoding]::UTF8.GetBytes($payloadQ);
        $local:payloadI = @{ 'func' = 'markAsSchool'; 'params' = @($School.AccountAddress); 'account' = $As.AccountAddress };
        $payloadI = $payloadI | ConvertTo-Json -Compress -Depth 32;
        Write-Host $payloadI;
        $payloadI = [System.Text.Encoding]::UTF8.GetBytes($payloadI);
        Invoke-RestMethod -Uri "https://baas.ink.plus/public-api/call/$ChainName/$ContractName/query?apikey=$ApiKey" `
            -Method Post -UseBasicParsing -ContentType 'application/json; charset=utf-8' -Body $payloadI;
        Invoke-RestMethod -Uri "https://baas.ink.plus/public-api/call/$ChainName/$ContractName/invoke?apikey=$ApiKey" `
            -Method Post -UseBasicParsing -ContentType 'application/json; charset=utf-8' -Body $payloadI;
    }
}

Function Unregister-ABCSchool
{
    [CmdletBinding()]
    Param
    (
        [object]$School,
        [object]$As = $global:BoEducation
    )
    Process
    {
        $School = Resolve-ABCEntity -InputObject $School;
        $As = Resolve-ABCEntity -InputObject $As;
        $local:payloadQ = @{ 'func' = 'unmarkAsSchool'; 'params' = @($School.AccountAddress) };
        $payloadQ = $payloadQ | ConvertTo-Json -Compress -Depth 32;
        Write-Host $payloadQ;
        $payloadQ = [System.Text.Encoding]::UTF8.GetBytes($payloadQ);
        $local:payloadI = @{ 'func' = 'unmarkAsSchool'; 'params' = @($School.AccountAddress); 'account' = $As.AccountAddress };
        $payloadI = $payloadI | ConvertTo-Json -Compress -Depth 32;
        Write-Host $payloadI;
        $payloadI = [System.Text.Encoding]::UTF8.GetBytes($payloadI);
        Invoke-RestMethod -Uri "https://baas.ink.plus/public-api/call/$ChainName/$ContractName/query?apikey=$ApiKey" `
            -Method Post -UseBasicParsing -ContentType 'application/json; charset=utf-8' -Body $payloadI;
        Invoke-RestMethod -Uri "https://baas.ink.plus/public-api/call/$ChainName/$ContractName/invoke?apikey=$ApiKey" `
            -Method Post -UseBasicParsing -ContentType 'application/json; charset=utf-8' -Body $payloadI;
    }
}

Unregister-ABCSchool 'Tsinghua';
