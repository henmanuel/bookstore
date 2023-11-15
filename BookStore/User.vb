Imports System.Security.Cryptography
Imports System.Text

' Clase base para un usuario con propiedades comunes
Public Class User
    Private _salt As Byte()
    Public Property Username As String
    Private _passwordHash As Byte()

    ' Constructor que se utiliza al crear un nuevo usuario
    Public Sub New(username As String, password As String)
        Me.Username = username
        GenerateSaltAndPasswordHash(password)
    End Sub

    ' Constructor que se utiliza durante la deserialización
    Public Sub New(username As String, salt As Byte(), passwordHash As Byte())
        Me.Username = username
        Me._salt = salt
        Me._passwordHash = passwordHash
    End Sub

    ' Genera salt y hash para la contraseña
    Private Sub GenerateSaltAndPasswordHash(password As String)
        Using rng As New RNGCryptoServiceProvider()
            _salt = New Byte(16) {}
            rng.GetBytes(_salt)
        End Using
        _passwordHash = HashPassword(password)
    End Sub

    ' Verifica si la contraseña proporcionada coincide con el hash almacenado
    Public Function VerifyPassword(password As String) As Boolean
        Dim hashOfInput As Byte() = HashPassword(password)
        Return _passwordHash.SequenceEqual(hashOfInput)
    End Function

    ' Genera el hash de la contraseña con el salt
    Private Function HashPassword(password As String) As Byte()
        Using hasher As SHA256 = SHA256Managed.Create()
            Dim saltedPassword As Byte() = _salt.Concat(Encoding.UTF8.GetBytes(password)).ToArray()
            Return hasher.ComputeHash(saltedPassword)
        End Using
    End Function

    ' Propiedades para obtener los hashes en base64
    Public ReadOnly Property SaltBase64 As String
        Get
            Return Convert.ToBase64String(_salt)
        End Get
    End Property

    Public ReadOnly Property PasswordHashBase64 As String
        Get
            Return Convert.ToBase64String(_passwordHash)
        End Get
    End Property
End Class

' DTO para la serialización
Public Class UserDto
    Public Property Username As String
    Public Property SaltBase64 As String
    Public Property PasswordHashBase64 As String
End Class