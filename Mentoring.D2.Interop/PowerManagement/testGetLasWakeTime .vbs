set pm = CreateObject("PowerManagement.PowerManager")

a = pm.GetLastWakeTime()

WScript.Echo(a)