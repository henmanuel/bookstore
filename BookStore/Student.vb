Public Class Student
    Inherits User

    Public Sub New(username As String, password As String)
        MyBase.New(username, password, "student")
    End Sub

    ' Constructor para la deserialización
    Public Sub New(username As String, salt As Byte(), passwordHash As Byte())
        MyBase.New(username, salt, passwordHash, "student")
    End Sub
End Class