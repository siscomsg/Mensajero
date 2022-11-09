Imports System.Data.SqlClient
Imports System.Net.Sockets
Imports System.Net.Dns
Imports Mensajero.elGuille.Util



Public Class Form1
    Dim con As New SqlConnection
    Dim nombre_Host As String = GetHostName.ToString

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        'e.Cancel = True
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        coneccion()
        CargaDatos(Me.CheckedListBox1, "Select * from host where estado = 1")
        Me.txtIP.Text = nombre_Host
        mostrar_mensaje()
    End Sub
   

    Private Sub btnEnviarMensaje_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnviarMensaje.Click

        'Envio el texto escrito en el textbox txtMensaje a todos los clientes

        If txtMensaje.Text = Nothing Then
            MsgBox("El Mensaje Esta Vacio", MsgBoxStyle.OkOnly + MsgBoxStyle.Critical, "Error")
            Me.txtMensaje.Focus()
            Exit Sub
        End If
        
        Dim cmdgrabar As SqlCommand
        
        Try

            For x = 0 To Me.CheckedListBox1.CheckedItems.Count - 1
                con.Open()
                cmdgrabar = New SqlCommand("insert into mensajes (host_name, mensaje, fec_hora, emisor, estado) " & _
                                           "values('" & Me.CheckedListBox1.CheckedItems(x) & "', '" & Me.txtMensaje.Text & "',  getdate(), '" & Me.txtIP.Text & "','1')", con)
                cmdgrabar.ExecuteNonQuery()
                cmdgrabar.Dispose()
                con.Close()
            Next

            MsgBox("Mensaje enviado correctamente")
            Me.txtMensaje.Text = Nothing
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly)
        End Try

    End Sub
    


    Private Sub btnListarPCS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnListarPCS.Click
        CargaDatos(Me.CheckedListBox1, "Select * from host where estado = 1")
        
    End Sub
    Public Sub CargaDatos(ByVal list As CheckedListBox, ByVal Ssql As String)
        On Error Resume Next
        con.Open()
        Dim sql As String = Ssql
        Dim Sda As New SqlDataAdapter(sql, con)
        Dim Dtt As New DataTable
        Sda.Fill(Dtt)
        list.Items.Clear()
        For x = 1 To Dtt.Rows.Count
            list.Items.Add(Dtt.Rows(x - 1)("host_name"))

        Next
        con.Close()
        Exit Sub


    End Sub

    Private Sub btnSalir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSalir.Click
        End
    End Sub
    Public Sub coneccion()
        Dim host, db, id, pwd As String
        host = "192.168.1.30\sqlexpress" 'SHADOW" '192.168.1.30\sqlexpress" '192.168.1.48"
        db = "mensajes"
        id = "sa"
        pwd = ""
        con.ConnectionString = ("Data Source=" & host & "; Initial Catalog=" & db & ";Persist Security Info=True;User ID=" & id & ";Pwd=" & pwd & ";")
        Try
            

        Catch ex As Exception
            Exit Sub
            'resultado = MsgBox("Error de conección desea reintentar???", MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Conección")
            'If resultado = vbYes Then
            ' MsgBox("Error al conectar a db", MsgBoxStyle.OkOnly + MsgBoxStyle.Critical, "Conexión")
            ' End If
        End Try
    End Sub
    
    Private Sub mostrar_mensaje()

        Try

            con.Open()
            'aqui obtenemos el nombre de nuestra pc

            'buscamos en la base de datos si existe algun mensaje para nuestra pc
            Dim sql As String = "Select * from mensajes where host_name = @host and (fec_hora between @fecha1 and @fecha2) " 'DateTime.Now.AddMinutes(0)
            Dim cmd As New SqlCommand(sql, con)
            Dim dt As New DataTable
            cmd.Parameters.AddWithValue("@host", Me.txtIP.Text)
            cmd.Parameters.AddWithValue("@fecha1", DateTime.Now.AddMinutes(-1))
            cmd.Parameters.AddWithValue("@fecha2", DateTime.Now.AddMinutes(0))
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
            'DataGridView1.DataSource = dt
            If dt.Rows.Count > 0 Then
                If Me.WindowState = 1 Then Me.WindowState = 0
                'WinAPI.SiempreEncima(Me.Handle.ToInt32)
                'Else
                'If Me.WindowState = 0 Then Me.WindowState = 1
                'WinAPI.NoSiempreEncima(Me.Handle.ToInt32)
            End If
            For x = 0 To dt.Rows.Count - 1
                MsgBox(".:: " & dt.Rows(x)("emisor") & " Dice .:: " & dt.Rows(x)("mensaje"), MsgBoxStyle.OkOnly + MsgBoxStyle.Information, ".:: Mensaje ::.")
            Next
            con.Close()
            
        Catch ex As Exception
            Exit Sub
            'Me.WindowState = 1
            'MessageBox.Show(ex.Message)
        End Try
        
        'MsgBox(Dtt.Rows(0)("mensaje"), MsgBoxStyle.OkOnly + MsgBoxStyle.Information, Dtt.Rows(0)("emisor"))
        'con.Close()
    End Sub

    Private Sub ListBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox1.MouseDown
        Dim operaciones As DragDropEffects
        If e.Button = Windows.Forms.MouseButtons.Left Then
            operaciones = ListBox1.DoDragDrop(Me.ListBox1.Items(Me.ListBox1.SelectedIndex), DragDropEffects.Move Or DragDropEffects.Copy)
        End If
        If operaciones = DragDropEffects.Move Then
            Me.ListBox1.Items.RemoveAt(Me.ListBox1.SelectedIndex)
        End If
    End Sub

    Private Sub ListBox2_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListBox2.DragDrop
        Me.ListBox2.Items.Add(e.Data.GetData(DataFormats.Text))
    End Sub

    
    Private Sub ListBox2_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListBox2.DragEnter
        If e.KeyState = 9 Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.Move
        End If
    End Sub

    
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If con.State = ConnectionState.Open Then con.Close()
        mostrar_mensaje()
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        Dim x As Integer
        If CheckBox1.Checked = True Then
            For x = 0 To CheckedListBox1.Items.Count - 1
                CheckedListBox1.SetItemCheckState(x, CheckState.Checked)
            Next
        Else
            For x = 0 To CheckedListBox1.Items.Count - 1
                CheckedListBox1.SetItemCheckState(x, CheckState.Unchecked)
            Next
        End If
    End Sub
End Class
