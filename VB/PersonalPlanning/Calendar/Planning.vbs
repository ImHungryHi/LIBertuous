Rem Attribute VBA_ModuleType=VBAModule
Option VBASupport 1

Sub btnExportTime(intYear As Integer)
    Dim strReturn As String, strFolder As String, strFileName As String, strCreateFolder As String, strCreateFileName As String, strParentFolder As String
    Dim intMonthStart As Integer, intCol As Integer, intColStart As Integer, intDay As Integer, intMonthEnd As Integer, intCurMonth As Integer, intTimeRow As Integer, intPeriod As Integer
    Dim wrkTimesheet As Workbook, wrkPlanning As Workbook
    Dim fileObject, fileCreated, blnGo
    Dim arrTime(1 To 50, 1 To 2) As Variant, arrExtraTime(1 To 20, 1 To 2) As Variant, arrInfo(1 To 50, 1 To 2) As Variant, arrExtraInfo(1 To 20, 1 To 2) As Variant
    
    intColStart = 2
    intCol = 2
    intDay = 0
    intMonthEnd = 0
    intTimeRow = 1
    intPeriod = 1
    strReturn = InputBox("Gelieve de maand op te geven:")
    Set fileObject = CreateObject("Scripting.FileSystemObject")
    blnGo = False
    
    Select Case True
        Case InStr(strReturn, "anuari") Or strReturn = "01" Or strReturn = "1"
            intMonthStart = 4
            strFolder = "01 - Januari"
            strFileName = "Timesheet KBC Januari " & intYear
            intCurMonth = 1
        Case InStr(strReturn, "ebruari") Or strReturn = "02" Or strReturn = "2"
            intMonthStart = 7
            strFolder = "02 - Februari"
            strFileName = "Timesheet KBC Februari " & intYear
            strCreateFolder = "01 - Januari"
            strCreateFileName = "Timesheet KBC Januari " & intYear
            intCurMonth = 2
        Case InStr(strReturn, "aart") Or strReturn = "03" Or strReturn = "3"
            intMonthStart = 10
            strFolder = "03 - Maart"
            strFileName = "Timesheet KBC Maart " & intYear
            strCreateFolder = "02 - Februari"
            strCreateFileName = "Timesheet KBC Februari " & intYear
            intCurMonth = 3
        Case InStr(strReturn, "pril") Or strReturn = "04" Or strReturn = "4"
            intMonthStart = 13
            strFolder = "04 - April"
            strFileName = "Timesheet KBC April " & intYear
            strCreateFolder = "03 - Maart"
            strCreateFileName = "Timesheet KBC Maart " & intYear
            intCurMonth = 4
        Case InStr(strReturn, "ei") Or strReturn = "05" Or strReturn = "5"
            intMonthStart = 16
            strFolder = "05 - Mei"
            strFileName = "Timesheet KBC Mei " & intYear
            strCreateFolder = "04 - April"
            strCreateFileName = "Timesheet KBC April " & intYear
            intCurMonth = 5
        Case InStr(strReturn, "uni") Or strReturn = "06" Or strReturn = "6"
            intMonthStart = 19
            strFolder = "06 - Juni"
            strFileName = "Timesheet KBC Juni " & intYear
            strCreateFolder = "05 - Mei"
            strCreateFileName = "Timesheet KBC Mei " & intYear
            intCurMonth = 6
        Case InStr(strReturn, "uli") Or strReturn = "07" Or strReturn = "7"
            intMonthStart = 22
            strFolder = "07 - Juli"
            strFileName = "Timesheet KBC Juli " & intYear
            strCreateFolder = "06 - Juni"
            strCreateFileName = "Timesheet KBC Juni " & intYear
            intCurMonth = 7
        Case InStr(strReturn, "ugustus") Or strReturn = "08" Or strReturn = "8"
            intMonthStart = 25
            strFolder = "08 - Augustus"
            strFileName = "Timesheet KBC Augustus " & intYear
            strCreateFolder = "07 - Juli"
            strCreateFileName = "Timesheet KBC Juli " & intYear
            intCurMonth = 8
        Case InStr(strReturn, "eptember") Or strReturn = "09" Or strReturn = "9"
            intMonthStart = 28
            strFolder = "09 - September"
            strFileName = "Timesheet KBC September " & intYear
            strCreateFolder = "08 - Augustus"
            strCreateFileName = "Timesheet KBC Augustus " & intYear
            intCurMonth = 9
        Case InStr(strReturn, "ktober") Or strReturn = "10"
            intMonthStart = 31
            strFolder = "10 - Oktober"
            strFileName = "Timesheet KBC Oktober " & intYear
            strCreateFolder = "09 - September"
            strCreateFileName = "Timesheet KBC September " & intYear
            intCurMonth = 10
        Case InStr(strReturn, "ovember") Or strReturn = "11"
            intMonthStart = 34
            strFolder = "11 - November"
            strFileName = "Timesheet KBC November " & intYear
            strCreateFolder = "10 - Oktober"
            strCreateFileName = "Timesheet KBC Oktober " & intYear
            intCurMonth = 11
        Case InStr(strReturn, "ecember") Or strReturn = "12"
            intMonthStart = 37
            strFolder = "12 - December"
            strFileName = "Timesheet KBC December " & intYear
            strCreateFolder = "11 - November"
            strCreateFileName = "Timesheet KBC November " & intYear
            intCurMonth = 12
        Case Else
            MsgBox ("Hmm...")
    End Select
    
    For x = 1 To Workbooks.Count
        If (InStr(1, Workbooks(x).Name, "Planning.xlsm")) Then
            Set wrkPlanning = Workbooks(x)
        End If
        
        If (InStr(1, Workbooks(x).Name, strFileName)) Then
            Set wrkTimesheet = Workbooks(x)
        End If
    Next x
    
    With CreateObject("Scripting.FileSystemObject")
        strParentFolder = .GetParentFolderName(wrkPlanning.Path)
    End With
    
    If (fileObject.FileExists(strParentFolder & intYear & "\" & strCreateFolder & "\" & strCreateFileName & ".xlsx")) Then
        If Not (fileObject.FolderExists(strParentFolder & intYear & "\" & strFolder)) Then
            fileObject.CreateFolder (strParentFolder & intYear & "\" & strFolder)
        End If
        
        If Not (fileObject.FileExists(strParentFolder & intYear & "\" & strFolder & "\" & strFileName & ".xlsx")) Then
            fileCreated = fileObject.CopyFile(strParentFolder & intYear & "\" & strCreateFolder & "\" & strCreateFileName & ".xlsx", strParentFolder & intYear & "\" & strFolder & "\" & strFileName & ".xlsx")
        End If
        
        blnGo = True
    ElseIf (fileObject.FileExists(strParentFolder & intYear & "\" & strFolder & "\" & strFileName & ".xlsx")) Then
        blnGo = True
    Else
        MsgBox ("Trump didn't succeed in deporting the Mexicans, you're planning too far ahead into the future")
        blnGo = False
    End If
    
    If (blnGo) Then
        If (wrkTimesheet Is Nothing) Then
            Set wrkTimesheet = Workbooks.Open(strParentFolder & intYear & "\" & strFolder & "\" & strFileName & ".xlsx")
        End If
        
        wrkTimesheet.Worksheets("Time Sheet").Cells(8, 4) = DateValue(1 & "-" & intCurMonth & "-" & intYear)
        
        Dim datEnd As Date
        
        If (intCurMonth < 12) Then
            datEnd = DateSerial(intYear, intCurMonth + 1, 0)
        Else
            datEnd = DateValue("31-12-2017")
        End If
        
        wrkTimesheet.Worksheets("Time Sheet").Cells(74, 4) = DateAdd("d", 1, datEnd)
        wrkTimesheet.Worksheets("Time Sheet").Cells(74, 7) = DateAdd("d", 1, datEnd)
        wrkTimesheet.Worksheets("Time Sheet").Cells(10, 4) = datEnd
        
        For intCol = intColStart To intColStart + 30
            intMonthEnd = intMonthEnd + 1
            
            If (wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart + 2, intCol) <> False) Then
                arrInfo(intTimeRow, 1) = DateValue(intMonthEnd & "-" & intCurMonth & "-" & intYear)
                arrTime(intTimeRow, 1) = wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol).Value
                
                If (wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol) > TimeValue("20:30")) Then
                    arrInfo(intTimeRow, 2) = "Half a Working Day"
                    arrTime(intTimeRow, 2) = wrkPlanning.Worksheets("Time " & intYear).Cells(2, 13)
                    
                    intTimeRow = intTimeRow + 1
                    
                    If (wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, (intCol + 1)) = "X" Or (intCol + 1) = 33) Then
                        arrInfo(intTimeRow, 1) = DateValue(1 & "-" & (intCurMonth + 1) & "-" & intYear)
                    Else
                        arrInfo(intTimeRow, 1) = DateValue((intMonthEnd + 1) & "-" & intCurMonth & "-" & intYear)
                    End If
                    
                    arrInfo(intTimeRow, 2) = "Half a Working Day"
                    arrTime(intTimeRow, 1) = wrkPlanning.Worksheets("Time " & intYear).Cells(2, 15).Value
                Else
                    arrInfo(intTimeRow, 2) = "Working Day"
                End If
                
                arrTime(intTimeRow, 2) = wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart + 1, intCol).Value
                intDay = intDay + 1
                intTimeRow = intTimeRow + 1
            ElseIf Not (wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol).Comment Is Nothing Or wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart + 1, intCol).Comment Is Nothing) Then
                Dim arrStartComments() As String, arrEndComments() As String, blnSpecial As Boolean
                blnSpecial = False
            
                arrStartComments = Split(wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol).Comment.Text, Chr(10))
                arrEndComments = Split(wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart + 1, intCol).Comment.Text, Chr(10))
                
                For j = LBound(arrStartComments) To UBound(arrStartComments)
                    Dim dtStartTime As Date, dtEndTime As Date, dtTemp As Date
                    Dim intDOW As Integer
                    Dim blnHoliday As Boolean
                    
                    dtStartTime = TimeValue(Left(arrStartComments(j), 5))
                    dtEndTime = TimeValue(Left(arrEndComments(j), 5))
                    dtTemp = DateValue(intMonthEnd & "-" & intCurMonth & "-" & intYear)
                    intDOW = Weekday(DateValue(intMonthEnd & "-" & intCurMonth & "-" & intYear), vbMonday)  'Make monday the first day
                    
                    If (dtTemp = DateValue("1/1/" & intYear)) Then
                        blnHoliday = True
                    ElseIf (dtTemp = DateValue("1/5/" & intYear)) Then
                        blnHoliday = True
                    ElseIf (dtTemp = DateValue("11/7/" & intYear)) Then
                        blnHoliday = True
                    ElseIf (dtTemp = DateValue("21/7/" & intYear)) Then
                        blnHoliday = True
                    ElseIf (dtTemp = DateValue("15/8/" & intYear)) Then
                        blnHoliday = True
                    ElseIf (dtTemp = DateValue("1/11/" & intYear)) Then
                        blnHoliday = True
                    ElseIf (dtTemp = DateValue("11/11/" & intYear)) Then
                        blnHoliday = True
                    ElseIf (dtTemp = DateValue("25/12/" & intYear)) Then
                        blnHoliday = True
                    ElseIf (dtTemp = DateValue("26/12/" & intYear)) Then
                        blnHoliday = True
                    Else
                        blnHoliday = False
                    End If
                    
                    If (Hour(dtStartTime) >= 5) Then
                        arrExtraInfo(intPeriod, 1) = DateValue(intMonthEnd & "-" & intCurMonth & "-" & intYear)
                        
                        If (Hour(dtStartTime) < 22) Then
                            If ((intDOW = 6) And Not (blnHoliday Or wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol) = "FO" Or wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol) = "FL" Or wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol) = "FN")) Then
                                arrExtraInfo(intPeriod, 2) = "150% (rate for Saturdays)"
                            Else
                                arrExtraInfo(intPeriod, 2) = "200% (rate for nights, Sundays and public holidays)"
                                blnSpecial = True
                            End If
                        Else
                            arrExtraInfo(intPeriod, 2) = "200% (rate for nights, Sundays and public holidays)"
                        End If
                    Else
                        arrExtraInfo(intPeriod, 1) = DateValue(intMonthEnd + 1 & "-" & intCurMonth & "-" & intYear)
                    End If
                    
                    arrExtraTime(intPeriod, 1) = dtStartTime
                    
                    If (Hour(dtStartTime) < 22 And Hour(dtStartTime) >= 6 And (Hour(dtEndTime) > 21 Or Hour(dtEndTime) < 6)) Then
                        Dim strTemp As String
                        strTemp = wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol)
                        If (wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol) = "FL" Or wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol) = "FN") Then
                            If (Hour(dtStartTime) < 23 And Hour(dtEndTime) < 21 And Hour(dtEndTime) < 12) Then
                                arrExtraTime(intPeriod, 2) = TimeValue("23:59:00")
                                
                                intPeriod = intPeriod + 1
                                arrExtraTime(intPeriod, 1) = TimeValue("00:00:00")
                                arrExtraInfo(intPeriod, 1) = DateValue((intMonthEnd + 1) & "-" & intCurMonth & "-" & intYear)
                                arrExtraInfo(intPeriod, 2) = "200% (rate for nights, Sundays and public holidays)"
                                arrExtraTime(intPeriod, 2) = dtEndTime
                            Else
                                arrExtraTime(intPeriod, 2) = dtEndTime
                            End If
                        Else
                            If (Not blnSpecial) Then
                                arrExtraTime(intPeriod, 2) = TimeValue("21:59:00")
                                intPeriod = intPeriod + 1
                                arrExtraTime(intPeriod, 1) = TimeValue("22:00:00")
                                arrExtraInfo(intPeriod, 1) = DateValue(intMonthEnd & "-" & intCurMonth & "-" & intYear)
                                arrExtraInfo(intPeriod, 2) = "200% (rate for nights, Sundays and public holidays)"
                            End If
                        
                            If (Hour(dtEndTime) < 6) Then
                                arrExtraTime(intPeriod, 2) = TimeValue("23:59:00")
                                intPeriod = intPeriod + 1
                                arrExtraTime(intPeriod, 1) = TimeValue("00:00:00")
                                arrExtraInfo(intPeriod, 1) = DateValue((intMonthEnd + 1) & "-" & intCurMonth & "-" & intYear)
                                arrExtraInfo(intPeriod, 2) = "200% (rate for nights, Sundays and public holidays)"
                            End If
                            
                            arrExtraTime(intPeriod, 2) = dtEndTime
                        End If
                    Else
                        If (Hour(dtStartTime) <= 24 And Hour(dtEndTime) < 22) Then
                            If (Hour(dtEndTime) < 12 And Hour(dtStartTime) > 12) Then
                                If (Hour(dtStartTime) < 22 And Not blnSpecial) Then
                                    arrExtraTime(intPeriod, 2) = TimeValue("21:59:00")
                                    intPeriod = intPeriod + 1
                                    arrExtraTime(intPeriod, 1) = TimeValue("22:00:00")
                                    arrExtraInfo(intPeriod, 1) = DateValue(intMonthEnd & "-" & intCurMonth & "-" & intYear)
                                    arrExtraInfo(intPeriod, 2) = "200% (rate for nights, Sundays and public holidays)"
                                End If
                                
                                arrExtraTime(intPeriod, 2) = TimeValue("23:59:00")
                                intPeriod = intPeriod + 1
                                arrExtraTime(intPeriod, 1) = TimeValue("00:00:00")
                                arrExtraInfo(intPeriod, 1) = DateValue((intMonthEnd + 1) & "-" & intCurMonth & "-" & intYear)
                                arrExtraInfo(intPeriod, 2) = "200% (rate for nights, Sundays and public holidays)"
                            End If
                            
                            If (Hour(dtStartTime) < 6 And Hour(dtEndTime) <= 6) Then
                                arrExtraInfo(intPeriod, 2) = "200% (rate for nights, Sundays and public holidays)"
                            End If
                            
                            arrExtraTime(intPeriod, 2) = dtEndTime
                        ElseIf (Hour(dtStartTime) < 24 And Hour(dtStartTime) > 21 And Hour(dtEndTime) > 21) Then
                            arrExtraTime(intPeriod, 1) = dtStartTime
                            arrExtraTime(intPeriod, 2) = dtEndTime
                            arrExtraInfo(intPeriod, 1) = DateValue(intMonthEnd & "-" & intCurMonth & "-" & intYear)
                            arrExtraInfo(intPeriod, 2) = "200% (rate for nights, Sundays and public holidays)"
                        End If
                    End If
                    
                    intPeriod = intPeriod + 1
                Next j
            End If
        Next intCol
        
        For intTimeRow = intTimeRow To 50
            arrInfo(intTimeRow, 1) = ""
            arrInfo(intTimeRow, 2) = ""
            arrTime(intTimeRow, 1) = ""
            arrTime(intTimeRow, 2) = ""
        Next intTimeRow
        
        For intPeriod = intPeriod To 20
            arrExtraInfo(intPeriod, 1) = ""
            arrExtraInfo(intPeriod, 2) = ""
            arrExtraTime(intPeriod, 1) = ""
            arrExtraTime(intPeriod, 2) = ""
        Next intPeriod
        
        wrkTimesheet.Worksheets("Time Sheet").Range("$C$18:$D$67").Value = arrInfo
        wrkTimesheet.Worksheets("Time Sheet").Range("$F$18:$G$67").Value = arrTime
        wrkTimesheet.Worksheets("Request for payment").Range("$C$16:$D$35").Value = arrExtraInfo
        wrkTimesheet.Worksheets("Request for payment").Range("$F$16:$G$35").Value = arrExtraTime
        
        MsgBox ("Yo Adrian, we did it. We did it." & vbNewLine & "Rocky Balboa")
    End If
End Sub

Sub Tank()
    Dim ownPlanning As Workbook, arrTimeDates() As Variant, arrTank() As Variant
    Dim x As Long, y As Long, n As Long, m As Long, strSheet As String
    Dim lngTankDay As Long, lngTankMonth As Long, dblTotalDistance As Double, dblInit As Double
    Dim arrTotalData() As Variant, intStartData As Integer, intEndData As Integer, intCurYear As Integer, intEndTanked As Integer
    Dim dtLastTanked As Date, lngDay As Long, lngMonth As Long, lngStartMonth As Integer
    Dim dtPreviouslyTanked As Date, lngPDay As Long, lngPMonth As Long, lngPStartMonth As Long

    Set ownPlanning = ActiveWorkbook
    
    intCurYear = Year(Now())
    intStartData = ((intCurYear - 2018) * 12) + 2
    intEndData = ownPlanning.Sheets("Routes").Cells(Rows.Count, 9).End(xlUp).Row
    intEndTanked = ownPlanning.Sheets("Routes").Cells(Rows.Count, 7).End(xlUp).Row
    x = 1
    y = 0
    dblTotalDistance = 0
    dblInit = 0
    strSheet = "Tank " & CStr(intCurYear)
    dtPreviouslyTanked = ownPlanning.Sheets("Routes").Range("G" & intEndTanked - 1).Value
    dtLastTanked = ownPlanning.Sheets("Routes").Range("G" & intEndTanked).Value
    lngDay = Day(dtLastTanked)
    lngMonth = Month(dtLastTanked)
    lngStartMonth = (lngMonth * 3) + 1
    lngPDay = Day(dtPreviouslyTanked)
    lngPMonth = Month(dtPreviouslyTanked)
    lngPStartMonth = (lngPMonth * 3) + 1
    lngTankMonth = (Month(dtLastTanked) * 3) - 2
    lngTankDay = Day(dtLastTanked)
    arrTimeDates = ownPlanning.Sheets(strSheet).Range("B4:AF39").Value
    arrTank = ownPlanning.Sheets(strSheet).Range("B4:AF39").Formula
    ownPlanning.Sheets(strSheet).Range(Cells(lngStartMonth + 1, lngDay + 1), Cells(lngStartMonth + 2, 40)).ClearComments
    ownPlanning.Sheets(strSheet).Range(Cells(lngStartMonth + 3, 2), Cells(39, 40)).ClearComments
    
    If (intCurYear > 2018) Then
        intStartData = intStartData - 4
    End If
    
    ReDim arrTotalData(1 To (intEndData - intStartData), 1 To 1)
    
    If (InStr(1, arrTimeDates((lngStartMonth - 3), lngDay), "N", vbTextCompare) Or InStr(1, arrTimeDates((lngStartMonth - 3), lngDay), "O", vbTextCompare) Or arrTimeDates((lngStartMonth - 3), lngDay) = "D" Or arrTimeDates((lngStartMonth - 3), lngDay) = "") Then
        dblInit = 102.8
    Else
        dblInit = 44.1
    End If
    
    dblTotal = 0
    
    Select Case ownPlanning.Sheets(strSheet).Cells(lngStartMonth, lngDay + 1)
        Case ""
            dblTotal = dblTotal + 102.8
        Case "D"
            dblTotal = dblTotal + 102.8
        Case "N"
            dblTotal = dblTotal + 102.8
        Case "O"
            dblTotal = dblTotal + 102.8
        Case "L"
            dblTotal = dblTotal + 44.1
        Case Else
            dblTotal = dblTotal
    End Select
    
    Select Case ownPlanning.Sheets(strSheet).Cells(lngPStartMonth, lngPDay + 1)
        Case ""
            dblTotal = dblTotal + 44.1
        Case "D"
            dblTotal = dblTotal + 44.1
        Case "N"
            dblTotal = dblTotal + 44.1
        Case "O"
            dblTotal = dblTotal + 44.1
        Case "L"
            dblTotal = dblTotal + 102.8
        Case Else
            dblTotal = dblTotal
    End Select

    For y = 0 To (lngMonth - lngPMonth)
        Dim curMonthDays As Integer, intRow As Integer, intCol As Integer, intTop As Double, intBottom As Double
        curMonthDays = Day(DateSerial(intCurYear, lngMonth - y + 1, 1) - 1)
        intRow = lngStartMonth + 1 - (y * 3)
        
        For x = 1 To (curMonthDays - 1)
            If (y = 0) Then
                intCol = lngDay - x + 1
            Else
                intCol = curMonthDays - x + 1
            End If
            
            If (((curMonthDays - x) < 1 And (lngMonth - lngPMonth) > 0) Or ((lngMonth - lngPMonth) = 0 And (lngDay - x) <= (lngPDay + 1)) Or ((curMonthDays - x) <= lngPDay And (lngMonth - lngPMonth) > 0 And (lngMonth - y) = lngPMonth)) Then
                x = 50  ' Maand skippen en overgaan naar de volgende
            End If
            
            If (ownPlanning.Sheets(strSheet).Cells(intRow - 1, intCol).Text = "Z") Then
                ownPlanning.Sheets(strSheet).Cells(intRow, intCol).Value = ""
                ownPlanning.Sheets(strSheet).Cells(intRow + 1, intCol).Value = ""
                arrTank(intRow - 3, intCol - 1) = ""
                arrTank(intRow - 2, intCol - 1) = ""
                arrTimeDates(intRow - 3, intCol - 1) = ""
                arrTimeDates(intRow - 2, intCol - 1) = ""
                
                If (Not ownPlanning.Sheets(strSheet).Cells(intRow, intCol).Comment Is Nothing) Then
                    ownPlanning.Sheets(strSheet).Range(Cells(intRow, intCol), Cells(intRow, intCol)).ClearComments
                    ownPlanning.Sheets(strSheet).Cells(intRow, intCol).Interior.Color = xlNone
                End If
            Else
                intTop = arrTimeDates(intRow - 3, intCol - 1)
                intBottom = arrTimeDates(intRow - 2, intCol - 1)
                
                If (intTop > 0 Or intBottom > 0) Then
                    dblTotal = dblTotal + intTop + intBottom
                End If
            End If
        Next x
    Next y
    
    ' Geen B, S, X, R, V, F, Z
    For n = lngStartMonth To 37
        For m = 2 To 32
            Dim strNextShift As String
            strNextShift = "NA"
            
            If (n = lngStartMonth And (m - 1) <= lngDay) Then
                m = lngDay + 1
            End If
            
            Do
                For y = 1 To 12
                    If (((n - 1) / 3) > y) Then
                        y = (n - 1) / 3
                    End If
                    
                    For x = 1 To Day(DateSerial(Year(Now()), y + 1, 1) - 1)
                        If ((y = (n - 1) / 3) And (x < (m - 1))) Then
                            x = m - 1
                        End If
                        
                        If (arrTimeDates((n - 3), (m - 1)) = "") Or (arrTimeDates((n - 3), (m - 1)) = "D") Or (arrTimeDates((n - 3), (m - 1)) = "O") Or (arrTimeDates((n - 3), (m - 1)) = "L") Or (arrTimeDates((n - 3), (m - 1)) = "N") Then
                            strNextShift = arrTimeDates((n - 3), (m - 1))
                            Exit Do
                        End If
                        
                        If (y = 12 And x = 31) Then
                            Exit Do
                        End If
                    Next x
                Next y
            Loop While False
            
            If (Weekday(DateSerial(2018, (n - 1) / 3, m - 1), vbMonday) = 5) Then
                strNextShift = strNextShift
            End If
            
            If (arrTimeDates((n - 3), (m - 1)) = "N" Or arrTimeDates((n - 3), (m - 1)) = "O" Or arrTimeDates((n - 3), (m - 1)) = "D" Or arrTimeDates((n - 3), (m - 1)) = "") Then
                dblTotalDistance = dblTotalDistance + 102.8
            
                If (dblTotalDistance > 510 And arrTimeDates(lngTankMonth, lngTankDay) = "L") Then
                    ownPlanning.Sheets(strSheet).Cells(n + 1, m).Interior.Color = RGB(155, 194, 230)
                    ownPlanning.Sheets(strSheet).Cells(n + 1, m).AddComment (Replace(CStr(dblTotalDistance), ",", "."))
                    arrTank(n - 2, m - 1) = "102.8"
                    arrTank(n - 1, m - 1) = "44.1"
                    
                    dblTotalDistance = 44.1
                    lngTankMonth = n - 3
                    lngTankDay = m - 1
                ElseIf (dblTotalDistance > 480 Or dblTotalDistance = 102.8) Then
                    ownPlanning.Sheets(strSheet).Cells(n + 1, m).Interior.Color = RGB(155, 194, 230)
                    arrTank(n - 2, m - 1) = "102.8"
                    arrTank(n - 1, m - 1) = "44.1"
                    
                    If (dblTotalDistance = 102.8) Then
                        ownPlanning.Sheets(strSheet).Cells(n + 1, m).AddComment (Replace(CStr(dblTotal), ",", "."))
                    Else
                        ownPlanning.Sheets(strSheet).Cells(n + 1, m).AddComment (Replace(CStr(dblTotalDistance), ",", "."))
                    End If
                    
                    dblTotalDistance = 44.1
                    lngTankMonth = n - 3
                    lngTankDay = m - 1
                Else
                    ownPlanning.Sheets(strSheet).Cells(n + 1, m).Interior.Color = xlNone
                    arrTank(n - 2, m - 1) = "68.0"
                    arrTank(n - 1, m - 1) = "68.0"
                    
                    ' Corrigeer 68 * 2 - 102.8 erbij als we niet moeten tanken
                    dblTotalDistance = dblTotalDistance + 33.2
                End If
            ElseIf (arrTimeDates((n - 3), (m - 1)) = "L") Then
                dblTotalDistance = dblTotalDistance + 44.1
            
                If ((dblTotalDistance > 510 And arrTimeDates(lngTankMonth, lngTankDay) = "L") Or dblTotalDistance < 50) Then
                    ownPlanning.Sheets(strSheet).Cells(n + 1, m).Interior.Color = RGB(155, 194, 230)
                    arrTank(n - 2, m - 1) = "44.1"
                    arrTank(n - 1, m - 1) = "102.8"
                    
                    If (dblTotalDistance < 50) Then
                        ownPlanning.Sheets(strSheet).Cells(n + 1, m).AddComment (Replace(CStr(dblTotal), ",", "."))
                    Else
                        ownPlanning.Sheets(strSheet).Cells(n + 1, m).AddComment (Replace(CStr(dblTotalDistance), ",", "."))
                    End If
                    
                    dblTotalDistance = 102.8
                    lngTankMonth = n - 3
                    lngTankDay = m - 1
                ElseIf (dblTotalDistance > 590) Then
                    ownPlanning.Sheets(strSheet).Cells(n + 1, m).Interior.Color = RGB(155, 194, 230)
                    ownPlanning.Sheets(strSheet).Cells(n + 1, m).AddComment (Replace(CStr(dblTotalDistance), ",", "."))
                    arrTank(n - 2, m - 1) = "44.1"
                    arrTank(n - 1, m - 1) = "102.8"
                    
                    dblTotalDistance = 102.8
                    lngTankMonth = n - 3
                    lngTankDay = m - 1
                'ElseIf (((dblTotalDistance + 102.8) > 395 And (InStr(1, arrTimeDates(n - 3, m), "WL", vbTextCompare) Or arrTimeDates(n - 3, m + 1) = "WO")) Or (True)) Then
                Else
                    'row, col
                    ownPlanning.Sheets(strSheet).Cells(n + 1, m).Interior.Color = xlNone
                    arrTank(n - 2, m - 1) = "68.0"
                    arrTank(n - 1, m - 1) = "68.0"
                    
                    ' Corrigeer 68 * 2 - 44.1 erbij als we niet moeten tanken
                    dblTotalDistance = dblTotalDistance + 91.9
                End If
            ElseIf (InStr(1, arrTimeDates((n - 3), (m - 1)), "W", vbTextCompare) Or InStr(1, arrTimeDates((n - 3), (m - 1)), "FO", vbTextCompare) Or InStr(1, arrTimeDates((n - 3), (m - 1)), "FL", vbTextCompare) Or InStr(1, arrTimeDates((n - 3), (m - 1)), "FN", vbTextCompare)) Then
                arrTank(n - 2, m - 1) = "68.0"
                arrTank(n - 1, m - 1) = "68.0"
                
                dblTotalDistance = dblTotalDistance + 136
            Else
                ownPlanning.Sheets(strSheet).Cells(n + 1, m).Interior.Color = xlNone
                arrTank(n - 2, m - 1) = ""
                arrTank(n - 1, m - 1) = ""
            End If
        Next m
        
        n = n + 2
    Next n
    
    ownPlanning.Sheets(strSheet).Range("B4:AF39") = arrTank
    
    MsgBox ("We came. We saw. We kicked its ass." & vbNewLine & "Dr. Peter Venkman, Ghostbusters")
End Sub

Sub ExportTank()
    Dim arrLPGCosts() As Variant, arrLPGDates() As Variant, lngRowCount As Long, ownPlanning As Workbook, wbkHuis As Workbook, arrTimeDates() As Variant, strTank As String, arrHome() As Variant, intHomeRows As Integer, intLatestHomeRow As Integer
    Dim x As Long, y As Long, arrTemp() As Variant, n As Long, m As Long, lngDay As Long, lngMonth As Long, lngTankDay As Long, lngTankMonth As Long, dblTotalDistance As Double, blnExit As Boolean, dtLastTanked As Date, lngStartMonth As Integer, lngLastSum As Long
    
    intLatestHomeRow = 1
    strSheet = "Tank " & CStr(Year(Now()))
    Set ownPlanning = ActiveWorkbook
    arrTimeDates = ownPlanning.Sheets(strSheet).Range("B4:AF39").Value
    
    For x = 1 To Workbooks.Count
        If (Workbooks(x).Name = "Huis.xlsm") Then
            Set wbkHuis = Workbooks(x)
        End If
        
        If Not (wbkHuis Is Nothing) Then
            intHomeRows = wbkHuis.Sheets("Lopende kosten").Cells(Rows.Count, 1).End(xlUp).Row
            ReDim arrHome(1 To intHomeRows, 1 To 10)
            arrHome = wbkHuis.Sheets("Lopende kosten").Range("A3:J" & intHomeRows).Formula
            Exit For
        End If
    Next x
    
    'Create csv file
    Dim fso As Object
    Set fso = CreateObject("Scripting.FileSystemObject")
    Dim Fileout As Object
    Set Fileout = fso.CreateTextFile(ownPlanning.Path & "\tank.csv", True, True)
    Fileout.Close
    Open ownPlanning.Path & "\tank.csv" For Output As #1
    Print #1, "Subject,Start Date,End Date,All Day Event,Start Time,End Time,Location,Description", vbNewLine;
    
    'Geen B, S, X, R, V, F, Z
    For n = 1 To 36
        For m = 1 To 31
            'If (arrTimeDates(n + 1, m) = 44.1 Or arrTimeDates(n + 1, m) = 102.8) Then
            If (Not (ownPlanning.Sheets(strSheet).Cells(n + 4, m + 1).Comment Is Nothing)) Then
                Dim dtDate As Date, varTmpKm As Variant
                dtDate = DateValue(CStr(m) & "/" & CStr((n + 2) / 3) & "/" & CStr(Year(Now())))
                varTmpKm = ownPlanning.Sheets(strSheet).Cells(n + 4, m + 1).Comment.Text
                Print #1, Replace(varTmpKm, ",", ".") & " km," & dtDate & "," & dtDate & ",TRUE,,,,", vbNewLine;
                
                If (dtDate <= Now() And Not (wbkHuis Is Nothing)) Then
                    For i = intLatestHomeRow To (intHomeRows - 2)
                        If (arrHome(i, 1) = "LPG brandstof" And arrHome(i, 2) = dtDate And arrHome(i, 3) > 5) Then
                            arrHome(i, 9) = varTmpKm
                            Exit For
                        End If
                        
                        intLatestHomeRow = intLatestHomeRow + 1
                    Next i
                End If
            End If
        Next m
        
        n = n + 2
    Next n
    
    Close #1
    
    If (Not (wbkHuis Is Nothing)) Then
        wbkHuis.Sheets("Lopende kosten").Range("A3:J" & intHomeRows) = arrHome
    End If
    
    MsgBox ("I’ve lost the bleeps, I’ve lost the sweeps, and I’ve lost the creeps." & vbNewLine & "Radar Technician, Spaceballs")
End Sub

Function ColLetter(lngCol As Long) As String
    ColLetter = Split(Cells(1, lngCol).Address(True, False), "$")(0)
End Function

Sub GetJulianDate()
    Cells(1, 2151).FormulaR1C1 = _
        "=YEAR(TODAY())-2000 & IF(DAYS(TODAY(),DATE(YEAR(TODAY())-1,12,31)) < 100,""0"","""") & DAYS(TODAY(),DATE(YEAR(TODAY())-1,12,31))"
    
    MsgBox ("De datum van vandaag is " & Cells(1, 2151).Text)
    Cells(1, 2151).ClearContents
End Sub

Function GetSheets() As Variant()
    Dim wb As Workbook
    Set wb = Workbooks("Planning.xlsm")
    Dim arrSheets() As Variant
    Dim i As Integer
    i = 1
    
    For x = 1 To wb.Sheets.Count
        If (InStr(1, wb.Sheets(x).Name, "20") = 1) Then
            If (x < wb.Sheets.Count) Then
                i = i + 1
            End If
        End If
    Next x
    
    ReDim arrSheets(1 To (i - 1)) As Variant
    i = 1
    
    For x = 1 To wb.Sheets.Count
        If (InStr(1, wb.Sheets(x).Name, "20") = 1) Then
            arrSheets(i) = wb.Sheets(x).Name
            i = i + 1
        End If
    Next x
    
    GetSheets = arrSheets
End Function

Function GetSheetsMul(intFrom As Integer, intTo As Integer) As Variant()
    Dim wb As Workbook
    Set wb = Workbooks("Planning.xlsm")
    Dim started As Boolean
    Dim arrSheets() As Variant
    Dim i As Integer, prevI As Integer
    i = 1
    prevI = 1
    started = False
    
    For x = 1 To wb.Sheets.Count
        If (InStr(1, wb.Sheets(x).Name, "20") = 1) Then
            If Not (started) And (IsNumeric(wb.Sheets(x).Name)) And (CInt(wb.Sheets(x).Name) = intFrom Or CInt(wb.Sheets(x).Name) = intTo) Then
                started = True
            End If
            
            If (started) And (prevI <> i) And (IsNumeric(wb.Sheets(x).Name)) And (CInt(wb.Sheets(x).Name) = intFrom Or CInt(wb.Sheets(x).Name) = intTo) Then
                started = False
            End If
            
            If (started) And (InStr(1, wb.Sheets(x).Name, "20") = 1) Then
                prevI = i
                i = i + 1
            End If
        End If
    Next x
    
    ReDim arrSheets(1 To i) As Variant
    i = 1
    
    For x = 1 To wb.Sheets.Count
        If (InStr(1, wb.Sheets(x).Name, "20") = 1) Then
            If Not (started) And (IsNumeric(wb.Sheets(x).Name)) And (CInt(wb.Sheets(x).Name) = intFrom Or CInt(wb.Sheets(x).Name) = intTo) Then
                started = True
            End If
            
            If (started) And (InStr(1, wb.Sheets(x).Name, "20") = 1) Then
                arrSheets(i) = wb.Sheets(x).Name
            End If
            
            If (started) And (prevI <> i) And (IsNumeric(wb.Sheets(x).Name)) And (CInt(wb.Sheets(x).Name) = intFrom Or CInt(wb.Sheets(x).Name) = intTo) Then
                started = False
            End If
            
            If (started) And (InStr(1, wb.Sheets(x).Name, "20") = 1) Then
                prevI = i
                i = i + 1
            End If
        End If
    Next x
    
    GetSheetsMul = arrSheets
End Function

Function ExportSheets(arrSheets() As Variant) As Boolean
    Dim varDate As Date
    Dim z As Integer
    Dim blub As Integer
    Dim expWorkbook As Workbook
    Dim expSheet As Worksheet
    Dim thisWorkbook
    Dim tempCell As String
    Set thisWorkbook = Workbooks("Planning.xlsm")
    
    'Create csv file
    Dim fso As Object
    Set fso = CreateObject("Scripting.FileSystemObject")
    Dim Fileout As Object
    Set Fileout = fso.CreateTextFile(thisWorkbook.Path & "\werk.csv", True, True)
    Fileout.Close
    Open thisWorkbook.Path & "\werk.csv" For Output As #1
    Print #1, "Subject,Start Date,End Date,All Day Event,Start Time,End Time,Location,Description", vbNewLine;
    
    For i = UBound(arrSheets) To LBound(arrSheets) Step -1
        varDate = DateValue("jan 1, " & arrSheets(i))
    
        For x = 4 To 15
            For y = 2 To 32
                tempCell = thisWorkbook.Sheets(arrSheets(i)).Cells(x, y)
                Dim amDate As String
                Dim amDatePlusEen As String
                amDate = Format(varDate, "mm/dd/yyyy")
                amDatePlusEen = Format(DateAdd("d", 1, varDate), "mm/dd/yyyy")
                
                If (tempCell <> "X") And (tempCell <> "x") And (tempCell <> "S") And (tempCell <> "s") Then
                    Select Case tempCell
                        Case Is = ""
                            Print #1, "d - Niet-verplichte dag," & amDate & "," & amDate & ",FALSE,7:00 AM,4:00 PM,Werk,Niet-verplichte dag", vbNewLine;
                        Case Is = "D"
                            Print #1, "D - Verplichte dag," & amDate & "," & amDate & ",FALSE,7:00 AM,4:00 PM,Werk,Verplichte dag", vbNewLine;
                        Case Is = "W"
                            Print #1, "W - Weekend," & amDate & "," & amDate & ",TRUE,,,Werk,Weekend", vbNewLine;
                        Case Is = "B"
                            Print #1, "B - Beeper," & amDate & "," & amDate & ",TRUE,,,Thuis,Beeper", vbNewLine;
                        Case Is = "F"
                            Print #1, "F - Feestdag," & amDate & "," & amDate & ",TRUE,,,Thuis,Feestdag", vbNewLine;
                        Case Is = "Z"
                            Print #1, "Z - Ziekendag," & amDate & "," & amDate & ",TRUE,,,Thuis,Ziekendag", vbNewLine;
                        Case Is = "V"
                            Print #1, "V - Verlof," & amDate & "," & amDate & ",TRUE,,,Thuis,Verlof", vbNewLine;
                        Case Is = "R"
                            Print #1, "V - Verlof," & amDate & "," & amDate & ",TRUE,,,Thuis,Verlof", vbNewLine;
                        Case Is = "1/R"
                            Print #1, "1/R - Halve dag," & amDate & "," & amDate & ",TRUE,,,Werk,Halve dag", vbNewLine;
                        Case Is = "O"
                            If Not thisWorkbook.Sheets(arrSheets(i)).Cells(x, y).Comment Is Nothing Then
                                If (InStr(1, thisWorkbook.Sheets(arrSheets(i)).Cells(x, y).Comment.Text, "Uitwijk") = 0) Then
                                    Print #1, "O - Vroege shift," & amDate & "," & amDate & ",FALSE,5:40 AM,2:10 PM,Werk,Vroege shift", vbNewLine;
                                Else
                                    Print #1, "O - Vroege shift uitwijk," & amDate & "," & amDate & ",FALSE,5:40 AM,2:10 PM,LEUTIENS,Vroege shift", vbNewLine;
                                End If
                            Else
                                Print #1, "O - Vroege shift," & amDate & "," & amDate & ",FALSE,5:40 AM,2:10 PM,Werk,Vroege shift", vbNewLine;
                            End If
                        Case Is = "WO"
                            Print #1, "WO - Vroege weekendshift," & amDate & "," & amDate & ",FALSE,5:40 AM,2:10 PM,Werk,Vroege weekendshift", vbNewLine;
                        Case Is = "FO"
                            Print #1, "FO - Vroege feestdagshift," & amDate & "," & amDate & ",FALSE,5:40 AM,2:10 PM,Werk,Vroege feestdagshift", vbNewLine;
                        Case Is = "L"
                            Print #1, "L - Late shift," & amDate & "," & amDate & ",FALSE,1:20 PM,10:10 PM,Werk,Late shift", vbNewLine;
                        Case Is = "WL"
                            Print #1, "WL - Late weekendshift," & amDate & "," & amDate & ",FALSE,1:20 PM,10:10 PM,Werk,Late weekendshift", vbNewLine;
                        Case Is = "FL"
                            Print #1, "FL - Late feestdagshift," & amDate & "," & amDate & ",FALSE,1:20 PM,10:10 PM,Werk,Late feestdagshift", vbNewLine;
                        Case Is = "N"
                            Print #1, "N - Nachtshift," & amDate & "," & amDatePlusEen & ",FALSE,9:20 PM,6:10 AM,Werk,Nachtshift", vbNewLine;
                        Case Is = "WN"
                            Print #1, "WN - Nachtelijkse weekendshift," & amDate & "," & amDatePlusEen & ",FALSE,9:20 PM,6:10 AM,Werk,Nachtelijke weekendshift", vbNewLine;
                        Case Is = "FN"
                            Print #1, "FN - Nachtelijkse feestdagshift," & amDate & "," & amDatePlusEen & ",FALSE,9:20 PM,6:10 AM,Werk,Nachtelijke feestdagshift", vbNewLine;
                        Case Else
                    End Select
                End If
    
                If Not thisWorkbook.Sheets(arrSheets(i)).Cells(x, y).Comment Is Nothing Then
                    Dim arrComments() As String
                    arrComments = Split(thisWorkbook.Sheets(arrSheets(i)).Cells(x, y).Comment.Text, Chr(10))
                    
                    For j = LBound(arrComments) To UBound(arrComments)
                        Dim dtTime As Date
                        Dim dtEndTime As Date
                        dtTime = TimeValue(Left(arrComments(j), 5))
                        dtEndTime = DateAdd("h", 1, dtTime)
                        
                        If (InStr(1, arrComments(j), "Uitwijk") = 0) Then
                            Print #1, Mid(arrComments(j), 7, Len(arrComments(j)) - 6); "," & amDate & "," & amDate & ",FALSE," & Format(dtTime, "hh:mm AM/PM") & "," & Format(dtEndTime, "hh:mm AM/PM") & ",,", vbNewLine;
                        End If
                    Next j
                End If
                
                If (tempCell <> "X") And (tempCell <> "x") Then
                    varDate = DateAdd("d", 1, varDate)
                End If
            Next y
        Next x
    Next i
    
    Close #1
    
    ExportSheets = True
End Function

Sub LoadDirect(strYear As String)
    Dim kbcPlanning As Workbook, ownPlanning As Workbook, intYear As Integer, clrRed As Variant, clrUitwijk As Variant, canIClose As Boolean, strChanges As String, strStart As String, strEnd As String, strFrom As String, strTo As String, strStartNew As String, strEndNew As String, strFromNew As String, strToNew As String, blnWriteChanges As Boolean
    intYear = CInt(strYear)
    clrRed = RGB(255, 0, 0)
    clrUitwijk = RGB(192, 0, 0)
    canIClose = False
    blnWriteChanges = False
    strChanges = ""
    strStart = ""
    strEnd = ""
    strFrom = ""
    strTo = ""
    strStartNew = ""
    strEndNew = ""
    strFromNew = ""
    strToNew = ""
    
    For x = 1 To Workbooks.Count
        If (InStr(1, Workbooks(x).Name, "Planning") <> 0) And (InStr(1, Workbooks(x).Name, strYear) <> 0) Then
            Set kbcPlanning = Workbooks(x)
        ElseIf (Workbooks(x).Name = "Planning.xlsm") Then
            Set ownPlanning = Workbooks(x)
        End If
        
        If Not (kbcPlanning Is Nothing) And Not (ownPlanning Is Nothing) Then
            Exit For
        End If
    Next x
    
    If (kbcPlanning Is Nothing) Then
        Set kbcPlanning = Workbooks.Open("/** REDACTED **/\" & strYear & "\Planning " & strYear & ".xlsm", False, True)
        kbcPlanning.Application.WindowState = xlMinimized
        canIClose = True
    End If

    For i = 1 To kbcPlanning.Sheets.Count
        Dim strSheet As String
        strSheet = kbcPlanning.Sheets(i).Name
        
        If (InStr(1, strSheet, "PLANNING") > 0) Then
            Dim arrOwnShifts As Variant, arrNewShifts() As String, arrKBCShifts As Variant, ownColumn As Integer, strColumn As String, varChangeDates() As Variant, subCounter As Integer, curCel As String
            
            For x = 1 To 48
                If (InStr(1, kbcPlanning.Sheets(i).Cells(2, x), "Chris De Smedt") > 0) Then
                    ownColumn = x
                    strColumn = ColLetter(CLng(x))
                End If
            Next x
            
            arrOwnShifts = ownPlanning.Sheets(strYear).Range("B4:AF15")
            arrKBCShifts = kbcPlanning.Sheets(i).Range(strColumn + "4:" + strColumn + "370")
            ReDim Preserve arrNewShifts(1 To 12, 1 To 31) As String
            subCounter = 1
            
            For x = 1 To 12
                For y = 1 To 31
                    curCel = ColLetter(y + 1) & CStr(x + 3)

                    If (Not ownPlanning.Sheets(strYear).Cells((x + 3), (y + 1)).Comment Is Nothing) Then
                        If (kbcPlanning.Sheets(i).Cells(subCounter + 3, ownColumn).Interior.Color <> clrUitwijk And InStr(1, ownPlanning.Sheets(strYear).Cells((x + 3), (y + 1)).Comment.Text, "05:40 Uitwijk") <> 0) Then
                            Dim strDeleteComment As String
                            strDeleteComment = ownPlanning.Sheets(strYear).Cells((x + 3), (y + 1)).Comment.Text
                            ownPlanning.Sheets(strYear).Cells((x + 3), (y + 1)).Comment.Delete
                            
                            If (strDeleteComment <> "05:40 Uitwijk") Then
                                ownPlanning.Sheets(strYear).Cells((x + 3), (y + 1)).AddComment (Replace(Replace(strDeleteComment, Chr(10) & "05:40 Uitwijk", ""), "05:40 Uitwijk" & Chr(10), ""))
                            End If
                        End If
                    End If
                
                    If (x = 6 And y = 23) Then
                        subCounter = subCounter
                    End If
                
                    'x boven 7 de even, onder 8 de oneven = 31
                    'rest = 30 behalve februari
                    If (((x = 4 Or x = 6 Or x = 9 Or x = 11) And y > 30) Or (x = 2 And y > 29 And (intYear Mod 4) = 0)) Then
                        arrNewShifts(x, y) = "X"
                    ElseIf (x = 2 And y > 28) Then
                        arrNewShifts(x, y) = "X"
                    Else
                        If (kbcPlanning.Sheets(i).Cells(subCounter + 3, ownColumn).Interior.Color = clrRed) Then
                            If (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "O") <> 0) Then
                                arrNewShifts(x, y) = "FO"
                            ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "A") <> 0) Then
                                arrNewShifts(x, y) = "FL"
                            ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "N") <> 0) Then
                                arrNewShifts(x, y) = "FN"
                            ElseIf ((InStr(1, UCase(arrKBCShifts(subCounter, 1)), "BP") And (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "R") < 1))) Then
                                    arrNewShifts(x, y) = "B"
                            ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "W1") <> 0) Then
                                'W1 = inclusief W14 + W16
                                arrNewShifts(x, y) = "WL"
                            ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "W2") <> 0) Then
                                'W2 = inclusief W22
                                arrNewShifts(x, y) = "WN"
                            ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "W6") <> 0 Or InStr(1, UCase(arrKBCShifts(subCounter, 1)), "W06") <> 0) Then
                                arrNewShifts(x, y) = "WO"
                            Else
                                arrNewShifts(x, y) = "F"
                            End If
                        ElseIf (kbcPlanning.Sheets(i).Cells(subCounter + 3, ownColumn).Interior.Color = clrUitwijk) Then
                            arrNewShifts(x, y) = "O"
                            
                            If ownPlanning.Sheets(strYear).Cells((x + 3), (y + 1)).Comment Is Nothing Then
                                ownPlanning.Sheets(strYear).Cells((x + 3), (y + 1)).AddComment ("05:40 Uitwijk")
                            Else
                                If (InStr(1, ownPlanning.Sheets(strYear).Cells((x + 3), (y + 1)).Comment.Text, "05:40 Uitwijk") = 0) Then
                                    Dim strComment As String
                                    strComment = ownPlanning.Sheets(strYear).Cells((x + 3), (y + 1)).Comment.Text
                                    ownPlanning.Sheets(strYear).Cells((x + 3), (y + 1)).Comment.Delete
                                    
                                    If (CInt(Left(strComment, 2)) < 5 Or (CInt(Left(strComment, 2)) = 5 And CInt(Mid(strComment, 4, 2)) < 40)) Then
                                        ownPlanning.Sheets(strYear).Cells((x + 3), (y + 1)).AddComment (strComment & Chr(10) & "05:40 Uitwijk")
                                    Else
                                        ownPlanning.Sheets(strYear).Cells((x + 3), (y + 1)).AddComment ("05:40 Uitwijk" & Chr(10) & strComment)
                                    End If
                                End If
                            End If
                        ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "O") <> 0) Then
                            arrNewShifts(x, y) = "O"
                        ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "A") <> 0) Then
                            arrNewShifts(x, y) = "L"
                        ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "N") <> 0) Then
                            arrNewShifts(x, y) = "N"
                        ElseIf ((InStr(1, UCase(arrKBCShifts(subCounter, 1)), "BP") <> 0) And (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "R") < 1)) Then
                            arrNewShifts(x, y) = "B"
                        ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "W1") <> 0) Then
                            'W1 = inclusief W14 + W16
                            arrNewShifts(x, y) = "WL"
                        ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "W2") <> 0) Then
                            'W2 = inclusief W22
                            arrNewShifts(x, y) = "WN"
                        ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "W6") <> 0 Or InStr(1, UCase(arrKBCShifts(subCounter, 1)), "W06") <> 0) Then
                            arrNewShifts(x, y) = "WO"
                        ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "D") <> 0) Then
                            arrNewShifts(x, y) = "D"
                        ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "V") <> 0) Then
                            If (arrOwnShifts(x, y) <> "1/R" And arrOwnShifts(x, y) <> "R") Then
                                arrNewShifts(x, y) = "V"
                            ElseIf (arrOwnShifts(x, y) <> "1/R" And arrOwnShifts(x, y) <> "V") Then
                                arrNewShifts(x, y) = "R"
                            Else
                                arrNewShifts(x, y) = "1/R"
                            End If
                        ElseIf (InStr(1, UCase(arrKBCShifts(subCounter, 1)), "Z") <> 0 Or InStr(1, UCase(arrKBCShifts(subCounter, 1)), "KV") <> 0) Then
                            arrNewShifts(x, y) = "Z"
                        ElseIf (Weekday(DateValue(y & "-" & x & "-" & intYear), vbMonday) > 5) Then
                            arrNewShifts(x, y) = "S"
                        ElseIf (arrOwnShifts(x, y) = "1/R") Then
                            arrNewShifts(x, y) = "1/R"
                        Else
                            arrNewShifts(x, y) = ""
                        End If
                        
                        If (InStr(1, arrKBCShifts(subCounter, 1), "?") <> 0) Then
                            arrNewShifts(x, y) = arrNewShifts(x, y) + "?"
                        End If
                        
                        subCounter = subCounter + 1
                    End If
                    
                    If (arrOwnShifts(x, y) <> arrNewShifts(x, y)) Then
                        If (strStart = "") Then
                            strStart = y & "/" & x
                            strFrom = arrOwnShifts(x, y)
                            strTo = arrNewShifts(x, y)
                        End If
                        
                        If (arrOwnShifts(x, y) <> strFrom Or arrNewShifts(x, y) <> strTo) Then
                            strEnd = (y - 1) & "/" & x
                            strStartNew = y & "/" & x
                            strFromNew = arrOwnShifts(x, y)
                            strToNew = arrNewShifts(x, y)
                            blnWriteChanges = True
                        End If
                    ElseIf (strStart <> "") Then
                        If (strStart <> ((y - 1) & "/" & x)) Then
                            strEnd = (y - 1) & "/" & x
                        End If
                        
                        blnWriteChanges = True
                    End If
                    
                    If (blnWriteChanges) Then
                        If (strStart <> strEnd And strEnd <> "") Then
                            strChanges = strChanges & strStart & "-" & strEnd & " - " & strFrom & " -> " & strTo & Chr(10)
                        Else
                            strChanges = strChanges & strStart & " - " & strFrom & " -> " & strTo & Chr(10)
                        End If
                        
                        If (strStartNew <> "") Then
                            strStart = strStartNew
                            strEnd = strEndNew
                            strFrom = strFromNew
                            strTo = strToNew
                        Else
                            strStart = ""
                            strEnd = ""
                            strFrom = ""
                            strTo = ""
                        End If
                        
                        strStartNew = ""
                        strEndNew = ""
                        strFromNew = ""
                        strToNew = ""
                        blnWriteChanges = False
                    End If
                Next y
            Next x
        End If
    Next i
    
    ownPlanning.Sheets(strYear).Range("B4:AF15") = arrNewShifts
    
    If (canIClose) Then
        kbcPlanning.Close (False)
    End If
    
    If (strChanges <> "") Then
        MsgBox (strChanges)
    Else
        MsgBox ("Ehh, nothing to see here, doc" & vbNewLine & "Bugs Bunny")
        'End
    End If
End Sub

Sub btnDirect_Click()
    LoadDirect (Workbooks("Planning.xlsm").Sheets("Refresh").Cells(1, 1).Text)
End Sub

Sub btnDirectTemplate_Click(strName As Variant)
    LoadDirect (strName)
End Sub

Sub btnExport_Click()
    Cells(1, 6) = ""
    Cells(2, 6) = ""
    
    Dim varDate As Date
    Dim z As Integer
    Dim blub As Integer
    Dim expWorkbook As Workbook
    Dim expSheet As Worksheet
    Dim thisWorkbook
    Dim tempCell As String
    Set thisWorkbook = Workbooks("Planning.xlsm")
    varDate = DateValue("jan 1, " & thisWorkbook.Sheets("Refresh").Cells(1, 1))
    
    If (thisWorkbook.Sheets("Refresh").Cells(1, 1).Text <> "") Then
        Dim voidResult As Boolean
        Dim arrSheets() As Variant
        Dim i As Integer, intFrom As Integer, intTo As Integer
        Dim strFilter As String
        
        If (thisWorkbook.Sheets("Refresh").Cells(2, 1).Text <> "") Then
            strFilter = thisWorkbook.Sheets("Refresh").Cells(2, 1).Text
        Else
            strFilter = ""
        End If
        
        'Get sheet names
        If (strFilter = "ALL") Then
            arrSheets = GetSheets()
            
            Dim allIsNumeric As Boolean
            allIsNumeric = True
            
            For x = LBound(arrSheets) To UBound(arrSheets)
                If Not (IsNumeric(arrSheets(x))) Then
                    allIsNumeric = False
                End If
            Next x
            
            If (allIsNumeric) Then
                voidResult = ExportSheets(arrSheets)
            End If
        Else
            Dim intPos As Integer
            intPos = InStr(1, strFilter, "-")
        
            If (intPos <> 0) Then
                Dim arrPos() As String
                arrPos = Split(strFilter, "-")
                If (IsNumeric(arrPos(0))) And (IsNumeric(arrPos(1))) Then
                    intFrom = CInt(arrPos(0))
                    intTo = CInt(arrPos(1))
                    
                    arrSheets = GetSheetsMul(intFrom, intTo)
                    voidResult = ExportSheets(arrSheets)
                Else
                    thisWorkbook.Sheets("Refresh").Cells(1, 6) = "No numeric fields found"
                End If
            ElseIf (thisWorkbook.Sheets("Refresh").Cells(1, 1).Text <> "") Then
                arrSheets = Array(thisWorkbook.Sheets("Refresh").Cells(1, 1).Text)
                voidResult = ExportSheets(arrSheets)
            Else
                thisWorkbook.Sheets("Refresh").Cells(1, 6) = "Don't be saucy, Bearnaise!"
            End If
        End If
        
        thisWorkbook.Sheets("Refresh").Cells(2, 6) = "Here's Johnny! - Jack Torrance, The Shining"
    Else
        thisWorkbook.Sheets("Refresh").Cells(1, 6) = "Fill in the year or filter and try again"
    End If
End Sub

Sub btnUpdate_Click(strYear As String)
    Dim col As Long
    Dim Row As Long
    Dim defCol As Long

    Sheets("Refresh").Cells(1, 6) = ""
    Sheets("Refresh").Cells(2, 6) = ""
    
    'start test
    col = 2
    defCol = 2
    Row = 3
    Dim varDate As Date
    varDate = DateValue("jan 1, " & strYear)
    
    For x = 1 To 366
        Dim intDay As Integer
        intDay = Day(DateAdd("d", 1, varDate))
        If (intDay = 1) Then
            Dim dupCol As Long, dupRow As Long, dupCel As String
            dupRow = Row + 1
            dupCol = col + 1
        
            If (col < 32) Then
                For dupCol = (col + 1) To 32
                    dupCel = ColLetter(dupCol) & CStr(dupRow)
                    Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(dupRow, dupCol) = "=IF(MONTH(DATE($A$3,MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & dupCel & "),1))&"" 1"")),INDIRECT(ADDRESS(3,COLUMN(" & dupCel & ")))))>MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & dupCel & "),1))&"" 1"")),""X"",IF(WEEKDAY(DATE($A$3,MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & dupCel & "),1))&"" 1"")),INDIRECT(ADDRESS(3,COLUMN(" & dupCel & ")))))=1,""S"",IF(WEEKDAY(DATE($A$3,MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & dupCel & "),1))&"" 1"")),INDIRECT(ADDRESS(3,COLUMN(" & dupCel & ")))))=7,""S"","""")))"
                Next dupCol
            End If
            
        End If
        
        varDate = DateAdd("d", 1, varDate)
        col = col + 1
        
        If (Day(varDate) = 1) Then
            Row = Row + 1
            col = defCol
        End If
    Next x
    'end test
    
    col = 2
    defCol = 2
    Row = 3
    Red = RGB(255, 0, 0)
    blue = RGB(0, 176, 240)
    If (Sheets("Refresh").Cells(1, 1).Text <> "") Then
        For x = 1 To 366
            If (Sheets("Refresh").Cells(x, 2) <> "") Then
                If (Day(thisWorkbook.Sheets("Refresh").Cells(x, 2)) = 1) Then
                    Row = Row + 1
                    col = defCol
                End If
            End If
        
            Select Case Trim(Sheets("Refresh").Cells(x, 3))
                Case Is = "W1"
                    Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "W"
                Case Is = "W1/extra"
                    Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "W"
                Case Is = "W2"
                    Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "W"
                Case Is = "W2/extra"
                    Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "W"
                Case Is = "W3"
                    Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "W"
                Case Is = "W3/extra"
                    Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "W"
                Case Is = "Bp"
                    Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "B"
                Case Is = "W14"
                    Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "WL"
                Case Is = "W22"
                    Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "WN"
                Case Is = "D"
                    Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "D"
                Case Is = "O"
                    If (Sheets("Refresh").Cells(x, 3).Interior.Color = RGB(255, 0, 0)) Then
                        Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "FO"
                    ElseIf (Sheets("Refresh").Cells(x, 3).Interior.Color = RGB(0, 176, 240)) Then
                        Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "WO"
                    Else
                        Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "O"
                    End If
                    
                    If (Sheets("Refresh").Cells(x, 3).Interior.Color = RGB(55, 86, 35) Or Sheets("Refresh").Cells(x, 3).Interior.Color = RGB(151, 71, 6)) Then
                        If Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col).Comment Is Nothing Then
                            Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col).AddComment ("05:40 Uitwijk")
                        ElseIf (InStr(1, Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col).Comment.Text, "05:40 Uitwijk") = 0) Then
                            Dim strComment As String
                            strComment = Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col).Comment.Text
                            Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col).Comment.Delete
                            Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col).AddComment (strComment & vbNewLine & "05:40 Uitwijk")
                        End If
                    End If
                Case Is = "A"
                    If (Sheets("Refresh").Cells(x, 3).Interior.Color = RGB(255, 0, 0)) Then
                        Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "FL"
                    ElseIf (Sheets("Refresh").Cells(x, 3).Interior.Color = RGB(0, 176, 240)) Then
                        Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "WL"
                    Else
                        Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "L"
                    End If
                Case Is = "N"
                    If (Sheets("Refresh").Cells(x, 3).Interior.Color = RGB(255, 0, 0)) Then
                        Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "FN"
                    ElseIf (Sheets("Refresh").Cells(x, 3).Interior.Color = RGB(0, 176, 240)) Then
                        Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "WN"
                    Else
                        Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "N"
                    End If
                Case Is = "V"
                    If (Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col).Text <> "R") And (Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col).Text <> "r") Then
                        Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "V"
                    ElseIf (Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col).Text = "r") Then
                        Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "R"
                    End If
                Case Is = "Z"
                    Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "Z"
                Case Is = "Kv"
                    Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "Z"
                Case Else
                    If (Sheets("Refresh").Cells(x, 3).Interior.Color = RGB(255, 0, 0)) Then
                        Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "F"
                    Else
                        curCel = ColLetter(col) & CStr(Row)
                        Sheets(Sheets("Refresh").Cells(1, 1).Text).Cells(Row, col) = "=IF(MONTH(DATE($A$3,MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & curCel & "),1))&"" 1"")),INDIRECT(ADDRESS(3,COLUMN(" & curCel & ")))))>MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & curCel & "),1))&"" 1"")),""X"",IF(WEEKDAY(DATE($A$3,MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & curCel & "),1))&"" 1"")),INDIRECT(ADDRESS(3,COLUMN(" & curCel & ")))))=1,""S"",IF(WEEKDAY(DATE($A$3,MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & curCel & "),1))&"" 1"")),INDIRECT(ADDRESS(3,COLUMN(" & curCel & ")))))=7,""S"","""")))"
                    End If
            End Select
            
            col = col + 1
        Next x
        
        Sheets("Refresh").Cells(1, 6) = "I did the deed"
    Else
        Sheets("Refresh").Cells(1, 6) = "Fill in the year and try again"
    End If
End Sub

Sub btnExportSelected()
    frmName.Show
End Sub

Sub ExportSelected(strName As String, intYear As Integer)
    Dim kbcPlanning As Workbook
    Dim ownPlanning As Workbook
    
    For x = 1 To Workbooks.Count
        If (InStr(1, Workbooks(x).Name, "Planning") <> 0) And (InStr(1, Workbooks(x).Name, CStr(intYear)) <> 0) Then
            Set kbcPlanning = Workbooks(x)
        ElseIf (Workbooks(x).Name = "Planning.xlsm") Then
            Set ownPlanning = Workbooks(x)
        End If
        
        If Not (kbcPlanning Is Nothing) And Not (ownPlanning Is Nothing) Then
            Exit For
        End If
    Next x
    
    If (kbcPlanning Is Nothing) Then
        Set kbcPlanning = Workbooks.Open("/** REDACTED **/\" + CStr(intYear) + "\Planning " + CStr(intYear) + ".xlsm", False, True)
    End If
    
    For i = 1 To kbcPlanning.Sheets.Count
        Dim strSheet As String
        Dim nameCol As Integer
        Dim varDate As Date
        strSheet = kbcPlanning.Sheets(i).Name
        
        For a = 4 To 31
            If (kbcPlanning.Sheets(i).Cells(2, a).Text = strName) Then
                nameCol = a
            End If
        Next a
        
        If (InStr(1, strSheet, "PLANNING") > 0) Then
            'Create csv file
            Dim fso As Object
            Set fso = CreateObject("Scripting.FileSystemObject")
            Dim Fileout As Object
            Set Fileout = fso.CreateTextFile(ownPlanning.Path & "\" & RTrim(strName) & ".csv", True, True)
            Fileout.Close
            Open ownPlanning.Path & "\" & RTrim(strName) & ".csv" For Output As #1
            Print #1, "Subject,Start Date,End Date,All Day Event", vbNewLine;
    
            ownPlanning.Sheets("Refresh").Cells(1, 6) = ""
            ownPlanning.Sheets("Refresh").Cells(2, 6) = ""
            
            For x = 1 To 366
                If (kbcPlanning.Sheets(i).Cells((x + 3), 2) = "" Or InStr(kbcPlanning.Sheets(i).Cells((x + 3), 2), " ")) Then  'in case first date is empty or has spaces - dates don't have spaces
                    If (IsDate(kbcPlanning.Sheets(i).Cells((x + 4), 2))) Then
                        varDate = DateAdd("d", -1, DateValue(kbcPlanning.Sheets(i).Cells((x + 4), 2)))
                    Else
                        x = x + 1
                    End If
                Else
                    varDate = DateValue(kbcPlanning.Sheets(i).Cells((x + 3), 2))
                End If
                
                If (intYear <> Year(varDate)) Then
                    varDate = DateAdd("yyyy", 1, varDate)
                End If
                
                Dim amDate As String
                amDate = Format(varDate, "mm/dd/yyyy")
                
                Select Case Trim(kbcPlanning.Sheets(i).Cells((x + 3), nameCol))
                    Case Is = "W1"
                        Print #1, "W1," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "w1"
                        Print #1, "W1," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "W1/extra"
                        Print #1, "W1 extra," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "w1/extra"
                        Print #1, "W1 extra," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "W2"
                        Print #1, "W2," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "w2"
                        Print #1, "W2," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "W2/extra"
                        Print #1, "W2 extra," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "w2/extra"
                        Print #1, "W2 extra," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "W3"
                        Print #1, "W3," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "w3"
                        Print #1, "W3," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "W3/extra"
                        Print #1, "W3 extra," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "w3/extra"
                        Print #1, "W3 extra," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "Bp"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Font.Color = RGB(0, 0, 0)) Then
                            Print #1, "Bp mainframe," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        Else
                            Print #1, "Bp O/S," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        End If
                    Case Is = "Bpr"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Font.Color = RGB(0, 0, 0)) Then
                            Print #1, "Bpr mainframe," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        Else
                            Print #1, "Bpr O/S," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        End If
                    Case Is = "W14"
                        Print #1, "W14," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "w14"
                        Print #1, "W14," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "W14/extra"
                        Print #1, "W14 extra," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "w14/extra"
                        Print #1, "W14 extra," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "W22"
                        Print #1, "W22," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "w22"
                        Print #1, "W22," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "W22/extra"
                        Print #1, "W22 extra," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "w22/extra"
                        Print #1, "W22 extra," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "D"
                        Print #1, "D," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "O"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(255, 0, 0)) Then
                            Print #1, "O feestdag," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        ElseIf (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(55, 86, 35) Or kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(151, 71, 6)) Then
                            Print #1, "O uitwijk," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        Else
                            Print #1, "O," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        End If
                    Case Is = "o"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), 2).Interior.Color = RGB(255, 0, 0)) Then
                            Print #1, "O feestdag," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        ElseIf (kbcPlanning.Sheets(i).Cells((x + 3), 2).Interior.Color = RGB(55, 86, 35) Or kbcPlanning.Sheets(i).Cells((x + 3), 2).Interior.Color = RGB(151, 71, 6)) Then
                            Print #1, "O uitwijk," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        Else
                            Print #1, "O," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        End If
                    Case Is = "A"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), 2).Interior.Color = RGB(255, 0, 0)) Then
                            Print #1, "A feestdag," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        Else
                            Print #1, "A," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        End If
                    Case Is = "a"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), 2).Interior.Color = RGB(255, 0, 0)) Then
                            Print #1, "A feestdag," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        Else
                            Print #1, "A," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        End If
                    Case Is = "N"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), 2).Interior.Color = RGB(255, 0, 0)) Then
                            Print #1, "N feestdag," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        Else
                            Print #1, "N," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        End If
                    Case Is = "n"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), 2).Interior.Color = RGB(255, 0, 0)) Then
                            Print #1, "N feestdag," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        Else
                            Print #1, "N," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        End If
                    Case Is = "V"
                        Print #1, "V," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "v"
                        Print #1, "V," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "Z"
                        Print #1, "Z," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "z"
                        Print #1, "Z," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "KV"
                        Print #1, "Kv," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "Kv"
                        Print #1, "Kv," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "kV"
                        Print #1, "Kv," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Is = "kv"
                        Print #1, "Kv," & amDate & "," & amDate & ",TRUE", vbNewLine;
                    Case Else
                        If (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(255, 255, 255)) Then
                            Print #1, "d (wit)," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        ElseIf Not (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(255, 0, 0)) And Not (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(255, 255, 0)) And Not (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(0, 176, 240)) And Not (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(151, 71, 6)) And Not (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(0, 176, 80)) Then
                            Print #1, "******************Fout - zie planning******************," & amDate & "," & amDate & ",TRUE", vbNewLine;
                        End If
                End Select
            Next x
            
            Close #1
            
            ownPlanning.Sheets("Refresh").Cells(1, 6) = "Trump has launched the Mexicans over the wall"
            Exit For
        End If
    Next i
    
    kbcPlanning.Close (False)
End Sub
