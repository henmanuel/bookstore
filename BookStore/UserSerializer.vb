Imports Newtonsoft.Json
Imports System.IO

' Módulo para la serialización y deserialización
Public Module UserSerializer
    Private Const UserFilePath As String = "users.json"

    Public Sub SerializeUsers(users As List(Of User))
        ' Validar que no hay usuarios duplicados
        If users.GroupBy(Function(u) u.Username).Any(Function(g) g.Count() > 1) Then
            Throw New InvalidOperationException("No se puede serializar porque hay usuarios duplicados.")
        End If

        Dim userDtos = users.Select(Function(u) New UserDto With {
            .Role = u.Role,
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
        If String.IsNullOrWhiteSpace(json) Then
            Return New List(Of User)()
        End If

        Dim userDtos As List(Of UserDto) = JsonConvert.DeserializeObject(Of List(Of UserDto))(json)
        If userDtos Is Nothing Then
            Return New List(Of User)()
        End If

        Dim users = New List(Of User)()
        Dim usernames = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each dto In userDtos
            If String.IsNullOrEmpty(dto.Role) Then
                Console.WriteLine($"Error: El rol del usuario '{dto.Username}' es inválido y será omitido.")
                Continue For
            ElseIf usernames.Contains(dto.Username) Then
                Console.WriteLine($"Error: El usuario '{dto.Username}' está duplicado y será omitido.")
                Continue For
            End If

            Try
                Dim user As User = User.FromDto(dto)
                users.Add(user)
                usernames.Add(dto.Username) ' Añadir el nombre de usuario al conjunto para controlar duplicados
            Catch ex As Exception
                Console.WriteLine($"Error: No se pudo crear el usuario '{dto.Username}'. Error: {ex.Message}")
            End Try
        Next

        Return users
    End Function

End Module