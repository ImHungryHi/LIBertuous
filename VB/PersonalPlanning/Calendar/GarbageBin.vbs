Rem Attribute VBA_ModuleType=VBAModule
Option VBASupport 1

Sub TestChange()
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
            If (n = lngStartMonth And (m - 1) <= lngDay) Then
                m = lngDay + 1
            End If
            
            If (InStr(1, arrTimeDates((n - 3), (m - 1)), "N", vbTextCompare) Or InStr(1, arrTimeDates((n - 3), (m - 1)), "O", vbTextCompare) Or arrTimeDates((n - 3), (m - 1)) = "D" Or arrTimeDates((n - 3), (m - 1)) = "") Then
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
            ElseIf (InStr(1, arrTimeDates((n - 3), (m - 1)), "L", vbTextCompare)) Then
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
                Else
                    ownPlanning.Sheets(strSheet).Cells(n + 1, m).Interior.Color = xlNone
                    arrTank(n - 2, m - 1) = "68.0"
                    arrTank(n - 1, m - 1) = "68.0"
                    
                    ' Corrigeer 68 * 2 - 44.1 erbij als we niet moeten tanken
                    dblTotalDistance = dblTotalDistance + 91.9
                End If
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

Sub btnExportTimeOld(intYear As Integer)
    Dim strReturn As String, strFolder As String, strFileName As String, strCreateFolder As String, strCreateFileName As String, strParentFolder As String
    Dim intMonthStart As Integer, intCol As Integer, intColStart As Integer, intDay As Integer, intMonthEnd As Integer, intCurMonth As Integer, intTimeRow As Integer, intPeriod As Integer
    Dim wrkTimesheet As Workbook, wrkPlanning As Workbook
    Dim fileObject, fileCreated, blnGo
    
    intColStart = 2
    intCol = 2
    intDay = 0
    intMonthEnd = 0
    intTimeRow = 18
    intPeriod = 16
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
        strParentFolder = .GetParentFolderName(wbkHuis.Path)
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
                wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 3) = DateValue(intMonthEnd & "-" & intCurMonth & "-" & intYear)
                wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 6) = wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol)
                
                If (wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol) > TimeValue("20:30")) Then
                    wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 4) = "Half a Working Day"
                    wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 7) = wrkPlanning.Worksheets("Time " & intYear).Cells(2, 13)
                    
                    intTimeRow = intTimeRow + 1
                    
                    If (wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, (intCol + 1)) = "X" Or (intCol + 1) = 33) Then
                        wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 3) = DateValue(1 & "-" & (intCurMonth + 1) & "-" & intYear)
                    Else
                        wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 3) = DateValue((intMonthEnd + 1) & "-" & intCurMonth & "-" & intYear)
                    End If
                    
                    wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 4) = "Half a Working Day"
                    wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 6) = wrkPlanning.Worksheets("Time " & intYear).Cells(2, 15)
                Else
                    wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 4) = "Working Day"
                End If
                
                wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 7) = wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart + 1, intCol)
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
                        wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 3) = DateValue(intMonthEnd & "-" & intCurMonth & "-" & intYear)
                        
                        If (Hour(dtStartTime) < 22) Then
                            If ((intDOW = 6) And Not (blnHoliday Or wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol) = "FO" Or wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol) = "FL" Or wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol) = "FN")) Then
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 4) = "150% (rate for Saturdays)"
                            Else
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 4) = "200% (rate for nights, Sundays and public holidays)"
                                blnSpecial = True
                            End If
                        Else
                            wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 4) = "200% (rate for nights, Sundays and public holidays)"
                        End If
                    Else
                        wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 3) = DateValue(intMonthEnd + 1 & "-" & intCurMonth & "-" & intYear)
                    End If
                    
                    wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 6) = dtStartTime
                    
                    If (Hour(dtStartTime) < 22 And Hour(dtStartTime) >= 6 And (Hour(dtEndTime) > 21 Or Hour(dtEndTime) < 6)) Then
                        Dim strTemp As String
                        strTemp = wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol)
                        If (wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol) = "FL" Or wrkPlanning.Worksheets("Time " & intYear).Cells(intMonthStart, intCol) = "FN") Then
                            If (Hour(dtStartTime) < 23 And Hour(dtEndTime) < 21 And Hour(dtEndTime) < 12) Then
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 7) = TimeValue("23:59:00")
                                
                                intPeriod = intPeriod + 1
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 6) = TimeValue("00:00:00")
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 3) = DateValue((intMonthEnd + 1) & "-" & intCurMonth & "-" & intYear)
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 4) = "200% (rate for nights, Sundays and public holidays)"
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 7) = dtEndTime
                            Else
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 7) = dtEndTime
                            End If
                        Else
                            If (Not blnSpecial) Then
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 7) = TimeValue("21:59:00")
                                intPeriod = intPeriod + 1
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 6) = TimeValue("22:00:00")
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 3) = DateValue(intMonthEnd & "-" & intCurMonth & "-" & intYear)
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 4) = "200% (rate for nights, Sundays and public holidays)"
                            End If
                        
                            If (Hour(dtEndTime) < 6) Then
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 7) = TimeValue("23:59:00")
                                intPeriod = intPeriod + 1
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 6) = TimeValue("00:00:00")
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 3) = DateValue((intMonthEnd + 1) & "-" & intCurMonth & "-" & intYear)
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 4) = "200% (rate for nights, Sundays and public holidays)"
                            End If
                            
                            wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 7) = dtEndTime
                        End If
                    Else
                        If (Hour(dtStartTime) <= 23 And Hour(dtEndTime) < 22) Then
                            If (Hour(dtEndTime) < 12 And Hour(dtStartTime) > 12) Then
                                If (Hour(dtStartTime) < 22 And Not blnSpecial) Then
                                    wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 7) = TimeValue("21:59:00")
                                    intPeriod = intPeriod + 1
                                    wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 6) = TimeValue("22:00:00")
                                    wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 3) = DateValue(intMonthEnd & "-" & intCurMonth & "-" & intYear)
                                    wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 4) = "200% (rate for nights, Sundays and public holidays)"
                                End If
                                
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 7) = TimeValue("23:59:00")
                                intPeriod = intPeriod + 1
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 6) = TimeValue("00:00:00")
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 3) = DateValue((intMonthEnd + 1) & "-" & intCurMonth & "-" & intYear)
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 4) = "200% (rate for nights, Sundays and public holidays)"
                            End If
                            
                            If (Hour(dtStartTime) < 6 And Hour(dtEndTime) <= 6) Then
                                wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 4) = "200% (rate for nights, Sundays and public holidays)"
                            End If
                            
                            wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 7) = dtEndTime
                        End If
                    End If
                    
                    intPeriod = intPeriod + 1
                Next j
            End If
        Next intCol
        
        For intTimeRow = intTimeRow To 66
            wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 3) = ""
            wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 4) = ""
            wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 6) = ""
            wrkTimesheet.Worksheets("Time Sheet").Cells(intTimeRow, 7) = ""
        Next intTimeRow
        
        For intPeriod = intPeriod To 35
            wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 3) = ""
            wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 4) = ""
            wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 6) = ""
            wrkTimesheet.Worksheets("Request for payment").Cells(intPeriod, 7) = ""
        Next intPeriod
        
        MsgBox ("May the force be with you" & vbNewLine & "Just about anyone, Star Wars")
    End If
End Sub

Sub TestSub()
    Dim n As Integer, m As Integer, x As Integer, y As Integer
    
    For n = 34 To 37
        For m = 2 To 32
            Dim strNextShift As String
            strNextShift = "NA"
            
            If (n = 34 And (m - 1) <= 27) Then
                m = 27 + 1
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
        Next m
    Next n
End Sub

Sub OldTank()
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
    
    ' Dummy do wa diddy diddy dum diddy do - dient enkel en alleen om geneste for te ontsnappen
    Do
        For y = 0 To 1
            For x = 1 To 20
                Dim intRow As Integer, intCol As Integer, intTop As Double, intBottom As Double
                intRow = lngStartMonth + 1 - (y * 3)
                intCol = lngDay + 1
                
                If (intCol - x <= 1) Then
                    y = 1
                    intRow = lngStartMonth + 1 - (y * 3)  ' Voor het geval y moet veranderen om zeker te zijn dat er niet teveel wordt afgetrokken (Ha Ha!)
                    intCol = 31 + 1 - (x - lngDay)
                Else
                    intCol = lngDay + 2 - x
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
                    
                    If (Not ownPlanning.Sheets(strSheet).Cells(intRow, intCol).Comment Is Nothing) Then
                        dblTotal = dblTotal + intBottom
                        Exit Do
                    ElseIf (intTop = 44.1 Or intTop = 102.8) Then
                        dblTotal = dblTotal + intTop
                    Else
                        dblTotal = dblTotal + intTop + intBottom
                    End If
                End If
            Next x
            
            If (lngDay - x > 0) Then
                y = y + 1
            End If
        Next y
    Loop While False
    
    ' Geen B, S, X, R, V, F, Z
    For n = lngStartMonth To 37
        For m = 2 To 32
            If (n = lngStartMonth And (m - 1) <= lngDay) Then
                m = lngDay + 1
            End If
            
            If (InStr(1, arrTimeDates((n - 3), (m - 1)), "N", vbTextCompare) Or InStr(1, arrTimeDates((n - 3), (m - 1)), "O", vbTextCompare) Or arrTimeDates((n - 3), (m - 1)) = "D" Or arrTimeDates((n - 3), (m - 1)) = "") Then
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
            ElseIf (InStr(1, arrTimeDates((n - 3), (m - 1)), "L", vbTextCompare)) Then
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
                Else
                    ownPlanning.Sheets(strSheet).Cells(n + 1, m).Interior.Color = xlNone
                    arrTank(n - 2, m - 1) = "68.0"
                    arrTank(n - 1, m - 1) = "68.0"
                    
                    ' Corrigeer 68 * 2 - 44.1 erbij als we niet moeten tanken
                    dblTotalDistance = dblTotalDistance + 91.9
                End If
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

Function LoadDirectOld(strYear As String)
    Dim kbcPlanning As Workbook
    Dim ownPlanning As Workbook
    Dim canIClose As Boolean
    Dim ownColumn As Integer
    canIClose = False
    
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
    
    Dim col As Long, Row As Long, defCol As Long
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
                    ownPlanning.Sheets(strYear).Cells(dupRow, dupCol) = "=IF(MONTH(DATE($A$3,MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & dupCel & "),1))&"" 1"")),INDIRECT(ADDRESS(3,COLUMN(" & dupCel & ")))))>MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & dupCel & "),1))&"" 1"")),""X"",IF(WEEKDAY(DATE($A$3,MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & dupCel & "),1))&"" 1"")),INDIRECT(ADDRESS(3,COLUMN(" & dupCel & ")))))=1,""S"",IF(WEEKDAY(DATE($A$3,MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & dupCel & "),1))&"" 1"")),INDIRECT(ADDRESS(3,COLUMN(" & dupCel & ")))))=7,""S"","""")))"
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
    
    If (kbcPlanning Is Nothing) Then
        Set kbcPlanning = Workbooks.Open("/** REDACTED **/\" & strYear & "\Planning " & strYear & ".xlsm", False, True)
        kbcPlanning.Application.WindowState = xlMinimized
        canIClose = True
    End If
    
    For i = 1 To kbcPlanning.Sheets.Count
        Dim strSheet As String
        strSheet = kbcPlanning.Sheets(i).Name
        
        If (InStr(1, strSheet, "PLANNING") > 0) Then
            'MsgBox (kbcPlanning.Sheets(i).Cells(4, 2).Text & ":" & kbcPlanning.Sheets(i).Cells(4, 17).Text)
        
            ownPlanning.Sheets("Refresh").Cells(1, 6) = ""
            ownPlanning.Sheets("Refresh").Cells(2, 6) = ""
            col = 2
            defCol = 2
            Row = 3
            Red = RGB(255, 0, 0)
            blue = RGB(0, 176, 240)
            
            For x = 1 To 48
                If (InStr(1, kbcPlanning.Sheets(i).Cells(2, x), "Chris De Smedt") > 0) Then
                    ownColumn = x
                End If
            Next x

            If (strYear = "") Then
                If (ownPlanning.Sheets("Refresh").Cells(1, 1).Text = "") Then
                    MsgBox ("Fill in the year and try again")
                Else
                    strYear = ownPlanning.Sheets("Refresh").Cells(1, 1).Text
                End If
            End If

            If (strYear <> "") Then
                For x = 1 To 366
                    Dim blnSkip As Boolean
                    Dim varNextYear As Date, varKBCDate As Date
                    
                    blnSkip = False
                    varNextYear = DateValue("jan 1, " & (CInt(strYear) + 1))
                
                    If (kbcPlanning.Sheets(i).Cells((x + 3), 2) <> "" And IsDate(kbcPlanning.Sheets(i).Cells((x + 3), 2))) Then
                        varKBCDate = kbcPlanning.Sheets(i).Cells((x + 3), 2)
                        
                        If (kbcPlanning.Sheets(i).Cells(x, 2) = varNextYear) Then
                            blnSkip = True
                        End If
                    
                        If (Day(kbcPlanning.Sheets(i).Cells((x + 7), 2)) = 5) Then
                            Row = Row + 1
                            col = defCol
                        End If
                        
                        If ((varKBCDate = DateValue("08-04-2017") Or varKBCDate = DateValue("09-04-2017") Or varKBCDate = DateValue("08-07-2017") Or varKBCDate = DateValue("09-07-2017")) And strYear = "2017") Then
                            blnSkip = True
                        End If
                    Else
                        Dim varTempCheckDate As Date
                        varTempCheckDate = kbcPlanning.Sheets(i).Cells((x + 5), 2)
                        
                        If (Day(varTempCheckDate) = 3 And Month(varTempCheckDate) = 1) Then
                            varKBCDate = DateAdd("d", -2, varTempCheckDate)
                        End If
                    
                        If (Day(kbcPlanning.Sheets(i).Cells((x + 7), 2)) = 5) Then
                            Row = Row + 1
                            col = defCol
                        End If
                    End If
                    
                    If ((Row = 15 And col = 33) Or Row > 16 Or (Day(DateSerial(CInt(strYear), 2, 28) + 1) <> 29 And x > 365)) Then
                        blnSkip = True
                    End If
                    
                    If Not blnSkip Then
                        If Not (ownPlanning.Sheets(strYear).Cells(Row, col).Comment Is Nothing) Then
                            Dim strCommenter As String, blnBooberella As Boolean, strMinus As String, strStuff As String, varSplitArray As Variant
                            strCommenter = ""
                            
                            If (InStr(1, ownPlanning.Sheets(strYear).Cells(Row, col).Comment.Text, Chr(10) & "05:40 Uitwijk") > 0) Then
                                strCommenter = ownPlanning.Sheets(strYear).Cells(Row, col).Comment.Text
                                strMinus = Replace(strCommenter, Chr(10) & "05:40 Uitwijk", "")
                                ownPlanning.Sheets(strYear).Cells(Row, col).Comment.Delete
                                ownPlanning.Sheets(strYear).Cells(Row, col).AddComment (strMinus)
                            End If
                            
                            If (InStr(1, ownPlanning.Sheets(strYear).Cells(Row, col).Comment.Text, "05:40 Uitwijk" & Chr(10)) > 0) Then
                                strCommenter = ownPlanning.Sheets(strYear).Cells(Row, col).Comment.Text
                                strMinus = Replace(strCommenter, "05:40 Uitwijk" & Chr(10), "")
                                ownPlanning.Sheets(strYear).Cells(Row, col).Comment.Delete
                                ownPlanning.Sheets(strYear).Cells(Row, col).AddComment (strMinus)
                            End If
                            
                            If (ownPlanning.Sheets(strYear).Cells(Row, col).Comment.Text = "05:40 Uitwijk") Then
                                ownPlanning.Sheets(strYear).Cells(Row, col).Comment.Delete
                            End If
                        End If
                        
                        Select Case Trim(kbcPlanning.Sheets(i).Cells((x + 3), ownColumn))
                            Case Is = "W1"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "W"
                            Case Is = "W1/extra"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "W"
                            Case Is = "W2"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "W"
                            Case Is = "W2/extra"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "W"
                            Case Is = "W3"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "W"
                            Case Is = "W3/extra"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "W"
                            Case Is = "Bp"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "B"
                            Case Is = "W14"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "WL"
                            Case Is = "W22"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "WN"
                            Case Is = "w1"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "W"
                            Case Is = "w1/extra"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "W"
                            Case Is = "w2"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "WN"
                            Case Is = "w2/extra"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "WN"
                            Case Is = "w3"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "W"
                            Case Is = "w3/extra"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "W"
                            Case Is = "bp"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "B"
                            Case Is = "w14"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "WL"
                            Case Is = "w22"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "WN"
                            Case Is = "D"
                                If varKBCDate <> DateValue("10-02-2017") Then ownPlanning.Sheets(strYear).Cells(Row, col) = "D"
                            Case Is = "O"
                                If (kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(255, 0, 0) And varKBCDate <> DateValue("22-02-2017") And varKBCDate <> DateValue("24-02-2017")) Then
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = "FO"
                                ElseIf (kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(0, 176, 240)) Then
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = "WO"
                                Else
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = "O"
                                End If
                                
                                If Not (kbcPlanning.Sheets(i).Range("A" & (x + 3), "ZZ" & (x + 3)).Find(What:="itwijk") Is Nothing Or kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(255, 255, 255) Or kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(255, 0, 0) Or kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(255, 255, 0) Or kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(218, 150, 148)) Then
                                'If (kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(55, 86, 35) Or kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(151, 71, 6) Or kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(192, 0, 0)) Then
                                    If ownPlanning.Sheets(strYear).Cells(Row, col).Comment Is Nothing Then
                                        ownPlanning.Sheets(strYear).Cells(Row, col).AddComment ("05:40 Uitwijk")
                                    ElseIf (InStr(1, ownPlanning.Sheets(strYear).Cells(Row, col).Comment.Text, "05:40 Uitwijk") = 0) Then
                                        Dim strComment As String
                                        strComment = ownPlanning.Sheets(strYear).Cells(Row, col).Comment.Text
                                        ownPlanning.Sheets(strYear).Cells(Row, col).Comment.Delete
                                        
                                        If (CInt(Left(strComment, 2)) < 5 Or (CInt(Left(strComment, 2)) = 5 And CInt(Mid(strComment, 4, 2)) < 40)) Then
                                            ownPlanning.Sheets(strYear).Cells(Row, col).AddComment (strComment & Chr(10) & "05:40 Uitwijk")
                                        Else
                                            ownPlanning.Sheets(strYear).Cells(Row, col).AddComment ("05:40 Uitwijk" & Chr(10) & strComment)
                                        End If
                                    End If
                                End If
                            Case Is = "A"
                                If (kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(255, 0, 0)) Then
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = "FL"
                                ElseIf (kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(0, 176, 240)) Then
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = "WL"
                                Else
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = "L"
                                End If
                            Case Is = "N"
                                If (kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(255, 0, 0)) Then
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = "FN"
                                ElseIf (kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(0, 176, 240)) Then
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = "WN"
                                Else
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = "N"
                                End If
                            Case Is = "V"
                                If (ownPlanning.Sheets(strYear).Cells(Row, col).Text <> "R") And (ownPlanning.Sheets(strYear).Cells(Row, col).Text <> "r") And (ownPlanning.Sheets(strYear).Cells(Row, col).Text <> "F") And (ownPlanning.Sheets(strYear).Cells(Row, col).Text <> "f") And (ownPlanning.Sheets(strYear).Cells(Row, col).Text <> "Z") And (ownPlanning.Sheets(strYear).Cells(Row, col).Text <> "z") Then
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = "V"
                                ElseIf (ownPlanning.Sheets(strYear).Cells(Row, col).Text = "F") Or (ownPlanning.Sheets(strYear).Cells(Row, col).Text = "f") Then
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = "F"
                                ElseIf (ownPlanning.Sheets(strYear).Cells(Row, col).Text = "r") Then
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = "R"
                                End If
                            Case Is = "Z"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "Z"
                            Case Is = "Kv"
                                ownPlanning.Sheets(strYear).Cells(Row, col) = "Z"
                            Case Else
                                If (kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color = RGB(255, 0, 0)) Then
                                    If (ownPlanning.Sheets(strYear).Cells(Row, col).Text <> "R") And (ownPlanning.Sheets(strYear).Cells(Row, col).Text <> "r") Then
                                        ownPlanning.Sheets(strYear).Cells(Row, col) = "F"
                                    End If
                                ElseIf (InStr(1, kbcPlanning.Sheets(i).Cells((x + 3), ownColumn), "?")) Then
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = kbcPlanning.Sheets(i).Cells((x + 3), ownColumn)
                                Else
                                    curCel = ColLetter(col) & CStr(Row)
                                    ownPlanning.Sheets(strYear).Cells(Row, col) = "=IF(MONTH(DATE($A$3,MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & curCel & "),1))&"" 1"")),INDIRECT(ADDRESS(3,COLUMN(" & curCel & ")))))>MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & curCel & "),1))&"" 1"")),""X"",IF(WEEKDAY(DATE($A$3,MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & curCel & "),1))&"" 1"")),INDIRECT(ADDRESS(3,COLUMN(" & curCel & ")))))=1,""S"",IF(WEEKDAY(DATE($A$3,MONTH(DATEVALUE(INDIRECT(ADDRESS(ROW(" & curCel & "),1))&"" 1"")),INDIRECT(ADDRESS(3,COLUMN(" & curCel & ")))))=7,""S"","""")))"
                                End If
                        End Select
                    End If
                    
                    col = col + 1
                Next x
    
                If (canIClose = True) Then
                    kbcPlanning.Close (False)
                End If
                
                MsgBox ("It's alive! It's alive!" & vbNewLine & "Frankenstein")
            Else
                If (canIClose = True) Then
                    kbcPlanning.Close (False)
                End If
                
                MsgBox ("Fill in the year and try again")
            End If
            Exit For
        End If
    Next i
End Function

Sub test()
    Dim kbcPlanning As Workbook, strName As String, stPath As String, sjaar As String, smaand As String, startdatum As Date, stAttachment As String
    Dim fileObject
    Set fileObject = CreateObject("Scripting.FileSystemObject")
    startdatum = Format(DateValue("02/01/2018"), "dd mmmm yyyy")
    MsgBox (startdatum)
    sjaar = Format(startdatum, "yyyy") & "\"
    smaand = Format(startdatum, "mmmm") & "\"
    
    MsgBox (sjaar & ";" & smaand)
    'strName = Application.Caller

    'Als den brol via SharePoint moet worden geopend, best eerst de versie van SharePoint openen in de lokale Excel en vandaar de link kopiëren
    'Set kbcPlanning = Workbooks.Open("/** REDACTED **//Planning%202016.xlsm?web=1", False, True)
    
    'For i = 1 To kbcPlanning.Sheets.Count
    '    Dim strSheet As String
    '    strSheet = kbcPlanning.Sheets(i).Name
        
    '    If (InStr(1, strSheet, "PLANNING") > 0) Then
    '        strName = kbcPlanning.Sheets(i).Cells(2, 17).Text
    '    End If
    'Next i
    
    'kbcPlanning.Close (False)
    stPath = "/** REDACTED **/\" & sjaar & smaand
    stAttachment = stPath & "\test.xls"
    
    If Not (fileObject.FolderExists(stPath)) Then
        fileObject.CreateFolder (stPath)
    End If
    
    With ActiveWorkbook
        .SaveAs stAttachment
    End With
    'MsgBox ("You clicked " & strName)
End Sub

Sub Tester(strName As Variant)
    MsgBox ("You clicked " & strName)
End Sub

Sub Testerer()
    Dim bbDate As Date
    bbDate = kbcPlanning.Sheets(i).Cells((x + 3), 2)
    
    If (Day(bbDate) = 30 And Month(bbDate) = 10) Then
        Dim boober As Boolean, clrStuff As Variant, clrStuffer As Variant
        clrStuff = kbcPlanning.Sheets(i).Cells((x + 3), ownColumn).Interior.Color
        clrStuffer = (clrStuff Mod 256) & ", " & ((clrStuff \ 256) Mod 256) & ", " & (clrStuff \ 65536)
        boober = True
    End If
    
    'MsgBox ("" & (Day(DateSerial(CInt("2016"), 2, 28) + 1) = 29))
End Sub

Sub Testerest()
    Dim kbcPlanning As Workbook, ownPlanning As Workbook
    Dim intUitwijk As Integer, intWeekend As Integer, intHoliday As Integer, intDay As Integer, intO As Integer, intA As Integer, intN As Integer, intV As Integer, intZ As Integer, intU As Integer, intAbnormal As Integer
    Dim strUitwijk As String, strU As String, strV As String, strZ As String, strH As String
    
    For x = 1 To Workbooks.Count
        If (InStr(1, Workbooks(x).Name, "Planning") <> 0) And (InStr(1, Workbooks(x).Name, "2018") <> 0) Then
            Set kbcPlanning = Workbooks(x)
        ElseIf (Workbooks(x).Name = "Planning.xlsm") Then
            Set ownPlanning = Workbooks(x)
        End If
        
        If Not (kbcPlanning Is Nothing) And Not (ownPlanning Is Nothing) Then
            Exit For
        End If
    Next x
    
    intUitwijk = 0
    intWeekend = 0
    intHoliday = 0
    intAbnormal = 0
    intDay = 0
    intO = 0
    intA = 0
    intN = 0
    intV = 0
    intZ = 0
    intU = 0
    strUitwijk = ""
    strU = ""
    strV = ""
    strZ = ""
    strH = ""
    
    Dim dtDate As Date
    Dim clrPloeg As Variant, clrRGB As Variant
    Dim strUCheck As String
    
    dtDate = kbcPlanning.Sheets("PLANNING ").Cells(29, 2)
    clrPloeg = kbcPlanning.Sheets("PLANNING ").Cells(29, 21).Interior.Color
    clrRGB = (clrPloeg Mod 256) & "," & ((clrPloeg \ 256) Mod 256) & "," & (clrPloeg \ 65536)
    strUCheck = kbcPlanning.Sheets("PLANNING ").Cells(29, 38)
    
    'For x = 5 To 368
        'Dim dtDate As Date
        'Dim clrPloeg As Variant, clrRGB As Variant
        'Dim strUCheck As String
        
        'dtDate = kbcPlanning.Sheets("PLANNING ").Cells(x, 2)
        'clrPloeg = kbcPlanning.Sheets("PLANNING ").Cells(x, 20).Interior.Color
        'clrRGB = (clrPloeg Mod 256) & "," & ((clrPloeg \ 256) Mod 256) & "," & (clrPloeg \ 65536)
        'strUCheck = kbcPlanning.Sheets("PLANNING ").Cells(x, 38)
        
        'If Not (kbcPlanning.Sheets("PLANNING ").Range("A" & x, "ZZ" & x).Find(What:="itwijk") Is Nothing Or clrRGB = "255,255,255") Then
        'If (InStr(1, strUCheck, "itwijk")) Then
                'intUitwijk = intUitwijk + 1
                'strUitwijk = strUitwijk & ";" & dtDate
        'End If
        
        'If (clrRGB = "192,0,0") Then
            'intU = intU + 1
            'strU = strU & ";" & dtDate
        'End If
        
        'If (clrRGB = "255,0,0") Then
            'intHoliday = intHoliday + 1
            'strH = strH & ";" & dtDate
        'End If
        
        'If (clrRGB = "0,176,240") Then
            'intWeekend = intWeekend + 1
        'End If
        
        'If ((clrRGB = "255,255,255" And Trim(kbcPlanning.Sheets("PLANNING ").Cells(x, 20)) = "D") Or (clrRGB = "255,255,255" And Trim(kbcPlanning.Sheets("PLANNING ").Cells(x, 20)) = "d") Or (clrRGB = "255,255,255" And Trim(kbcPlanning.Sheets("PLANNING ").Cells(x, 20)) = "") Or (clrRGB = "255,255,0" And Trim(kbcPlanning.Sheets("PLANNING ").Cells(x, 20)) = "D") Or (clrRGB = "255,255,0" And Trim(kbcPlanning.Sheets("PLANNING ").Cells(x, 20)) = "d") Or (clrRGB = "255,255,0" And Trim(kbcPlanning.Sheets("PLANNING ").Cells(x, 20)) = "")) Then
            'intDay = intDay + 1
        'End If
        
        'If Not (clrRGB = "255,255,255") Then
            'intAbnormal = intAbnormal + 1
        'End If
        
        'Select Case (Trim(kbcPlanning.Sheets("PLANNING ").Cells(x, 20)))
            'Case Is = "O": intO = intO + 1
            'Case Is = "o": intO = intO + 1
            'Case Is = "A": intA = intA + 1
            'Case Is = "a": intA = intA + 1
            'Case Is = "N": intN = intN + 1
            'Case Is = "n": intN = intN + 1
            'Case Is = "V":
                'intV = intV + 1
                'strV = strV & ";" & dtDate
            'Case Is = "v":
                'intV = intV + 1
                'strV = strV & ";" & dtDate
            'Case Is = "Z":
                'intZ = intZ + 1
                'strZ = strZ & ";" & dtDate
            'Case Is = "z":
                'intZ = intZ + 1
                'strZ = strZ & ";" & dtDate
            'Case Else
        'End Select
    'Next x
    
    'MsgBox ("Onderstaande gevonden:" & vbNewLine & "O: " & intO & vbNewLine & "A: " & intA & vbNewLine & "N: " & intN & vbNewLine & "Uitwijk: " & intUitwijk & " - " & strUitwijk & vbNewLine & "U: " & intU & " - " & strU & vbNewLine & "V: " & intV & " - " & strV & vbNewLine & "H: " & intHoliday & " - " & strH & vbNewLine & "Z: " & intZ & " - " & strZ & vbNewLine & "D: " & intDay & vbNewLine & "Weekend: " & intWeekend & vbNewLine & "Abby something: " & intAbnormal)
End Sub
