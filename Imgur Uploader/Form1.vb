Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Text.RegularExpressions

Public Class Form1

    'CODED BY : X-SLAYER
    'YOUTUBE : https://www.youtube.com/c/XSLAYER404
    'FACEBOOK : https://www.facebook.com/ihebbriki1
    'INSTAGRAM : https://www.instagram.com/brikiiheb
    'BIUSNESS MAIL : xslayer404@gmail.com


    Public Sub New()
        InitializeComponent()
        Control.CheckForIllegalCrossThreadCalls = False
    End Sub
    Dim ClientId As String = "66665db7b4b0608"
    Dim Dictionner As New Dictionary(Of WebClient, ListViewItem)

    Private Sub Completed(sender As Object, e As UploadValuesCompletedEventArgs)
        Dim Result As String = (New UTF8Encoding).GetString(e.Result)
        If e.Cancelled = True Then
            lvfiles.Items(Dictionner.Item(sender).Index).SubItems(1).ForeColor = Color.RoyalBlue
            lvfiles.Items(Dictionner.Item(sender).Index).SubItems(1).Text = "Cancelled"
        ElseIf e.Error IsNot Nothing Then
            lvfiles.Items(Dictionner.Item(sender).Index).SubItems(1).ForeColor = Color.Red
            lvfiles.Items(Dictionner.Item(sender).Index).SubItems(1).Text = "Error"
        ElseIf e.Result IsNot Nothing Then
            Dim K As Match = Regex.Match(Result, ",""link"":""(.*?)""}")
            lvfiles.Items(Dictionner.Item(sender).Index).ToolTipText = K.Groups(1).Value.Replace("\", "")
            My.Settings.History.Add(Dictionner.Item(sender).Text & "|" & K.Groups(1).Value.Replace("\", "") & "|" & Date.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            My.Settings.Save()
            lvfiles.Items(Dictionner.Item(sender).Index).SubItems(1).ForeColor = Color.Blue
            lvfiles.Items(Dictionner.Item(sender).Index).SubItems(1).Font = New Font("Segoe UI", 9, FontStyle.Regular Or FontStyle.Underline)
            lvfiles.Items(Dictionner.Item(sender).Index).SubItems(1).Text = K.Groups(1).Value.Replace("\", "")

        End If

    End Sub
    Private Sub Progress(sender As Object, e As UploadProgressChangedEventArgs)
        On Error Resume Next
        lvfiles.Items(Dictionner.Item(sender).Index).SubItems(1).ForeColor = Color.RoyalBlue
        lvfiles.Items(Dictionner.Item(sender).Index).SubItems(1).Text = e.ProgressPercentage & "%"
    End Sub

    Private Sub lvfiles_DoubleClick(sender As Object, e As EventArgs) Handles lvfiles.DoubleClick
        If Not lvfiles.FocusedItem.SubItems(1).ForeColor = Color.Blue Then : Exit Sub : End If
        Clipboard.SetText(lvfiles.FocusedItem.ToolTipText)
        MsgBox(lvfiles.FocusedItem.ToolTipText & "Copied To Clipoard", MsgBoxStyle.Information)
    End Sub

    Private Sub lvfiles_DragDrop(sender As Object, e As DragEventArgs) Handles lvfiles.DragDrop
        Try
            For Each x In e.Data.GetData(DataFormats.FileDrop)
                Dim NIcon As Icon = Icon.ExtractAssociatedIcon(x)
                ImageList1.Images.Add(NIcon)
                Dim itm As New ListViewItem
                itm.UseItemStyleForSubItems = False
                itm.Text = Path.GetFileName(x)
                itm.SubItems.Add("Uploading...").ForeColor = ColorTranslator.FromHtml("#3b4451")
                itm.ImageIndex = ImageList1.Images.Count - 1
                lvfiles.Items.Add(itm)

                Dim w As New WebClient()
                w.Headers.Add("Authorization", "Client-ID " & ClientId)
                AddHandler w.UploadValuesCompleted, AddressOf Completed
                AddHandler w.UploadProgressChanged, AddressOf Progress
                Dim Keys As New System.Collections.Specialized.NameValueCollection
                Keys.Add("image", Convert.ToBase64String(File.ReadAllBytes(x)))
                w.UploadValuesAsync(New Uri("https://api.imgur.com/3/image"), Keys)
                Dictionner.Add(w, itm)
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
 
    Private Sub lvfiles_DragEnter(sender As Object, e As DragEventArgs) Handles lvfiles.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.All
        End If
    End Sub



    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        For i = 0 To My.Settings.History.Count - 1
            Dim x As New ListViewItem
            x.UseItemStyleForSubItems = False
            x.Text = My.Settings.History(i).Split("|")(0)
            x.SubItems.Add(My.Settings.History(i).Split("|")(1)).ForeColor = Color.Blue
            x.SubItems.Add(My.Settings.History(i).Split("|")(2)).ForeColor = Color.Red
            x.SubItems(1).Font = New Drawing.Font("Tahoma", 9, FontStyle.Regular Or FontStyle.Underline)
            lhistory.Items.Add(x)
        Next
        removeduplicates(lhistory)
    End Sub
    Public Sub removeduplicates(ByRef lv As ListView)
        Dim MyItems As New ArrayList
        For Each MyItem As ListViewItem In lv.Items
            If MyItems.Contains(MyItem.Text) Then
                MyItem.Remove()
            Else
                MyItems.Add(MyItem.Text)
            End If
        Next
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        If (Me.Width = 852) Then
            Me.Width = 384
        Else
            Me.Width = 852
        End If
    End Sub

    Private Sub lhistory_DoubleClick(sender As Object, e As EventArgs) Handles lhistory.DoubleClick
        Clipboard.SetText(lhistory.FocusedItem.SubItems(1).Text)
    End Sub

    Private Sub lvfiles_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvfiles.SelectedIndexChanged

    End Sub
End Class
