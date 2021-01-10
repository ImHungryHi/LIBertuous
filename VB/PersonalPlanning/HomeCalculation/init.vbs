Rem Attribute VBA_ModuleType=VBADocumentModule
Option VBASupport 1
Private Sub Workbook_Open()
    Dim x As Long, a As Long, lngResult As Long, lngVar As Long, lngCostRows As Long
    Dim varDate As Variant, arrCostCol() As Variant, arrUniqueCosts() As Variant, arrNoGood() As Variant, arrTemp() As Variant, varFilter As Variant
    Dim blnCheck As Boolean, arrInkomsten() As Variant, arrTotalen() As Variant
    Dim arrCostDates() As Variant, arrNewCostDates() As Variant
    blnCheck = True
    
    With ThisWorkbook.Sheets("Data")
        .Range("A:D").Clear
    
        a = 0
        lngResult = 0
        y = 0
        
        lngVar = .Cells(Rows.Count, 24).End(xlUp).Row
        ReDim Preserve arrNoGood(lngVar)
        arrNoGood() = .Range("X1:X" & lngVar).Value
        
        lngCostRows = ThisWorkbook.Sheets("Lopende kosten").Cells(Rows.Count, 1).End(xlUp).Row
        ReDim Preserve arrCostCol(lngCostRows)
        arrCostCol() = ThisWorkbook.Sheets("Lopende kosten").Range("A1:A" & lngCostRows).Value
        
        For x = 3 To lngCostRows
            If (arrCostCol(x, 1) = "") Then
                blnCheck = False
            ElseIf (a = 0) Then
                lngResult = 0
            Else
                If (InStr(1, arrCostCol(x, 1), "nstallatie LPG", vbTextCompare)) Then
                    arrCostCol(x, 1) = "Installatie LPG"
                End If
                
                If (InStr(1, arrCostCol(x, 1), "LPG brandstof", vbTextCompare)) Then
                    arrCostCol(x, 1) = "LPG brandstof"
                End If
                
                If (Not IsError(Application.Match(arrCostCol(x, 1), arrUniqueCosts, 0))) Then
                    lngResult = 1
                Else
                    lngResult = 0
                End If
            End If
            
            If (lngResult = 0) Then
                If (IsError(Application.Match(arrCostCol(x, 1), arrNoGood, 0))) Then
                    ReDim Preserve arrUniqueCosts(a)
                    arrUniqueCosts(a) = arrCostCol(x, 1)
                    a = a + 1
                End If
            End If
        Next x
        
        'ReDim to multidimensional
        ReDim Preserve arrTemp(1 To a, 1 To 1)
        
        For i = 0 To a - 1
            arrTemp(i + 1, 1) = arrUniqueCosts(i)
        Next i
        
        arrUniqueCosts = arrTemp
        
        'Set range
        Dim rngTarget As Range
        Set rngTarget = .Range("B1:B" & a)
        rngTarget = arrUniqueCosts
        
        .Range("$B$1:$B$" & a).Sort Key1:=['Data'!B1], Order1:=xlAscending, Header:=xlNo
        ThisWorkbook.Sheets("Lopende kosten").Cells(3, 18).Value = "LPG brandstof"
    
        'blnCheck = True
        'x = 3
        'a = 1
        'lngResult = 0
        '
        'Do While (blnCheck)
        '    If (ThisWorkbook.Sheets("Lopende kosten").Cells(x, 2) = "") Then
        '        blnCheck = False
        '    Else
        '        If (ThisWorkbook.Sheets("Lopende kosten").Cells(x, 5) = 1) Then
        '            varDate = ThisWorkbook.Sheets("Lopende kosten").Cells(x, 2)
        '            varDate = DateValue("1-" & Month(varDate) & "-" & Year(varDate))
        '
        '            If (a = 1) Then
        '                If (varDate = .Range("D1").Value) Then
        '                    lngResult = 1
        '                End If
        '            Else
        '                If (.Range("D1:D" & a).Find(varDate, LookIn:=xlValues) Is Nothing) Then
        '                    lngResult = 0
        '                Else
        '                    lngResult = 1
        '                End If
        '            End If
        '
        '            If (lngResult = 0) Then
        '                .Cells(a, 4) = varDate
        '                a = a + 1
        '            End If
        '        End If
        '
        '        x = x + 1
        '    End If
        'Loop
        
        ReDim arrCostDates(1 To lngCostRows, 1 To 4)
        
        arrCostDates() = ThisWorkbook.Sheets("Lopende kosten").Range("B1:E" & lngCostRows).Value
        blnCheck = True
        x = 3
        a = 1
        lngResult = 0
        
        Do While (blnCheck)
            If (arrCostDates(x, 1) = "") Then
                blnCheck = False
            Else
                If (arrCostDates(x, 4) = 1) Then
                    ReDim Preserve arrNewCostDates(a)
                    arrNewCostDates(a) = arrCostDates(x, 3)
                    a = a + 1
                End If
        
                x = x + 1
            End If
        
            If (x > lngCostRows) Then
                blnCheck = False
            End If
        Loop
        
        'ReDim to multidimensional
        ReDim arrTemp(1 To a, 1 To 1)
        
        For i = 0 To a - 1
            arrTemp(i + 1, 1) = arrNewCostDates(i)
        Next i
        
        ReDim arrNewCostDates(1 To a, 1 To 1)
        arrNewCostDates = arrTemp
        
        .Range("$D$1:$D$" & a) = arrNewCostDates
        
        .Range("$D$1:$D$" & a).Sort Key1:=['Data'!D1], Order1:=xlDescending, Header:=xlNo
        
        ReDim rngInkomsten(1 To 7, 1 To 1)
        ReDim rngTotalen(1 To 3, 1 To 1)
        
        rngInkomsten(1, 1) = ""
        rngInkomsten(2, 1) = "13e maand"
        rngInkomsten(3, 1) = "Belastingen"
        rngInkomsten(4, 1) = "Km vergoeding"
        rngInkomsten(5, 1) = "Nettoloon"
        rngInkomsten(6, 1) = "Vakantiegeld"
        rngInkomsten(7, 1) = "Ziekte-uitkering"
        .Range("C1:C7") = rngInkomsten
        ThisWorkbook.Sheets("Inkomsten").Cells(1, 3).Value = "Nettoloon"
        
        rngTotalen(1, 1) = "Totaal auto"
        rngTotalen(2, 1) = "Totaal wonen"
        rngTotalen(3, 1) = "Totaal eten en andere"
        .Range("A1:A3") = rngTotalen
        ThisWorkbook.Sheets("Lopende kosten").Cells(1, 18).Value = "Totaal auto"
    End With
End Sub

Private Sub Workbook_BeforeSave(ByVal SaveAsUI As Boolean, Calcen As Boolean)
    Dim arrLPGCosts() As Variant, arrLPGDates() As Variant, lngRowCount As Long, ownPlanning As Workbook, wbkHuis As Workbook, arrTimeDates() As Variant
    Dim x As Long, y As Long, arrTemp() As Variant, lngVar As Long, n As Long, m As Long, lngDay As Long, lngMonth As Long, dblTotalDistance As Double, blnExit As Boolean

    For x = 1 To Workbooks.Count
        If (Workbooks(x).Name = "Planning.xlsm") Then
            Set ownPlanning = Workbooks(x)
        ElseIf (Workbooks(x).Name = "Huis.xlsm") Then
            Set wbkHuis = Workbooks(x)
        End If
        
        If Not (wbkHuis Is Nothing) And Not (ownPlanning Is Nothing) Then
            Exit For
        End If
    Next x
    
    If (ownPlanning Is Nothing) Then
        Set ownPlanning = Workbooks.Open(wbkHuis.Path & "\Planning.xlsm")
        ownPlanning.Application.WindowState = xlMinimized
    End If
    
    y = 0
    blnExit = False
    dblTotalDistance = 0
    lngVar = wbkHuis.Sheets("Lopende kosten").Cells(Rows.Count, 1).End(xlUp).Row
    
    ReDim Preserve arrLPGCosts(lngVar)
    ReDim Preserve arrLPGDates(lngVar)
    arrLPGCosts = ThisWorkbook.Sheets("Lopende kosten").Range("A1:B" & lngVar).Value
        
    For x = 3 To lngVar
        If (arrLPGCosts(x, 1) = "LPG brandstof") Then
            ReDim Preserve arrLPGDates(y)
            arrLPGDates(y) = arrLPGCosts(x, 2)
            y = y + 1
        End If
    Next x
    
    'ReDim to multidimensional
    ReDim arrTemp(1 To y, 1 To 1)
    
    For i = 0 To y - 1
        arrTemp(i + 1, 1) = arrLPGDates(i)
    Next i
    
    ReDim arrLPGDates(1 To y, 1 To 1)
    arrLPGDates = arrTemp
    
    ownPlanning.Sheets("Routes").Range("G2:G1048576").Clear
    ownPlanning.Sheets("Routes").Range("G2:G" & (y + 1)) = arrLPGDates
End Sub

