Rem Attribute VBA_ModuleType=VBAModule
Option VBASupport 1
Sub btnExport_Click()
    frmName.Show
End Sub

Sub ExportSelected(strName As String, intYear As Integer, blnTime As Boolean)
    Dim kbcPlanning As Workbook
    Dim ownPlanning As Workbook
    
    For x = 1 To Workbooks.Count
        If (InStr(1, Workbooks(x).Name, "Planning") <> 0) And (InStr(1, Workbooks(x).Name, CStr(intYear)) <> 0) Then
            Set kbcPlanning = Workbooks(x)
        Else
            If (Workbooks(x).Name = "Planning.xlsm") Then
                Set ownPlanning = Workbooks(x)
            End If
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
            Dim strAdditive As String
            strAdditive = ""
            Set fso = CreateObject("Scripting.FileSystemObject")
            Dim Fileout As Object
            Set Fileout = fso.CreateTextFile("Y:\" & RTrim(strName) & ".csv", True, True)
            Fileout.Close
            Open "Y:\" & RTrim(strName) & ".csv" For Output As #1
            
            If (blnTime) Then
                strAdditive = ",Start Time,End Time"
            End If
            
            Print #1, "Subject,Start Date,End Date,All Day Event" & strAdditive, vbNewLine;
    
            ownPlanning.Sheets("Export").Cells(1, 1) = ""
            
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
                    varDate = DateAdd("yyyy", (intYear - Year(varDate)), varDate)
                End If
                
                Dim amDate As String, amDateTwo As String, strFullDay As String, strDay As String, strEarly As String, strLate As String, strNight As String, strBeep As String, strW1 As String, strW2 As String, strW3 As String, strW14 As String, strW16 As String, strW22 As String
                amDate = Format(varDate, "mm/dd/yyyy")
                amDateTwo = Format(varDate, "mm/dd/yyyy")
                strFullDay = "TRUE"
                strDay = ""
                strEarly = ""
                strLate = ""
                strNight = ""
                strBeep = ""
                strW1 = ""
                strW2 = ""
                strW3 = ""
                strW14 = ""
                strW16 = ""
                strW22 = ""
                
                If (blnTime) Then
                    amDateTwo = Format(DateAdd("d", 1, varDate), "mm/dd/yyyy")
                    strFullDay = "FALSE"
                    strDay = ",7:00 AM,15:00 PM"
                    strEarly = ",6:00 AM,2:04 PM"
                    strLate = ",2:00 PM,10:09 PM"
                    strNight = ",10:00 PM,6:05 AM"
                    strBeep = ",6:00 AM,6:00 AM"
                    strW1 = ",4:00 PM,10:00 PM"
                    strW2 = ",10:00 PM,4:00 AM"
                    strW3 = ",4:00 AM,10:00 AM"
                    strW14 = ",2:00 PM,10:00 PM"
                    strW16 = ",4:00 PM,12:00 AM"
                    strW22 = ",10:00 PM,6:00 AM"
                End If
                
                Select Case Trim(kbcPlanning.Sheets(i).Cells((x + 3), nameCol))
                    Case Is = "W1"
                        Print #1, "W1," & amDate & "," & amDate & "," & strFullDay & strW1, vbNewLine;
                    Case Is = "w1"
                        Print #1, "W1," & amDate & "," & amDate & "," & strFullDay & strW1, vbNewLine;
                    Case Is = "W1/extra"
                        Print #1, "W1 extra," & amDate & "," & amDate & "," & strFullDay & strW1, vbNewLine;
                    Case Is = "w1/extra"
                        Print #1, "W1 extra," & amDate & "," & amDate & "," & strFullDay & strW1, vbNewLine;
                    Case Is = "W2"
                        Print #1, "W2," & amDate & "," & amDateTwo & "," & strFullDay & strW2, vbNewLine;
                    Case Is = "w2"
                        Print #1, "W2," & amDate & "," & amDateTwo & "," & strFullDay & strW2, vbNewLine;
                    Case Is = "W2/extra"
                        Print #1, "W2 extra," & amDate & "," & amDateTwo & "," & strFullDay & strW2, vbNewLine;
                    Case Is = "w2/extra"
                        Print #1, "W2 extra," & amDate & "," & amDateTwo & "," & strFullDay & strW2, vbNewLine;
                    Case Is = "W3"
                        Print #1, "W3," & amDateTwo & "," & amDateTwo & "," & strFullDay & strW3, vbNewLine;
                    Case Is = "w3"
                        Print #1, "W3," & amDateTwo & "," & amDateTwo & "," & strFullDay & strW3, vbNewLine;
                    Case Is = "W3/extra"
                        Print #1, "W3 extra," & amDateTwo & "," & amDateTwo & "," & strFullDay & strW3, vbNewLine;
                    Case Is = "w3/extra"
                        Print #1, "W3 extra," & amDateTwo & "," & amDateTwo & "," & strFullDay & strW3, vbNewLine;
                    Case Is = "Bp"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Font.Color = RGB(0, 0, 0)) Then
                            Print #1, "Bp mainframe," & amDate & "," & amDateTwo & "," & strFullDay & strBeep, vbNewLine;
                        Else
                            Print #1, "Bp O/S," & amDate & "," & amDateTwo & "," & strFullDay & strBeep, vbNewLine;
                        End If
                    Case Is = "Bpr"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Font.Color = RGB(0, 0, 0)) Then
                            Print #1, "Bpr mainframe," & amDate & "," & amDateTwo & "," & strFullDay & strBeep, vbNewLine;
                        Else
                            Print #1, "Bpr O/S," & amDate & "," & amDateTwo & "," & strFullDay & strBeep, vbNewLine;
                        End If
                    Case Is = "W14"
                        Print #1, "W14," & amDate & "," & amDate & "," & strFullDay & strW14, vbNewLine;
                    Case Is = "w14"
                        Print #1, "W14," & amDate & "," & amDate & "," & strFullDay & strW14, vbNewLine;
                    Case Is = "W14/extra"
                        Print #1, "W14 extra," & amDate & "," & amDate & "," & strFullDay & strW14, vbNewLine;
                    Case Is = "w14/extra"
                        Print #1, "W14 extra," & amDate & "," & amDate & "," & strFullDay & strW14, vbNewLine;
                    Case Is = "W16"
                        Print #1, "W16," & amDate & "," & amDateTwo & "," & strFullDay & strW16, vbNewLine;
                    Case Is = "w16"
                        Print #1, "W16," & amDate & "," & amDateTwo & "," & strFullDay & strW16, vbNewLine;
                    Case Is = "W16/extra"
                        Print #1, "W16 extra," & amDate & "," & amDateTwo & "," & strFullDay & strW16, vbNewLine;
                    Case Is = "w16/extra"
                        Print #1, "W16 extra," & amDate & "," & amDateTwo & "," & strFullDay & strW16, vbNewLine;
                    Case Is = "W22"
                        Print #1, "W22," & amDate & "," & amDateTwo & "," & strFullDay & strW22, vbNewLine;
                    Case Is = "w22"
                        Print #1, "W22," & amDate & "," & amDateTwo & "," & strFullDay & strW22, vbNewLine;
                    Case Is = "W22/extra"
                        Print #1, "W22 extra," & amDate & "," & amDateTwo & "," & strFullDay & strW22, vbNewLine;
                    Case Is = "w22/extra"
                        Print #1, "W22 extra," & amDate & "," & amDateTwo & "," & strFullDay & strW22, vbNewLine;
                    Case Is = "D"
                        Print #1, "D," & amDate & "," & amDate & "," & strFullDay & strDay, vbNewLine;
                    Case Is = "O"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(255, 0, 0)) Then
                            Print #1, "O feestdag," & amDate & "," & amDate & "," & strFullDay & strEarly, vbNewLine;
                        Else
                            If (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(55, 86, 35) Or kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(151, 71, 6) Or kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(192, 0, 0)) Then
                                Print #1, "O uitwijk," & amDate & "," & amDate & "," & strFullDay & strEarly, vbNewLine;
                            Else
                                Print #1, "O," & amDate & "," & amDate & "," & strFullDay & strEarly, vbNewLine;
                            End If
                        End If
                    Case Is = "o"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), 2).Interior.Color = RGB(255, 0, 0)) Then
                            Print #1, "O feestdag," & amDate & "," & amDate & "," & strFullDay & strEarly, vbNewLine;
                        Else
                            If (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(55, 86, 35) Or kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(151, 71, 6) Or kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(192, 0, 0)) Then
                                Print #1, "O uitwijk," & amDate & "," & amDate & "," & strFullDay & strEarly, vbNewLine;
                            Else
                                Print #1, "O," & amDate & "," & amDate & "," & strFullDay & strEarly, vbNewLine;
                            End If
                        End If
                    Case Is = "A"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), 2).Interior.Color = RGB(255, 0, 0)) Then
                            Print #1, "A feestdag," & amDate & "," & amDate & "," & strFullDay & strLate, vbNewLine;
                        Else
                            Print #1, "A," & amDate & "," & amDate & "," & strFullDay & strLate, vbNewLine;
                        End If
                    Case Is = "a"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), 2).Interior.Color = RGB(255, 0, 0)) Then
                            Print #1, "A feestdag," & amDate & "," & amDate & "," & strFullDay & strLate, vbNewLine;
                        Else
                            Print #1, "A," & amDate & "," & amDate & "," & strFullDay & strLate, vbNewLine;
                        End If
                    Case Is = "N"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), 2).Interior.Color = RGB(255, 0, 0)) Then
                            Print #1, "N feestdag," & amDate & "," & amDateTwo & "," & strFullDay & strNight, vbNewLine;
                        Else
                            Print #1, "N," & amDate & "," & amDateTwo & "," & strFullDay & strNight, vbNewLine;
                        End If
                    Case Is = "n"
                        If (kbcPlanning.Sheets(i).Cells((x + 3), 2).Interior.Color = RGB(255, 0, 0)) Then
                            Print #1, "N feestdag," & amDate & "," & amDateTwo & "," & strFullDay & strNight, vbNewLine;
                        Else
                            Print #1, "N," & amDate & "," & amDateTwo & "," & strFullDay & strNight, vbNewLine;
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
                        If (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(255, 255, 255) Or kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(255, 255, 0)) Then
                            Print #1, "d (wit)," & amDate & "," & amDate & "," & strFullDay & strDay, vbNewLine;
                        Else
                            If Not (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(255, 0, 0)) And Not (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(255, 255, 0)) And Not (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(0, 176, 240)) And Not (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(151, 71, 6)) And Not (kbcPlanning.Sheets(i).Cells((x + 3), nameCol).Interior.Color = RGB(0, 176, 80)) Then
                                Print #1, "******************Fout - zie planning******************," & amDate & "," & amDate & ",TRUE", vbNewLine;
                            End If
                        End If
                End Select
            Next x
            
            Close #1
            
            ownPlanning.Sheets("Export").Cells(1, 1) = "Je CSV staat klaar onder Y:\" & RTrim(strName) & ".csv"
            Exit For
        End If
    Next i
    
    kbcPlanning.Close (False)
End Sub



