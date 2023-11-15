Imports Newtonsoft.Json
Imports System.IO

' Módulo para la serialización y deserialización
Public Module UserSerializer
    Private Const UserFilePath As String = "users.json"

    Public Sub SerializeUsers(users As List(Of User))
        Dim userDtos = users.Select(Function(u) New UserDto With {
            .Username = u.Username,
            .SaltBase64 = u.SaltBase64,
            .PasswordHashBase64 = u.PasswordHashBase64
        }).ToList()

        Dim json As String = JsonConvert.SerializeObject(userDtos, Formatting.Indented)
        File.WriteAllText(UserFilePath, json)
    End Sub

    Public Function DeserializeUsers() As List(Of User)
        If Not File.Exists(UserFilePath) Then
            Return New List(Of User)()
        End If

        Dim json As String = File.ReadAllText(UserFilePath)
        Dim userDtos = JsonConvert.DeserializeObject(Of List(Of UserDto))(json)
        Dim users = New List(Of User)()

        For Each dto In userDtos
            Dim salt As Byte() = Convert.FromBase64String(dto.SaltBase64)
            Dim passwordHash As Byte() = Convert.FromBase64String(dto.PasswordHashBase64)
            Dim user = New User(dto.Username, salt, passwordHash)
            users.Add(user)
        Next

        Return users
    End Function
End Module