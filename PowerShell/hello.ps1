$wshell = New-Object -ComObject Wscript.Shell
$intAnswer = $wshell.Popup("Dag collega, kun je even een oogje werpen op eventuele kritische abends?`nControletoren dankt u!",0,"Poke a mon...itor",0x1)
$wshell.Popup("$intAnswer")