Rem Attribute VBA_ModuleType=VBAModule
Option VBASupport 1


Sub Button1_Click()
    'Sheets("Sheet1").Activate
    
    'alle properties nakijken als ze prio 1 hebben en deze in de
    Dim RodeAlertProperties As Collection
    Set RodeAlertProperties = New Collection
    For Cursor = 13 To 169
        Line = ActiveWorkbook.Worksheets("Sheet1").Cells(Cursor, 1).Value
        
        If InStr(1, Line, "[PROPERTIES") = 1 Then
            PropertyNaam = Right(Line, Len(Line) - 12)
            PropertyNaam = Left(PropertyNaam, Len(PropertyNaam) - 1)
        End If
        
        If InStr(1, Line, "PRI=1") = 1 Then
            RodeAlertProperties.Add (PropertyNaam)
        End If
    Next
    Sheets("Sheet2").Activate
    For i = 1 To RodeAlertProperties.Count
        Worksheets("Sheet2").Cells(i, 1).Value = RodeAlertProperties.Item(i)
    Next
    
    Dim RodeAlerts As Collection
    Set RodeAlerts = New Collection
    
    For Cursor = 170 To 3111 'waardes hardcoded voor het gemak
        Line = ActiveWorkbook.Worksheets("Sheet1").Cells(Cursor, 1).Value
         If InStr(1, Line, "[ALERT") = 1 Then
            AlertNaam = Right(Line, Len(Line) - 7)
            AlertNaam = Left(AlertNaam, Len(AlertNaam) - 1)
            AlertToegevoegd = False
         End If
         
         If InStr(1, Line, "PRI=1") = 1 And Not AlertToegevoegd Then
            RodeAlerts.Add (AlertNaam)
            AlertToegevoegd = True
         End If
         
         'controleren voor sub allerts vb OPERINFO-01
         If InStr(1, Line, "PRI-") = 1 Then
            If (StrComp(Mid(Line, 8, 1), "1") = 0) Then
                RodeAlerts.Add (AlertNaam & Mid(Line, 4, 3))
            End If
         End If
         
         'controleren als de allert via property een prio 1 heeft
         If InStr(1, Line, "PROPERTIES=") = 1 Then
            Property = Right(Line, Len(Line) - 11)
            isRodeAllert = False
            For curs = 1 To RodeAlertProperties.Count
                If StrComp(RodeAlertProperties.Item(curs), Property) = 0 Then
                    isRodeAllert = True
                End If
            Next
            If isRodeAllert And Not AlertToegevoegd Then
                RodeAlerts.Add (AlertNaam)
                AlertToegevoegd = True
            End If
        End If
    Next
    
    For i = 1 To RodeAlerts.Count
        Worksheets("Sheet2").Cells(i, 2).Value = RodeAlerts.Item(i)
    Next
End Sub
