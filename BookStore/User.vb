Imports System.Security.Cryptography
Imports System.Text

' Clase base para un usuario con propiedades comunes
Public Class User
    Private _salt As Byte()
    Private _passwordHash As Byte()
    Public Property Username As String
    Public Property Role As String ' Propiedad para el rol

    ' Constructor que se utiliza al crear un nuevo usuario
    Public Sub New(username As String, password As String, role As String)
        Me.Username = username
        Me.Role = role
        GenerateSaltAndPasswordHash(password)
    End Sub

    ' Constructor que se utiliza durante la deserialización
    Public Sub New(username As String, salt As Byte(), passwordHash As Byte(), role As String)
        Me.Username = username
        Me._salt = salt
        Me._passwordHash = passwordHash
        Me.Role = role
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

    Public Function ToDto() As UserDto
        Return New UserDto() With {
            .Username = Me.Username,
            .SaltBase64 = Convert.ToBase64String(_salt),
            .PasswordHashBase64 = Convert.ToBase64String(_passwordHash),
            .Role = Me.Role
        }
    End Function

    Public Shared Function FromDto(dto As UserDto) As User
        If dto Is Nothing Then
            Throw New ArgumentNullException(NameOf(dto))
        End If

        If String.IsNullOrEmpty(dto.SaltBase64) OrElse String.IsNullOrEmpty(dto.PasswordHashBase64) Then
            Throw New ArgumentException("Salt or password hash cannot be null or empty.")
        End If

        Dim salt As Byte() = Convert.FromBase64String(dto.SaltBase64)
        Dim passwordHash As Byte() = Convert.FromBase64String(dto.PasswordHashBase64)
        Dim role As String = dto.Role

        If String.IsNullOrEmpty(role) Then
            Throw New ArgumentException("Role cannot be null or empty.")
        End If

        Select Case role.ToLower()
            Case "librarian"
                Return New Librarian(dto.Username, salt, passwordHash)
            Case "student"
                Return New Student(dto.Username, salt, passwordHash)
            Case Else
                Throw New Exception("Unknown role type")
        End Select
    End Function

End Class

' DTO para la serialización
Public Class UserDto
    Public Property Username As String
    Public Property SaltBase64 As String
    Public Property PasswordHashBase64 As String
    Public Property Role As String
End Class