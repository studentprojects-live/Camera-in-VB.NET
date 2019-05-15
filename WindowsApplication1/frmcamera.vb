Public Class frmcamera
    Private WithEvents cam As New DSCamCapture
    Dim MyPicturesFolder As String = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        cam.Dispose()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox_Devices.Items.AddRange(cam.GetCaptureDevices)
        If ComboBox_Devices.Items.Count > 0 Then ComboBox_Devices.SelectedIndex = 0

        For Each sz As String In [Enum].GetNames(GetType(DSCamCapture.FrameSizes))
            ComboBox_FrameSize.Items.Add(sz.Replace("s", ""))
        Next
        If ComboBox_FrameSize.Items.Count > 2 Then ComboBox_FrameSize.SelectedIndex = 2

        Button_Connect.Enabled = (ComboBox_Devices.Items.Count > 0)
        Button_Save.Enabled = False
    End Sub

    Private Sub ComboBox_Devices_DropDown(sender As Object, e As EventArgs) Handles ComboBox_Devices.DropDown
        ComboBox_Devices.Items.Clear()
        ComboBox_Devices.Items.AddRange(cam.GetCaptureDevices)
        Button_Connect.Enabled = (ComboBox_Devices.Items.Count > 0)
        If ComboBox_Devices.SelectedIndex = -1 And ComboBox_Devices.Items.Count > 0 Then ComboBox_Devices.SelectedIndex = 0
    End Sub

    Private Sub Button_Connect_Click(sender As Object, e As EventArgs) Handles Button_Connect.Click
        If Not cam.IsConnected Then
            Dim si As Integer = ComboBox_FrameSize.SelectedIndex
            Dim SelectedSize As DSCamCapture.FrameSizes = CType(si, DSCamCapture.FrameSizes)
            If cam.ConnectToDevice(ComboBox_Devices.SelectedIndex, 15, PictureBox1.ClientSize, SelectedSize, PictureBox1.Handle) Then
                cam.Start()
                Button_Connect.Text = "Disconnect"
            End If
        Else
            cam.Dispose()
            Button_Connect.Text = "Connect"
        End If
        Button_Save.Enabled = cam.IsConnected
        ComboBox_Devices.Enabled = Not cam.IsConnected
        ComboBox_FrameSize.Enabled = Not cam.IsConnected
    End Sub

    Private Sub Button_Save_Click(sender As Object, e As EventArgs) Handles Button_Save.Click
        If Not IO.Directory.Exists(MyPicturesFolder) Then IO.Directory.CreateDirectory(MyPicturesFolder)
        Dim fName As String = Now.ToString.Replace("/", "-").Replace(":", "-").Replace(" ", "_") & ".jpg"
        Dim SaveAs As String = IO.Path.Combine(MyPicturesFolder, fName)
        cam.SaveCurrentFrame(SaveAs, Imaging.ImageFormat.Jpeg)
    End Sub

    Dim appPath As String = My.Application.Info.DirectoryPath
    Private Sub PictureBox1_SizeChanged(sender As Object, e As EventArgs) Handles PictureBox1.SizeChanged
        cam.ResizeWindow(0, 0, PictureBox1.ClientSize.Width, PictureBox1.ClientSize.Height)
    End Sub

    Private Sub cam_FrameSaved(capImage As Bitmap, imgPath As String) Handles cam.FrameSaved
        PictureBox2.Image = New Bitmap(capImage)
        Label2.Text = "Saved As - " & IO.Path.GetFileName(imgPath)
        Label3.Text = "Image Size - " & PictureBox2.Image.Size.ToString
    End Sub

End Class
