' Clase para el rol de Estudiante con funcionalidades específicas
Public Class Student
    Inherits User

    Public Sub New(username As String, password As String)
        MyBase.New(username, password)
    End Sub
End Class